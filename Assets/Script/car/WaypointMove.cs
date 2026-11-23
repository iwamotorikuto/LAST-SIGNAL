using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WaypointMove : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float speed = 15.0f;
    [SerializeField] private float turnSpeed = 5.0f;
    [SerializeField] private float arrivalDistance = 0.3f;

    private int currentIndex = 0;

    //他のスクリプトから読むための情報
    public Vector3 CurrentVelocity { get; private set; }
    public float CurrentSpeed => CurrentVelocity.magnitude;
    public bool IsMoving => CurrentSpeed > 0.01f;

    private void Update()
    {
        //毎フレームごとに停止状態にする
        CurrentVelocity = Vector3.zero;

        if (!HasTarget()) return;

        Transform target = waypoints[currentIndex];
        Vector3 toTarget = target.position - transform.position;

        //到着したら次のWaypointへ移動する
        if(toTarget.magnitude <= arrivalDistance)
        {
            currentIndex++;
            return;
        }
        Rotate(toTarget);
        Move(target.position);
    }

    //目的地が有効かどうかを決める
    private bool HasTarget()
    {
        return waypoints != null && currentIndex < waypoints.Length && waypoints[currentIndex] != null;
    }

    //目的地の方向へ滑らかに回転する
    private void Rotate(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);

        //フレームに依存しない補間係数
        float t = 1.0f - Mathf.Exp(-turnSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
    }

    //前進して、速度を記録する
    private void Move(Vector3 targetPosition)
    {
        Vector3 before = transform.position;

        transform.position = Vector3.MoveTowards(before, targetPosition, speed * Time.deltaTime);

        CurrentVelocity = (transform.position - before) / Mathf.Max(Time.deltaTime, 0.0001f);
    }
}
