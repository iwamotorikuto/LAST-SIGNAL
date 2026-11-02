using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearReturn : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private string titleSceneName = "TitleScene"; //タイトル画面のシーン

    void Start()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Enterキーが押されたときにタイトル画面に遷移する
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SceneManager.LoadScene(titleSceneName);
        }
    }
}
