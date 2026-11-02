using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiAI : MonoBehaviour
{
    // エネミーの状態
    public enum ZombiState
    {
        Idle,
        Chase,
        Attack,
        Dead
    }

    [Header("HP設定")]
    [SerializeField] private int maxHP = 100;
   
    private int currentHP;

    [Header("移動設定")]
    [SerializeField] private float chaseRange = 10.0f;
    [SerializeField] private float attackRange = 2.0f;

    [Header("攻撃設定")]
    [SerializeField] private float attackCooldown = 1.0f;
    private float attackTimer = 0.0f;

    [Header("参照")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;

    // 現在の状態
    private ZombiState currentState;

    void Start()
    {
        currentHP = maxHP;

        // 初期状態
        ChangeState(ZombiState.Idle);
    }

    void Update()
    {
        // 死亡中は処理しない
        if (currentState == ZombiState.Dead)
            return;

        if (player == null)
            return;

        // 攻撃クールタイム
        if (attackTimer > 0.0f)
        {
            attackTimer -= Time.deltaTime;
        }

        // ステートマシン
        switch (currentState)
        {
            case ZombiState.Idle:
                IdleUpdate();
                break;

            case ZombiState.Chase:
                ChaseUpdate();
                break;

            case ZombiState.Attack:
                AttackUpdate();
                break;
        }
    }

    // =========================
    // 待機
    // =========================
    void IdleUpdate()
    {
        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance <= chaseRange)
        {
            ChangeState(ZombiState.Chase);
        }
    }

    // =========================
    // 追従
    // =========================
    void ChaseUpdate()
    {
        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        // 追従範囲外
        if (distance > chaseRange)
        {
            ChangeState(ZombiState.Idle);
            return;
        }

        // 攻撃範囲内
        if (distance <= attackRange && attackTimer <= 0.0f)
        {
            ChangeState(ZombiState.Attack);
            return;
        }

        // プレイヤーを追従
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    // =========================
    // 攻撃
    // =========================
    void AttackUpdate()
    {
        // 移動停止
        agent.isStopped = true;

        // プレイヤーの方向を向く
        Vector3 direction = player.position - transform.position;
        direction.y = 0.0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // アニメーションの状態取得
        AnimatorStateInfo stateInfo =
            animator.GetCurrentAnimatorStateInfo(0);

        // 攻撃アニメーション終了判定
        if (stateInfo.IsName("Attack") &&
            stateInfo.normalizedTime >= 1.0f &&
            !animator.IsInTransition(0))
        {
            attackTimer = attackCooldown;

            float distance = Vector3.Distance(
                transform.position,
                player.position
            );

            // 追従範囲内なら追従
            if (distance <= chaseRange)
            {
                ChangeState(ZombiState.Chase);
            }
            else
            {
                ChangeState(ZombiState.Idle);
            }
        }
    }

    // =========================
    // ステート切り替え
    // =========================
    void ChangeState(ZombiState newState)
    {
        if (currentState == ZombiState.Dead)
            return;

        currentState = newState;

        switch (currentState)
        {
            case ZombiState.Idle:

                agent.isStopped = true;
                animator.SetBool("IsWalking", false);

                break;

            case ZombiState.Chase:

                agent.isStopped = false;
                animator.SetBool("IsWalking", true);

                break;

            case ZombiState.Attack:

                agent.isStopped = true;
                animator.SetBool("IsWalking", false);

                animator.ResetTrigger("Attack");
                animator.SetTrigger("Attack");

                break;

            case ZombiState.Dead:

                agent.isStopped = true;
                agent.enabled = false;

                animator.SetBool("IsWalking", false);
                animator.ResetTrigger("Attack");
                animator.SetTrigger("Die");

                break;
        }
    }

    // =========================
    // ダメージ処理
    // =========================
    public void TakeDamage(int damage)
    {
        if (currentState == ZombiState.Dead)
            return;

        currentHP -= damage;

        if (currentHP <= 0)
        {
            currentHP = 0;
            ChangeState(ZombiState.Dead);
        }
    }

    // HP取得
    public int GetCurrentHP()
    {
        return currentHP;
    }

    // 状態取得
    public ZombiState GetCurrentState()
    {
        return currentState;
    }
}