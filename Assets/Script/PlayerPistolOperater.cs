using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ピストル専用スクリプト
/// 弾の発射・リロード・弾の切り替えを担当する。
/// 弾の数(マガジン)と選んでいる弾の種類は WeaponManager が持つ。
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

    [Header("--- 狙い(カメラの中央に向けて撃つ) ---")]
    [SerializeField] private Camera aimCamera;              // 空なら Camera.main
    [SerializeField] private float aimMaxDistance = 100f;    // 狙いを探す最大距離
    [SerializeField] private LayerMask aimMask = ~0;         // 狙いの当たり判定に使うレイヤー

    [Header("--- 弾切替 ---")]
    [SerializeField] private KeyCode switchAmmoKey = KeyCode.T;

    [Header("--- エフェクト ---")]
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private float flashLifetime = 0.1f;

    private float reloadTimer;
    private float shotTimer;
    private bool isReloading;

    public bool IsReloading => isReloading;

    // -------------------------------------------------
    // 初期化
    // -------------------------------------------------
    private void Start()
    {
        if (weaponManager == null) weaponManager = GetComponentInParent<WeaponManager>();
        if (weaponManager == null) weaponManager = FindObjectOfType<WeaponManager>();

        if (inventory == null) inventory = GetComponentInParent<PlayerInventory>();

        // Inspector に Project のプレハブ(アセット)が入っていても、シーン上のものに差し替える
        if (inventory == null || !inventory.gameObject.scene.IsValid())
        {
            Debug.LogWarning($"[Pistol] inventory がシーン上のものではありません: {inventory}");
            inventory = FindObjectOfType<PlayerInventory>();
        }

        if (playerController == null) playerController = GetComponentInParent<PlayerController>();
        if (playerController == null) playerController = FindObjectOfType<PlayerController>();

        Debug.Log($"[Pistol] weaponManager={weaponManager} inventory={inventory} " +
                  $"/ シーン上={(inventory != null && inventory.gameObject.scene.IsValid())} " +
                  $"/ 通常弾の予備={(inventory != null ? inventory.Count(ItemType.Ammo) : -1)}");
    }

    // -------------------------------------------------
    // 更新
    // -------------------------------------------------
    private void Update()
    {
        if (weaponManager == null || inventory == null || playerController == null) return;

        shotTimer += Time.deltaTime;

        if (isReloading)
        {
            HandleReload();
            return;
        }

        HandleInput();
    }

    private void OnDisable()
    {
        isReloading = false; // 持ち替えたら、リロードはキャンセルする
    }

    private void HandleInput()
    {
        // 弾の切り替え
        if (Input.GetKeyDown(switchAmmoKey))
        {
            SwitchAmmo();
            if (isReloading) return; // 切り替えでリロードが始まったら、この回は終わり
        }

        bool hasReserve = weaponManager.ReserveAmmo > 0;

        // R キー、またはマガジンが空のとき、予備があればリロード
        if ((Input.GetKeyDown(KeyCode.R) || weaponManager.CurrentAmmo <= 0)
            && hasReserve
            && weaponManager.CurrentAmmo < weaponManager.MagazineSize)
        {
            StartReload();
        }

        // 構え中に、左クリックで発射
        if (playerController.IsAiming && Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    // -------------------------------------------------
    // 発射
    // -------------------------------------------------
    private void Fire()
    {
        if (shotTimer < fireRate) return;

        if (weaponManager.CurrentAmmo <= 0)
        {
            Debug.Log("弾がありません！");
            return;
        }

        shotTimer = 0f;

        weaponManager.ConsumeAmmo();

        SpawnBullet();

        Debug.Log($"発射！ 残り弾数:{weaponManager.CurrentAmmo}/{weaponManager.MagazineSize}");
    }

    private void SpawnBullet()
    {
        if (bulletPrefab == null || muzzlePoint == null)
        {
            Debug.LogWarning("弾丸生成設定が不足しています");
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

        // 狙った点に向かう向き(長さ1)。カメラが上下を向けば、弾も上下に飛ぶ
        Vector3 dir = GetAimDirection(muzzlePoint.position);

        GameObject bullet = Instantiate(
            bulletPrefab,
            muzzlePoint.position,
            Quaternion.LookRotation(dir)); // 弾の見た目も、飛ぶ向きに合わせる

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = dir * bulletSpeed; // dir は長さ1なので、速さは bulletSpeed のまま一定
        }

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null && weaponStats != null)
        {
            float damageMultiplier =
                weaponManager.CurrentAmmoData != null
                ? weaponManager.CurrentAmmoData.damageMultiplier
                : 1f;

            bulletScript.SetDamage(
                Mathf.RoundToInt(weaponStats.AttackPower * damageMultiplier));
        }
    }

    /// <summary>
    /// 画面の中央から飛ばしたレイが当たる点を狙い、銃口からその点への向き(長さ1)を返す
    /// </summary>
    private Vector3 GetAimDirection(Vector3 muzzlePos)
    {
        Camera cam = aimCamera != null ? aimCamera : Camera.main;
        if (cam == null) return muzzlePoint.forward; // カメラがなければ、従来どおり銃口の向き

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 target = ray.GetPoint(aimMaxDistance); // 何にも当たらなければ、遠くの点

        RaycastHit[] hits = Physics.RaycastAll(
            ray, aimMaxDistance, aimMask, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit h in hits)
        {
            // プレイヤー自身と、飛んでいる弾は、狙いの対象にしない
            if (playerController != null && h.collider.transform.IsChildOf(playerController.transform)) continue;
            if (h.collider.GetComponentInParent<Bullet>() != null) continue;

            target = h.point;
            break;
        }

        Vector3 dir = (target - muzzlePos).normalized; // 正規化して、向きだけにする

        // 狙った点が、銃口より手前(背後)にあるときは、カメラの向きに撃つ
        if (Vector3.Dot(dir, ray.direction) < 0f) dir = ray.direction;

        return dir;
    }

    // -------------------------------------------------
    // リロード
    // -------------------------------------------------
    private void StartReload()
    {
        isReloading = true;
        reloadTimer = 0f;

        Debug.Log("リロード開始");
    }

    private void HandleReload()
    {
        reloadTimer += Time.deltaTime;

        if (reloadTimer < reloadTime) return;

        int need = weaponManager.MagazineSize - weaponManager.CurrentAmmo;

        // 選んでいる弾の種類から、予備を取る
        int taken = inventory.TakeAmmo(weaponManager.CurrentAmmoType, need);

        weaponManager.AddAmmoToMagazine(taken);

        isReloading = false;

        Debug.Log($"リロード完了 {weaponManager.CurrentAmmo}/{weaponManager.MagazineSize} " +
                  $"(取った={taken} / 予備={weaponManager.ReserveAmmo})");
    }

    // -------------------------------------------------
    // 弾の切り替え
    // -------------------------------------------------
    private void SwitchAmmo()
    {
        ItemData[] ammos = weaponManager.AmmoOptions;
        if (ammos == null || ammos.Length < 2) return;

        int current = weaponManager.AmmoIndex;

        for (int i = 1; i < ammos.Length; i++)
        {
            int next = (current + i) % ammos.Length;

            if (inventory.Count(ammos[next].type) <= 0)
                continue; // 持っていない弾は飛ばす

            Debug.Log($"弾切替 : {ammos[next].itemName}");

            // マガジンに残っている弾を、今の種類の予備に戻す
            // (AmmoIndex を変える前に行う。変えたあとだと、種類が分からなくなる)
            if (weaponManager.CurrentAmmo > 0 && weaponManager.CurrentAmmoData != null)
            {
                inventory.Add(weaponManager.CurrentAmmoData, weaponManager.CurrentAmmo);
            }
            weaponManager.CurrentAmmo = 0;

            weaponManager.AmmoIndex = next;
            StartReload(); // 新しい弾を込める
            return;
        }

        Debug.Log("切り替えられる弾がありません");
    }
}