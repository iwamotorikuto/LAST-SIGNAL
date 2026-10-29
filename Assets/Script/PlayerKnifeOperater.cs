using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
/// <summary>
/// ナイフ専用スクリプト
/// Jキーでの攻撃、範囲判定、ダメージ処理を担当
/// </summary>
public class PlayerKnifeOperater : MonoBehaviour
{
    [Header("--- ナイフの設定 ---")]
    [SerializeField] private float attackInterval = 0.5f; // 攻撃間隔（クールダウン）
    [SerializeField] private float knifeRange = 2.0f;     // ナイフの届く距離
    [SerializeField] private int knifeDamage = 25;       // ナイフの攻撃力

    [Header("--- 当たり判定用設定 ---")]
    [SerializeField] private Transform attackPoint;       // ナイフの判定中心
    [SerializeField] private LayerMask enemyLayer;        // 敵を検知するレイヤー

    private float timeSinceLastAttack = 0f;

    [Header("--- アニメーション連携 ---")]
    [SerializeField] private PlayerAnimation playerAnimation; // プレイヤーアニメーションの参照

    private void Start()
    {
        if (playerAnimation == null)
        {
            playerAnimation = GetComponent<PlayerAnimation>();
        }
    }

    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;

        // Jキーを押したときにナイフ攻撃を実行
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q Press");
            SwingKnife();
        }
    }
   

    /// <summary>
    /// ナイフを振る攻撃処理
    /// </summary>
    private void SwingKnife()
    {
       
        Debug.Log("SwingKnife Start");
        // クールダウン中のチェック
        if (timeSinceLastAttack < attackInterval) return;
        timeSinceLastAttack = 0f;

        if (playerAnimation != null) playerAnimation.PlaySlash();
        Debug.Log("ナイフで斬りつけた！");

        // 1. 判定基準となる位置を決める
        Vector3 center = (attackPoint != null) ? attackPoint.position : transform.position + transform.forward;

        // 2. 範囲内の敵を球状の当たり判定（OverlapSphere）で検知する
        Collider[] hitEnemies = Physics.OverlapSphere(center, knifeRange, enemyLayer);
        
        Debug.Log($"[Knife] 範囲内のコライダー数: {hitEnemies.Length} / mask={enemyLayer.value}");

        // 3. ヒットした敵に対してダメージを与える
        foreach (Collider enemy in hitEnemies)
        {
            DamageHelper.Apply(enemy, knifeDamage);
        }


    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = (attackPoint != null) ? attackPoint.position : transform.position + transform.forward;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, knifeRange);
    }

}
