using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector2 framingOffset;
    
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float distanceToPlayer = 5f;

    [SerializeField] private float minVerticalAngle = -45f;
    [SerializeField] private float maxVerticalAngle = 45f;

    [SerializeField] private bool invertX;
    [SerializeField] private bool invertY;

    private float rotationX;
    private float rotationY;

    private float invertXvalue;
    private float invertYvalue;

    private void Start() 
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        SetupCameraPlayer();
    }

    private void SetupCameraPlayer()
    {
        invertXvalue = (invertX) ? -1 : 1;
        invertYvalue = (invertY) ? -1 : 1;

        rotationX += Input.GetAxis("Mouse Y") * invertYvalue * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);
        rotationY += Input.GetAxis("Mouse X") * invertXvalue * rotationSpeed;

        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
        var focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y);

        transform.position = focusPosition - targetRotation * new Vector3(0, 0, distanceToPlayer);
        transform.rotation = targetRotation;
    }

    public Quaternion GetPlanarRotation()
    {
        return Quaternion.Euler(0, rotationY, 0);
    }
}