using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 銃弾、回復アイテムなどのアイテムの種類
/// </summary>
public enum ItemType { Ammo,AmmoData, Bandage, FirstAidKit, AmmoHeavy }

/// <summary>
/// メニューを開いたときに表示するアイテムの情報を持つ ScriptableObject
/// </summary>
[CreateAssetMenu(menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType type;

    public float healAmount;
    public float useTime = 1f;
    public int maxStack = 99;

    public Sprite icon;

    public BulletData bulletData;
    internal float damageMultiplier;
}