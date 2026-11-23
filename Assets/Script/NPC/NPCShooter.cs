using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPCShooter : MonoBehaviour
{
    [SerializeField] private float searchRadius = 10.0f;  //NPCが敵を見つけれる半径
    [SerializeField] private float searchInterval = 0.3f; //敵を探す間隔(秒)
    [SerializeField] private LayerMask zombieLayer;       //敵(ゾンビ)のレイヤー
    [SerializeField] private LayerMask obstacleLayer;     //障害物などの球を遮るレイヤー
    [SerializeField] private float fireInterval = 0.5f;   //射撃間隔(秒)
    [SerializeField] private float rotateSpeed = 8.0f;    //敵に向く速さ 
    [SerializeField] private float aimAngle = 15.0f;      //敵に向く角度の範囲
    [SerializeField] private float eyeHeight = 1.4f;      //目の高さ
    [SerializeField] private UnityEvent onFire;//打つ処理

    private Transform target;       // 今狙っている敵
    private float searchTimer;      // 次の索敵までの時間
    private float fireTimer;        // 次の射撃までの時間
    public bool HasTarget => target != null; //ターゲットであるゾンビがいるかどうか

    void Update()
    {
        //一定間隔でターゲットを探し直す
        searchTimer += Time.deltaTime;
        if(searchTimer >= searchInterval)
        {
            searchTimer = 0.0f;
            target = FindNearestZombie();
        }

        //敵がいなければ何もしない
        if (target == null) return;

        //ターゲットが倒されて消えていたら何もしない
        if (!target.gameObject.activeInHierarchy)
        {
            target = null;
            return;
        }

        //敵の方向に体を向ける
        RotateToTarget();

        //発射間隔を進める
        fireTimer += Time.deltaTime;

        //敵の方を向けていて、間隔も経っていれば撃つ
        if(fireTimer >= fireInterval && IsFacingTarget())
        {
            fireTimer = 0.0f;
            onFire.Invoke();
        }
    }

    //範囲内で近いゾンビを探す
    private Transform FindNearestZombie()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, searchRadius, zombieLayer);

        Transform nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            //壁の向こうにいる敵は無視する
            if (!CanSee(hit.transform)) continue;

            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if(distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = hit.transform;
            }
        }

        return nearest;
    }

    //敵(ゾンビ)との間に障害物は無いかをチェック
    private bool CanSee(Transform enemy)
    {
        Vector3 start = transform.position + Vector3.up * eyeHeight;
        Vector3 end = enemy.position + Vector3.up * eyeHeight;
        Vector3 direction = end - start;

        //障害物に当たったら見えていない
        return !Physics.Raycast(start, direction.normalized, direction.magnitude, obstacleLayer);
    }

    //敵の方向に回転する
    private void RotateToTarget()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0.0f;  //上下への傾きを防ぐため

        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    //敵の方向に向いているか
    private bool IsFacingTarget()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0.0f;

        return Vector3.Angle(transform.forward, direction) <= aimAngle;
    }

    //シーンビューで索敵範囲を可視化するために赤い円で表示をする
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
