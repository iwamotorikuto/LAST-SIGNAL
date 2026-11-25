using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameOverUI : MonoBehaviour
{
    //文字とボタンをまとめた親のCanvasGroup
    [SerializeField] private CanvasGroup gameOverGroup;
    //最初に選択状態にするボタン
    [SerializeField] private GameObject firstButton;
    //死亡時に止めるスクリプト
    [SerializeField] private MonoBehaviour[] disableScripts;
    //死亡時に消したいオブジェクト
    [SerializeField] private GameObject[] hideObjects;
    //画面が赤くなるまでの待ち時間
    [SerializeField] private float waitTime = 2.0f;
    //文字が出るまでの時間
    [SerializeField] private float fadeTime = 1.5f;

    private bool isShown = false;

    private void Awake()
    {
        //起動時は必ず透明にして、押せない状態にする
        gameOverGroup.alpha = 0.0f;
        gameOverGroup.interactable = false;
        gameOverGroup.blocksRaycasts = false;
    }

    public void Show()
    {
        if (isShown) return;
        isShown = true;

        //HUDを消す
        foreach(GameObject obj in hideObjects)
        {
            obj.SetActive(false);
        }

        //プレイヤーの操作を止める
        foreach (MonoBehaviour script in disableScripts)
        {
            script.enabled = false;
        }

        //カーソルを表示してロック解除
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        yield return new WaitForSecondsRealtime(waitTime);

        //alphaを0から1に増やす
        float timer = 0.0f;
        while (timer < fadeTime)
        {
            timer += 0.0f;
            while (timer < fadeTime)
            {
                timer += Time.unscaledDeltaTime;
                gameOverGroup.alpha = timer / fadeTime;
                yield return null;
            }
            //端数のずれを防ぐ
            gameOverGroup.alpha = 1.0f;

            gameOverGroup.interactable = true;
            gameOverGroup.blocksRaycasts = true;

            //1フレーム待ってから、Retryを選択状態にする
            yield return null;
            EventSystem.current.SetSelectedGameObject(firstButton);

            //演出が終わったら最後に時間を止める
            Time.timeScale = 0.0f;
        }
    }
}
