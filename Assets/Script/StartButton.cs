using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public AudioClip seClip;
    public float waitTime;  //ゲームへの切り替えまでの待ち時間の設定(秒)
    

    //ボタンが押されたときに実行する関数
    public void OnClickStart()
    {
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
