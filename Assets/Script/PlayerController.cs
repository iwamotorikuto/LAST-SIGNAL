using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

////////////////////////////////////////// 
/// 操作方法：
/// 移動：キーボード (WASD / 矢印) または ゲームパッド (左スティック)
/// ジャンプ：Spaceキー または ゲームパッド (Aボタン)
/// しゃがみ：LeftControlキー または ゲームパッド (Bボタン)
/// カメラ回転：マウス または ゲームパッド (右スティック)
///////////////////////////////////////


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("--- 移動設定 ---")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float runSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float aimSpeed = 1.5f;
    [SerializeField] private float strafeSpeed = 2.0f;
    [SerializeField] private float gravity = -9.0f;
    [SerializeField] private float jumpHeight = 3.0f;
    [SerializeField] private float rotateSpeed = 100.0f;

    private CharacterController controller;
    private Vector3 velocity;

    public bool IsAiming { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsStrafing { get; private set; }

    public GameObject Cube1;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleStrafing();
        HandleMovement();
    }

    private void HandleMovement()
    {
        float moveX = 0f;
        float moveZ = 0f;

        Keyboard kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) moveX -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) moveX += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) moveZ -= 1f;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) moveZ += 1f;
        }

        Gamepad pad = Gamepad.current;
        if (pad != null)
        {
            Vector2 leftStick = pad.leftStick.ReadValue();
            if (Mathf.Abs(leftStick.x) > 0.1f) moveX = leftStick.x;
            if (Mathf.Abs(leftStick.y) > 0.1f) moveZ = leftStick.y;
        }

        IsJumping = false;
        bool jumpPressed = (kb != null && kb.spaceKey.wasPressedThisFrame) ||
                           (pad != null && pad.buttonSouth.wasPressedThisFrame);

        if (jumpPressed && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);
            IsJumping = true;
        }

        IsMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f;

        bool runRequested = (kb != null && kb.leftShiftKey.isPressed) ||
                            (pad != null && pad.leftStickButton.isPressed);

        //  Mouse.current.rightButton を使用するように変更
        IsAiming = (Mouse.current != null && Mouse.current.rightButton.isPressed) ||
                   (pad != null && pad.leftTrigger.isPressed);

        IsCrouching = (kb != null && kb.leftCtrlKey.isPressed) ||
                      (pad != null && pad.buttonEast.isPressed);

        IsRunning = runRequested && IsMoving && !IsAiming && !IsCrouching && !IsStrafing;

        float currentSpeed = walkSpeed;
        if (IsAiming) currentSpeed = aimSpeed;
        else if (IsCrouching) currentSpeed = crouchSpeed;
        else if (IsStrafing) currentSpeed = strafeSpeed;
        else if (IsRunning) currentSpeed = runSpeed;

        Vector3 move = Vector3.zero;
        if (IsMoving)
        {
            move = (transform.right * moveX + transform.forward * moveZ).normalized;
        }

        float lookDeltaX = 0f;
        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            lookDeltaX = mouse.delta.x.ReadValue() * 0.1f;
        }

        if (pad != null)
        {
            Vector2 rightStick = pad.rightStick.ReadValue();
            if (Mathf.Abs(rightStick.x) > 0.1f)
            {
                lookDeltaX += rightStick.x * 2.5f;
            }
        }

        if (Mathf.Abs(lookDeltaX) > 0.001f)
        {
            transform.RotateAround(transform.position, Vector3.up, lookDeltaX * rotateSpeed * Time.deltaTime);
        }

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2.0f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleStrafing()
    {
        Keyboard kb = Keyboard.current;
        Gamepad pad = Gamepad.current;

        IsStrafing = (kb != null && kb.iKey.isPressed) ||
                     (pad != null && pad.buttonNorth.isPressed);
    }
}