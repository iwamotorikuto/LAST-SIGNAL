using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//タイトルシーンからゲームシーンに遷移する際のフェードイン・アウトを管理する
public class FadeManager : MonoBehaviour
{
    //どこからでもアクセスできるようにする
    public static FadeManager Instance { get; private set; }

    [SerializeField] private CanvasGroup fadeGroup;  //黒画像
    [SerializeField] private float fadeTime = 1.0f;  //フェードインにかける秒数

    private bool isFading = false;  //フェードイン中に二重で実行するのを防ぐ
    void Awake()
    {
        //すでに存在していたら自分を消す(シーンを戻ったときの重複防止)
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);  //シーンをまたいで残す

        //最初は透明にしておく
        fadeGroup.alpha = 0.0f;
        fadeGroup.blocksRaycasts = false;
    }

    //外から関数:フェードイン→シーン読み込み→フェードアウト
    public void LoadScene(string sceneName)
    {
        if (isFading) return;
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        isFading = true;
        fadeGroup.blocksRaycasts = true;  // フェード中はボタンを押せなくなる

        //画面を暗くする
        yield return Fade(0.0f, 1.0f);

        //シーンを読み込む
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }

        //画面を明るくする
        yield return Fade(1.0f, 0.0f);

        fadeGroup.blocksRaycasts = false;
        isFading = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        float time = 0.0f;
        while (time < fadeTime)
        {
            time += Time.unscaledDeltaTime;
            fadeGroup.alpha = Mathf.Lerp(from, to, time / fadeTime);
            yield return null;
        }
        fadeGroup.alpha = to;
    }
}
