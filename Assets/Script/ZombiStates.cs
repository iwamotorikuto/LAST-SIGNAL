using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiStates : MonoBehaviour, IDamageable
{
    [Header("ゾンビのステータス")]
    [SerializeField] private float maxHP = 50f;
    [SerializeField] private float currentHP;
    [SerializeField] private float defense = 10f;

    public float CurrentHP => currentHP;
    public float MaxHP => maxHP;
    public float Defense => defense;

    private void Start()
    {
        // 最大HPを現在HPに設定
        currentHP = maxHP;
    }

    // =========================
    // ダメージ処理
    // =========================
    public void TakeDamage(float incomingDamage)
    {
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

        // HPが0になったら死亡
        if (currentHP <= 0f)
        {
            Debug.Log("ゾンビのHPが0になった！死亡！");
        }
    }

    // =========================
    // 死亡判定
    // =========================
    public bool IsDead()
    {
        return currentHP <= 0f;
    }
}