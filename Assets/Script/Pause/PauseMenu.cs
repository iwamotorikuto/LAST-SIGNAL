using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;                //ポーズパネル
    [SerializeField] GameObject firstSelectedButton;       //ポーズを開いた時に最初に選ぶボタン
    [SerializeField] GameObject settingsPanel;             //設定画面のパネル
    [SerializeField] SettingManager settingManager;        //設定画面のマネージャー
    [SerializeField] string titleSceneName = "TitleScene"; //タイトルシーンの名前
    [SerializeField] bool lockCursorInGame = true;         //ゲーム中にカーソルを隠すか

    private bool isPaused = false; //ポーズ中かどうかのフラグ

    private void Start()
    {
        //開始時はポーズ画面を閉じて、時間を通常に戻す
        pausePanel.SetActive(false);
        Time.timeScale = 1.0f;
    }

    private void Update()
    {
        //設定画面を開いている間はポーズ画面の切り替えをしない
        if (settingsPanel != null && settingsPanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    //ポーズ開始
    private void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0.0f;

        //カーソルの表示
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    //再開ボタン
    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1.0f;

        //カーソルの非表示
        if (lockCursorInGame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    //設定ボタン
    public void OpenSettings()
    {
        settingManager.Open(pausePanel, firstSelectedButton);
    }

    //タイトルに戻るボタン
    public void BackToTitle()
    {
        Time.timeScale = 1.0f;  //止まったままシーンを移動しないように戻す
        SceneManager.LoadScene(titleSceneName);
    }

}
