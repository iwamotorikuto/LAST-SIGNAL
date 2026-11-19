using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    [Header("UI")]
    public Slider progressBar;

    [Header("霧・暗転演出")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private Color startFogColor = new Color(0.75f, 0.8f, 0.85f);
    [SerializeField] private float startFogDensity = 0.02f;
    [SerializeField] private float endFogDensity = 0.05f;
    [SerializeField] private float darkenDuration = 5.0f;

    private float initialLightIntensity;
    private float initialAmbientIntensity;
    private Camera targetCamera;

    IEnumerator Start()
    {
        SetupInitialFogAndLight();

        //ロード画面へのフェードインを待つ
        while (FadeManager.Instance.IsFading)
        {
            yield return null;
        }

        progressBar.value = 0.0f;

        yield return StartCoroutine(LoadSceneAsync());
    }

    private void SetupInitialFogAndLight()
    {
        //初期の霧とライトの設定
        targetCamera = Camera.main;

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogColor = startFogColor;
        RenderSettings.fogDensity = startFogDensity;

        initialLightIntensity = RenderSettings.ambientIntensity;

        if(targetCamera != null)
        {
            targetCamera.clearFlags = CameraClearFlags.SolidColor;
            targetCamera.backgroundColor = startFogColor;
        }

        if (directionalLight != null)
        {
            initialLightIntensity = directionalLight.intensity;
        }
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("GameManagerScene");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;

            if (progress >= 1.0f)
            {
                //ロード完了後に1秒間待つ
                yield return new WaitForSecondsRealtime(1.0f);

                //5秒間かけて霧を濃くして、画面を暗くする
                yield return StartCoroutine(DarkenWithFog());

                //最後にUIのフェードをで完全に暗くする
                yield return FadeManager.Instance.FadeOut();

                operation.allowSceneActivation = true;

                FadeManager.Instance.StartCoroutine(FadeManager.Instance.FadeIn()); //切り替え後に明るく戻す
            }
            yield return null;
        }
    }

    private IEnumerator DarkenWithFog()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < darkenDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / darkenDuration);

            Color fogColor = Color.Lerp(startFogColor, Color.black, t);

            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = Mathf.Lerp(startFogDensity, endFogDensity, t);

            RenderSettings.ambientIntensity = Mathf.Lerp(initialAmbientIntensity, 0.0f, t);

            if(targetCamera != null)
            {
                targetCamera.backgroundColor = fogColor;
            }

            if(directionalLight != null)
            {
                directionalLight.intensity = Mathf.Lerp(initialAmbientIntensity, 0.0f, t);
            }

            yield return null;
        }
    }
}
