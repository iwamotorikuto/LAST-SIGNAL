using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverButton : MonoBehaviour
{
    //タイトルシーンの名前
    [SerializeField] private string titleSceneName = "TitleScene";

    //Retryボタンから呼ぶ
    public void OnClickRetry()
    {
        //止めていた時間に戻す
        Time.timeScale = 1.0f;
        //今のシーンを読み込み直す
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Titleボタンから呼ぶ
    public void OnClickTitle()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(titleSceneName);
    }
}
