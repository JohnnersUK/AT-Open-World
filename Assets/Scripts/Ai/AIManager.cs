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
                    a.GetComponentInChildren<Text>().text = name;
                    Debug.Log(name);

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
        int x = UnityEngine.Random.Range(0, 10);
        for (int i = 0; i < x; i++)
        {
            GameObject tempA = (GameObject)Instantiate(AIPrefab, str.transform);

            float newX = UnityEngine.Random.Range(-50, 50) + str.transform.position.x;
            float newZ = UnityEngine.Random.Range(-50, 50) + str.transform.position.z;

            tempA.GetComponent<NavMeshAgent>().Warp(new Vector3(newX, 0, newZ));

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
