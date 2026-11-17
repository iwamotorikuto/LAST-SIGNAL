using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [Header("プレイヤーのステータス")]
    [SerializeField] private float maxHP = 50f;
    [SerializeField] private float currentHP;

    public float CurrentHP => currentHP;
    public float MaxHP => maxHP;
    public bool IsDead { get; private set; }

    public event Action OnDied; // 死亡した瞬間に1回だけ通知する

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float incomingDamage)
    {
        if (IsDead) return; // 死んだあとは、ダメージを受けない

        currentHP = Mathf.Clamp(currentHP - incomingDamage, 0f, maxHP);
        Debug.Log($"プレイヤーがダメージを受けた！ 現在HP: {currentHP}/{maxHP}");

        if (currentHP <= 0f) Die();
    }
    /// <summary>
    /// 応急キットや包帯で使う(アイテムの実装で呼んでいますが此処は後に変更)
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(float amount)
    {
        if (IsDead) return;
        currentHP = Mathf.Clamp(currentHP + amount, 0f, maxHP);
        Debug.Log($"回復した！ 現在HP: {currentHP}/{maxHP}");
    }

    private void Die()
    {
        IsDead = true; 
        Debug.Log("プレイヤーは力尽きた……");
        OnDied?.Invoke();
    }
}