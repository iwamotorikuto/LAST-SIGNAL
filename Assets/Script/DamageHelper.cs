using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageHelper
{
    // 当たった相手にダメージを与える。与えられたら true
    public static bool Apply(Component hit, float damage)
    {
        // IDamageable を持つもの(PlayerStats など)
        IDamageable target = hit.GetComponentInParent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage);
            return true;
        }

        // ZombieAI は書き換えずに、直接呼ぶ
        ZombieAI zombie = hit.GetComponentInParent<ZombieAI>();
        if (zombie != null)
        {
            Debug.Log($"[Damage] {zombie.name} に {damage} のダメージ");
            zombie.TakeDamage(damage);
            return true;
        }

        return false;
    }
}