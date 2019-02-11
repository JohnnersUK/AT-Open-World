using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AggroPattern : AIPattern
{
    private AiController ctrl;

    public AggroPattern(GameObject m, Transform p, Vector3 t, NavMeshAgent n)
    {
        me = m;
        ctrl = me.GetComponent<AiController>();

        player = p;
        target = t;
        agent = n;
    }

    public override void UpdateIdle()
    {
        agent.enabled = true;
        float dist = agent.remainingDistance;
        if (dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
        {
            Vector3 point = RandomNavmeshLocation();
            agent.SetDestination(point);
        }
    }

    public override void UpdateAggro()
    {
        agent.SetDestination(player.position);

        if (Vector3.Distance(me.transform.position, player.position) < ctrl.attackRange)
        {
            int i = Random.Range(0, 10);

            if (i == 5)
            {
                Attack();
            }

        }
    }

    void Attack()
    {
        if (ctrl.grounded)
        {
            agent.enabled = false;
            me.transform.LookAt(player);
            me.GetComponent<Rigidbody>().AddForce(me.transform.forward * ctrl.jumpForce + me.transform.up * ctrl.jumpForce, ForceMode.Impulse);
        }

    }

    private Vector3 RandomNavmeshLocation()
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
}
