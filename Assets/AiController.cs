using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiController : MonoBehaviour {

    public float WalkRadius;

    private float count = 5;

    private Vector3 target;
    private NavMeshAgent agent;

    // Use this for initialization
    void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
        target = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        float dist = agent.remainingDistance;
        if (dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
        {
            Vector3 point = RandomNavmeshLocation();
            Debug.Log(point);
            agent.SetDestination(point);
            count = 0;
        }
	}

    private Vector3 RandomNavmeshLocation()
    {
        Vector3 randomDirection = Random.insideUnitSphere * WalkRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, WalkRadius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
