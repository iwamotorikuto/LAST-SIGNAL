using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

public class NPCSpawner : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private GameObject npcPrefab;   //救助NPCのプレハブ
    [SerializeField] private Transform player;       //プレイヤー
    [SerializeField] private Transform goalPoint;    //ゴール地点

    [Header("範囲")]
    [SerializeField] private Transform spawnArea;     //範囲を決める空のオブジェクト
    [SerializeField] private int maxTry = 10;
    [SerializeField] private float sampleDistance = 10.0f;  //NavMeshを探す距離

    [Header("建物")]
    [SerializeField] private LayerMask buildingMask;   //建物のレイヤー
    [SerializeField] private float checkRadius = 0.5f; //建物からどのくらい離すか

    [Header("寿命")]
    [SerializeField] private float lifeTime = 10.0f;   //助けられなかった場合の消えるまでの秒数
    [SerializeField] private float rescueDistance = 3.0f;  //プレイヤーが近づいた距離に応じて助けたかどうかを決める

    [Header("デバック")]
    [SerializeField] private bool showArea = true;     //Sceneビューに範囲を表示する

    [Header("スポーン間隔")]
    [SerializeField] private float spawnInterval = 10.0f; //何秒おきにスポーンするか
    [SerializeField] private bool spawnAtStart = true;    //開始直後にも1体出すか

    private float spawnTimer; //経過時間
    private GameObject currentNpc;


    void Start()
    {
        if(npcPrefab == null || spawnArea == null)
        {
            return;
        }

        if(spawnAtStart)
        {
            SpawnOne();
        }
    }

    void Update()
    {
        if (npcPrefab == null || spawnArea == null) return;

        if (currentNpc != null) return;

        //時間を数えて、間隔が来たらスポーンする
        spawnTimer += Time.deltaTime;
        if(spawnTimer >= spawnInterval)
        {
            spawnTimer = 0.0f;
            SpawnOne();
        }
    }

    private void SpawnOne()
    {
        for (int i = 0; i < maxTry; i++)
        {
            //枠の中のランダムな位置に出現(-0.5～0.5が枠の端から端)
            Vector3 localPos = new Vector3(Random.Range(-0.5f, 0.5f), 0.0f, Random.Range(-0.5f, 0.5f));
            Vector3 randomPos = spawnArea.TransformPoint(localPos);


            //NavMesh上を歩けるかの確認
            if (!NavMesh.SamplePosition(randomPos, out NavMeshHit hit, sampleDistance, NavMesh.AllAreas))
            {
                continue;
            }

            //NavMeshでずれた位置が枠の中に収まっているかを確認
            Vector3 localHit = spawnArea.InverseTransformPoint(hit.position);
            if (Mathf.Abs(localHit.x) > 0.5f || Mathf.Abs(localHit.z) > 0.5f)
            {
                continue;
            }

            //建物と重なっていないかの確認
            Vector3 checkPos = hit.position + Vector3.up * (checkRadius + 0.1f);
            if (Physics.CheckSphere(checkPos, checkRadius, buildingMask))
            {
                continue;
            }
            currentNpc = Instantiate(npcPrefab, hit.position, Quaternion.identity, transform);

            //寿命を管理するスクリプトを付けて、必要な情報を渡す
            NPCLifeTime life = currentNpc.AddComponent<NPCLifeTime>();
            life.Init(player, lifeTime, rescueDistance);

            //生成したNPCに、プレイヤーとゴール地点を渡す
            FollowPlayer follow = currentNpc.GetComponent<FollowPlayer>();
            if (follow != null)
            {
                follow.Init(player, goalPoint);
            }
            return;
        }
    }

    //選択中だけ、Sceneビューに枠を表示する
    private void OnDrawGizmosSelected()
    {
        if (spawnArea == null) return;

        Gizmos.color = Color.green;
        Gizmos.matrix = spawnArea.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(1.0f, 0.1f, 1.0f));
    }


}
