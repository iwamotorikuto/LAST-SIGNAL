using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingSelectList : MonoBehaviour
{
    [SerializeField] GameObject[] categoryButtons;
    [SerializeField] GameObject[] contentPanels;
    [SerializeField] GameObject[] firstItems;

    private bool isInContent = false;

    private void OnEnable()
    {
        ShowPanel(0);
        isInContent = false;
        StartCoroutine(SelectAfterFrame(categoryButtons[0]));
    }

    private void Update()
    {
        //Escapeキーまたは、XboxのコントローラーのBボタンで戻る
        if(isInContent && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1)))
        {

            ReturnToCategory();
        }
        
    }

    //カテゴリボタンのOnClickイベントに設定する
    public void OpenCategory(int index)
    {
        ShowPanel(index);

        //中身が無いページの場合は選択カテゴリを操作したまま
        if (firstItems[index] == null) return;

        isInContent = true;
        StartCoroutine(SelectAfterFrame(firstItems[index]));
    }

    private void ReturnToCategory()
    {
        isInContent = false;
        
        //今開いているページに対応しているカテゴリに選び直す
        for(int i = 0; i < contentPanels.Length; i++)
        {
            if (contentPanels[i].activeSelf)
            {
                StartCoroutine(SelectAfterFrame(categoryButtons[i]));
                break;
            }
        }
    }

    private void ShowPanel(int index)
    {
        for(int i = 0; i < contentPanels.Length; i++)
        {
            contentPanels[i].SetActive(i == index);
        }
    }
    private IEnumerator SelectAfterFrame(GameObject target)
    {
        yield return null; //1フレーム待つ
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(target);
    }

}
