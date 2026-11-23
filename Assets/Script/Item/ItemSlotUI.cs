using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [System.Serializable]
    private class SlotView
    {
        public Image frame;        // 枠そのもの(ItemSlotUI1 など)
        public Image icon;         // 枠の子: アイコン
        public TMP_Text countText; // 枠の子: 個数
    }

    [Header("--- 枠(ItemSlotUI1〜3 を、順に登録) ---")]
    [SerializeField] private SlotView[] slots = new SlotView[3];

    [Header("--- 枠に出すアイテム(包帯・応急キットだけ登録。弾は入れない) ---")]
    [SerializeField] private ItemData[] itemDatabase;

    [Header("--- 参照 ---")]
    [SerializeField] private PlayerInventory inventory;
    [SerializeField, Range(0f, 1f)] private float emptyFrameAlpha = 0.5f;

    private ItemData[] assigned; // 枠ごとの中身(null なら空き)

    private void Awake()
    {
        if (inventory == null) inventory = FindObjectOfType<PlayerInventory>();
        assigned = new ItemData[slots.Length];
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
        for (int i = 0; i < slots.Length; i++) Clear(i);

        // 起動時にすでに持っているものを、順に枠へ入れる
        if (inventory == null) return;
        foreach (ItemData data in itemDatabase)
        {
            if (data == null) continue;
            int count = inventory.Count(data.type);
            if (count > 0) Apply(data, count);
        }
    }
    private void OnChanged(ItemType type, int count)
    {
        Debug.Log($"取得通知: {type} / {count}");

        ItemData data = FindData(type);

        if (data == null)
        {
            Debug.LogWarning($"ItemDatabaseに未登録: {type}");
            return;
        }

        Apply(data, count);
    }
    //private void OnChanged(ItemType type, int count)
    //{
    //    ItemData data = FindData(type);
    //    if (data == null) return; // 枠に出さないアイテム(弾など)は、何もしない

    //    Apply(data, count);
    //}

    private void Apply(ItemData data, int count)
    {
        int index = IndexOf(data);

        if (count > 0)
        {
            if (index < 0) index = FirstEmpty(); // 初めて持った物は、空いている枠へ
            if (index < 0)
            {
                Debug.Log("[ItemSlotUI] 枠がいっぱいです");
                return;
            }
            assigned[index] = data;
            Draw(index, data, count);
        }
        else if (index >= 0)
        {
            assigned[index] = null; // 0個になったら、枠を空ける
            Clear(index);
        }
    }

    private void Draw(int i, ItemData data, int count)
    {
        Debug.Log($"Draw実行: Slot={i} Item={data.itemName} Count={count}");
        SlotView s = slots[i];
        if (s.icon != null)
        {
            s.icon.sprite = data.icon;
            s.icon.enabled = data.icon != null;
        }
        if (s.countText != null) s.countText.text = count.ToString();
        SetFrameAlpha(s, 1f);
    }

    private void Clear(int i)
    {
        SlotView s = slots[i];
        if (s.icon != null) s.icon.enabled = false;
        if (s.countText != null) s.countText.text = "";
        SetFrameAlpha(s, emptyFrameAlpha);
    }

    private void SetFrameAlpha(SlotView s, float alpha)
    {
        if (s.frame == null) return;
        Color c = s.frame.color;
        c.a = alpha;
        s.frame.color = c;
    }

    private int IndexOf(ItemData data)
    {
        for (int i = 0; i < assigned.Length; i++)
            if (assigned[i] == data) return i;
        return -1;
    }

    private int FirstEmpty()
    {
        for (int i = 0; i < assigned.Length; i++)
            if (assigned[i] == null) return i;
        return -1;
    }

    private ItemData FindData(ItemType type)
    {
        foreach (ItemData d in itemDatabase)
            if (d != null && d.type == type) return d;
        return null;
    }
}