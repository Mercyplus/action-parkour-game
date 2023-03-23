using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerCameraController cameraController;
    private CharacterController characterController;
    private Animator animator;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 500f;
    public float RotationSpeed => rotationSpeed;

    [Header("Ground Check info")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] private LayerMask whatIsGround;

    private float ySpeed;
    private Quaternion targerRotation;
    private bool isGrounded;
    private bool hasControl = true;

    private void Start()
    {
        cameraController = Camera.main.GetComponent<PlayerCameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        PlayerMove();
        GroundCheck();
    }
    
    private void PlayerMove()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        var moveInput = (new Vector3(horizontal, 0, vertical)).normalized;

        var moveDirection = cameraController.GetPlanarRotation() * moveInput;

        if (!hasControl) return;

        if (isGrounded)
        {
            ySpeed = -0.5f;
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }
        
        var velocity = moveDirection * moveSpeed;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        if (moveAmount > 0)
        {
            targerRotation = Quaternion.LookRotation(moveDirection);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targerRotation, rotationSpeed * Time.deltaTime);

        animator.SetFloat("moveAmount", moveAmount);
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, whatIsGround);
    }

    public void SetControl(bool hasControl)
    {
        this.hasControl = hasControl;
        characterController.enabled = hasControl;

        if (!hasControl)
        {
            animator.SetFloat("moveAmount", 0f);
            targerRotation = transform.rotation;
        }
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }
}
