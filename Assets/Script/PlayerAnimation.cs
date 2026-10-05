using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    enum PlayAnimation
    {
        Idle,   // 立つ
        Walk,   // 歩く
        Run,    // 走る
        Jumping,// ジャンプ
        Strafing //ストレイフ
    }

    private PlayerController playerController;
    private Animator animator;

    // 現在再生しているアニメーションを覚えておく変数
    private PlayAnimation currentAnimation = PlayAnimation.Idle;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (playerController == null) return;

        // 1. 次に再生すべきアニメーションを決定する
        PlayAnimation nextAnimation = PlayAnimation.Idle;

        if (playerController.IsJumping || !GetComponent<CharacterController>().isGrounded)
        {
            nextAnimation = PlayAnimation.Jumping;
        }
        else if (playerController.IsRunning)
        {
            nextAnimation = PlayAnimation.Run;
        }
        else if (playerController.IsMoving)
        {
            nextAnimation = PlayAnimation.Walk;
        }
        else if (playerController.IsStrafing)
        {
            nextAnimation = PlayAnimation.Strafing;
        }
        else
        {
            nextAnimation = PlayAnimation.Idle;
        }

        // 2. 「前回のフレームからアニメーションが変わった時」だけ再生し直す
        if (currentAnimation != nextAnimation)
        {
            currentAnimation = nextAnimation;
            PlayAnim(currentAnimation);
        }
    }

    /// <summary>
    /// アニメーションを再生する
    /// </summary>
    void PlayAnim(PlayAnimation playAnimation)
    {
        Debug.Log("アニメーションの切り替え" + playAnimation.ToString());
        switch (playAnimation)
        {
            case PlayAnimation.Idle:
                animator.Play("Idle");
                break;

            case PlayAnimation.Walk:
                animator.Play("Walk");
                break;

            case PlayAnimation.Run:
                animator.Play("Run");
                break;

            case PlayAnimation.Jumping:
                animator.Play("Jumping");
                break;

            case PlayAnimation.Strafing:
                animator.Play("Strafing");
                break;
        }
    }
}
