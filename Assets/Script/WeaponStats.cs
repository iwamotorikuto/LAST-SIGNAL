using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    [Header("武器のステータス")]
    [SerializeField] private float attackPower = 10f; // 変数の頭文字は小文字にするのがC#の一般的な命名規則です

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
