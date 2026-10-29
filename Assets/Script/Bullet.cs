using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;
    private float damage = 10f; // 銃から受け取る攻撃力の初期値

    // 銃から発射されるときに、WeaponStatsの攻撃力をセットする
    public void SetDamage(float power)
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

        DamageHelper.Apply(collision.collider, damage);

        Destroy(gameObject);
    }
}