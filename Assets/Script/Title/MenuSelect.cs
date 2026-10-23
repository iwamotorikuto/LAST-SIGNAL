using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuSelect : MonoBehaviour
{
    [SerializeField] TMP_Text targetText;     //選択されたテキストが色を変える
    [SerializeField] float blinkSpeed = 3.0f; //点滅の速度
    [SerializeField] float minAlpha   = 0.2f; //一番暗い時の透明度

    private void Awake()
    {
        if (targetText == null)
            targetText = GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        //EventSystemが選んでいる状態のみ点滅
        bool isSelected = EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject;

        if (isSelected)
        {
            float t = Mathf.PingPong(Time.unscaledTime * blinkSpeed, 1.0f);
            SetAlpha(Mathf.Lerp(minAlpha, 1.0f, t));
        }
        else
        {
            SetAlpha(1.0f);
        }
    }

    private void SetAlpha(float alpha)
    {
        Color color = targetText.color;
        color.a = alpha;
        targetText.color = color;
    }
}

