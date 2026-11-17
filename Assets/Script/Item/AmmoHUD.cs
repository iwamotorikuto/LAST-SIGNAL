using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoHUD : MonoBehaviour
{
    [SerializeField] private PlayerPistolOperater pistol;
    [SerializeField] private TMP_Text text;     // 弾薬数の文字
    [SerializeField] private Image ammoIcon;    // 弾のアイコン

    private void Awake()
    {
        // ピストルは持ち替えで非表示になることがあるので、非表示のものも探す
        if (pistol == null) pistol = FindObjectOfType<PlayerPistolOperater>(true);
    }

    private void Update()
    {
        if (pistol == null) return;

        text.text = $"{pistol.CurrentAmmo:00} / {pistol.ReserveAmmo}";

        ItemData data = pistol.CurrentAmmoData;
        if (ammoIcon != null && data != null && data.icon != null)
        {
            ammoIcon.sprite = data.icon;
        }
    }
}