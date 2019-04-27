using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPattern
{
    public GameObject me;
    public AiController ctrl;
    public NavMeshAgent agent;

    public Transform player;
    public Vector3 target;


    public AIPattern(GameObject m, Transform p, Vector3 t, NavMeshAgent n)
    {
        me = m;
        ctrl = me.GetComponent<AiController>();

        player = p;
        target = t;
        agent = n;
    }

    public virtual void UpdateIdle() 
    {

    }

    public virtual void UpdateAggro()
    {

    }

    protected Vector3 RandomNavmeshLocation()
    {
        Vector3 randomDirection = Random.insideUnitSphere * ctrl.WalkRadius;
        randomDirection += me.transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, ctrl.WalkRadius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    public virtual void Caught()
    {

    }
}
