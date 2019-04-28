using System;
using System.IO;

using UnityEngine;

public class StreamingScript : MonoBehaviour
{
    public float UnloadDistance;
    public float MaxDetailDistance;
    public float QualityDropOff;

    private string JsonDataPath;
    private bool Loaded;

    public ObjectList ObjectsList = new ObjectList();
    public Transform Player;

    private Vector3 LastPos;
    private Terrain sectorTerrain;

    // Observer pattern
    public delegate void LevelLoadedEventHandler(object source, EventArgs args);
    public event LevelLoadedEventHandler SectorLoaded;

    public delegate void LevelUnloadedEventHandler(object source, EventArgs args);
    public event LevelUnloadedEventHandler SectorUnloaded;

    // Use this for initialization
    void Start()
    {
        Loaded = false;
        JsonDataPath = name + ".json";
        LastPos = new Vector3(-1, -1, -1);

        // Clear any unsaved data
        UnloadGameData();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 ThisTwoD;
        Vector3 PlayerTwoD;

        float distance = UnloadDistance + 1;

        // If the player has moved
        if (Player.position != LastPos)
        {
            PlayerTwoD = new Vector3(Player.position.x, 0, Player.position.z);
            ThisTwoD = new Vector3(transform.position.x, 0, transform.position.z);

            distance = Vector3.Distance(PlayerTwoD, ThisTwoD);

            if (Loaded)
            {
                if (distance > UnloadDistance)
                {
                    UnloadGameData();
                }
            }
            else
            {
                if (distance <= UnloadDistance)
                {
                    LoadGameData();
                }
            }

            UpdateTerrain(distance);

        }
        LastPos = Player.position;
    }

    void UpdateTerrain(float distance)
    {
        float pixelError = 0f;
        if (sectorTerrain == null)
        {
            return;
        }

        // Calculate the pixel error (Render detail) of the terrain
        pixelError = (float)Math.Tanh((distance / UnloadDistance) / QualityDropOff) * 40;
        sectorTerrain.heightmapPixelError = pixelError;

        // Toggle details
        if (distance > MaxDetailDistance)
        {
            // Base terrain settings
            sectorTerrain.castShadows = false;

            // Trees and Foliage
            sectorTerrain.drawTreesAndFoliage = false;
        }
        else
        {
            // Base terrain settings
            sectorTerrain.castShadows = true;

            // Trees and Foliage
            sectorTerrain.drawTreesAndFoliage = true;
        }
    }

    // Loads data from a JSON file
    void LoadGameData()
    {
        UnityEngine.Object tempResource;
        GameObject tempObject;

        string path = Path.Combine(Application.streamingAssetsPath, JsonDataPath);

        // Load the terrain
        TerrainData tdata = (TerrainData)Resources.Load("t_" + name);
        GameObject tempT = Terrain.CreateTerrainGameObject(tdata);

        sectorTerrain = tempT.GetComponent<Terrain>();

        tempT.transform.parent = transform;
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
        other.transform.parent = transform;
    }
}
