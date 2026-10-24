using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingButton : MonoBehaviour
{
    public AudioClip seClip;
    [SerializeField] AudioSource seSource;

    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject settingsPanel;

    public void OnClickSettings()
    {
        //if (seClip != null && seSource != null)
            seSource.PlayOneShot(seClip);

        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
}
