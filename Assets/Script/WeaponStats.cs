using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    [Header("武器のステータス")]
    [SerializeField] private float attackPower = 10f; 

    // 外部から攻撃力を安全に取得できるようにするプロパティ
    public float AttackPower => attackPower;

  
    public float GetAttackPower()
    {
        return attackPower;
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
