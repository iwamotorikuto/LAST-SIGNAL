using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiStates : MonoBehaviour, IDamageable
{
    [Header("ゾンビのステータス")]
    [SerializeField] private float maxHP = 50f;
    [SerializeField] private float currentHP;
    [SerializeField] private float defense = 10f;

    [Header("死亡設定")]
    [SerializeField] private float destroyDelay = 2f;

    // 死亡しているか
    private bool isDead = false;

    public float CurrentHP => currentHP;
    public float MaxHP => maxHP;
    public float Defense => defense;

    private void Start()
    {
        // 最大HPを現在HPに設定
        currentHP = maxHP;

        Debug.Log(
            $"ゾンビ生成：HP {currentHP}/{maxHP}"
        );
    }

    // =========================
    // ダメージ処理
    // =========================
    public void TakeDamage(float incomingDamage)
    {
        // すでに死亡していたらダメージを受けない
        if (isDead)
        {
            return;
        }

        // 防御力を考慮してダメージ計算
        float actualDamage =
            Mathf.Max(1f, incomingDamage - (defense * 0.5f));

        // HPを減らす
        currentHP -= actualDamage;

        // HPが0未満にならないようにする
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        Debug.Log(
            $"ゾンビがダメージを受けた！ 現在HP: {currentHP}/{maxHP}"
        );

        // =========================
        // 死亡判定
        // =========================
        if (currentHP <= 0f)
        {
            Die();
        }
    }

    // =========================
    // 死亡処理
    // =========================
    private void Die()
    {
        // 二重で死亡処理しない
        if (isDead)
        {
            return;
        }

        isDead = true;

        Debug.Log("★★ ゾンビ死亡！ ★★");

        // ここで死亡アニメーションなどを再生できる
        // Animator animator = GetComponent<Animator>();
        // animator.SetTrigger("Die");

        // NavMeshAgentなどがある場合は停止
        UnityEngine.AI.NavMeshAgent agent =
            GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // 一定時間後にゾンビを削除
        Destroy(gameObject, destroyDelay);
    }

    // =========================
    // 死亡判定
    // =========================
    public bool IsDead()
    {
        return isDead;
    }
}