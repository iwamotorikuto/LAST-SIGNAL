using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using System.Runtime.CompilerServices;

//設定画面の管理クラス
//解像度/FPS上限/感度/視野角/明るさ
public class SettingManager : MonoBehaviour
{
    //他のスクリプトからアクセスできるようにインスタンで参照する
    public static SettingManager Instance { get; private set; }

    [Header("画面・パフォーマンス")]
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] Slider fpsSlider;

    [Header("操作")]
    [SerializeField] Slider sensitivitySlider;

    [Header("グラフィック")]
    [SerializeField] Camera playerCamera;      //FOVの変更をするカメラ
    [SerializeField] Slider fovSlider;         //視野角Slider
    [SerializeField] Slider brightnessSlider;  //明るさを調整
    [SerializeField] PostProcessVolume postProcessVolume;  

    //カメラ感度(PlayerLookがSettingMangerとLookSensitivityで参照)
    public float LookSensitivity { get; private set; } = 0.1f;

    //解像度の候補リスト(Dropdownの番号と対応)
    private readonly List<Vector2Int> resolutionList = new List<Vector2Int>();

    private ColorGrading colorGrading;

    private const string BrightnessPrefsKey = "BrightnessPostExposure";

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetupResolution();   //解像度
        SetupFps();          //FPS制限
        SetupSensitivity();  //感度
        SetupFov();          //視野角
        SetupBrightness();   //明るさ
    }

    //解像度
    private void SetupResolution()
    {
        resolutionDropdown.ClearOptions();
        resolutionList.Clear();

        var labels = new List<string>();
        int currentIndex = 0;

        foreach (Resolution r in Screen.resolutions)
        {
            //リフレッシュレート違いでの重複を防ぐために、縦横は1つにまとめる
            Vector2Int size = new Vector2Int(r.width, r.height);
            if(resolutionList.Contains(size)) continue;

            resolutionList.Add(size);
            labels.Add($"{r.width} x {r.height}");

            //現在の解像度を初期選択にする
            if(r.width == Screen.width && r.height == Screen.height)
            {
                currentIndex = resolutionList.Count - 1;
            }
        }

        resolutionDropdown.AddOptions(labels);
        resolutionDropdown.SetValueWithoutNotify(currentIndex);
        resolutionDropdown.RefreshShownValue();

        //Dropdownの選択が変わった時の処理
        resolutionDropdown.onValueChanged.AddListener(index =>
        {
            if (index < 0 || index >= resolutionList.Count) return;
            Vector2Int size = resolutionList[index];
            //エディタ上のGameビューでは変わらない。ビルドで確認をする
            Screen.SetResolution(size.x, size.y, Screen.fullScreenMode);
        });
    }

    //FPS制限
    private void SetupFps()
    {
        ApplyFps(fpsSlider.value);
        fpsSlider.onValueChanged.AddListener(ApplyFps);
    }

    private void ApplyFps(float value)
    {
        //VSyncが有効だとtargetFrameRateが無視されるため、必ずオフにする必要がある
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Mathf.RoundToInt(value);
    }

    //感度
    private void SetupSensitivity()
    {
        LookSensitivity = sensitivitySlider.value;
        sensitivitySlider.onValueChanged.AddListener(value =>
        {
            LookSensitivity = value;
        });
    }

    //視野角(FOV)
    private void SetupFov()
    {
        ApplyFov(fovSlider.value);
        fovSlider.onValueChanged.AddListener(ApplyFov);
    }

    private void ApplyFov(float value)
    {
        if (playerCamera == null) return;
        playerCamera.fieldOfView = value;
    }

    //明るさ
    private void SetupBrightness()
    {
        if (brightnessSlider == null) return;

        //プロファイルからColorGradingを取得
        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            postProcessVolume.profile.TryGetSettings(out colorGrading);
        }

        float brightness = PlayerPrefs.GetFloat(
            BrightnessPrefsKey,
            brightnessSlider.value);

        brightnessSlider.SetValueWithoutNotify(brightness);
        ApplyBrightness(brightness);

        //初期値の保存をし、ゲーム側との食い違いを防ぐ
        PlayerPrefs.SetFloat(BrightnessPrefsKey, brightness);
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
    }

    private void OnBrightnessChanged(float value)
    {
        ApplyBrightness(value);
        PlayerPrefs.SetFloat(BrightnessPrefsKey, value);
    }

    private void OnDisable()
    {
        //画面を離れるときに明るさを保存にする
        PlayerPrefs.Save();
    }

    private void ApplyBrightness(float value)
    {
        if (colorGrading == null) return;
        //ポストエクスポージャーで明るさを調整する
        colorGrading.postExposure.overrideState = true;
        colorGrading.postExposure.value = value;
    }
}
