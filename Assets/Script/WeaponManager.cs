using System.Collections;
using System.Collections.Generic;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public enum WeaponType
    {
        Pistol, // ピストル
        Knife,  // ナイフ
    }

    [Header("--- 武器オブジェクトの参照 ---")]
    [SerializeField] private GameObject pistolObject;
    [SerializeField] private GameObject knifeObject;

    [Header("--- 武器スクリプトの参照 ---")]
    [SerializeField] private PlayerPistolOperater pistolOperater;
    //[SerializeField] private PlayerKnifeOperater knifeOperater;

    private WeaponType currentWeapon = WeaponType.Pistol;

    void Start()
    {
        // 最初はピストルを装備
        ChangeWeapon(WeaponType.Pistol);
    }

    void Update()
    {
        // 数字キー「1」「2」またはマウスホイールで武器切り替え
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeapon(WeaponType.Pistol);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeapon(WeaponType.Knife);
        }
    }

    public void ChangeWeapon(WeaponType weaponType)
    {
        currentWeapon = weaponType;

        // すべての武器を非表示にする
        if (pistolObject != null) pistolObject.SetActive(false);
        //if (knifeObject != null) knifeObject.SetActive(false);

        // スクリプトの有効/無効化
        if (pistolOperater != null) pistolOperater.enabled = false;
       // if (knifeOperater != null) knifeOperater.enabled = false;

        // 選択された武器だけを表示
        switch (weaponType)
        {
            case WeaponType.Pistol:
                if (pistolObject != null) pistolObject.SetActive(true);
                if (pistolOperater != null) pistolOperater.enabled = true; // ピストル操作を有効化
                Debug.Log("ピストルに切り替え");
                break;

            //case WeaponType.Knife:

            //    Debug.Log("knifeObject = " + knifeObject);
            //    Debug.Log("knifeOperater = " + knifeOperater);
            //    if (knifeObject != null) knifeObject.SetActive(true);
            //    if (knifeOperater != null) knifeOperater.enabled = true;
            //    Debug.Log("ナイフに切り替え");
            //    break;
        }
    }

    public WeaponType CurrentWeapon => currentWeapon;
}
