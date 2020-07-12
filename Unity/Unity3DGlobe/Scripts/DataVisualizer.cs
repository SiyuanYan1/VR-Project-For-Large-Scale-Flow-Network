using Arcs;
using Facilities;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ArcDrawer))]
public class DataVisualizer : MonoBehaviour
{
    [Tooltip("The material which will be applied to all created points")]
    public Material PointMaterial;

    [Tooltip("The prefab used to create Facility nodes")]
    public GameObject PointPrefab;

    [Tooltip("The cap on how many Arcs will be loaded")]
    public int ArcLimit = 10000;

    private List<GameObject> points = new List<GameObject>();
    private Dictionary<string, string> states = new Dictionary<string, string>();
    private Dictionary<string, List<FacData>> locations = new Dictionary<string, List<FacData>>();
    private Dictionary<Vector3, string> buildings = new Dictionary<Vector3, string>();
    private Dictionary<string, Vector3> coordinates = new Dictionary<string, Vector3>();
    private Dictionary<string, List<string>> path = new Dictionary<string, List<string>>();
    private Dictionary<string, string> samebuilding = new Dictionary<string, string>();

    public void InitialiseStates(SeriesLocData[] allSeries)
    {
        SeriesLocData seriesData = allSeries[0];
        for (int i = 0; i < seriesData.Data.Length; i += 2)
        {
            string state = seriesData.Data[i + 1];
            if (state == " DC")
            {
                state = " VA";
            }
            states[seriesData.Data[i]] = state;
        }
    }

    public void CreateFacilities(SeriesLocData[] allSeries)
    {
        Facility.PointMaterial = PointMaterial;
        SeriesLocData seriesData = allSeries[0];
        Transform folder = new GameObject("Facilities").transform;
        folder.SetParent(transform, false);
        //Debug.Log(seriesData.Data.Length);
        for (int j = 0; j < seriesData.Data.Length; j += 9)
        {
            string SLIC = seriesData.Data[j].ToString();
            float latitude = float.Parse(seriesData.Data[j + 1]);
            float longitude = float.Parse(seriesData.Data[j + 2]);
            string CC = seriesData.Data[j + 3].ToString();
            string SORT = seriesData.Data[j + 4].ToString();
            string CAP = seriesData.Data[j + 5].ToString();
            string SPAN = seriesData.Data[j + 6].ToString();
            string STRT = seriesData.Data[j + 7].ToString();
            string STATE = seriesData.Data[j + 8].ToString();
            if (STATE == " DC")
            {
                STATE = " WA";
            }
            //if (STATE == " FL")
            //{
            //    Debug.Log("Florida Facility: " + SLIC);
            //}
            FacData fac = GetFacility(SLIC, latitude, longitude, CC, SORT, CAP, SPAN, STRT, STATE);
            
            if (!locations.ContainsKey(fac.SLIC))
            {
                List<FacData> loc = new List<FacData>();
                loc.Add(fac);
                locations.Add(fac.SLIC, loc);
            }
            else
            {
                locations[fac.SLIC].Add(fac);
            }

            //draw facility only once
            if (!buildings.ContainsKey(fac.pos))
            {
                buildings.Add(fac.pos, fac.SLIC);
                GameObject p = Instantiate(PointPrefab);
                p.transform.SetParent(folder, false);
                p.transform.localScale = new Vector3(1, 1, 0.001f);
                p.transform.localPosition = fac.pos;
                p.transform.forward = fac.pos;  // make cube point outwards
                p.GetComponent<MeshRenderer>().material = PointMaterial;
                p.AddComponent<BoxCollider>().isTrigger = true;
                p.AddComponent<Facility>().SetFac(fac);
                //p.AddComponent<HelloWorld>();
                p.tag = "Location";
                p.name = "Facility SLIC: " + fac.SLIC;
                points.Add(p);
            }
            //else
            //{
            //    //Debug.Log("adding facility" + fac.SLIC);
            //    //buildings[fac.pos].Add(fac);
            //}
        }
    }

    public void CreateCoordinates(SeriesLocData[] allSeries)
    {
        SeriesLocData seriesData = allSeries[0];
        for (int j = 0; j < seriesData.Data.Length; j += 3)
        {
            string OSLIC = seriesData.Data[j];
            float latitude = float.Parse(seriesData.Data[j + 1]);
            float longitude = float.Parse(seriesData.Data[j + 2]);
            Vector3 pos = LatLongToVector3(latitude, longitude);
            //Debug.Log(fac.SLIC);
            coordinates[OSLIC] = pos;
            if (buildings.ContainsKey(pos))
            {
                //Debug.Log(OSLIC + " == " + buildings[pos]);
                samebuilding[OSLIC] = buildings[pos];
            }
        }
        
    }

    public void CreateArcs(SeriesLocData[] allSeries)
    {
        SeriesLocData seriesData = allSeries[0];
        ArcDrawer Drawer = GetComponent<ArcDrawer>();
        List<ArcData> arcData = new List<ArcData>();
        //Debug.Log(seriesData.Data.Length / 3500);
        for (int j = 0; (j < seriesData.Data.Length) & (j < 7 * ArcLimit); j += 7)
        {
            string OSLIC = seriesData.Data[j];
            float latstart = float.Parse(seriesData.Data[j + 1]);
            float lngstart = float.Parse(seriesData.Data[j + 2]);
            string DSLIC = seriesData.Data[j + 3].ToString();
            float latend = float.Parse(seriesData.Data[j + 4]);
            float lngend = float.Parse(seriesData.Data[j + 5]);
            float volume = float.Parse(seriesData.Data[j + 6]);
            //Debug.Log(OSLIC);
            //Debug.Log(latstart + "," + lngstart + "," + latend + "," + lngend + "," + volume);
            //Debug.Log(OSLIC);
            Vector3 startpos = LatLongToVector3(latstart, lngstart);
            Vector3 endpos = LatLongToVector3(latend, lngend);
            string ostate = " WA", dstate = " WA";
            //Debug.LogFormat("OSLIC:{0},DSLIC:{1},OSTATE:{2}", OSLIC, DSLIC, states[OSLIC]);
            if (states.ContainsKey(OSLIC))
            {
                ostate = states[OSLIC];
            }
            if (states.ContainsKey(DSLIC))
            {
                dstate = states[DSLIC];
            }
            arcData.Add(new ArcData(startpos, endpos, volume, ostate, dstate, OSLIC, DSLIC));
        }
        Drawer.CreateArcs(arcData);
    }

    public void CreatePath(SeriesLocData[] allSeries)
    {
        string OSLIC;
        List<string> passed;
        string od;

        //get each path
        for (int i = 0; i < allSeries.Length; i++)
        {
            string[] pathData = allSeries[i].Data;
            int j = 3;
            passed = new List<string>();
            if (samebuilding.ContainsKey(pathData[2]))
            {
                OSLIC = samebuilding[pathData[2]];
                passed.Add(OSLIC);
            }
            while (j < pathData.Length)
            {
                if (pathData[j] == pathData[j - 1] || !samebuilding.ContainsKey(pathData[j]))
                {
                    j += 1;
                    continue;
                }
                passed.Add(samebuilding[pathData[j]]);
                //Debug.Log(pathData[j] + " == " + samebuilding[pathData[j]]);
                j += 1;
            }
            if (passed.Count < 2)
            {
                continue;
            }
            od = passed[0] + " - " + passed[passed.Count - 1];
            path[od] = passed;
            //if (i == 1)
            //{
            //    foreach (string a in passed)
            //    {
            //        Debug.Log(a);
            //    }
            //}
        }
        //Debug.Log("finish creatin path");
    }

    public void DrawPath(string oslic, string dslic)
    {
        //many locations do not have a path so it is hard to test
        //random generate path
        string p = oslic + " - " + dslic;
        //Debug.Log("drawing " + p);
        List<string> nodes = new List<string>();
        List<Vector3> origins = new List<Vector3>();
        List<Vector3> dests = new List<Vector3>();
        if (path.ContainsKey(p))
        {
            List<string> passed = path[p];
            //Debug.Log(passed.Count);

            for (int i = 0; i < passed.Count; i++)
            {
                if (coordinates.ContainsKey(passed[i]))
                {
                    nodes.Add(passed[i]);
                }
            }
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                origins.Add(coordinates[nodes[i]]);
                dests.Add(coordinates[nodes[i + 1]]);
                //Debug.Log("from " + nodes[i] + " to " + nodes[i + 1]);
            }
        }
        GetComponent<ArcDrawer>().DrawPath(origins, dests);
    }
    
    private FacData GetFacility(string SLIC, float latstart, float lngstart, string CC = null, string SORT = null, string CAP = null, string SPAN = null, string START = null, string STATE = null)
    {
        Vector3 pos = LatLongToVector3(latstart, lngstart);
        float inverselat, inverselong;
        inverselat = Mathf.Asin(pos.y) * Mathf.Rad2Deg;
        inverselong = -Mathf.Acos(pos.x / Mathf.Cos(latstart * Mathf.Deg2Rad)) * Mathf.Rad2Deg;
        //Debug.Log(SLIC + ", lat original: " + latstart + ", reversed: " + inverselat);
        //Debug.Log(SLIC + ", long original: " + lngstart + ", reversed: " + inverselong);
        return new FacData(pos, SLIC, CC, SORT, CAP, SPAN, START, STATE);
    }

    // convert latitude and longitude coordinates into 3D space as a Vector3
    private static Vector3 LatLongToVector3(float latitude, float longitude)
    {
        Vector3 pos;
        pos.x = Mathf.Cos((longitude) * Mathf.Deg2Rad) * Mathf.Cos(latitude * Mathf.Deg2Rad);
        pos.y = Mathf.Sin(latitude * Mathf.Deg2Rad);
        pos.z = Mathf.Sin((longitude) * Mathf.Deg2Rad) * Mathf.Cos(latitude * Mathf.Deg2Rad);
        pos.Normalize();
        pos *= 0.5f;
        return pos;
    }
}

[System.Serializable]
public class SeriesLocData
{
    public string Name;
    public string[] Data;
}

[System.Serializable]
public class SeriesData
{
    public string Name;
    public float[] Data;
}
