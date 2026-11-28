using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalManager : MonoBehaviour
{
    [Header("クリア条件")]
    [SerializeField] private int requiredCount = 3; //クリアするために必要なNPCの数

    [Header("表示")]
    [SerializeField] private TextMeshProUGUI countText; //数字側のテキスト

    [Header("クリア")]
    [SerializeField] private string clearSceneName = "GameClearScene"; //クリア画面のシーン
    [SerializeField] private float clearDelay      = 3.0f;             //クリア条件を満たしてから遷移するまでの時間

    private int arrivedCount = 0; //現在の到達数
    private bool isCleared;       //クリア済みかどうか

    private void Start()
    {
        UpdateText();
    }

    //NPCがゴールに着いたときに呼ばれる
    public void ReportArrived(int amount = 1)
    {
        if (isCleared) return;
        if (amount <= 0) return;

        arrivedCount = Mathf.Min(arrivedCount + amount, requiredCount);
        UpdateText();

        if(arrivedCount >= requiredCount)
        {
            isCleared = true;
            Invoke(nameof(GoClear), clearDelay);
        }
    }

    private void UpdateText()
    {
        countText.text = arrivedCount + "/" + requiredCount;
    }

    void GoClear()
    {
        SceneManager.LoadScene(clearSceneName);
    }
}
