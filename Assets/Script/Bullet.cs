using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ZombiAI;


public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int damage = 10; // 銃から受け取る攻撃力の初期値

    // 銃から発射されるときに、WeaponStatsの攻撃力をセットする
    public void SetDamage(int power)
    {
        damage = power;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // プレイヤー自身には当てず、弾も消さない
        if (collision.collider.CompareTag("Player")) return;

        // ぶつかった相手のタグが "Zombie" の場合
        if (collision.gameObject.CompareTag("Zombie"))
        {
            Debug.Log($"ゾンビに弾が命中！ {damage} のダメージを与えます。");
        }

        // ゾンビの ZombieAI スクリプトを取得してダメージを与える
        ZombiAI zombie = collision.gameObject.GetComponent<ZombiAI>();
        if (zombie != null)
        {
            zombie.TakeDamage(damage); // ゾンビの体力が削られる！
        }

        else
        {
            Debug.LogWarning("当たったオブジェクトに 'ZombiAI' スクリプトがついていません！");

        }

          
    
        Destroy(gameObject);
    }
}