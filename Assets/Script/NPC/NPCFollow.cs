using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCFollow : MonoBehaviour
{
    [SerializeField] private Transform player;               //追従する対象
    [SerializeField] private Animator animator;              //アニメーションの制御
    [SerializeField] private NPCShooter shooter;             //射撃スクリプト
    [SerializeField] private float followDistance   = 3.0f;  //この距離を離れたら追従
    [SerializeField] private float stopDistance     = 2.0f;  //この距離までに近づいたら止まる
    [SerializeField] private float aimBlendSpeed    = 8.0f;     //構えるのにかかる時間

    private const int UpperBodyLayer = 1;  //上半身のレイヤーの番号
    private float aimWeight;               
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
        bool hasTarget = shooter != null && shooter.HasTarget;

        //ゾンビがいる間は上半身のみ構える
        aimWeight = Mathf.MoveTowards(aimWeight, hasTarget ? 1.0f : 0.0f, aimBlendSpeed * Time.deltaTime);
        animator.SetLayerWeight(UpperBodyLayer, aimWeight);

        //ゾンビと戦っている間は追従をしない
        if(shooter != null && shooter.HasTarget)
        {
            agent.ResetPath();                    //移動をやめる
            animator.SetBool("IsWalking", false);  //歩きのアニメーションを保つ
            return;                               //以降の追従処理を行わない
            
        }
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

        //実際の処理状態に応じてアニメーションを切り替える
        bool isWalking = agent.velocity.magnitude > 0.1f;
        animator.SetBool("IsWalking", isWalking);
    }
}
