using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NPCPistol : MonoBehaviour
{
    [Header("武器の設定")]
    [SerializeField] private int damage = 10;           //敵に与える銃弾のダメージ
    [SerializeField] private float fireRate = 0.3f;     //連射の最短間隔
    [SerializeField] private float bulletSpeed = 10.0f; //弾の速さ

    [Header("参照設定")]
    [SerializeField] private GameObject bulletPrefab;   //弾のPrefab
    [SerializeField] private Transform nuzzlePoint;     //銃口の位置

    [Header("エフェクト")]
    [SerializeField] private GameObject nuzzleFlashPrefab; //発射時の光
    [SerializeField] private float flashLifeTime = 0.1f;

    private float timeSinceLastShot;                       //最後に打ってからの時間
    
    void Start()
    {
        timeSinceLastShot = fireRate;                      //最初の1発をすぐ撃てるようにする
    }
    
    void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }

    public void Shoot()
    {
        //連射しないようにする
        if (timeSinceLastShot < fireRate) return;

        if (bulletPrefab == null || nuzzlePoint == null)
        {
            return;
            
        }

        timeSinceLastShot = 0.0f;

        //発射時に光を出す
        if(nuzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(nuzzleFlashPrefab,nuzzlePoint.position,nuzzlePoint.rotation,nuzzlePoint);
            Destroy(flash, flashLifeTime);
        }

        //弾を生成して銃口の向きを合わせる
        GameObject bullet = Instantiate(bulletPrefab, nuzzlePoint.position, nuzzlePoint.rotation);
        bullet.transform.forward = nuzzlePoint.forward;

        //弾にダメージを渡す
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if(bulletScript != null)
        {
            bulletScript.SetDamage(damage);
        }

        //弾を前に飛ばす
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = nuzzlePoint.forward * bulletSpeed;
        }
    }
}
