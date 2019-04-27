﻿using System;
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
    public event LevelLoadedEventHandler SectorLoaded;

    public delegate void LevelUnloadedEventHandler(object source, EventArgs args);
    public event LevelUnloadedEventHandler SectorUnloaded;

    // Use this for initialization
    void Start()
    {
        Loaded = false;
        JsonDataPath = this.name + ".json";
        LastPos = new Vector3(-1,-1,-1);

        // Clear any unsaved data
        UnloadGameData();
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
        UnityEngine.Object tempResource;
        GameObject tempObject;

        string path = Path.Combine(Application.streamingAssetsPath, JsonDataPath);

        // Load the terrain
        TerrainData tdata = (TerrainData)Resources.Load("t_" + this.name);
        GameObject tempT = Terrain.CreateTerrainGameObject(tdata);

        tempT.transform.parent = transform;
        tempT.transform.localPosition = new Vector3(-125, -10, -125);

        // If an object file exists, Load it
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            ObjectsList = JsonUtility.FromJson<ObjectList>(data);
            foreach(Object o in ObjectsList.List)
            { 
                if (o.Type == "Object")
                {
                    tempResource = Resources.Load(o.PrefabName);
                    if(tempResource == null)
                    {
                        Debug.LogError("Resource not found: " + o.PrefabName);
                    }
                    else
                    {
                        tempObject = (GameObject)Instantiate(tempResource, o.Position, Quaternion.Euler(o.Rotation), transform);
                        tempObject.transform.localScale = o.Scale;
                    }
                }
            }
        }
        else
        {
            Debug.Log("No list found");
        }

        Loaded = true;
        OnSectorLoaded();
    }

    void UnloadGameData()
    {
        OnSectorUnloaded();

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Loaded = false;
    }

    protected virtual void OnSectorLoaded()
    {
        if (SectorLoaded != null)
        {
            SectorLoaded(this, EventArgs.Empty);
        }
    }

    protected virtual void OnSectorUnloaded()
    {
        if (SectorUnloaded != null)
        {
            SectorUnloaded(this, EventArgs.Empty);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        other.transform.parent = this.transform;
    }
}
