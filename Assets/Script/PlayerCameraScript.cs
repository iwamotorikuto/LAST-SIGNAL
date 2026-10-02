using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCameraScript : MonoBehaviour
{
    public GameObject Cube1; //playerのゲームオブジェクトを入れる変数を設定

    public float rotateSpeed = 100.0f; //回転の速さ
    public float distance = 15.0f;      //Cube1からカメラまでの距離
    public float height = 2.0f;        //Cube1からカメラまでの高さ
    public float followSpeed = 100.0f; //追従するスピード

    public Camera cam;
    public float normalFOV = 60.0f;
    public float zoomedFOV = 30.0f;
    public float zoomSpeed = 5.0f;
    public LayerMask wallMask;

    void Start()
    {
        //マウスの左右移動で、Cube1本体を回転させる
        float mx = Input.GetAxis("Mouse X");
        Cube1.transform.Rotate(Vector3.up, mx * rotateSpeed * Time.deltaTime);

        if (cam == null) cam = GetComponent<Camera>();
    }

    void Update()
    {
        //Cube1の「後ろ」の位置を計算(Cube1の向きに応じて自動で変わる)
        Vector3 desiredPos = Cube1.transform.position - Cube1.transform.forward * distance + Vector3.up * height;

        Vector3 pivot = Cube1.transform.position + Vector3.up * height; // Cube1側の基準点
        Vector3 dirToCam = desiredPos - pivot;
        float desiredDist = dirToCam.magnitude;

        Vector3 targetPos = desiredPos;

        if (Physics.Raycast(pivot, dirToCam.normalized, out RaycastHit hit, desiredDist, wallMask))
        {
            // 壁に当たったら、その手前(少し余裕を持たせる)に位置を修正
            targetPos = hit.point - dirToCam.normalized * 0.2f;
        }


        //滑らかにその位置へ移動
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);

        //常にCube1の方を向く
        transform.LookAt(Cube1.transform.position + Vector3.up * height * 0.5f);

        bool isBlocked = CheckWallBlocking();
        float targetFOV = isBlocked ? zoomedFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }

    bool CheckWallBlocking()
    {
        Vector3 dir = Cube1.transform.position - transform.position;
        float dist = dir.magnitude;

        if(Physics.Raycast(transform.position, dir.normalized, out RaycastHit hit,dist, wallMask))
        {
            return true; //途中で壁がある
        }
        return false;
    }
}
