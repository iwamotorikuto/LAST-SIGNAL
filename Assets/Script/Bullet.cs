using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int damage = 10;

    public void SetDamage(int power)
    {
        damage = power;
    }

    private void Start()
    {
        Debug.Log("★★ Bullet.cs が実行されています ★★");

        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(
            "★★ 弾が当たった：" + collision.gameObject.name + " ★★"
        );

        ZombiStates zombie =
            collision.collider.GetComponentInParent<ZombiStates>();

        if (zombie != null)
        {
            Debug.Log("★★ ZombiStatesを発見！ ★★");

            zombie.TakeDamage(damage);

            Debug.Log("★★ ダメージを与えました：" + damage + " ★★");
        }
        else
        {
            Debug.LogWarning(
                "ZombiStatesが見つからない：" + collision.gameObject.name
            );
        }

        Destroy(gameObject);
    }
}