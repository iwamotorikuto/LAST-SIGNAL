using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 銃弾、回復アイテムなどのアイテムの種類
/// </summary>
public enum ItemType { Ammo,AmmoData, Bandage, FirstAidKit, AmmoHeavy }

[CreateAssetMenu(menuName = "Game/Item Data", fileName = "NewItem")]
public class ItemData : ScriptableObject
{
    public string itemName;　　　　　　//　アイテムの名前
    public ItemType type;　　　　　　　// アイテムの種類
    public float healAmount;           // 回復量(回復アイテムのみ)
    public float useTime = 1f;         // 使用にかかる時間(秒)
    public int maxStack = 99;          // 持てる最大数
    public float damageMultiplier = 1f; // 弾の威力倍率(通常弾は1、強装弾は1.5など)
    public Sprite icon;                // HUDに出す絵
}