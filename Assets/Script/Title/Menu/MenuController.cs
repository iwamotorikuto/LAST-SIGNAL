using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject startButton;

    public void ShowMenu()
    {
        menuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(startButton);
    }
}
