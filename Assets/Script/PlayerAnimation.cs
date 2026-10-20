using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    enum PlayAnimation
    {
        Idle,
        Walk,
        Run,
        Jumping,
        Strafing,
        KneelingDown, // 第1段階のしゃがみアニメーション
        Kneel,        // 第2段階の蘇生アニメーション
    }

    private PlayerController playerController;
    private PlayerRescueOperation playerRescueOperation;
    private Animator animator;

    private PlayAnimation currentAnimation = PlayAnimation.Idle;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        // CharacterControllerがついている前提で安全に取得
        playerController = GetComponent<PlayerController>();
        playerRescueOperation = GetComponent<PlayerRescueOperation>();
    }

    void Update()
    {
        if (playerController == null) return;

        PlayAnimation nextAnimation = PlayAnimation.Idle;

        // 優先度：蘇生中 > しゃがみ込み中 > 移動・ジャンプ等
        if (playerRescueOperation != null && playerRescueOperation.IsRescuing)
        {
            nextAnimation = PlayAnimation.Kneel;
        }
        else if (playerRescueOperation != null && playerRescueOperation.IsKneelingDown)
        {
            nextAnimation = PlayAnimation.KneelingDown;
        }
        else if (playerController.IsJumping || !GetComponent<CharacterController>().isGrounded)
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

        if (currentAnimation != nextAnimation)
        {
            currentAnimation = nextAnimation;
            PlayAnim(currentAnimation);
        }
    }

    void PlayAnim(PlayAnimation playAnimation)
    {
        Debug.Log("アニメーション切り替え: " + playAnimation.ToString());
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
            case PlayAnimation.KneelingDown:
                animator.Play("Kneeling"); // 最初に行うしゃがみアニメ
                break;
            case PlayAnimation.Kneel:
                animator.Play("Kneel");        // ボタンを押した後の蘇生アニメ
                break;
        }
    }
}