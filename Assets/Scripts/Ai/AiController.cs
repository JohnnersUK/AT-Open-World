﻿
using UnityEngine;
using UnityEngine.AI;


public class AiController : MonoBehaviour
{
    enum States
    {
        Idle = 0,
        Aggro
    }
    private States state = States.Idle;

    public bool grounded;

    public float WalkRadius;
    public float aggroRange = 10;
    public float attackRange = 2;

    public float jumpForce;

    public AIPattern pattern;
    public bool hasPattern = false;

    private float count = 5;

    private Vector3 target;
    private NavMeshAgent agent;
    private Transform player;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        target = transform.position;

        if (hasPattern == false)
        {
            switch (Random.Range(0, 2))
            {
                default:
                case 0:
                    pattern = new RunnerPattern(gameObject, player, target, agent);
                    GetComponent<MeshRenderer>().material.color = new Color(0, 1, 0);
                    break;
                case 1:
                    pattern = new AggroPattern(gameObject, player, target, agent);
                    GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckState();
        switch (state)
        {
            case States.Idle:
                {
                    pattern.UpdateIdle();
                    break;
                }
            case States.Aggro:
                {
                    pattern.UpdateAggro();
                    break;
                }
        }

        if (grounded)
        {
            agent.enabled = true;
        }

        grounded = false;
    }

    void CheckState()
    {
        if (Vector3.Distance(transform.position, player.position) < aggroRange)
        {
            state = States.Aggro;
        }
        else
        {
            state = States.Idle;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        grounded = true;
    }

    private void OnDestroy()
    {
        AIManager.Instance.RemoveAgent(gameObject);
    }

    public void Caught()
    {
        pattern.Caught();
    }
}
