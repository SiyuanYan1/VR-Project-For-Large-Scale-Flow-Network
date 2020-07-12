using System.Collections.Generic;
using UnityEngine;

namespace Arcs
{
    public class ArcDrawer : MonoBehaviour
    {
        [Tooltip("How many segments each Arc will have")]
        public int ArcSegmentCount = 100;

        [Tooltip("The maximum height for Arcs")]
        public float ArcMaxHeight = 0.7f;

        // storage for arcs and paths
        private List<GameObject> arcs = new List<GameObject>();
        private List<GameObject> path = new List<GameObject>();
        private Dictionary<string, List<GameObject>> origin_dict = new Dictionary<string, List<GameObject>>();
        private Dictionary<string, List<GameObject>> dest_dict = new Dictionary<string, List<GameObject>>();

        // whether a path (instead of arcs) is currently being shown
        private bool showing_path = false;

        // where the Paths will be stored (as a child of original object)
        private Transform path_folder;

        // create Arc objects with data from DataVisualiser
        public void CreateArcs(List<ArcData> arcData)
        {
            // A simple 2 color gradient with a fixed alpha of 1.0f.
            float alpha = 1.0f;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.red, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );

            Arc.ArcGradient = gradient;
            Arc.SegmentCount = ArcSegmentCount;
            Arc.MaxHeight = ArcMaxHeight;

            Transform folder = new GameObject("Arcs").transform;
            folder.SetParent(transform, false);

            for (int arc = 0; arc < arcData.Count; arc += 1)
            {
                ArcData data = arcData[arc];
                GameObject new_arc = new GameObject();
                new_arc.SetActive(false);
                new_arc.transform.SetParent(folder, false);

                //string tag_name = string.Format("{0},{1},{2}", data.startpos, data.endpos, data.volume);
                new_arc.name = data.OSLIC + " to " + data.DSLIC + " (" + data.OriginState + " to " + data.DestState + ")";
                new_arc.tag = "Arc";

                //add data to the object
                var arc_component = new_arc.AddComponent<Arc>();
                arc_component.SetArcData(arcData[arc]);

                // remember reference
                arcs.Add(new_arc);

                if (!origin_dict.ContainsKey(data.OriginState))
                {
                    //Debug.Log("New ostate:" + data.OriginState);
                    origin_dict[data.OriginState] = new List<GameObject>();
                }
                origin_dict[data.OriginState].Add(new_arc);

                if (!dest_dict.ContainsKey(data.DestState))
                {
                    //Debug.Log("New dstate:" + data.DestState);
                    dest_dict[data.DestState] = new List<GameObject>();
                }
                dest_dict[data.DestState].Add(new_arc);
            }

            UpdateArcs();
        }

        // go through Arcs and toggle relevant flags
        public void ToggleState(string state_name)
        {
            if (origin_dict.ContainsKey(state_name))
            {
                foreach (GameObject arc_obj in origin_dict[state_name])
                {
                    Arc arc = arc_obj.GetComponent<Arc>();
                    arc.o_enabled = !arc.o_enabled;
                }
            }

            if (dest_dict.ContainsKey(state_name))
            {
                foreach (GameObject arc_obj in dest_dict[state_name])
                {
                    Arc arc = arc_obj.GetComponent<Arc>();
                    arc.d_enabled = !arc.d_enabled;
                }
            }
            showing_path = false;
            UpdateArcs();
        }

        // enable or disable Arcs, update the static max height
        public void UpdateArcs()
        {
            if (!showing_path)
            {
                var activated_arcs = new List<Arc>();
                float max_volume = 0f;
                foreach (GameObject arc_obj in arcs)
                {
                    Arc arc = arc_obj.GetComponent<Arc>();
                    if (arc.IsEnabled())
                    {
                        arc_obj.SetActive(true);
                        max_volume = max_volume < arc.GetArcData().volume ? arc.GetArcData().volume : max_volume;
                        activated_arcs.Add(arc);
                    }
                    else
                    {
                        arc_obj.SetActive(false);
                    }
                }
                Arc.MaxVolume = max_volume;
                foreach (Arc arc in activated_arcs)
                {
                    arc.UpdateDrawing();
                }
            }
        }

        // outdated, does not really work
        public float GetArcVolume(string oSLIC, string dSLIC)
        {
            //Debug.Log(oSLIC + " " + dSLIC);
            foreach (GameObject arc in arcs)
            {
                var ai = arc.GetComponent<Arc>().GetArcData();
                //if (i++ > 3000 & i < 3050)
                //{
                //    Debug.Log(ai.OSLIC + " " + ai.DSLIC);
                //}
                if ((ai.OSLIC == oSLIC) & (ai.DSLIC == dSLIC))
                {
                    Debug.Log("Found arc");
                    return ai.volume;
                }
                //else
                //{
                //    Debug.Log("Failure");
                //}
            }
            return 0f;
        }

        public void DisableArcs()
        {
            foreach (GameObject arc in arcs)
            {
                arc.SetActive(false);
            }
        }

        // draw a series of arcs representing a path with data from DataVisualiser
        public void DrawPath(List<Vector3> origins, List<Vector3> dests)
        {
            if (!path_folder)
            {
                path_folder = new GameObject("Paths").transform;
                path_folder.SetParent(transform, false);
            }
            ClearPath();
            DisableArcs();
            showing_path = true;
            for (int i = 0; i < origins.Count; i++)
            {
                Vector3 origin = origins[i];
                Vector3 dest = dests[i];
                GameObject new_arc = new GameObject();
                path.Add(new_arc);

                new_arc.transform.SetParent(path_folder, false);

                Arc arc_component = new_arc.AddComponent<Arc>();
                arc_component.SetArcData(new ArcData(origin, dest));
                //arc_component.SetGradient(gradient);

                new_arc.name = "Path Arc " + i.ToString();
                new_arc.tag = "arcPath";

                arc_component.Draw(0.55f);
            }
        }

        // destroy the path objects
        public void ClearPath()
        {
            for (int i = 0; i < path.Count; i++)
            {

                Destroy(path[i]);
            }
            path.Clear();
            //Debug.Log(path.Count);
        }
    }
}
