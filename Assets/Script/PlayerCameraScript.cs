using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCameraScript : MonoBehaviour
{
    public GameObject Cube1; //playerのゲームオブジェクトを入れる変数を設定

    public float rotateSpeed = 100.0f; //回転の速さ
    public float distance = 5.0f;      //Cube1からカメラまでの距離
    public float height = 2.0f;        //Cube1からカメラまでの高さ
    public float followSpeed = 100.0f; //追従するスピード

    void Start()
    {
        //マウスの左右移動で、Cube1本体を回転させる
        float mx = Input.GetAxis("Mouse X");
        Cube1.transform.Rotate(Vector3.up, mx * rotateSpeed * Time.deltaTime);
    }

    void Update()
    {
        //Cube1の「後ろ」の位置を計算(Cube1の向きに応じて自動で変わる)
        Vector3 targetPos = Cube1.transform.position - Cube1.transform.forward * distance + Vector3.up * height;

        //滑らかにその位置へ移動
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);

        //常にCube1の方を向く
        transform.LookAt(Cube1.transform.position + Vector3.up * height * 0.5f);
    
    }
}
