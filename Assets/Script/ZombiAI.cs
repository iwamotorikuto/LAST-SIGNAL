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

    // =========================
    // HP設定
    // =========================
    [Header("HP設定")]
    [SerializeField] private int maxHP = 100;

    private int currentHP;

    // =========================
    // 移動設定
    // =========================
    [Header("移動設定")]
    [SerializeField] private float chaseRange = 10.0f;
    [SerializeField] private float attackRange = 2.0f;

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
        // HPを最大値にする
        currentHP = maxHP;

        // 初期状態
        ChangeState(ZombiState.Idle);
    }

    // =========================
    // Update
    // =========================
    void Update()
    {
        // 死亡中は処理しない
        if (currentState == ZombiState.Dead)
            return;

        // プレイヤーが設定されていない場合
        if (player == null)
            return;

        // =========================
        // 攻撃クールタイム
        // =========================
        if (attackTimer > 0.0f)
        {
            attackTimer -= Time.deltaTime;
        }

        // =========================
        // ステートマシン
        // =========================
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

        // プレイヤーが追跡範囲に入った
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

        // =========================
        // 追跡範囲外
        // =========================
        if (distance > chaseRange)
        {
            ChangeState(ZombiState.Idle);
            return;
        }

        // =========================
        // 攻撃範囲内
        // =========================
        if (distance <= attackRange &&
            attackTimer <= 0.0f)
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

        // =========================
        // Animatorの状態取得
        // =========================
        if (animator == null)
            return;

        AnimatorStateInfo stateInfo =
            animator.GetCurrentAnimatorStateInfo(0);

        // =========================
        // 攻撃アニメーション終了判定
        // =========================
        if (stateInfo.IsName("Attack") &&
            stateInfo.normalizedTime >= 1.0f &&
            !animator.IsInTransition(0))
        {
            // 次の攻撃までのクールタイム
            attackTimer = attackCooldown;

            // 攻撃後の硬直時間
            attackWaitTimer = attackWaitTime;

            // AttackWaitへ
            ChangeState(ZombiState.AttackWait);
        }
    }

    // =========================
    // 攻撃後の硬直
    // =========================
    void AttackWaitUpdate()
    {
      
        // =========================
        // 硬直中はIdleアニメーション
        // =========================
        SetAnimatorState(ANIM_IDLE);

        // =========================
        // 硬直時間を減らす
        // =========================
        attackWaitTimer -= Time.deltaTime;

        // まだ硬直中
        if (attackWaitTimer > 0.0f)
        {
            return;
        }

        // =========================
        // プレイヤーとの距離
        // =========================
        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        // =========================
        // プレイヤーが追跡範囲外
        // =========================
        if (distance > chaseRange)
        {
            ChangeState(ZombiState.Idle);
            return;
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

        currentState = newState;

        switch (currentState)
        {
            // =========================
            // 待機
            // =========================
            case ZombiState.Idle:

                // 確実に停止
                if (agent != null && agent.enabled)
                {
                    agent.isStopped = true;
                }

                // Animator
                SetAnimatorState(ANIM_IDLE);

                break;

            // =========================
            // 追従
            // =========================
            case ZombiState.Walk:

                if (agent != null && agent.enabled)
                {
                    agent.isStopped = false;
                }

                // Animator
                SetAnimatorState(ANIM_WALK);

                break;

            // =========================
            // 攻撃
            // =========================
            case ZombiState.Attack:

                if (agent != null && agent.enabled)
                {
                    agent.isStopped = true;
                }

                // Animator
                SetAnimatorState(ANIM_ATTACK);

                break;

            // =========================
            // 攻撃後の硬直
            // =========================
            case ZombiState.AttackWait:

                if (agent != null && agent.enabled)
                {
                    agent.isStopped = true;
                }

                // AttackWait専用アニメーションは使用しない
                // Idleアニメーションにする
                SetAnimatorState(ANIM_IDLE);

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
    // ダメージ処理
    // =========================
    public void TakeDamage(int damage)
    {
        // 死亡している場合は処理しない
        if (currentState == ZombiState.Dead)
            return;

        // ダメージを受ける
        currentHP -= damage;

        // HPが0以下
        if (currentHP <= 0)
        {
            currentHP = 0;

            ChangeState(ZombiState.Dead);
        }
    }

    // =========================
    // HP取得
    // =========================
    public int GetCurrentHP()
    {
        return currentHP;
    }

    // =========================
    // 状態取得
    // =========================
    public ZombiState GetCurrentState()
    {
        return currentState;
    }
}
