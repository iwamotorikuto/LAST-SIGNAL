using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public Slider progressBar;

    IEnumerator Start()
    {
        //ロード画面へのフェードインを待つ
        while (FadeManager.Instance.IsFading)
        {
            yield return null;
        }

        progressBar.value = 0.0f;
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("GameManagerScene");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;

            if(progress >= 1.0f)
            {
                //ここで少し待ったり演出することが出来る
                yield return new WaitForSeconds(1.0f);
                yield return FadeManager.Instance.FadeOut();
                operation.allowSceneActivation = true;
                FadeManager.Instance.StartCoroutine(FadeManager.Instance.FadeIn()); //切り替え後に明るく戻す
            }

            yield return null;
        }
    }
}
