using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLifeTime : MonoBehaviour
{
    private Transform player;     //プレイヤー
    private float lifeTime;       //消えるまでの秒数
    private float rescueDistance; //この距離まで来たら助けた扱いになり消えなくなる
    private float timer;          //経過時間
    private bool rescued;         //助けられたかどうか

    public void Init(Transform playerTransform, float time, float distance)
    {
        player = playerTransform;
        lifeTime = time;
        rescueDistance = distance;
    }

    //他のスクリプトから「助けた」と知らせたいとき用
    public void MarkRescued()
    {
        rescued = true;
    }

    void Update()
    {
        //助けられた後は、一切消えない
        if (rescued) return;

        //プレイヤーガ近くに来た時には、助けられた扱いになる
        if(player != null && FlatDistance(transform.position,player.position) <= rescueDistance)
        {
            rescued = true;
            return;
        }

        //時間が来たら消える
        timer += Time.deltaTime;
        if(timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    //高さを無視した距離
    static float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0.0f;
        b.y = 0.0f;
        return Vector3.Distance(a, b);
    }
}
