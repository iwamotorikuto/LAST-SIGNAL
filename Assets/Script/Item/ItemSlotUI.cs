using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [Header("--- この枠が見るアイテム ---")]
    [SerializeField] private ItemData itemData;

    [Header("--- 表示 ---")]
    [SerializeField] private Image iconImage;      // アイコン(枠の子の Image)
    [SerializeField] private TMP_Text countText;   // 個数の文字
    [SerializeField] private PlayerInventory inventory;

    [Header("--- 弾の枠だけ設定 ---")]
    [SerializeField] private PlayerPistolOperater pistol;
    [SerializeField] private GameObject selectedMark; // 選んでいる弾だけ表示

    private void Awake()
    {
        if (inventory == null) inventory = FindObjectOfType<PlayerInventory>();
        if (pistol == null) pistol = FindObjectOfType<PlayerPistolOperater>(true);
    }

    private void OnEnable()
    {
        if (inventory != null) inventory.OnChanged += OnChanged;
    }

    private void OnDisable()
    {
        if (inventory != null) inventory.OnChanged -= OnChanged;
    }

    private void Start()
    {
        if (itemData == null)
        {
            Debug.LogWarning($"[ItemSlotUI] {name} に ItemData が入っていません");
            return;
        }

        // アイコンは ItemData から取る
        if (iconImage != null) iconImage.sprite = itemData.icon;

        // 初期表示は Start で行う(インベントリの初期化が終わったあとに読むため)
        if (inventory != null) Refresh(inventory.Count(itemData.type));
    }

    private void Update()
    {
        // 弾の枠: 今選んでいる弾だけ、印を出す
        if (itemData != null && pistol != null && selectedMark != null)
        {
            selectedMark.SetActive(pistol.CurrentAmmoType == itemData.type);
        }
    }

    private void OnChanged(ItemType type, int count)
    {
        if (itemData != null && type == itemData.type) Refresh(count);
    }

    private void Refresh(int count)
    {
        bool has = count > 0;
        if (iconImage != null) iconImage.enabled = has;        // 持っているときだけ、アイコンを出す
        if (countText != null) countText.text = has ? count.ToString() : "";
    }
}