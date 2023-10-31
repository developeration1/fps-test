using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private readonly static float gravity = 9.8f;
    private float vForce = 0;

    [SerializeField] Transform head; //Represents the head, where the camera is.
    [SerializeField] float speed, mouseSensibility;
    [SerializeField] Vector2 xRotationLimits; //X represents the limit on the bottom, and Y represents the limit for the top
    [SerializeField] bool invertedYRotation;
    [SerializeField] float jumpForce;

    private CharacterController charCon;

    private Controls controls;

    private Vector2 eulerRotation = Vector2.zero; //Represents the rotation in Eulers, in case is needed later.

    private void Awake()
    {
        controls = new();
        charCon = GetComponent<CharacterController>();
        controls.Player.Jump.performed += Jump;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void FixedUpdate()
    {
        OnGroundMovement();
        OffGroundMovement();
    }

    private void Update()
    {
        Rotation();
    }

    private void OnGroundMovement() //Controls the Movement. Goes in the FixedUpdate.
    {
        Vector2 input = controls.Player.Move.ReadValue<Vector2>();
        Vector3 direction = (transform.forward * input.y + transform.right * input.x).normalized;
        charCon.Move(speed * Time.deltaTime * direction);
    }

    private void Rotation()//Controls the rotation depending on the mouse. Goes in Update.
    {
        Vector2 delta = controls.Player.PointerDelta.ReadValue<Vector2>().normalized;
        transform.Rotate(new(0, delta.x * mouseSensibility, 0));
        eulerRotation.y = transform.rotation.y;
        eulerRotation.x = Mathf.Clamp(delta.y * mouseSensibility + eulerRotation.x, xRotationLimits.x, xRotationLimits.y);
        head.localRotation = Quaternion.Euler(invertedYRotation ? -eulerRotation.x : eulerRotation.x, 0, 0);
    }

    private void OffGroundMovement()
    {
        vForce -= gravity * Time.deltaTime;
        charCon.Move(new(0, vForce * Time.deltaTime, 0));
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (charCon.isGrounded)
        {
            vForce = Mathf.Sqrt(jumpForce * -2 * -gravity);
        }
    }
}