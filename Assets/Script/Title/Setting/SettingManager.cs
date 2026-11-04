using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using UnityEngine.EventSystems;

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

    [Header("開く・戻る")]
    [SerializeField] GameObject settingsPanel;       //設定画面のパネル
    [SerializeField] GameObject firstSelectedButton; //設定画面を開いた時に最初に選ぶボタン
    [SerializeField] GameObject defaultReturnPanel;  //戻り先の指定が無い時に戻るパネル
    [SerializeField] GameObject defaultReturnButton; //その時に選ぶボタン

    //カメラ感度(PlayerLookがSettingMangerとLookSensitivityで参照)
    public float LookSensitivity { get; private set; } = 0.1f;

    //解像度の候補リスト(Dropdownの番号と対応)
    private readonly List<Vector2Int> resolutionList = new List<Vector2Int>();

    private ColorGrading colorGrading;

    private GameObject returnPanel;    //戻り先パネル
    private GameObject returnButton;   //戻った時に選ぶボタン

    public const string FovPrefsKey = "Fov";
    public const string SensitivityPrefsKey = "LookSensitivity";
    public const string BrightnessPrefsKey = "BrightnessPostExposure";

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
        float sens = PlayerPrefs.GetFloat(SensitivityPrefsKey, sensitivitySlider.value);
        sensitivitySlider.SetValueWithoutNotify(sens);
        LookSensitivity = sens;
        sensitivitySlider.onValueChanged.AddListener(value =>
        {
            LookSensitivity = value;
            PlayerPrefs.SetFloat(SensitivityPrefsKey, value); //保存
        });
    }

    //視野角(FOV)
    private void SetupFov()
    {
        if (playerCamera == null) playerCamera = Camera.main;

        float fov = PlayerPrefs.GetFloat(FovPrefsKey, fovSlider.value);
        fovSlider.SetValueWithoutNotify(fov);
        ApplyFov(fov);

        fovSlider.onValueChanged.AddListener(value =>
        {
            ApplyFov(value);
            PlayerPrefs.SetFloat(FovPrefsKey, value); //保存

        });
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

    //保存した設定を消して、初期値に戻す
    public static void ResetSavedSettings()
    {
        PlayerPrefs.DeleteKey(FovPrefsKey);
        PlayerPrefs.DeleteKey(SensitivityPrefsKey);
        PlayerPrefs.DeleteKey(BrightnessPrefsKey);

        PlayerPrefs.Save();
    }

    //ゲーム終了時に呼び出される
    private void OnApplicationQuit()
    {
        ResetSavedSettings();
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

    //戻り先を指定して設定画面を開く(ポーズ画面から使う)
    public void Open(GameObject panel, GameObject button)
    {
        returnPanel = panel;
        returnButton = button;

        if (returnPanel != null)
            returnPanel.SetActive(false);

        settingsPanel.SetActive(true);
        Select(firstSelectedButton);
    }

    //戻り先なしで開く(タイトルから使う)
    public void Open()
    {
        Open(defaultReturnPanel, defaultReturnButton);
    }

    //設定画面のExitボタンに設定する
    public void OnExit()
    {
        settingsPanel.SetActive(false);

        if(returnPanel != null)
        {
            returnPanel.SetActive(true);
            Select(returnButton);
        }
    }

    //EventSystemで選択状態にする
    private void Select(GameObject target)
    {
        if (EventSystem.current == null || target == null) return;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(target);
    }
}
