using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("--- 移動設定 ---")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float runSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float aimSpeed = 1.5f;
    [SerializeField] private float strafeSpeed = 2.0f; // ストレイフ中の速度
    [SerializeField] private float gravity = -9.0f;
    [SerializeField] private float jumpHeight = 3.0f;

    // 内部状態管理
    private CharacterController controller;
    private Vector3 velocity;

    // フラグ管理（外部からも参照できるように公開）
    public bool IsAiming { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsStrafing { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleStrafing(); // Iキーの状態を取得
        HandleMovement(); // 移動と速度の処理
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // ジャンプ操作
        IsJumping = false; // 毎フレームリセット
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);
            IsJumping = true;
        }

        // 入力状態の判定（少しでも入力があれば移動中とする）
        IsMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f;
        bool runRequested = Input.GetKey(KeyCode.LeftShift);

        IsAiming = Input.GetMouseButton(1);
        IsCrouching = Input.GetKey(KeyCode.LeftControl);

        // 走る条件（ストレイフ中や構え中、しゃがみ中は走らない）
        IsRunning = runRequested && IsMoving && !IsAiming && !IsCrouching && !IsStrafing;

        // 速度の決定
        float currentSpeed = walkSpeed;

        if (IsAiming)
        {
            currentSpeed = aimSpeed;
        }
        else if (IsCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (IsStrafing)
        {
            currentSpeed = strafeSpeed;
        }
        else if (IsRunning)
        {
            currentSpeed = runSpeed;
        }

        // 移動方向の計算
        Vector3 move = Vector3.zero;

        if (IsMoving)
        {
            move = (transform.right * moveX + transform.forward * moveZ).normalized;
        }

        // ★入力がないときは強制的に移動量をゼロにして、勝手に滑る・動くのを防止する
        if (!IsMoving)
        {
            move = Vector3.zero;
        }

        // 移動を実行
        controller.Move(move * currentSpeed * Time.deltaTime);

        // 接地時の重力リセット
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2.0f;
        }

        // 重力の適用
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleStrafing()
    {
        // ストレイフの処理（Iキーを押している間はTrue）
        IsStrafing = Input.GetKey(KeyCode.I);
    }
}
