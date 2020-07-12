using System.IO;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
    [Tooltip("Where to send loaded data")]
    public DataVisualizer Visualizer;

    // Use this for initialization
    void Start()
    {
        // load all the data
        // the data is loaded from the StreamingAssets folder so they can be directly accessed after building


        //TextAsset stateData = Resources.Load<TextAsset>("fullstate");
        //string stateJson = stateData.text;
        string stateJson = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "fullstate.json"));
        SeriesLocArray state = JsonUtility.FromJson<SeriesLocArray>(stateJson);
        Visualizer.InitialiseStates(state.AllData);

        //TextAsset facData = Resources.Load<TextAsset>("fac_state");
        //string facJson = facData.text;
        string facJson = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "fac_state.json"));
        SeriesLocArray fac = JsonUtility.FromJson<SeriesLocArray>(facJson);
        Visualizer.CreateFacilities(fac.AllData);

        //TextAsset coorData = Resources.Load<TextAsset>("coordinates_data");
        //string coorJson = coorData.text;
        string coorJson = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "coordinates_data.json"));
        SeriesLocArray coordinates = JsonUtility.FromJson<SeriesLocArray>(coorJson);
        Visualizer.CreateCoordinates(coordinates.AllData);

        //TextAsset pathData = Resources.Load<TextAsset>("path_data");
        //string pathJson = pathData.text;
        string pathJson = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "path_data.json"));
        SeriesLocArray path = JsonUtility.FromJson<SeriesLocArray>(pathJson);
        Visualizer.CreatePath(path.AllData);

        //TextAsset arcData = Resources.Load<TextAsset>("vol_data");
        //string arcJson = arcData.text;
        string arcJson = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "vol_data.json"));
        SeriesLocArray arc = JsonUtility.FromJson<SeriesLocArray>(arcJson);
        Visualizer.CreateArcs(arc.AllData);


    }
}

[System.Serializable]
public class SeriesLocArray
{
    public SeriesLocData[] AllData;
}

//[System.Serializable]
//public class SeriesArray
//{
//    public SeriesData[] AllData;

//}