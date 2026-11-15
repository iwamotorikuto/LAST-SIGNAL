using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//1つのタイヤの情報を入力する
[System.Serializable]
public class WheelInfo
{
    public Transform wheel;  //回すタイヤ
    public Vector3 axis;     //タイヤの回転軸
}

public class TireRotation : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private WaypointMove carMovement;
    [SerializeField] private WheelInfo[] wheels;

    [Header("タイヤ設定")]
    [SerializeField] private float wheelRadius = 0.35f;

    private void Update()
    {
        if (carMovement == null) return;

        float angle = CalcRotationAngle();

        foreach (WheelInfo info in wheels)
        {
            if (info.wheel == null) continue;
            info.wheel.Rotate(info.axis, angle, Space.Self);
        }
    }

    //このフレームでタイヤを回す角度
    private float CalcRotationAngle()
    {
        float radiansPerSecond = carMovement.CurrentSpeed / wheelRadius;
        return radiansPerSecond * Mathf.Rad2Deg * Time.deltaTime;
    }
}
