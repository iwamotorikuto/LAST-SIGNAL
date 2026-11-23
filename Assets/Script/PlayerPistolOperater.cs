using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ピストル専用スクリプト
/// 弾の管理・物理弾丸の発射・リロード処理
/// </summary>
public class PlayerPistolOperater : MonoBehaviour
{
    [Header("--- 武器設定 ---")]
    [SerializeField] private float fireRate = 0.3f;
    [SerializeField] private float reloadTime = 2.0f;
    [SerializeField] private float bulletSpeed = 10f;

    [Header("--- 参照 ---")]
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private WeaponStats weaponStats;

    [Header("--- 発射設定 ---")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzlePoint;

    [Header("--- 弾切替 ---")]
    [SerializeField] private KeyCode switchAmmoKey = KeyCode.T;

    [Header("--- エフェクト ---")]
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private float flashLifetime = 0.1f;

    private float reloadTimer;
    private float shotTimer;

    private bool isReloading;

    public bool IsReloading => isReloading;
    private void Start()
    {
        if (weaponManager == null) weaponManager = GetComponentInParent<WeaponManager>();
        if (weaponManager == null) weaponManager = FindObjectOfType<WeaponManager>();

        if (inventory == null) inventory = GetComponentInParent<PlayerInventory>();
        if (inventory == null) inventory = FindObjectOfType<PlayerInventory>();

        if (playerController == null) playerController = GetComponentInParent<PlayerController>();
        if (playerController == null) playerController = FindObjectOfType<PlayerController>();

        Debug.Log($"[Pistol] weaponManager={weaponManager} inventory={inventory}");
    }


    private void Update()
    {
        shotTimer += Time.deltaTime;

        if (isReloading)
        {
            HandleReload();
            return;
        }

        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(switchAmmoKey))
        {
            SwitchAmmo();
        }

        bool hasReserve =
            weaponManager.ReserveAmmo > 0;

        if ((Input.GetKeyDown(KeyCode.R) ||
             weaponManager.CurrentAmmo <= 0)
            && hasReserve
            && weaponManager.CurrentAmmo < weaponManager.MagazineSize)
        {
            StartReload();
        }

        if (playerController.IsAiming &&
            Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (shotTimer < fireRate)
            return;

        if (weaponManager.CurrentAmmo <= 0)
        {
            Debug.Log("弾がありません！");
            return;
        }

        shotTimer = 0f;

        weaponManager.ConsumeAmmo();

        SpawnBullet();

        Debug.Log(
            $"発射！ 残り弾数:{weaponManager.CurrentAmmo}/{weaponManager.MagazineSize}");
    }

    private void SpawnBullet()
    {
        if (bulletPrefab == null ||
            muzzlePoint == null)
        {
            Debug.LogWarning("弾丸生成設定が不足しています");
            return;
        }

        if (muzzleFlashPrefab != null)
        {
            GameObject flash =
                Instantiate(
                    muzzleFlashPrefab,
                    muzzlePoint.position,
                    muzzlePoint.rotation,
                    muzzlePoint);

            Destroy(flash, flashLifetime);
        }

        GameObject bullet =
            Instantiate(
                bulletPrefab,
                muzzlePoint.position,
                muzzlePoint.rotation);

        Rigidbody rb =
            bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity =
                muzzlePoint.forward * bulletSpeed;
        }

        Bullet bulletScript =
            bullet.GetComponent<Bullet>();

        if (bulletScript != null &&
            weaponStats != null)
        {
            float damageMultiplier =
                weaponManager.CurrentAmmoData != null
                ? weaponManager.CurrentAmmoData.damageMultiplier
                : 1f;

            bulletScript.SetDamage(
                Mathf.RoundToInt(
                    weaponStats.AttackPower
                    * damageMultiplier));
        }
    }

    private void StartReload()
    {
        isReloading = true;
        reloadTimer = 0f;

        Debug.Log("リロード開始");
    }

    private void HandleReload()
    {
        reloadTimer += Time.deltaTime;

        if (reloadTimer < reloadTime)
            return;

        int need =
            weaponManager.MagazineSize
            - weaponManager.CurrentAmmo;

        int taken =
            inventory.TakeAmmo(
                weaponManager.CurrentAmmoType,
                need);

        weaponManager.AddAmmoToMagazine(taken);

        isReloading = false;

        Debug.Log(
            $"リロード完了 " +
            $"{weaponManager.CurrentAmmo}/" +
            $"{weaponManager.MagazineSize}");
    }
    private void SwitchAmmo()
    {
        Debug.Log("Tキー押された");

        ItemData[] ammos = weaponManager.AmmoOptions;
        int current = weaponManager.AmmoIndex;

        for (int i = 1; i < ammos.Length; i++)
        {
            int next = (current + i) % ammos.Length;

            if (inventory.Count(ammos[next].type) <= 0)
                continue;

            Debug.Log($"弾切替 : {ammos[next].itemName}");

            // マガジンに残っている弾を、今の種類の予備に戻す
            if (weaponManager.CurrentAmmo > 0 && weaponManager.CurrentAmmoData != null)
            {
                inventory.Add(weaponManager.CurrentAmmoData, weaponManager.CurrentAmmo);
            }
            weaponManager.CurrentAmmo = 0;

            weaponManager.AmmoIndex = next;
            StartReload();   // 新しい弾を込める
            return;
        }

        Debug.Log("切り替えられる弾がありません");
    }
  
   
}