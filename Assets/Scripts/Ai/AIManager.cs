using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIManager : MonoBehaviour
{
    public GameObject AIPrefab;

    private StreamingScript[] ss;
    private List<GameObject> agents;

    private float count = 5;

    // Use this for initialization
    void Start()
    {
        agents = new List<GameObject>();
        ss = FindObjectsOfType<StreamingScript>();

        foreach (StreamingScript s in ss)
        {
            s.LevelLoaded += OnLevelLoaded;
            s.LevelUnloaded += OnLevelUnloaded;
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
                    int posZ = Mathf.Clamp(Mathf.FloorToInt(a.transform.position.z / 100), 0, 2);
                    int posX = Mathf.Clamp(Mathf.FloorToInt(a.transform.position.x / 100), 0, 2);

                    string name = "Sector[" + posX + posZ + "]";

                    a.transform.parent = GameObject.Find(name).transform;
                }
                else
                {
                    agents.Remove(a);
                }
            }
            count = 5;
        }
    }

    private void OnLevelLoaded(object source, EventArgs args)
    {
        StreamingScript str = source as StreamingScript;

        // Generate a random number of agents
        int x = UnityEngine.Random.Range(0, 10);
        for (int i = 0; i < x; i++)
        {
            // Instanciate the bot
            

            // Randomize a location
            float newX = UnityEngine.Random.Range(-50, 50) + str.transform.position.x;
            float newZ = UnityEngine.Random.Range(-50, 50) + str.transform.position.z;
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

            GameObject tempA = Instantiate(AIPrefab, targetPos, new Quaternion(0,0,0,0), str.transform);
            agents.Add(tempA);
        }
    }

    private void OnLevelUnloaded(object source, EventArgs args)
    {
        // Clear the old list of agents and remove all empty items
        agents.Clear();
        agents.RemoveAll(item => item == null);

        // Get all of the remaining AI
        GameObject[] tempA = GameObject.FindGameObjectsWithTag("AI");

        // Add them to the list
        foreach (GameObject a in tempA)
        {
            agents.Add(a);
        }
        
    }
}
