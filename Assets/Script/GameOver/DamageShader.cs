using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageShader : MonoBehaviour
{
    //全面を覆うImage
    [SerializeField] private Image redImage;

    //HPを見る相手
    [SerializeField] private PlayerStats playerState;

    //変化する速さ
    [SerializeField] private float speed = 1.0f;

    //段階の数
    [SerializeField] private int stepCount = 5;

    private Material material;
    private float currentProgress;


    void Awake()
    {
        //マテリアルを複製して、アセット本体を書き換えないようにする
        material = new Material(redImage.material);
        redImage.material = material;
    }
    void Update()
    {
        // t(赤さの割合)を出す。0=HPが満タン,1=HPが0
        float t = 1.0f - (float)playerState.CurrentHP / playerState.MaxHP;

        //tをstepCountの段階に丸める
        float stepT = Mathf.Ceil(t * stepCount) / stepCount;

        //currentProgressをtに向かって少しずつ近づける
        currentProgress = Mathf.MoveTowards(currentProgress, stepT, speed * Time.deltaTime);

        //マテリアルの_Progressに渡す
        material.SetFloat("_Progress", currentProgress);
    }
}
