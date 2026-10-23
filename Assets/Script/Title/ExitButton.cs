using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public AudioClip seClip;
    public float waitTime = 0.5f;

    private bool isExiting = false;

    public void OnClickExit()
    {
        if (isExiting) return;
        isExiting = true;

        if (seClip != null)
            AudioSource.PlayClipAtPoint(seClip, Camera.main.transform.position);

        StartCoroutine(WaitAndExit());
    }

    private IEnumerator WaitAndExit()
    {
        yield return new WaitForSeconds(waitTime);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //再生停止

#else
        Application.Quit(); //アプリケーションの停止

#endif
    }
}
