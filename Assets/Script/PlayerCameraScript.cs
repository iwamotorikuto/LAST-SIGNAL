using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraScript : MonoBehaviour
{
    public GameObject Player; //playerのゲームオブジェクトを入れる変数を設定

    public float rotateSpeed = 100.0f; //左右の回転速度
    public float pitchSpeed  = 2.0f;   //上下の回転速度
    public float minPitch    = -20.0f; //下限(マイナスでカメラが低くなる)
    public float maxPitch    = 10.0f;  //上限(プラスでカメラが高くなる)

    public float distance    = 15.0f;
    public float height      = 2.0f;
    public float followSpeed = 100.0f;

    public Camera cam;
    public float normalFOV = 60.0f;
    public float zoomedFOV = 30.0f;
    public float zoomSpeed = 5.0f;
    public LayerMask wallMask;

    float pitch = 0.0f; //現状の上下角度

    void Start()
    {
        if (cam == null) cam = GetComponent<Camera>();
    }

    void Update()
    {
        //左右:Player回転
        float mx = Input.GetAxis("Mouse X");
        Player.transform.Rotate(Vector3.up, mx * rotateSpeed * Time.deltaTime);

        //上下:角度を更新し、範囲内に制限
        float my = Input.GetAxis("Mouse Y");
        pitch -= my * pitchSpeed;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Player側の基準点
        Vector3 pivot = Player.transform.position + Vector3.up * height;

        //Playerの向き + 上下角度から、カメラの方向を決める
        Quaternion rot = Player.transform.rotation * Quaternion.Euler(pitch, 0.0f, 0.0f);
        Vector3 desiredPos = pivot - rot * Vector3.forward * distance;

        Vector3 dirToCam  = desiredPos - pivot;
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
        transform.LookAt(Player.transform.position + Vector3.up * height * 0.5f);

        //bool isBlocked = CheckWallBlocking();
        float targetFOV = CheckWallBlocking() ? zoomedFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }

    bool CheckWallBlocking()
    {
        Vector3 dir = Player.transform.position - transform.position;
        return Physics.Raycast(transform.position, dir.normalized, dir.magnitude, wallMask);
    }
}
