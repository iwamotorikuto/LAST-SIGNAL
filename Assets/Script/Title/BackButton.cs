using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackButton : MonoBehaviour
{
    public AudioClip seClip;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject settingsButton; //戻った時に選択をし直すことが出来るボタン

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnClickBack();
        }
    }

    public void OnClickBack()
    {
        if (seClip != null)
            AudioSource.PlayClipAtPoint(seClip, Camera.main.transform.position);

        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(settingsButton);
    }
}
