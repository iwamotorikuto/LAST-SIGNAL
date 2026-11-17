using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int startAmmo = 24; // 開始時の予備弾

    private readonly Dictionary<ItemType, int> counts = new Dictionary<ItemType, int>();

    public event Action<ItemType, int> OnChanged; // UI更新用

    private void Awake()
    {
        counts[ItemType.Ammo] = startAmmo;
    }
    /// <summary>
    /// アイテムの所持数を返す。所持していない場合は0を返す
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int Count(ItemType type)
    {
        return counts.TryGetValue(type, out int c) ? c : 0;
    }

    // 実際に追加できた数を返す(上限で拾えなかった分は、地面に残す判断に使う)
    public int Add(ItemData data, int amount)
    {
        int cur = Count(data.type);
        int added = Mathf.Clamp(amount, 0, data.maxStack - cur);
        counts[data.type] = cur + added;
        OnChanged?.Invoke(data.type, counts[data.type]);
        return added;
    }
    
    /// <summary>
    /// アイテムを消費する
    /// </summary>
    /// <param name="type"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool Consume(ItemType type, int amount = 1)
    {
        if (Count(type) < amount) return false;
        counts[type] -= amount;
        OnChanged?.Invoke(type, counts[type]);
        return true;
    }

    
    // リロード用: 指定した弾の種類から、欲しい数のうち渡せる数を返して消費する
    public int TakeAmmo(ItemType type, int want)
    {
        int taken = Mathf.Min(want, Count(type));
        if (taken > 0) Consume(type, taken);
        return taken;
    }
}

