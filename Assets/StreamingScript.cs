using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class StreamingScript : MonoBehaviour
{
    private string JsonDataPath;
    public ObjectList ObjectsList = new ObjectList();

    // Use this for initialization
    void Start()
    {
        JsonDataPath = this.name + ".json";
        StartCoroutine(Example());
        SaveGameData();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator<WaitForSeconds> Example()
    {
        yield return new WaitForSeconds(5);
        LoadGameData();
    }

    void SaveGameData()
    {
        
    }

    // Loads data from a JSON file
    void LoadGameData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, JsonDataPath);
        if(File.Exists(path))
        {
            string data = File.ReadAllText(path);
            ObjectsList = JsonUtility.FromJson<ObjectList>(data);
            foreach(Object o in ObjectsList.Object)
            { 
                if (o.Type == "Object")
                {
                    GameObject temp = (GameObject)Instantiate(Resources.Load(o.PrefabName), o.Position, Quaternion.Euler(o.Rotation), transform);
                    temp.transform.parent = transform;
                    temp.transform.localPosition = o.Position;
                    temp.transform.localRotation = Quaternion.Euler(o.Rotation);
                    temp.transform.localScale = o.Scale;
                }
                else
                {
                    TerrainData tdata = (TerrainData)Resources.Load(o.PrefabName);
                    GameObject temp = Terrain.CreateTerrainGameObject(tdata);
                    temp.transform.parent = transform;
                    temp.transform.localPosition = o.Position;
                    temp.transform.localRotation = Quaternion.Euler(o.Rotation);
                    temp.transform.localScale = o.Scale;
                }

            }
        }
        else
        {
            Debug.Log("No list found");
        }
    }
}
