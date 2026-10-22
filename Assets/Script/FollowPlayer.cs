using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class FollowPlayer : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform goalPoint;  //お辞儀をする場所
    [SerializeField] private Animator animator;

    [Header("追従")]
    [SerializeField] private float followDistance = 2.5f; //この距離に近づいてきたら止まる
    [SerializeField] private float resumeDistance = 3.0f; //この距離超えたら再度追従開始
    [SerializeField] private float updateInterval = 0.2f; //プレイヤーの位置確認

    [Header("ゴール")]
    [SerializeField] private float goalDistance = 1.5f;   //ここまで来たらお辞儀

    [Header("アニメーション")]
    [SerializeField] private float fadeTime = 0.1f;       //立ち止まりと歩きの切り替え時間

    //Animatorのステート名
    private const string WalkState = "Injured Walking_animation";
    private const string IdleState = "Injured Idle_animation";
    private const string BowState  = "Quick Formal Bow_animation";

    private NavMeshAgent agent;
    private float timer;
    private bool isFollowing;
    private bool arrived;
    private bool hasStood;  //起き上がったかどうか
    private bool isIdle;    //現在はIdle状態か

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        agent.isStopped = true; //起き上がるまで動かさない
        isFollowing = true;
    }

    void Update()
    {
        if (arrived || player == null) return;

        //歩き初めたら起き上がったとして記録
        if(!hasStood)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(WalkState))
            {
                hasStood = true;
            }
            else
            {
                agent.isStopped = true;
                return;
            }
        }

        //ゴール判定
        if(goalPoint != null && FlatDistance(transform.position, goalPoint.position) <= goalDistance)
        {
             Arrive();
             return;
        }

        //追従
        float d = FlatDistance(transform.position, player.position);
        if (isFollowing && d <= followDistance) isFollowing = false;
        else if (!isFollowing && d > resumeDistance) isFollowing = true;

        if(isFollowing)
        {
            agent.isStopped = false;
            timer -= Time.deltaTime;
            if(timer <= 0.0f)
            {
                agent.SetDestination(player.position);
                timer = updateInterval;
            }
        }
        else
        {
            agent.isStopped = true;
        }

        //止まっている間はIdle状態になる
        bool shouldIdle = agent.isStopped;
        if(shouldIdle != isIdle)
        {
            isIdle = shouldIdle;
            animator.CrossFade(isIdle ? IdleState : WalkState, fadeTime);
        }
    }

    void Arrive()
    {
        arrived = true;
        agent.isStopped = true;
        agent.ResetPath();
        animator.CrossFade(BowState, fadeTime); //animatorでの遷移矢印が無くてもコードから再生が可能
    }

    static float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0.0f;
        b.y = 0.0f;
        return Vector3.Distance(a, b);
    }
}
