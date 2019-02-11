using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPattern
{
    public GameObject me;
    public Transform player;
    public Vector3 target;
    public NavMeshAgent agent;

    public AIPattern()
    {

    }

    public virtual void UpdateIdle() 
    {

    }

    public virtual void UpdateAggro()
    {

    }


}
