using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ZombieAI;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;
    private float damage = 10f; // 銃から受け取る攻撃力の初期値

    // 銃から発射されるときに、WeaponStatsの攻撃力をセットする関数
    public void SetDamage(float power)
    {
        damage = power;
    }

    private void Start()
    {
        // 一定時間後に弾を消去する
        Destroy(gameObject, lifeTime);
        
    }

    // 弾が何かにぶつかった瞬間に呼ばれる
    private void OnCollisionEnter(Collision collision)
    {
        // ぶつかった相手のタグが "Zombie" の場合
        if (collision.gameObject.CompareTag("Zombie"))
        {
            Debug.Log($"ゾンビに弾が命中！ {damage} のダメージを与えます。");

            // ゾンビの ZombieAI スクリプトを取得してダメージを与える
            ZombieAI zombie = collision.gameObject.GetComponent<ZombieAI>();
            if (zombie != null)
            {
                zombie.TakeDamage(damage); // ゾンビの体力が削られる！
            }
            
            else
            {
                Debug.LogWarning("当たったオブジェクトに 'ZombieAI' スクリプトがついていません！");
            }
        }

        // 弾自身は消滅させる
        Destroy(gameObject);
    }
}