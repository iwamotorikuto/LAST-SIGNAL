using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider seSlider;
    
    private void Start()
    {
        SetupSlider(masterSlider, "MasterVol");
        SetupSlider(bgmSlider, "BGMVol");
        SetupSlider(seSlider, "SEVol");
    }

    //スライダーが動いた時の処理
    private void SetupSlider(Slider slider, string prameterName)
    {
        float volume = PlayerPrefs.GetFloat(prameterName, 1.0f);
        //小さい値が保存されたとしても、起動時には最大音量に戻す
        if (volume < 0.1f) volume = 1.0f;
        slider.value = volume;
        ApplyVolume(prameterName, volume);

        slider.onValueChanged.AddListener(value =>
        {
            ApplyVolume(prameterName, value);
            PlayerPrefs.SetFloat(prameterName, value);

        });
    }

    //スライダーの値をdB(デシベル)に変換し、Mixerに適用させる
    private void ApplyVolume(string parameterName, float value)
    {
        float db = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20.0f;
        mixer.SetFloat(parameterName, db);
    }
}
