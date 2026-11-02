using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

//ゲームマネージャーシーンに置き、保存された設定を読み込んでゲーム内で反省させるクラス
public class GameSettingApplier : MonoBehaviour
{
    //他のクラスからのアクセスが出来るようにインスタンスで参照する
    public static GameSettingApplier Instance { get; private set; }

    [Header("反映先")]
    [SerializeField] Camera playerCamera;                 //FoVを変更するカメラ
    [SerializeField] PostProcessVolume postProcessVolume;

    [Header("保存が無い時の初期値")]
    [SerializeField] float defaultFov = 60.0f;         //視野角の初期値
    [SerializeField] float defaultSensitivity = 0.1f;  //感度の調整
    [SerializeField] float defaultBrightness = 0.0f;   //明るさの初期値

    //カメラの感度
    public float LookSensitivity { get; private set; }

    private void Awake()
    {
        Instance = this;

        LookSensitivity = PlayerPrefs.GetFloat(SettingManager.SensitivityPrefsKey, defaultSensitivity);
    }

    
    void Start()
    {
        ApplyFov();
        ApplyBrightness();
    }


    //視野角　
    private void ApplyFov()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (playerCamera == null) return;

        playerCamera.fieldOfView = PlayerPrefs.GetFloat(SettingManager.FovPrefsKey, defaultFov);
    }

    //明るさ
    private void ApplyBrightness()
    {
        if (postProcessVolume == null) return;
        if (!postProcessVolume.profile.TryGetSettings(out ColorGrading colorGrading)) return;

        float brightness = PlayerPrefs.GetFloat(SettingManager.BrightnessPrefsKey, defaultBrightness);
        colorGrading.postExposure.overrideState = true;
        colorGrading.postExposure.value = brightness;
    }

    //ゲーム終了時に呼ばれる
    private void OnApplicationQuit()
    {
        SettingManager.ResetSavedSettings();
    }


}
