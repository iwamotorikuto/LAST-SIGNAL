///
///プレイヤーの救助操作を管理するスクリプト
///
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

////////////////////////////////////////////
///操作方法
///救助状態へ移行 : Eキー　または　ゲームパッド　（Xボタン）
///////////////////////////////////////////


/// <summary>
/// 救助状態を管理する列挙型
/// </summary>
enum RescueState
{
    Idle,       // 待機状態
    Searching,  // 捜索状態
    Rescuing,   // 救助状態
    Kneeling,   // ひざまずき状態
    Completing  // 救助完了状態
}

public class PlayerRescueOperation : MonoBehaviour
{
    [SerializeField] private RescueState currentState = RescueState.Idle;
    [SerializeField] private float searchDuration = 5.0f; // 捜索・救助にかかる時間

    // フラグ管理
    public bool IsRescuing { get; private set; }

    

    void Start()
    {

    }

    void Update()
    {
        // 待機中、Eキーでひざまずき状態へ移行
        if (currentState == RescueState.Idle && Input.GetKeyDown(KeyCode.E))
        {
            ChangeState(RescueState.Kneeling);
        }

        // ゲームパッドのXボタンでひざまずき状態へ移行
        if (currentState == RescueState.Idle && Gamepad.current != null && Gamepad.current.buttonNorth.wasPressedThisFrame)
        {
            ChangeState(RescueState.Kneeling);
        }

        RescueTimerCoroutine();
        CompleteRescue();

    }

   

    private void ChangeState(RescueState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case RescueState.Idle:
                IsRescuing = false;
                break;
            case RescueState.Searching:
                IsRescuing = true;
                break;
            case RescueState.Rescuing:
                IsRescuing = true;
                break;
            case RescueState.Kneeling:
                IsRescuing = true;
                break;
            case RescueState.Completing:
                IsRescuing = false;
                break;
        }
    }

    /// <summary>
    /// 一定時間待ったあとで次の状態（完了など）へ進めるコルーチン
    /// </summary>
    private IEnumerator RescueTimerCoroutine()
    {
        // searchDuration（設定された秒数）だけ待つ
        yield return new WaitForSeconds(searchDuration);

        // 待機時間が終わったら救助完了状態へ移行
        ChangeState(RescueState.Completing);
    }

    /// <summary>
    /// シェルターに到着した後の治療処理
    /// </summary>
    void CompleteRescue()
    {
        if(Input.GetKey(KeyCode.J))
        {
            // ここで治療処理を行う（未実装）
            Debug.Log("Rescue operation completed. Player is now safe.");
        }
    }
}
