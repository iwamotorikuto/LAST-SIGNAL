using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class ZombiAI : MonoBehaviour
{
    // =========================
    // エネミーの状態
    // =========================
    public enum ZombiState
    {
        Idle,
        Walk,
        Attack,
        AttackWait,
        Dead
    }


    [Header("移動設定")]
    [SerializeField] private float chaseRange = 10.0f;
    [SerializeField] private float attackRange = 2.0f;
    [SerializeField] private float moveSpeed = 1.5f;

    // =========================
    // 攻撃設定
    // =========================
    [Header("攻撃設定")]
    [SerializeField] private float attackCooldown = 2.0f;
    [SerializeField] private float attackWaitTime = 0.5f;

    private float attackTimer = 0.0f;
    private float attackWaitTimer = 0.0f;

    // =========================
    // 参照
    // =========================
    [Header("参照")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private ZombiStates states;

    // =========================
    // 現在の状態
    // =========================
    private ZombiState currentState;

    // =========================
    // Animator State
    // 0 = Idle
    // 1 = Walk
    // 2 = Attack
    // 3 = Death
    // =========================
    private const int ANIM_IDLE = 0;
    private const int ANIM_WALK = 1;
    private const int ANIM_ATTACK = 2;
    private const int ANIM_DEATH = 3;

    // =========================
    // Start
    // =========================
    void Start()
    {


        // 最初はIdle
        animator.SetInteger("State", 0);
        animator.CrossFade("Idle", 0.0f, 0);
    }

    // =========================
    // Update
    // =========================
    void Update()
    {
        // 死亡中は処理しない
        if (currentState == ZombiState.Dead)
            return;


        if (player == null)
            return;

        // =========================
        // 攻撃クールタイム
        // =========================
        if (attackTimer > 0.0f)
        {
            attackTimer -= Time.deltaTime;
        }


        switch (currentState)
        {
            case ZombiState.Idle:
                IdleUpdate();
                break;

            case ZombiState.Walk:
                WalkUpdate();
                break;

            case ZombiState.Attack:
                AttackUpdate();
                break;

            case ZombiState.AttackWait:
                AttackWaitUpdate();
                break;
        }
    }

    // =========================
    // 待機
    // =========================
    void IdleUpdate()
    {
        // 確実に停止
        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
        }

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );


        if (distance <= chaseRange)
        {
            ChangeState(ZombiState.Walk);
        }
    }

    // =========================
    // 追従
    // =========================
    void WalkUpdate()
    {
        float distance = Vector3.Distance(
            transform.position,
            player.position
        );


        if (distance > chaseRange)
        {
            ChangeState(ZombiState.Idle);
            return;
        }


        {
            ChangeState(ZombiState.Attack);
            return;
        }

        // =========================
        // プレイヤーを追従
        // =========================
        if (agent != null && agent.enabled)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    // =========================
    // 攻撃
    // =========================
    void AttackUpdate()
    {
        // 移動停止
        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
        }

        // =========================
        // プレイヤーの方向を向く
        // =========================
        Vector3 direction =
            player.position - transform.position;

        direction.y = 0.0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation =
                Quaternion.LookRotation(direction);
        }


        if (stateInfo.IsName("Attack") &&
            stateInfo.normalizedTime >= 1.0f &&
            !animator.IsInTransition(0))
        {
            // 次の攻撃までのクールタイム
            attackTimer = attackCooldown;


        }

        // =========================
        // 追跡範囲内
        // =========================
        ChangeState(ZombiState.Walk);
    }

    // =========================
    // ステート切り替え
    // =========================
    void ChangeState(ZombiState newState)
    {
        // 死亡後は状態変更しない
        if (currentState == ZombiState.Dead)
            return;

        if (currentState == newState)
            return;

        currentState = newState;

        switch (currentState)
        {
            // =========================


                if (agent != null && agent.enabled)
                {
                    agent.isStopped = true;
                }


                break;

            // =========================
            // 死亡
            // =========================
            case ZombiState.Dead:

                Die();

                break;
        }
    }

    // =========================
    // AnimatorのStateを変更
    // =========================
    void SetAnimatorState(int state)
    {
        if (animator == null)
            return;

        animator.SetInteger("State", state);
    }

    // =========================
    // 死亡処理
    // =========================
    void Die()
    {
        // =========================
        // NavMeshAgentを停止
        // =========================
        if (agent != null && agent.enabled)
        {
            // 必ずenabledをfalseにする前に停止
            agent.isStopped = true;

            // NavMeshAgentを無効化
            agent.enabled = false;
        }

        // =========================
        // Deathアニメーション
        // =========================
        SetAnimatorState(ANIM_DEATH);

        // =========================
        // Colliderを無効化
        // =========================
        Collider col = GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = false;
        }
    }

    // =========================

    // 状態取得
    // =========================
    public ZombiState GetCurrentState()
    {
        return currentState;
    }
}
