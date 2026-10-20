using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.AI;



[RequireComponent(typeof(NavMeshAgent))]

[RequireComponent(typeof(Animator))]

public class ZombiAI : MonoBehaviour

{

    public enum ZombiState

    {

        Idle,

        Walk,

        Attack,

        Die

    }



    [Header("ステータス設定")]

    [SerializeField] private float maxHealth = 100f;

    [SerializeField] private float walkSpeed = 1.5f;



    [Header("検知・攻撃エリア設定")]

    [SerializeField] private float detectionRange = 10f;

    [SerializeField] private float attackRange = 1.8f;



    [Header("参照")]

    [SerializeField] private Transform player;



    private float currentHealth;

    private ZombiState currentState = ZombiState.Idle;

    private NavMeshAgent agent;

    private Animator animator;

    private static readonly int StateHash = Animator.StringToHash("State");



    private void Awake()

    {

        agent = GetComponent<NavMeshAgent>();

        animator = GetComponent<Animator>();

        currentHealth = maxHealth;

    }



    private void Start()

    {

        if (player == null)

        {

            GameObject p = GameObject.FindWithTag("Player");

            if (p != null) player = p.transform;

        }

    }



    private void Update()

    {

        if (currentState == ZombiState.Die || player == null) return;



        float distanceToPlayer = Vector3.Distance(transform.position, player.position);



        switch (currentState)

        {

            case ZombiState.Idle:

                if (distanceToPlayer <= detectionRange)

                {

                    ChangeState(ZombiState.Walk);

                }

                break;



            case ZombiState.Walk:

                agent.isStopped = false;

                agent.speed = walkSpeed;

                agent.SetDestination(player.position);



                if (distanceToPlayer <= attackRange)

                {

                    ChangeState(ZombiState.Attack);

                }

                break;



            case ZombiState.Attack:

                agent.isStopped = true;

                LookAtTarget(player.position);



                if (distanceToPlayer > attackRange)

                {

                    ChangeState(ZombiState.Walk);

                }

                break;

        }

    }



    private void ChangeState(ZombiState newState)

    {

        if (currentState == newState || currentState == ZombiState.Die) return;



        currentState = newState;

        animator.SetInteger(StateHash, (int)newState);

    }



    public void TakeDamage(float damage)

    {

        if (currentState == ZombiState.Die) return;



        currentHealth -= damage;



        if (currentHealth <= 0)

        {

            Die();

        }

    }



    private void Die()

    {

        ChangeState(ZombiState.Die);

        agent.enabled = false;

        GetComponent<Collider>().enabled = false;

    }



    private void LookAtTarget(Vector3 targetPosition)

    {

        Vector3 direction = (targetPosition - transform.position).normalized;

        direction.y = 0;

        if (direction != Vector3.zero)

        {

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        }

    }

}