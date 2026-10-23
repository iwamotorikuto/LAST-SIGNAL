using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartButton : MonoBehaviour
{
    public AudioClip seClip;
    public float waitTime = 0.3f; //メニューに切り替わるまでの待機時間

    [SerializeField] GameObject titleObject;
    [SerializeField] GameObject menuObject;
    [SerializeField] GameObject firstButton;

    private bool isStarting = false;

    private void Update()
    {
        if (isStarting) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnClickStart();
        }
    }

    public void OnClickStart()
    {
        if (isStarting) return;
        isStarting = true;

        if (seClip != null)
            AudioSource.PlayClipAtPoint(seClip, Camera.main.transform.position);

        StartCoroutine(WaitAndOpenMenu());
    }

     
    private IEnumerator WaitAndOpenMenu()
    {
        yield return new WaitForSeconds(waitTime);

        titleObject.SetActive(false);
        menuObject.SetActive(true);

        //1フレームを待つ
        yield return null;
        EventSystem.current.SetSelectedGameObject(firstButton);
    }
}
