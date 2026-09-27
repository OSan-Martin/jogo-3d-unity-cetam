using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFPSController : MonoBehaviour
{
    [Header("Velocidades")]
    public float sneakSpeed = 1.8f;   // Ctrl
    public float walkSpeed = 2.8f;    // padrão
    public float runSpeed = 5f;       // Shift

    [Header("Pulo e Gravidade")]
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    private CharacterController controller;
    private float verticalVelocity;
    private float verticalRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // --- MOVIMENTO ---
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Define velocidade com base nas teclas
        float speed;

        if (Input.GetKey(KeyCode.LeftControl))
            speed = sneakSpeed;
        else if (Input.GetKey(KeyCode.LeftShift))
            speed = runSpeed;
        else
            speed = walkSpeed;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move *= speed;

        // --- GRAVIDADE E PULO ---
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        // --- MOUSE LOOK ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}