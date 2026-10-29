using System.Collections;
using System.Collections.Generic;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public enum WeaponType
    {
        None,   // 素手
        Pistol, // ピストル
        Knife,  // ナイフ
    }

    [Header("--- 武器オブジェクトの参照 ---")]
    [SerializeField] private GameObject pistolObject;
    [SerializeField] private GameObject knifeObject;
    [SerializeField] private PlayerAnimation playerAnimation;

    [Header("--- 武器スクリプトの参照 ---")]
    [SerializeField] private PlayerPistolOperater pistolOperater;
    [SerializeField] private PlayerKnifeOperater knifeOperater;

    [Header("--- 救助中は武器をしまう ---")]
    [SerializeField] private PlayerRescueOperation rescueOperation;

    private WeaponType currentWeapon = WeaponType.Pistol;
    private bool isBusy = false; // 救助中かどうか

    void Start()
    {
        if (rescueOperation == null)
            rescueOperation = GetComponentInParent<PlayerRescueOperation>();

        ApplyWeapon();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeWeapon(WeaponType.Pistol);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeWeapon(WeaponType.Knife);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeWeapon(WeaponType.None);

        // 救助状態が変わったときだけ反映する
        bool busy = rescueOperation != null &&
                    (rescueOperation.IsRescuing || rescueOperation.IsKneelingDown);
        if (busy != isBusy)
        {
            isBusy = busy;
            ApplyWeapon();
        }
    }

    public void ChangeWeapon(WeaponType weaponType)
    {
        currentWeapon = weaponType;
        ApplyWeapon();
    }

    // 実際の表示・有効化はここだけで行う
    private void ApplyWeapon()
    {
        Debug.Log($"[WM] knifeOperater={(knifeOperater != null ? knifeOperater.gameObject.name : "NULL")} / enabled={(knifeOperater != null ? knifeOperater.enabled.ToString() : "-")}");
        // 救助中は強制的に素手扱い。currentWeapon は変えない
        WeaponType effective = isBusy ? WeaponType.None : currentWeapon;

        if (pistolObject != null) pistolObject.SetActive(false);
        if (knifeObject != null) knifeObject.SetActive(false);
        if (pistolOperater != null) pistolOperater.enabled = false;
        if (knifeOperater != null) knifeOperater.enabled = false;

        if (playerAnimation != null)
            playerAnimation.SetHoldingKnife(effective == WeaponType.Knife);

        switch (effective)
        {
            case WeaponType.Pistol:
                if (pistolObject != null) pistolObject.SetActive(true);
                if (pistolOperater != null) pistolOperater.enabled = true;
                break;

            case WeaponType.Knife:
                if (knifeObject != null) knifeObject.SetActive(true);
                if (knifeOperater != null) knifeOperater.enabled = true;
                break;

            case WeaponType.None:
                break;
        }
    }

    public WeaponType CurrentWeapon => currentWeapon;
}