
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
        KneelingDown,
        Kneel,
        // --- ピストル構え中のアニメーションを追加 ---
        PistolIdle,
        PistolWalk,
        PistolRun,
    }

    [Header("--- 武器の動的生成設定 ---")]
    [SerializeField] private GameObject weaponPrefab;   // 生成する銃のプレハブ
    [SerializeField] private Transform rightHandBone;     // 右手ボーン（ik_hand_gun など）
    private GameObject spawnedWeapon;                   // 生成された銃のインスタンス

    private PlayerController playerController;
    private PlayerRescueOperation playerRescueOperation;
    private Animator animator;

    private PlayAnimation currentAnimation = PlayAnimation.Idle;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        playerRescueOperation = GetComponent<PlayerRescueOperation>();
    }

    void Update()
    {
        if (playerController == null) return;

        PlayAnimation nextAnimation = PlayAnimation.Idle;

        // 優先度：蘇生中 > しゃがみ込み中 > 構え中(Aim) > ジャンプ・移動等
        if (playerRescueOperation != null && playerRescueOperation.IsRescuing)
        {
            nextAnimation = PlayAnimation.Kneel;
        }
        else if (playerRescueOperation != null && playerRescueOperation.IsKneelingDown)
        {
            nextAnimation = PlayAnimation.KneelingDown;
        }
        // 銃を構えている（IsAiming が true）ときの優先分岐
        else if (playerController.IsAiming)
        {
            if (playerController.IsRunning)
            {
                nextAnimation = PlayAnimation.PistolRun;
            }
            else if (playerController.IsMoving)
            {
                nextAnimation = PlayAnimation.PistolWalk;
            }
            else
            {
                nextAnimation = PlayAnimation.PistolIdle;
            }
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

        // --- ピストル構え系アニメーションのときの生成・破棄処理 ---
        bool isPistolAnim = (playAnimation == PlayAnimation.PistolIdle ||
                             playAnimation == PlayAnimation.PistolWalk ||
                             playAnimation == PlayAnimation.PistolRun);

        if (isPistolAnim)
        {
            // まだ銃が生成されていなければ、右手ボーンの子として生成する
            if (spawnedWeapon == null && weaponPrefab != null && rightHandBone != null)
            {
                spawnedWeapon = Instantiate(weaponPrefab, rightHandBone);
                spawnedWeapon.transform.localPosition = Vector3.zero;
                spawnedWeapon.transform.localRotation = Quaternion.identity;
            }
        }
        else
        {
            // ピストル構え以外になったら、銃オブジェクトを消去する
            if (spawnedWeapon != null)
            {
                Destroy(spawnedWeapon);
            }
        }

        // --- アニメーションの再生 ---
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
            case PlayAnimation.KneelingDown:
                animator.Play("Kneeling");
                break;
            case PlayAnimation.Kneel:
                animator.Play("Kneel");
                break;
            case PlayAnimation.PistolIdle:
                animator.Play("Pistol Idle");
                break;
            case PlayAnimation.PistolWalk:
                animator.Play("Pistol Walk");
                break;
            case PlayAnimation.PistolRun:
                animator.Play("Pistol Run");
                break;
        }
    }
}
