using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [Header("プレイヤーのステータス")]
    [SerializeField] private float maxHP = 50f;
    [SerializeField] private float currentHP;
    [SerializeField] private float defense = 10f;

    public float CurrentHP => currentHP;
    public float MaxHP => maxHP;
    public float Defense => defense;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float incomingDamage)
    {
        float actualDamage =
            Mathf.Max(1f, incomingDamage - (defense * 0.5f));

        currentHP -= actualDamage;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        Debug.Log($"プレイヤーがダメージを受けた！ 現在HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("プレイヤーは力尽きた……");
    }
}