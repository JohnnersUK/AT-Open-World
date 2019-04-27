using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIManager : MonoBehaviour
{
    public GameObject AIPrefab;

    public static AIManager Instance;

    private StreamingScript[] ss;
    private List<GameObject> agents;

    private float count = 5;

    // Use this for initialization
    void Start()
    {
        // Destroy all other managers and set the instance
        AIManager[] tempManagers = FindObjectsOfType<AIManager>();
        foreach(AIManager manager in tempManagers)
        {
            if(manager != this)
            {
                Destroy(manager);
            }
        }
        Instance = this;

        agents = new List<GameObject>();
        ss = FindObjectsOfType<StreamingScript>();

        foreach (StreamingScript s in ss)
        {
            s.SectorLoaded += OnSectorLoaded;
            s.SectorUnloaded += OnSectorUnloaded;
        }
    }

    // Update is called once per frame
    void Update()
    {
        count -= Time.deltaTime;

        // Update the parent of the agents
        if (count <= 0 && agents.Count > 0)
        {
            foreach (GameObject a in agents)
            {
                if (a != null)
                {
                    int posZ = Mathf.Clamp(Mathf.FloorToInt(a.transform.position.z / 250), 0, 3);
                    int posX = Mathf.Clamp(Mathf.FloorToInt(a.transform.position.x / 250), 0, 3);

                    string name = "Sector[" + posX + posZ + "]";

                    foreach (StreamingScript s in ss)
                    {
                        if (s.name == name)
                        {
                            a.transform.parent = s.transform;
                            break;
                        }
                    }
                }
                else
                {
                    agents.Remove(a);
                }
            }
            count = 5;
        }
    }

    private void OnSectorLoaded(object source, EventArgs args)
    {
        StreamingScript str = source as StreamingScript;

        // Generate a random number of agents
        int x = UnityEngine.Random.Range(0, 50);
        for (int i = 0; i < x; i++)
        {
            // Randomize a location
            float newX = UnityEngine.Random.Range(-125, 125) + str.transform.position.x;
            float newZ = UnityEngine.Random.Range(-125, 125) + str.transform.position.z;
            Vector3 targetPos = new Vector3(newX, 0, newZ);

            // Raycast up/down to find the ground level, move agent to this point
            RaycastHit hit;
            if (Physics.Raycast(targetPos, transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity))
            {
                targetPos = hit.point;

            }
            else if (Physics.Raycast(targetPos, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
            {

                targetPos = hit.point;
            }

            // Instantiate the bot and add it to the list of agents
            GameObject tempA = Instantiate(AIPrefab, targetPos, new Quaternion(0, 0, 0, 0), str.transform);
            agents.Add(tempA);
        }
    }

    private void OnSectorUnloaded(object source, EventArgs args)
    {
        StreamingScript tempStreamingScript = source as StreamingScript;
        GameObject tempSector = tempStreamingScript.gameObject;
        GameObject tempChild;

        int numOfChildren = tempSector.transform.childCount - 1;

        for (int i = numOfChildren; i > -1; i--)
        {
            tempChild = tempSector.transform.GetChild(i).gameObject;

            if (agents.Contains(tempChild))
            {
                agents.Remove(tempChild);
                Destroy(tempChild);
            }
        }
    }

    public void RemoveAgent(GameObject a)
    {
        if (agents.Contains(a))
        {
            agents.Remove(a);
        }
    }
}
