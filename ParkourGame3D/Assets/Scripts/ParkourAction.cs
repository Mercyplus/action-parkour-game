using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Parkour System/New parkour action")]
public class ParkourAction : ScriptableObject
{
    [SerializeField] private string animatorName;
    
    [SerializeField] private float minHeight; // минимальная высота
    [SerializeField] private float maxHeight; // максимальная высота

    [SerializeField] private bool rotateToObjects;

    public Quaternion TargetRotation { get; set; }

    public bool CheckIfPossible(ObjectHitData hitData, Transform player)
    {
        float height = hitData.heightHit.point.y - player.position.y;
        if (height < minHeight || height > maxHeight)
        {
            return false;
        }

        if (rotateToObjects)
        {
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);
        }

        return true;
    }

    public string AnimatorName => animatorName;
    public bool RotateToObjects => rotateToObjects;
}