using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalManager : MonoBehaviour
{
    [Header("クリア条件")]
    [SerializeField] private int requiredCount = 3; //クリアするために必要なNPCの数

    [Header("クリア")]
    [SerializeField] private string clearSceneName = "GameClearScene"; //クリア画面のシーン
    [SerializeField] private float clearDelay      = 3.0f;             //クリア条件を満たしてから遷移するまでの時間

    private int arrivedCount = 0; //現在の到達数
    private bool isCleared;       //クリア済みかどうか

    //NPCがゴールに着いたときに呼ばれる
    public void ReportArrived()
    {
        if (isCleared) return;

        arrivedCount++;

        if(arrivedCount >= requiredCount)
        {
            isCleared = true;
            Invoke(nameof(GoClear), clearDelay);
        }
    }

    void GoClear()
    {
        SceneManager.LoadScene(clearSceneName);
    }
}
