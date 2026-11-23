using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoHUD : MonoBehaviour
{
    [SerializeField] private WeaponManager weaponManager;

    [SerializeField] private TMP_Text text;
    [SerializeField] private Image ammoIcon;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color emptyColor = Color.red;

    private void Awake()
    {
        if (weaponManager == null)
        {
            Debug.LogError("AmmoHUD: WeaponManagerが未設定です");
        }
        else
        {
            Debug.Log(
                $"HUD参照: 現在弾数={weaponManager.CurrentAmmo}");
        }
    }

    private void Update()
    {
        if (weaponManager == null || text == null)
            return;

        text.text = $"× {weaponManager.CurrentAmmo}";

        bool empty =
            weaponManager.CurrentAmmo <= 0 &&
            weaponManager.ReserveAmmo <= 0;

        text.color =
            empty ? emptyColor : normalColor;

        ItemData data =
            weaponManager.CurrentAmmoData;

        if (ammoIcon != null &&
            data != null &&
            data.icon != null)
        {
            ammoIcon.sprite = data.icon;
        }
    }
}