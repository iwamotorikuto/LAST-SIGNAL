using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteractTrigger : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private float interactRange = 2.0f; // 治療できる距離
    private Transform playerTransform;
    private NPCDown npcDown;

    void Start()
    {
        // 同じNPCについている NPCDown を取得
        npcDown = GetComponent<NPCDown>();

        // "Player" タグがついているオブジェクトを探す
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            playerTransform = p.transform;
        }
        else
        {
            Debug.LogWarning("プレイヤーのタグ（Player）が見つかりません！");
        }
    }

    void Update()
    {
        // NPCコンポーネントがない、またはすでに起き上がっているなら何もしない
        if (npcDown == null || !npcDown.IsDown) return;

        if (playerTransform != null)
        {
            // プレイヤーとの距離を計算
            float distance = Vector3.Distance(transform.position, playerTransform.position);

            // 範囲内にいて、Fキーが押されたら蘇生を実行
            if (distance <= interactRange)
            {
                Keyboard kb = Keyboard.current;
                if (kb != null && kb.fKey.wasPressedThisFrame)
                {
                    npcDown.Revive();
                    Debug.Log("NPCを治療して蘇生させました！");
                }
            }
        }
    }
}