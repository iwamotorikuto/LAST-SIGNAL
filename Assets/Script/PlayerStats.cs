using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("プレイヤーのステータス")]
    [SerializeField] private float maxHP = 50f;     // 最大体力
    [SerializeField] private float currentHP;       // 現在の体力
    [SerializeField] private float defense = 10f;   // 防御力

    // 外部から現在のHPや最大HPを参照できるようにするプロパティ
    public float CurrentHP => currentHP;
    public float MaxHP => maxHP;
    public float Defense => defense;

    void Start()
    {
        // ゲーム開始時に現在のHPを最大値で初期化する
        currentHP = maxHP;
    }

    // ダメージを受ける処理（敵や弾から呼び出される）
    public void TakeDamage(float incomingDamage)
    {
        // 簡単な防御力計算
        float actualDamage = Mathf.Max(1f, incomingDamage - (defense * 0.5f));

        currentHP -= actualDamage;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP); // 0未満や最大値超過を防ぐ

        Debug.Log($"プレイヤーがダメージを受けた！ 現在のHP: {currentHP} / {maxHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("プレイヤーは力尽きた……（ゲームオーバー処理）");
        // ここにゲームオーバー画面の呼び出し
    }
}