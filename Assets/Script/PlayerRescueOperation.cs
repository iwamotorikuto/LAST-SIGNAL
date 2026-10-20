///
/// プレイヤーの救助操作を管理するスクリプト
///
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum RescueState
{
    Idle,           // 待機状態
    KneelingDown,   // しゃがみ込み（第1段階）
    Rescuing,       // 蘇生・救助中（第2段階）
    Completing      // 救助完了
}

public class PlayerRescueOperation : MonoBehaviour
{
    [Header("--- 救助操作設定 ---")]
    [SerializeField] private RescueState currentState = RescueState.Idle;
    [SerializeField] private float rescueDuration = 5.0f; // 蘇生にかかる時間
    [SerializeField] private float interactRange = 3.0f;  // 救助できる距離

    // アニメーション側から参照するためのフラグ
    public bool IsKneelingDown { get; private set; }
    public bool IsRescuing { get; private set; }

    private NPCDown targetNPC; // 現在ターゲットにしているNPC

    void Update()
    {
        // 近くにいるダウン中のNPCを常に探索
        FindNearestNPC();

        // 1. 待機中：Eキー または ゲームパッドのXボタン でしゃがみ込み状態へ
        if (currentState == RescueState.Idle)
        {
            bool keyInput = Input.GetKeyDown(KeyCode.E);
            bool padInput = Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame;

            if ((keyInput || padInput) && targetNPC != null && targetNPC.IsDown)
            {
                ChangeState(RescueState.KneelingDown);
            }
        }
        // 2. しゃがみ込み中：Fキー または ゲームパッドのAボタン(buttonSouth) で蘇生を開始
        else if (currentState == RescueState.KneelingDown)
        {
            bool startKey = Input.GetKeyDown(KeyCode.F);
            bool startPad = Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame;

            if (startKey || startPad)
            {
                ChangeState(RescueState.Rescuing);
            }
        }
    }

    private void FindNearestNPC()
    {
        NPCDown[] npcs = FindObjectsOfType<NPCDown>();
        float shortestDistance = interactRange;
        NPCDown nearest = null;

        foreach (var npc in npcs)
        {
            if (!npc.IsDown) continue;

            float dist = Vector3.Distance(transform.position, npc.transform.position);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                nearest = npc;
            }
        }
        targetNPC = nearest;
    }

    private void ChangeState(RescueState newState)
    {
        currentState = newState;

        IsKneelingDown = (currentState == RescueState.KneelingDown);
        IsRescuing = (currentState == RescueState.Rescuing);

        switch (currentState)
        {
            case RescueState.Idle:
                Debug.Log("待機状態");
                break;

            case RescueState.KneelingDown:
                Debug.Log("しゃがみ込みました。蘇生ボタン(F または A)を押してください。");
                break;

            case RescueState.Rescuing:
                Debug.Log("蘇生（救助）開始！タイマー計測中...");
                StartCoroutine(RescueTimerCoroutine());
                break;

            case RescueState.Completing:
                Debug.Log("救助完了！");

                if (targetNPC != null)
                {
                    // 1. NPCを蘇生させる
                    targetNPC.Revive();

                    // 2. NPCからプレイヤーへの方向を計算（正方向でプレイヤーと対面する）
                    Vector3 lookDir = transform.position - targetNPC.transform.position;
                    lookDir.y = 0; // 上下を無視して水平方向のみ

                    if (lookDir != Vector3.zero)
                    {
                        // NPCの正面（Z軸）をプレイヤーの座標に向ける
                        targetNPC.transform.rotation = Quaternion.LookRotation(lookDir);
                    }
                }

                ChangeState(RescueState.Idle);
                break;
            
        }
    }

    private IEnumerator RescueTimerCoroutine()
    {
        yield return new WaitForSeconds(rescueDuration);
        ChangeState(RescueState.Completing);
    }
}