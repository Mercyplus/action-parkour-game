using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerCameraController cameraController;
    private CharacterController characterController;
    private Animator animator;
    private EnvironmentScanner environmentScanner;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 500f;
    public float RotationSpeed => rotationSpeed;

    [Header("Ground Check info")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] private LayerMask whatIsGround;

    public bool IsOnLedge { get; set; }
    public LedgeData LedgeData { get; set; }

    private Vector3 desiredMoveDirection;
    private Vector3 moveDirection;
    private Vector3 velocity;

    private float ySpeed;
    private Quaternion targerRotation;
    private bool isGrounded;
    private bool hasControl = true;

    public bool InAction { get; private set; }
    public bool IsHanging { get; set; }

    private void Start()
    {
        cameraController = Camera.main.GetComponent<PlayerCameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        environmentScanner = GetComponent<EnvironmentScanner>();
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

        desiredMoveDirection = cameraController.GetHorizontalRotation() * moveInput;
        moveDirection = desiredMoveDirection;

        if (!hasControl) return;
        if (IsHanging) return;

        velocity = Vector3.zero;

        animator.SetBool("IsGrounded", isGrounded);
        if (isGrounded)
        {
            ySpeed = -0.5f;
            velocity = desiredMoveDirection * moveSpeed;

            IsOnLedge = environmentScanner.ObjectLedgeCheck(desiredMoveDirection, out LedgeData ledgeData);
            if (IsOnLedge)
            {
                LedgeData = ledgeData;
                LedgeMovement();
            }

            animator.SetFloat("moveAmount", velocity.magnitude / moveSpeed, 0.2f, Time.deltaTime);

        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
            velocity = transform.forward * moveSpeed / 2;
        }
        
        
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        if (moveAmount > 0 && moveDirection.magnitude > 0.2f)
        {
            targerRotation = Quaternion.LookRotation(moveDirection);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targerRotation, rotationSpeed * Time.deltaTime);
    }

    private void LedgeMovement()
    {
        float signedAngle = Vector3.SignedAngle(LedgeData.surfaceHit.normal, desiredMoveDirection, Vector3.up);
        float angle = Mathf.Abs(signedAngle);

        if (Vector3.Angle(desiredMoveDirection, transform.forward) >= 80)
        {
            // перестаем двигаться, только поворот
            velocity = Vector3.zero;
            return;
        }

        if (angle < 60)
        {
            velocity = Vector3.zero;
            moveDirection = Vector3.zero;
        }
        else if (angle < 90)
        {
            //Угол составляет от 60 до 90, поэтому ограничиваем скорость горизонтальным направлением
            var left = Vector3.Cross(Vector3.up, LedgeData.surfaceHit.normal);
            var direction = left * Mathf.Sign(signedAngle);
            velocity = velocity.magnitude * direction;
            moveDirection = direction;
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, whatIsGround);
    }

    public IEnumerator DoAction(string animatorName, MatchTargetParams matchParams=null, Quaternion targetRotation=new Quaternion(), bool rotate=false, 
                float postDelay=0, bool mirror=false)
    {
        InAction = true;

        animator.SetBool("mirrorAction", mirror);
        animator.CrossFadeInFixedTime(animatorName, 0.2f);
        yield return null;

        var animatorState = animator.GetNextAnimatorStateInfo(0);
        if (!animatorState.IsName(animatorName))
        {
            Debug.Log("неправильная анимация паркура ");
        }

        float rotateStartTime = (matchParams != null) ? matchParams.startTime : 0f;

        float timer = 0;
        while (timer <= animatorState.length)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / animatorState.length;

            // Поворот игрока в сторону препятствия
            if (rotate && normalizedTime > rotateStartTime)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            if (matchParams != null)
            {
                MatchTarget(matchParams);
            }

            if (animator.IsInTransition(0) && timer > 0.5f) break;
            yield return null;
        }

        yield return new WaitForSeconds(postDelay);

        InAction = false;
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

    public void EnableCharacterController(bool enabled)
    {
        characterController.enabled = enabled;
    }

    public void RestTargetRotation()
    {
        targerRotation = transform.rotation;
    }

    public bool HasControl
    { 
        get => hasControl;
        set => hasControl = value;
    }

    private void MatchTarget(MatchTargetParams matchTargetParams)
    {
        if (animator.isMatchingTarget || animator.IsInTransition(0)) return;

        animator.MatchTarget(matchTargetParams.position,
                            transform.rotation, 
                            matchTargetParams.bodyPart, 
                            new MatchTargetWeightMask(matchTargetParams.positionWeight, 0), 
                            matchTargetParams.startTime, 
                            matchTargetParams.targetTime);
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }
}
