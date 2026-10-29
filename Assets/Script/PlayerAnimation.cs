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
        // --- ナイフ構え中のアニメーションを追加 ---
        SlashAdvance,
        KnifeWalk
    }

    [Header("--- 武器の動的生成設定 ---")]
    [SerializeField] private GameObject weaponPrefab;   // 生成する銃のプレハブ
    [SerializeField] private Transform rightHandBone;     // 右手ボーン（ik_hand_gun など）
    [SerializeField] private GameObject knifePrefab;
    [SerializeField] private Transform keftHandBone; //左手ボーンの
    [SerializeField] private float slashDuration = 0.6f; // 斬撃アニメの長さ(秒)
    private bool isAttacking = false;
    private float attackTimer = 0f;
    private GameObject spawnedKnife;
    private GameObject spawnedWeapon;                   // 生成された銃のインスタンス

    private PlayerController playerController;
    private PlayerRescueOperation playerRescueOperation;
    private Animator animator;
    private bool isHoldingKnife = false;

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

        // 斬撃中は他のアニメーションに切り替えない
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0.0f)
            {
                isAttacking = false; 
            }
            else
            {
                return;
            }
        }
        PlayAnimation nextAnimation = PlayAnimation.Idle;

        // 優先度：蘇生中 > しゃがみ込み中 > 構え中(Aim) ※ただしナイフ時は除外 > ジャンプ・移動等
        if (playerRescueOperation != null && playerRescueOperation.IsRescuing)
        {
            nextAnimation = PlayAnimation.Kneel;
        }
        else if (playerRescueOperation != null && playerRescueOperation.IsKneelingDown)
        {
            nextAnimation = PlayAnimation.KneelingDown;
        }
        // 銃を構えている（IsAiming が true）かつ、ナイフを持っていないときだけピストル系にする
        else if (playerController.IsAiming && !isHoldingKnife)
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
        else if (isHoldingKnife && playerController.IsMoving)
        {
            nextAnimation = PlayAnimation.KnifeWalk;
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
    
    public void SetHoldingKnife(bool value)
    {
        isHoldingKnife = value;
        Debug.Log($"[Knife] SetHoldingKnife={value} / prefab={(knifePrefab != null)} / hand={(rightHandBone != null)}");
        // 切り替えた瞬間に手へ出す・消す
        if (isHoldingKnife)
        {
            if (spawnedKnife == null && knifePrefab != null && rightHandBone != null)
            {
                spawnedKnife = Instantiate(knifePrefab, rightHandBone);
                spawnedKnife.transform.localPosition = Vector3.zero;
                spawnedKnife.transform.localRotation = Quaternion.identity;
                Debug.Log($"[Knife] 生成しました: {spawnedKnife.name}");
            }
        }
        else if (spawnedKnife != null)
        {
            Destroy(spawnedKnife);
        }
    }

    public void PlaySlash()
    {
        if (isAttacking) return;

        int hash = Animator.StringToHash("SlashAdvance");
        if (!animator.HasState(0, hash))
        {
            Debug.LogWarning("[Slash] Animator に SlashAdvance というステートがありません");
            return;
        }

        isAttacking = true;
        attackTimer = slashDuration;
        currentAnimation = PlayAnimation.SlashAdvance;
        animator.Play(hash, 0, 0f);
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

        if (isHoldingKnife)
        {
            if (spawnedKnife == null && knifePrefab != null && rightHandBone != null)
            {
                spawnedKnife = Instantiate(knifePrefab, rightHandBone);
                spawnedKnife.transform.localPosition = Vector3.zero;
                spawnedKnife.transform.localRotation = Quaternion.identity;
            }
        }
        else if (spawnedKnife != null)
        {
            Destroy(spawnedKnife);
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
            case PlayAnimation.SlashAdvance:
                animator.Play("SlashAdvance");
                break;
            case PlayAnimation.KnifeWalk:
                animator.Play("KnifeWalk");
                break;
        }
    }
}