using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartButton : MonoBehaviour
{
    public AudioClip seClip;
    public float waitTime;  //ゲームへの切り替えまでの待ち時間の設定(秒)

    private bool isStarting = false; //二重実装をされないようにするフラグ

    //毎フレーム呼ばれ、Enterキーが押されたらシーン遷移を行う
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnClickStart();
        }
    }


    //ボタンが押されたときに実行する関数
    public void OnClickStart()
    {
        //連打対策で開始処理中は何もしない
        if (isStarting) return;
        isStarting = true;

        AudioSource.PlayClipAtPoint(seClip, Camera.main.transform.position);

        //音が鳴り終わるまで待ってからシーンの切り替えを行う
        StartCoroutine(WaitAndLoadScene());
    }

    private IEnumerator WaitAndLoadScene()
    {
        //音の長さ待機
        yield return new WaitForSeconds(waitTime);

        SceneManager.LoadScene("GameManagerScene");
    }
}
