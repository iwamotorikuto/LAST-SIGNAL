using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor.ShaderGraph.Internal;

public class BlinkOnSelect : MonoBehaviour
{
    //点滅させる文字
    [SerializeField] private TextMeshProUGUI targetText;
    //点滅の速さ
    [SerializeField] private float blinkSpeed = 3.0f;
    //1番薄くなった時の透明度　
    [SerializeField] private float minAlpha = 0.3f;

    void Update()
    {
        //自分が選択中かどうか
        bool isSelected = EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject;

        if(isSelected)
        {
            float t = Mathf.PingPong(Time.unscaledTime * blinkSpeed, 1.0f);
            targetText.alpha = Mathf.Lerp(minAlpha, 1.0f, t);
        }
        else
        {
            //選択されていない時は通常の濃さ
            targetText.alpha = 1.0f;
        }
    }
}
