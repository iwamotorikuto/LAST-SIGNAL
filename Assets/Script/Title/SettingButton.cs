using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingButton : MonoBehaviour
{
    public AudioClip seClip;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject firstSettingsItem; //設定画面で最初に選択されるボタン

    public void OnClickSettings()
    {
        if (seClip != null)
            AudioSource.PlayClipAtPoint(seClip, Camera.main.transform.position);

        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        StartCoroutine(SelectItem());
    }

    private IEnumerator SelectItem()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(firstSettingsItem);
    }
}
