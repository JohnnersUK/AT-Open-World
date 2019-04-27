using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunnerPattern : AIPattern
{

    private float count = 0;

    public RunnerPattern(GameObject m, Transform p, Vector3 t, NavMeshAgent n) : base(m, p, t, n)
    {
        me = m;
        ctrl = me.GetComponent<AiController>();

        player = p;
        target = t;
        agent = n;
    }

    public override void UpdateIdle()
    {
        // If agent isn't moving, count down
        float dist = agent.remainingDistance;
        if (dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
        {
            count -= Time.deltaTime;
        }

        // If the count down is over, get a new position to go to
        if (count <= 0)
        {
            Vector3 point = RandomNavmeshLocation();
            agent.SetDestination(point);

            count = Random.Range(0, 10);
        }
    }

    public override void UpdateAggro()
    {
        Vector3 targetPos = player.transform.position - me.transform.position;
        agent.SetDestination(me.transform.position - targetPos);
    }

    public override void Caught()
    {
        GameManager.Instance.PlayerScore += 10;
    }
}
