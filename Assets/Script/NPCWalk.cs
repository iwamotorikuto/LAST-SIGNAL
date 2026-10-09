using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWalk : MonoBehaviour
{
    static readonly int ReachedGoalHash = Animator.StringToHash("ReachedGoal");

    [SerializeField] Animator animator;
    [SerializeField] float walkDistance = 1.0f; //歩く距離(m)
    [SerializeField] float walkSpeed = 1.2f;    //歩く速さ(m/秒)

    Vector3 startPos;
    bool started;
    bool finished;

    // Start is called before the first frame update
    void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        animator.SetBool(ReachedGoalHash, false);  //開始時にONからOFFに
    }

    // Update is called once per frame
    void Update()
    {
        if (finished) return;

        //Walkタグのステートにいる間だけは動くことが出来る
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Walk")) return;

        if(!started)
        {
            started = true;
            startPos = transform.position; //歩き始めの位置の記録
        }

        transform.position += transform.forward * walkSpeed * Time.deltaTime;

        if(Vector3.Distance(startPos, transform.position) >= walkDistance)
        {
            finished = true;
            animator.SetBool(ReachedGoalHash, true);  //お辞儀のアニメーションに遷移
        }
    }
}
