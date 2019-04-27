using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AggroPattern : AIPattern
{
    public AggroPattern(GameObject m, Transform p, Vector3 t, NavMeshAgent n) : base(m, p, t, n)
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

    public override void Caught()
    {
        GameManager.Instance.PlayerHealth -= 1;
    }

}
