using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmmoType { Ammo, HeavyAmmo }

[CreateAssetMenu(menuName = "Game/Bullet Data")]
public class BulletData : ScriptableObject
{
    public string bulletName;
    public float damageMultiplier;
    public Color uiColor;
    public GameObject hitEffect;
}