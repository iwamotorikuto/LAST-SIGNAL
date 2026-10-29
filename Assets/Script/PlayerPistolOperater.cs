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
    [SerializeField] private float fireRate = 0.3f;         // 発射間隔（秒）
    [SerializeField] private int magazineSize = 12;         // マガジン容量
    [SerializeField] private float reloadTime = 2.0f;       // リロード時間（秒）
    [SerializeField] private float bulletSpeed = 10.0f;     // 銃弾の飛ぶ速度

    [Header("--- 参照設定 ---")]
    [SerializeField] private GameObject bulletPrefab;      // 弾丸のプレハブ
    [SerializeField] private Transform muzzlePoint;         // 銃口の位置 (MuzzlePoint)
    [SerializeField] private PlayerController playerController;
    [SerializeField] private WeaponStats weaponStats;

    [Header("--- 武器の表示設定 ---")]
    [SerializeField] private GameObject weaponObject; // 構えたときに出現させる銃のオブジェクト


    // 内部状態
    private int currentAmmo;
    private float timeSinceLastShot = 0f;
    private float currentReloadTime = 0f;
    private bool isReloading = false;

    // 外部参照用プロパティ
    public int CurrentAmmo => currentAmmo;
    public int MagazineSize => magazineSize;
    public bool IsReloading => isReloading;
    public bool IsAiming { get; private set; }

    private void Start()
    {
        currentAmmo = magazineSize;

        // プレイヤーコントローラーが未割り当ての場合、自動取得
        if (playerController == null)
        {
            playerController = GetComponentInParent<PlayerController>();
        }

        // ゲーム開始時は銃を非表示（ホルスターにしまっている状態）にする
        if (weaponObject != null)
        {
            weaponObject.SetActive(false);
        }
    }

    private void Update()
    {
        // 連射間隔のタイマーを進める
        timeSinceLastShot += Time.deltaTime;

        // 右クリック長押し中のエイム判定
        if (Input.GetMouseButton(1))
        {
            IsAiming = true;

            // 銃を表示する
            if (weaponObject != null && !weaponObject.activeSelf)
            {
                weaponObject.SetActive(true);
            }
        }
        else
        {
            IsAiming = false;

            // リロード中でなければ、構えていないときは銃を非表示にする
            if (!isReloading && weaponObject != null && weaponObject.activeSelf)
            {
                weaponObject.SetActive(false);
            }
        }

        // リロード中の処理
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
        if (weaponObject != null) weaponObject.SetActive(false);
    }

    /// <summary>
    /// 武器の操作（発射・リロード）を処理する
    /// </summary>
    private void HandleWeaponOperation()
    {
        // Rキーでリロード開始（弾がない場合も自動でリロード）
        if ((Input.GetKeyDown(KeyCode.R) || currentAmmo <= 0) && !isReloading && currentAmmo < magazineSize)
        {
            StartReload();
        }

        // 構え中（右クリック）かつ 左クリックで発射
        if (IsAiming && Input.GetMouseButtonDown(0))
        {
            HandleWeaponFiring();
        }
    }

    /// <summary>
    /// リロードを開始する
    /// </summary>
    private void StartReload()
    {
        isReloading = true;
        currentReloadTime = 0f;
        Debug.Log("リロード中...");
    }

    /// <summary>
    /// リロード処理を行う
    /// </summary>
    private void HandleWeaponReloading()
    {
        currentReloadTime += Time.deltaTime;

        if (currentReloadTime >= reloadTime)
        {
            currentAmmo = magazineSize;
            isReloading = false;
            Debug.Log($"リロード完了！ 弾数: {currentAmmo}/{magazineSize}");
        }
    }

    /// <summary>
    /// 武器の発射処理を行う
    /// </summary>
    private void HandleWeaponFiring()
    {
        // レート制限＆リロード中のチェック
        if (timeSinceLastShot < fireRate || isReloading) return;

        // 弾切れチェック
        if (currentAmmo <= 0)
        {
            Debug.Log("弾がありません！Rキーでリロードしてください。");
            return;
        }

        timeSinceLastShot = 0f;
        currentAmmo--;

        // 銃弾の実体（Prefab）を生成して発射
        SpawnAndFireBullet();

        Debug.Log($"発射！ 残り弾数: {currentAmmo}/{magazineSize}");
    }

    /// <summary>
    /// 銃口から弾丸を生成し、銃口の向き（前方）に向かって直線で発射する
    /// </summary>
    private void SpawnAndFireBullet()
    {
        // 銃口（muzzlePoint）を自動で探す処理
        if (muzzlePoint == null && weaponObject != null)
        {
            Transform foundMuzzle = weaponObject.transform.Find("Muzzle Point");
            if (foundMuzzle != null)
            {
                muzzlePoint = foundMuzzle;
            }
        }

        // それでも見つからない場合のチェック
        if (bulletPrefab == null || muzzlePoint == null)
        {
            Debug.LogWarning("BulletPrefab または MuzzlePoint が実体を持っていません（nullです）！");
            return;
        }

        // 1. 銃口の位置と回転のまま、弾を生成する
        GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);

        // 2. 弾の向き（前方向）を銃口の向きに合わせる
        bullet.transform.forward = muzzlePoint.forward;

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null && weaponStats != null)
        {
            bulletScript.SetDamage(weaponStats.AttackPower);
        }
        else if (bulletScript == null)
        {
            Debug.LogWarning("生成した弾に Bullet スクリプトがアタッチされていません！");
        }

        // 3. 銃口が向いている正面（muzzlePoint.forward）へ向かって直線スピードを与える
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = muzzlePoint.forward * bulletSpeed;
        }

    }
  
}