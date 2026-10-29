using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SettingSelectList : MonoBehaviour
{
    [SerializeField] GameObject[] categoryButtons;
    [SerializeField] GameObject[] contentPanels;
    [SerializeField] GameObject[] firstItems;

    [Header("項目の行(Buttonのついているもの)")]
    [SerializeField] GameObject[] rows;

    private GameObject[] editTargets;  //行の中から自動で見つけたものを操作するための部品
    private bool isInContent = false;
    private bool isEditing = false;    //値の操作をしている
    private GameObject editingRow;     //操作中の項目

    private void Awake()
    {
        SetupCategoryButtons();
        SetupRows();
    }

    private void SetupCategoryButtons()
    {
        for(int i = 0; i < categoryButtons.Length; i++)
        {
            int index = i;
            Button button = categoryButtons[i].GetComponent<Button>();
            if (button != null) button.onClick.AddListener(() => OpenCategory(index));
        }
    }

    //行の決定処理を登録し、操作する部品を探す
    private void SetupRows()
    {
        editTargets = new GameObject[rows.Length];

        for(int i = 0; i < rows.Length; i++)
        {
            int index = i;

            //非表示中のページの中も探すために、trueを指定
            Slider slider = rows[i].GetComponentInChildren<Slider>(true);
            TMP_Dropdown dropdown = rows[i].GetComponentInChildren<TMP_Dropdown>(true);

            if (slider != null) editTargets[i] = slider.gameObject;
            else if (dropdown != null) editTargets[i] = dropdown.gameObject;

            Button button = rows[i].GetComponent<Button>();
            if (button != null) button.onClick.AddListener(() => StartEdit(index));
        }
    }

    private void OnEnable()
    {
        ShowPanel(0);
        isInContent = false;
        isEditing = false;
        StartCoroutine(SelectAfterFrame(categoryButtons[0]));
    }

    private void Update()
    {
        //Escapeキーまたは、XboxのコントローラーのBボタンで戻る
        if(isInContent && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1)))
        {
            if (isEditing)
            {
                //操作を終了し、選択に戻る
                isEditing = false;
                StartCoroutine(SelectAfterFrame(editingRow));
            }
            else if (isInContent)
            {
                ReturnToCategory();
            }
        }
        
    }

    //カテゴリボタンのOnClickイベントに設定する
    public void OpenCategory(int index)
    {
        ShowPanel(index);

        //中身が無いページの場合は選択カテゴリを操作したまま
        if (firstItems[index] == null) return;

        isInContent = true;
        isEditing = false;
        StartCoroutine(SelectAfterFrame(firstItems[index]));
    }

    private void StartEdit(int index)
    {
        if (editTargets[index] == null) return;

        isEditing = true;
        editingRow = rows[index];
        StartCoroutine(SelectAfterFrame(editTargets[index]));
    }

    private void ReturnToCategory()
    {
        isInContent = false;
        isEditing = false;
        
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
