using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public enum WeaponType
    {
        None,
        Pistol,
        Knife,
    }

    [Header("武器オブジェクト")]
    [SerializeField] private GameObject pistolObject;
    [SerializeField] private GameObject knifeObject;

    [Header("武器スクリプト")]
    [SerializeField] private PlayerPistolOperater pistolOperater;
    [SerializeField] private PlayerKnifeOperater knifeOperater;

    [Header("参照")]
    [SerializeField] private PlayerAnimation playerAnimation;
    [SerializeField] private PlayerRescueOperation rescueOperation;
    [SerializeField] private PlayerInventory inventory;

    [Header("弾種設定")]
    [SerializeField] private ItemData[] ammoOptions;

    [Header("マガジン設定")]
    [SerializeField] private int magazineSize = 12;

    //--------------------------------
    // 武器状態
    //--------------------------------

    private WeaponType currentWeapon = WeaponType.Pistol;
    private bool isBusy;

    //--------------------------------
    // 弾薬状態
    //--------------------------------

    private int currentAmmo;
    private int ammoIndex;

    //--------------------------------
    // プロパティ
    //--------------------------------

    public WeaponType CurrentWeapon => currentWeapon;

    public int CurrentAmmo
    {
        get => currentAmmo;
        set => currentAmmo = Mathf.Clamp(value, 0, magazineSize);
    }

    public int MagazineSize => magazineSize;

    public int AmmoIndex
    {
        get => ammoIndex;
        set
        {
            if (ammoOptions == null || ammoOptions.Length == 0)
                return;

            ammoIndex = Mathf.Clamp(
                value,
                0,
                ammoOptions.Length - 1);
        }
    }

    public ItemData[] AmmoOptions => ammoOptions;

    public ItemData CurrentAmmoData
    {
        get
        {
            if (ammoOptions == null)
                return null;

            if (ammoOptions.Length == 0)
                return null;

            if (ammoIndex < 0 ||
                ammoIndex >= ammoOptions.Length)
                return null;

            return ammoOptions[ammoIndex];
        }
    }

    public ItemType CurrentAmmoType
    {
        get
        {
            return CurrentAmmoData != null
                ? CurrentAmmoData.type
                : ItemType.Ammo;
        }
    }

    public int ReserveAmmo
    {
        get
        {
            if (inventory == null)
                return 0;

            return inventory.Count(CurrentAmmoType);
        }
    }

    //--------------------------------
    // 初期化
    //--------------------------------

    private void Awake()
    {
        currentAmmo = magazineSize;
        ammoIndex = 0;

        Debug.Log($"WeaponManager inventory={inventory}");
    }

    private void Start()
    {
        if (inventory == null)
            inventory = GetComponent<PlayerInventory>();

        Debug.Log($"Inventory={inventory}");

        if (rescueOperation == null)
            rescueOperation =
                GetComponent<PlayerRescueOperation>();

        ApplyWeapon();
    }

    //--------------------------------
    // 更新
    //--------------------------------

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeapon(WeaponType.Pistol);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeapon(WeaponType.Knife);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeWeapon(WeaponType.None);
        }

        bool busy =
            rescueOperation != null &&
            (rescueOperation.IsRescuing ||
             rescueOperation.IsKneelingDown);

        if (busy != isBusy)
        {
            isBusy = busy;
            ApplyWeapon();
        }
    }

    //--------------------------------
    // 武器切替
    //--------------------------------

    public void ChangeWeapon(WeaponType weaponType)
    {
        currentWeapon = weaponType;
        ApplyWeapon();
    }

    private void ApplyWeapon()
    {
        WeaponType effective =
            isBusy ? WeaponType.None : currentWeapon;

        if (pistolObject != null)
            pistolObject.SetActive(false);

        if (knifeObject != null)
            knifeObject.SetActive(false);

        if (pistolOperater != null)
            pistolOperater.enabled = false;

        if (knifeOperater != null)
            knifeOperater.enabled = false;

        if (playerAnimation != null)
            playerAnimation.SetHoldingKnife(
                effective == WeaponType.Knife);

        switch (effective)
        {
            case WeaponType.Pistol:

                if (pistolObject != null)
                    pistolObject.SetActive(true);

                if (pistolOperater != null)
                    pistolOperater.enabled = true;

                break;

            case WeaponType.Knife:

                if (knifeObject != null)
                    knifeObject.SetActive(true);

                if (knifeOperater != null)
                    knifeOperater.enabled = true;

                break;

            case WeaponType.None:
                break;
        }
    }

    //--------------------------------
    // 弾薬操作
    //--------------------------------

    public bool HasAmmo()
    {
        return currentAmmo > 0;
    }

    public void ConsumeAmmo(int amount = 1)
    {
        CurrentAmmo -= amount;
    }

    public void AddAmmoToMagazine(int amount)
    {
        CurrentAmmo += amount;
    }

    public void SetAmmoType(int index)
    {
        AmmoIndex = index;
    }
}
