using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCFollow : MonoBehaviour
{
    [SerializeField] private Transform player;               //追従する対象
    [SerializeField] private float followDistance   = 3.0f;  //この距離を離れたら追従
    [SerializeField] private float stopDistance     = 2.0f;  //この距離までに近づいたら止まる

    private NavMeshAgent agent;
    void Start()
    {
        //NavMeshAgentコンポーネントを取得
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stopDistance; //停止距離の設定
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーとの距離間の計算
        float distance = Vector3.Distance(transform.position, player.position);

        if(distance > followDistance)
        {
            //離れていたら、プレイヤーに向かって移動をする
            agent.SetDestination(player.position);
        }
        else if (distance <= stopDistance)
        {
            //近づきすぎたら止まる
            agent.ResetPath();
        }
    }
}
