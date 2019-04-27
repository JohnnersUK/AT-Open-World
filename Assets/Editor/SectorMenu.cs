using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class SectorMenu : EditorWindow
{
    public string sectorFile = "";
    StreamingScript currentSector;
    public ObjectList ObjectsList = new ObjectList();

    [MenuItem("Sectors/Sector Menu")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        SectorMenu window = (SectorMenu)GetWindow(typeof(SectorMenu));

        window.minSize = new Vector2(300f, 100f);
        window.maxSize = new Vector2(300f, 100f);

        window.autoRepaintOnSceneChange = true;
        window.title = "Sector Menu";
        window.Show();
    }

    void OnGUI()
    {
        string title;

        if (Selection.activeGameObject != null)
            currentSector = Selection.activeGameObject.GetComponent(typeof(StreamingScript)) as StreamingScript;

        // Check if an object is selected
        if (Selection.activeGameObject == null)
        {
            title = "null";
        }
        // Check if the object selected is a sector
        else if (currentSector == null)
        {
            title = "null";
        }
        else
        {
            title = currentSector.name;
        }

        GUILayout.Label("Current sector: " + title, EditorStyles.boldLabel);

        if (GUILayout.Button("Load Sector"))
        {
            Load();
        }

        if (GUILayout.Button("Save Sector"))
        {
            Save();
        }
    }

    void Load()
    {
        // Check if an object is selected
        if (Selection.activeGameObject == null)
        {
            Debug.LogError("No sector was selected");
            return;
        }

        currentSector = Selection.activeGameObject.GetComponent(typeof(StreamingScript)) as StreamingScript;

        // Check if the object selected is a sector
        if (currentSector == null)
        {
            Debug.LogError("Current selection is not a sector");
            return;
        }

        UnityEngine.Object tempResource;
        GameObject tempObject;

        string path = Path.Combine(Application.streamingAssetsPath, sectorFile + ".json");

        // Load the terrain
        TerrainData tdata = (TerrainData)Resources.Load("t_" + this.name);
        GameObject tempT = Terrain.CreateTerrainGameObject(tdata);

        tempT.transform.parent = currentSector.transform;
        tempT.transform.localPosition = new Vector3(-125, -10, -125);

        // If an object file exists, Load it
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            ObjectsList = JsonUtility.FromJson<ObjectList>(data);
            foreach (Object o in ObjectsList.List)
            {
                if (o.Type == "Object")
                {
                    tempResource = Resources.Load(o.PrefabName);
                    if (tempResource == null)
                    {
                        Debug.LogError("Resource not found: " + o.PrefabName);
                    }
                    else
                    {
                        tempObject = (GameObject)Instantiate(tempResource, o.Position, Quaternion.Euler(o.Rotation), currentSector.transform);
                        tempObject.transform.localScale = o.Scale;
                    }
                }
            }
        }
        else
        {
            Debug.Log("No list found");
        }
    }

    void Save()
    {
        int i = 0;

        StreamingScript currentSector;

        // Check if an object is selected
        if (Selection.activeGameObject == null)
        {
            Debug.LogError("No sector was selected");
            return;
        }

        currentSector = Selection.activeGameObject.GetComponent(typeof(StreamingScript)) as StreamingScript;

        // Check if the object selected is a sector
        if (currentSector == null)
        {
            Debug.LogError("Current selection is not a sector");
            return;
        }

        ObjectList rawData = new ObjectList();
        Object tempObject;

        string dataString;
        string path = Path.Combine(Application.streamingAssetsPath, (currentSector.name + ".json"));

        if (File.Exists(path))
        {
            foreach (Transform child in currentSector.transform)
            {
                EditorUtility.DisplayProgressBar("Save Sector", "Process " + i, (float)i / currentSector.transform.childCount);
                if (child.GetComponent<ResourceName>())
                {
                    tempObject = new Object();
                    tempObject.Type = "Object";
                    tempObject.PrefabName = child.GetComponent<ResourceName>().globalName;
                    tempObject.Position = child.position;
                    tempObject.Rotation = child.rotation.eulerAngles;
                    tempObject.Scale = child.localScale;

                    rawData.List.Add(tempObject);                    
                }

                i++;
            }

            EditorUtility.ClearProgressBar();

            dataString = JsonUtility.ToJson(rawData);

            File.WriteAllText(path, dataString);

            Debug.Log("Sector saved to " + currentSector.name + ".json");
        }

        return;
    }

}
