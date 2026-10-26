using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackButton : MonoBehaviour
{
    public AudioClip seClip;
    [SerializeField] AudioSource seSource;

    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject settingsButton; //戻った時に選択をし直すことが出来るボタン

    public void OnClickBack()
    {
        //if (seClip != null && seSource != null)
            seSource.PlayOneShot(seClip);

        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(settingsButton);
    }
}
