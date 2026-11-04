using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
/// <summary>
/// ナイフ専用スクリプト
/// Qキーでの攻撃、範囲判定、ダメージ処理を担当
/// </summary>

public class PlayerKnifeOperater : MonoBehaviour
{
    [Header("--- ナイフの設定 ---")]
    [SerializeField] private float attackInterval = 0.5f;
    [Min(0.1f)]
    [SerializeField] private float knifeRange = 2.0f;
    [SerializeField] private int knifeDamage = 25;

    [Header("--- 当たり判定用設定 ---")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;

    [Header("--- アニメーション連携 ---")]
    [SerializeField] private PlayerAnimation playerAnimation;

    private float timeSinceLastAttack = 0f;

    private void Start()
    {
        if (playerAnimation == null)
            playerAnimation = GetComponent<PlayerAnimation>();
    }

    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwingKnife();
        }
    }

    // Qを押したときは、アニメを再生するだけ
    private void SwingKnife()
    {
        if (playerAnimation != null && playerAnimation.IsAttacking)
        {
            Debug.Log("[Knife] 1. 斬撃中なので無視");
            return;
        }
        if (timeSinceLastAttack < attackInterval)
        {
            Debug.Log("[Knife] 1. クールダウン中なので無視");
            return;
        }
        timeSinceLastAttack = 0f;

        bool played = playerAnimation != null && playerAnimation.PlaySlash();
        Debug.Log($"[Knife] 2. アニメ再生 played={played}");

        // アニメが再生できなかったときだけ、その場で判定する
        if (!played) OnKnifeHit();
    }

    public void OnKnifeHit()
    {
        Debug.Log($"[Knife] 3. OnKnifeHit が呼ばれた enabled={enabled}"); // ← 無効のときも出る位置に置く
        if (!enabled) return;

        Vector3 center = (attackPoint != null) ? attackPoint.position : transform.position + transform.forward;
        Collider[] hitEnemies = Physics.OverlapSphere(center, knifeRange, enemyLayer);
        Debug.Log($"[Knife] 4. ヒット判定 コライダー数: {hitEnemies.Length}");

        foreach (Collider enemy in hitEnemies)
        {
            DamageHelper.Apply(enemy, knifeDamage);
        }
    }
    //private void SwingKnife()
    //{
    //    if (playerAnimation != null && playerAnimation.IsAttacking) return;
    //    if (timeSinceLastAttack < attackInterval) return;
    //    timeSinceLastAttack = 0f;

    //    bool played = playerAnimation != null && playerAnimation.PlaySlash();

    //    // アニメが再生できなかったときだけ、その場で判定する
    //    if (!played) OnKnifeHit();
    //}

    //// 斬撃クリップのアニメーションイベントから呼ばれる(public)
    //public void OnKnifeHit()
    //{
    //    if (!enabled) return;

    //    Vector3 center = (attackPoint != null) ? attackPoint.position : transform.position + transform.forward;
    //    Collider[] hitEnemies = Physics.OverlapSphere(center, knifeRange, enemyLayer);
    //    Debug.Log($"[Knife] ヒット判定 コライダー数: {hitEnemies.Length}");

    //    foreach (Collider enemy in hitEnemies)
    //    {
    //        DamageHelper.Apply(enemy, knifeDamage);
    //    }
    //}

    private void OnDrawGizmosSelected()
    {
        Vector3 center = (attackPoint != null) ? attackPoint.position : transform.position + transform.forward;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, knifeRange);
    }
}