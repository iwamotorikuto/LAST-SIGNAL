///
/// ダメージを受けた際に死亡するまでのテストプレイ
/// ※確認が終わったら、このスクリプトごと削除します
///
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// テスト用: 確認が終わったら、このスクリプトごと削除
public class DamageTester : MonoBehaviour
{
    [SerializeField] private PlayerStats target;
    [SerializeField] private float damage = 10f;
    [SerializeField] private KeyCode damageKey = KeyCode.K; // 10ダメージ
    [SerializeField] private KeyCode killKey = KeyCode.L;   // 一撃で倒す

    private void Awake()
    {
        if (target == null) target = FindObjectOfType<PlayerStats>();
    }

    private void Update()
    {
        if (target == null) return;
        if (Input.GetKeyDown(damageKey))
        {
            target.TakeDamage(damage);
            Debug.Log("[DamageTester] ダメージを与えた: " + damage);
        }
        else if(damageKey == null)
        {
            Debug.Log("damageKeyがnullです！");
        }

        if (Input.GetKeyDown(killKey))
        {
            target.TakeDamage(target.MaxHP);
            Debug.Log("[DamageTester] 一撃で倒した");
        }
        else if(killKey == null)
        { 
            Debug.Log("Nullです！"); 
        }
    }
}