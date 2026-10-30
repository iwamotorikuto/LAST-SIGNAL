using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartButton : MonoBehaviour
{
    public AudioClip seClip;
    public float waitTime;   //シーンの切り替え時間
    [SerializeField] string sceneName = "GameManagerScene";

    private bool isStarting = false; //二重実行の防止

    public void OnClickStart()
    {
        if (isStarting) return;
        isStarting = true;

        AudioSource.PlayClipAtPoint(seClip, Camera.main.transform.position);
        StartCoroutine(WaitAndLoadScene());
    }

    private IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSeconds(waitTime);
        FadeManager.Instance.LoadScene(sceneName);
    }
}
