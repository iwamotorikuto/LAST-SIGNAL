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

    [Header("移動設定")]
    [SerializeField] private float chaseRange = 10.0f;
    [SerializeField] private float attackRange = 2.0f;

    [Header("攻撃設定")]
    [SerializeField] private float attackCooldown = 1.0f;

    // ★追加：攻撃力
    [SerializeField] private float attackDamage = 10.0f;

    private float attackTimer = 0.0f;

    [Header("攻撃判定")]
    // ★追加：攻撃判定の大きさ
    [SerializeField] private float attackHitRadius = 1.0f;

    // ★追加：攻撃判定の高さ
    [SerializeField] private float attackHitHeight = 1.0f;

    // ★追加：ゾンビから前方への距離
    [SerializeField] private float attackHitForward = 1.2f;

    // ★追加：1回の攻撃で既にダメージを与えたか
    private bool hasHitPlayer = false;

    [Header("参照")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private ZombiStates states;

    // 現在の状態
    private ZombiState currentState;

    void Start()
    {
        currentState = ZombiState.Idle;

        agent.isStopped = true;

        // 最初はIdle
        animator.SetInteger("State", 0);
        animator.CrossFade("Idle", 0.0f, 0);
    }

    void Update()
    {
        // 死亡中は処理しない
        if (currentState == ZombiState.Dead)
            return;

        // HPが0なら即死亡
        if (states != null && states.IsDead())
        {
            ChangeState(ZombiState.Dead);
            return;
        }

        if (player == null)
            return;

        // 攻撃クールタイム
        if (attackTimer > 0.0f)
        {
            attackTimer -= Time.deltaTime;
        }

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

        // プレイヤーが範囲内に入ったら即追跡
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

        // プレイヤーが範囲外に出たら即待機
        if (distance > chaseRange)
        {
            ChangeState(ZombiState.Idle);
            return;
        }

        // 攻撃範囲に入ったら即攻撃
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

        // 攻撃アニメーションが終わったか確認
        AnimatorStateInfo stateInfo =
            animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Attack") &&
            stateInfo.normalizedTime >= 1.0f &&
            !animator.IsInTransition(0))
        {
            attackTimer = attackCooldown;

            float distance = Vector3.Distance(
                transform.position,
                player.position
            );

            // まだ追跡範囲内なら即Walk
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
    // 攻撃が当たった瞬間
    // =========================
    public void AttackHit()
    {
        // Attack状態じゃなければ何もしない
        if (currentState != ZombiState.Attack)
            return;

        // この攻撃ですでにダメージを与えていたら何もしない
        if (hasHitPlayer)
            return;

        // 攻撃判定の位置
        Vector3 hitPosition =
            transform.position +
            transform.forward * attackHitForward;

        hitPosition.y += attackHitHeight;

        // 攻撃判定の範囲内にあるColliderを取得
        Collider[] hitColliders = Physics.OverlapSphere(
            hitPosition,
            attackHitRadius
        );

        foreach (Collider hitCollider in hitColliders)
        {
            // PlayerStatsを探す
            PlayerStats playerStats =
                hitCollider.GetComponentInParent<PlayerStats>();

            // PlayerStatsが見つかった
            if (playerStats != null)
            {
                // プレイヤーにダメージ
                playerStats.TakeDamage(attackDamage);

                // 1回の攻撃では1回だけ
                hasHitPlayer = true;

                Debug.Log(
                    $"ゾンビの攻撃が命中！ {attackDamage}ダメージ"
                );

                break;
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

        if (currentState == newState)
            return;

        currentState = newState;

        switch (currentState)
        {
            // =========================
            // Idle
            // =========================
            case ZombiState.Idle:

                agent.isStopped = true;

                animator.SetInteger("State", 0);

                animator.CrossFade("Idle", 0.0f, 0);

                break;


            // =========================
            // Chase
            // =========================
            case ZombiState.Chase:

                agent.isStopped = false;

                animator.SetInteger("State", 1);

                animator.CrossFade("Walk", 0.0f, 0);

                break;


            // =========================
            // Attack
            // =========================
            case ZombiState.Attack:

                agent.isStopped = true;

                // ★追加
                // 新しい攻撃なのでダメージ判定をリセット
                hasHitPlayer = false;

                animator.SetInteger("State", 2);

                animator.CrossFade("Attack", 0.0f, 0);

                break;


            // =========================
            // Dead
            // =========================
            case ZombiState.Dead:

                agent.isStopped = true;
                agent.enabled = false;

                animator.SetInteger("State", 3);

                animator.CrossFade("Die", 0.0f, 0);

                break;
        }
    }

    // =========================
    // 状態取得
    // =========================
    public ZombiState GetCurrentState()
    {
        return currentState;
    }

    // =========================
    // 攻撃判定をSceneビューで確認
    // =========================
    private void OnDrawGizmosSelected()
    {
        Vector3 hitPosition =
            transform.position +
            transform.forward * attackHitForward;

        hitPosition.y += attackHitHeight;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            hitPosition,
            attackHitRadius
        );
    }
}