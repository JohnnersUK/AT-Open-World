using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.AI;

public class StreamingScript : MonoBehaviour
{
    public ObjectList ObjectsList = new ObjectList();
    public Transform Player;
    public float UnloadDistance;

    private string JsonDataPath;
    private bool Loaded;
    private Vector3 LastPos;

    // Observer pattern
    public delegate void LevelLoadedEventHandler(object source, EventArgs args);
    public event LevelLoadedEventHandler LevelLoaded;

    public delegate void LevelUnloadedEventHandler(object source, EventArgs args);
    public event LevelUnloadedEventHandler LevelUnloaded;

    // Use this for initialization
    void Start()
    {
        Loaded = false;
        JsonDataPath = this.name + ".json";
        LoadGameData();
        LastPos = Player.position;
    }

    // Update is called once per frame
    void Update()
    {
        // If the player has moved
        if (Player.position != LastPos)
        {
            Vector3 PlayerTwoD = new Vector3(Player.position.x, 0, Player.position.z);
            Vector3 ThisTwoD = new Vector3(transform.position.x, 0, transform.position.z);

            if (Loaded)
            {
                if (Vector3.Distance(PlayerTwoD, ThisTwoD) > UnloadDistance)
                {
                    UnloadGameData();
                }
            }
            else
            {
                if (Vector3.Distance(PlayerTwoD, ThisTwoD) <= UnloadDistance)
                {
                    LoadGameData();
                }
            }
        }
        LastPos = Player.position;
    }


    // Loads data from a JSON file
    void LoadGameData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, JsonDataPath);

        // Load the terrain
        TerrainData tdata = (TerrainData)Resources.Load("t_" + this.name);
        GameObject tempT = Terrain.CreateTerrainGameObject(tdata);
        tempT.transform.parent = transform;
        tempT.transform.localPosition = new Vector3(-50, -10, -50);

        // If an object file exists, Load it
        if (File.Exists(path))
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
            }
        }
        else
        {
            Debug.Log("No list found");
        }

        Loaded = true;
        OnLevelLoaded();
    }

    protected virtual void OnLevelLoaded()
    {
        if (LevelLoaded != null)
        {
            LevelLoaded(this, EventArgs.Empty);
        }
    }

    void UnloadGameData()
    { 

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Loaded = false;
        OnLevelUnloaded();
    }

    protected virtual void OnLevelUnloaded()
    {
        if (LevelUnloaded != null)
        {
            LevelUnloaded(this, EventArgs.Empty);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        other.transform.parent = this.transform;
    }
}
