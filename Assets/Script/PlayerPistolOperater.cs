using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ピストル専用スクリプト
/// 弾の管理・物理弾丸の発射・リロード処理
/// </summary>
public class PlayerPistolOperater : MonoBehaviour
{
    [Header("--- 武器の設定 ---")]
    [SerializeField] private float fireRate = 0.3f;
    [SerializeField] private int magazineSize = 12;
    [SerializeField] private float reloadTime = 2.0f;
    [SerializeField] private float bulletSpeed = 10.0f;

    [Header("--- 参照設定 ---")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private WeaponStats weaponStats;
    [SerializeField] private PlayerInventory inventory;

    [Header("--- 弾薬の種類 ---")]                                  
    [SerializeField] private ItemData[] ammoOptions;                // 使える弾。0番が最初の弾
    [SerializeField] private KeyCode switchAmmoKey = KeyCode.T;     // 切り替えキー

    [Header("--- 武器の表示設定 ---")]
    [SerializeField] private GameObject weaponObject;

    [Header("--- エフェクト ---")]
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private float flashLifetime = 0.1f;

    // 内部状態
    private int currentAmmo;
    private float timeSinceLastShot = 0f;
    private float currentReloadTime = 0f;
    private bool isReloading = false;
    private int ammoIndex = 0;                                       // 今選んでいる弾の番号

    // 外部参照用プロパティ
    public int CurrentAmmo => currentAmmo;
    public int MagazineSize => magazineSize;
    public bool IsReloading => isReloading;
    public bool IsAiming { get; private set; }

    // 選んでいる弾に関するプロパティ(HUDや枠が見る)
    public ItemData CurrentAmmoData =>
        (ammoOptions != null && ammoOptions.Length > 0) ? ammoOptions[ammoIndex] : null;
    public ItemType CurrentAmmoType =>
        CurrentAmmoData != null ? CurrentAmmoData.type : ItemType.Ammo;
    public int ReserveAmmo =>
        inventory != null ? inventory.Count(CurrentAmmoType) : 0;

    private void Start()
    {
        currentAmmo = magazineSize;

        if (playerController == null)
        {
            playerController = GetComponentInParent<PlayerController>();
        }

        if (inventory == null)
        {
            inventory = GetComponentInParent<PlayerInventory>();
        }

        if (weaponObject != null)
        {
            weaponObject.SetActive(false);
        }
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (Input.GetMouseButton(1))
        {
            IsAiming = true;

            if (weaponObject != null && !weaponObject.activeSelf)
            {
                weaponObject.SetActive(true);
            }
        }
        else
        {
            IsAiming = false;

            if (!isReloading && weaponObject != null && weaponObject.activeSelf)
            {
                weaponObject.SetActive(false);
            }
        }

        if (isReloading)
        {
            HandleWeaponReloading();
            return;
        }

        HandleWeaponOperation();
    }

    private void OnDisable()
    {
        IsAiming = false;
        isReloading = false;
        if (weaponObject != null) weaponObject.SetActive(false);
    }

    /// <summary>
    /// 武器の操作（発射・リロード・弾の切り替え）を処理する
    /// </summary>
    private void HandleWeaponOperation()
    {
        // 弾の切り替え
        if (Input.GetKeyDown(switchAmmoKey))
        {
            SwitchAmmo();
            if (isReloading) return; // 切り替えでリロードが始まったら、この回は終わり
        }

        // 予備弾の確認は、選んでいる弾で行う
        bool hasReserve = inventory == null || inventory.Count(CurrentAmmoType) > 0;

        if ((Input.GetKeyDown(KeyCode.R) || currentAmmo <= 0) && !isReloading && currentAmmo < magazineSize && hasReserve)
        {
            StartReload();
        }

        if (IsAiming && Input.GetMouseButtonDown(0))
        {
            HandleWeaponFiring();
        }
    }

    /// <summary>
    /// 次の弾に切り替える(持っていない弾は飛ばす)
    /// </summary>
    private void SwitchAmmo()
    {
        if (ammoOptions == null || ammoOptions.Length < 2 || inventory == null) return;

        for (int i = 1; i < ammoOptions.Length; i++)
        {
            int next = (ammoIndex + i) % ammoOptions.Length;
            if (inventory.Count(ammoOptions[next].type) <= 0) continue;

            // マガジンに残っている弾を、今の種類の予備に戻す
            if (currentAmmo > 0) inventory.Add(CurrentAmmoData, currentAmmo);
            currentAmmo = 0;

            ammoIndex = next;
            Debug.Log($"弾を切り替え: {CurrentAmmoData.itemName}");
            StartReload(); // 新しい弾を込める
            return;
        }

        Debug.Log("切り替えられる弾がありません");
    }

    private void StartReload()
    {
        isReloading = true;
        currentReloadTime = 0f;
        Debug.Log("リロード中...");
    }

    private void HandleWeaponReloading()
    {
        currentReloadTime += Time.deltaTime;

        if (currentReloadTime >= reloadTime)
        {
            int need = magazineSize - currentAmmo;
            //選んでいる弾の種類から取る
            int taken = inventory != null ? inventory.TakeAmmo(CurrentAmmoType, need) : need;
            currentAmmo += taken;
            isReloading = false;
            Debug.Log($"リロード完了！ 弾数: {currentAmmo}/{magazineSize} / 予備: {ReserveAmmo}");
        }
    }

    private void HandleWeaponFiring()
    {
        if (timeSinceLastShot < fireRate || isReloading) return;

        if (currentAmmo <= 0)
        {
            Debug.Log("弾がありません！Rキーでリロードしてください。");
            return;
        }

        timeSinceLastShot = 0f;
        currentAmmo--;

        SpawnAndFireBullet();

        Debug.Log($"発射！ 残り弾数: {currentAmmo}/{magazineSize}");
    }

    private void SpawnAndFireBullet()
    {
        if (muzzlePoint == null && weaponObject != null)
        {
            Transform foundMuzzle = weaponObject.transform.Find("Muzzle Point");
            if (foundMuzzle != null)
            {
                muzzlePoint = foundMuzzle;
            }
        }

        if (bulletPrefab == null || muzzlePoint == null)
        {
            Debug.LogWarning("BulletPrefab または MuzzlePoint が実体を持っていません（nullです）！");
            return;
        }

        if (muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(
                muzzleFlashPrefab,
                muzzlePoint.position,
                muzzlePoint.rotation,
                muzzlePoint);
            Destroy(flash, flashLifetime);
        }

        GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);
        bullet.transform.forward = muzzlePoint.forward;

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null && weaponStats != null)
        {
            //弾の威力倍率を掛ける
            float mult = CurrentAmmoData != null ? CurrentAmmoData.damageMultiplier : 1f;
            bulletScript.SetDamage(Mathf.RoundToInt(weaponStats.AttackPower * mult));
        }
        else if (bulletScript == null)
        {
            Debug.LogWarning("生成した弾に Bullet スクリプトがアタッチされていません！");
        }

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = muzzlePoint.forward * bulletSpeed;
        }
    }
}