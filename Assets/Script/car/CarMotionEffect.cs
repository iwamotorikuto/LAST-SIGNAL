using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMotionEffect : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private WaypointMove carMovement;

    [Header("傾き")]
    [SerializeField] private float maxPitch = 2.0f;
    [SerializeField] private float maxRoll = 3.0f;
    [SerializeField] private float pitchPerAcceleration = 0.15f;
    [SerializeField] private float rollPerTurnRate = 0.03f;

    [Header("揺れ")]
    [SerializeField] private float vibrationAmount = 0.008f;
    [SerializeField] private float vibrationSpeed = 10.0f;

    [Header("滑らかさ")]
    [SerializeField] private float smoothTime = 0.25f;

    //初期姿勢
    private Quaternion initialRotation;
    private Vector3 initialPosition;

    //前のフレームの情報
    private Vector3 previousVelocity;
    private Vector3 previousForward;

    //現在の傾きと、SmoothDampが内部で必要となる値
    private float currentPitch;
    private float currentRoll;
    private float pitchVelocity;
    private float rollVelocity;

    void Start()
    {
        if (carMovement == null)
        {
            enabled = false;
            return;
        }

        initialRotation = transform.localRotation;
        initialPosition = transform.localPosition;
        previousForward = carMovement.transform.forward;
    }

    void LateUpdate()
    {
        float dt = Mathf.Max(Time.deltaTime, 0.0001f);

        float targetPitch = CalcTargetPitch(dt);
        float targetRoll = CalcTargetRoll(dt);

        //目標へ滑らかに近づける
        currentPitch = Mathf.SmoothDamp(currentPitch, targetPitch, ref pitchVelocity, smoothTime);
        currentRoll = Mathf.SmoothDamp(currentRoll, targetRoll, ref rollVelocity, smoothTime);

        ApplyTilt();
        ApplyVibration();

        //次のフレームのためへの記録
        previousVelocity = carMovement.CurrentVelocity;
        previousForward = carMovement.transform.forward;
    }

    //加減速から前後の傾きを決める
    private float CalcTargetPitch(float dt)
    {
        Vector3 deltaVelocity = carMovement.CurrentVelocity - previousVelocity;
        float acceleration = Vector3.Dot(deltaVelocity, carMovement.transform.forward) / dt;

        return Mathf.Clamp(-acceleration * pitchPerAcceleration, -maxPitch, maxPitch);
    }

    //旋回から左右の傾きを求める
    private float CalcTargetRoll(float dt)
    {
        float turnAngle = Vector3.SignedAngle(previousForward, carMovement.transform.forward, Vector3.up);

        float turnRate = turnAngle / dt;

        return Mathf.Clamp(turnRate * rollPerTurnRate, -maxRoll, maxRoll);
    }

    private void ApplyTilt()
    {
        transform.localRotation = initialRotation * Quaternion.Euler(currentPitch, 0.0f, currentRoll);
    }

    //走行中のみ上下に少しの揺れを加える
    private void ApplyVibration()
    {
        float offset = 0.0f;

        if(carMovement.IsMoving)
        {
            offset = Mathf.Sin(Time.time * vibrationSpeed) * vibrationAmount;
        }

        transform.localPosition = initialPosition + Vector3.up * offset;
    }
}
