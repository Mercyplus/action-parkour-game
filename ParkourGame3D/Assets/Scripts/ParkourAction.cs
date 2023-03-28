using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Parkour System/New parkour action")]
public class ParkourAction : ScriptableObject
{
    [SerializeField] private string animatorName;
    [SerializeField] private string objectTag;
    
    [SerializeField] private float minHeight; // минимальная высота
    [SerializeField] private float maxHeight; // максимальная высота

    [SerializeField] private bool rotateToObjects;
    [SerializeField] private float postActionDelay;

    [Header("Target Matching")]
    [SerializeField] private bool enableTargetMatching = true;
    [SerializeField] protected AvatarTarget matchBodyPart;
    [SerializeField] private float matchStartTime;
    [SerializeField] private float matchTargetTime;
    [SerializeField] Vector3 matchPositionWeight = new Vector3(0, 1, 0);

    public Quaternion TargetRotation { get; set; }
    public Vector3 MatchPosition { get; set; }
    public bool MirrorAnimation { get; set; }

    public virtual bool CheckIfPossible(ObjectHitData hitData, Transform player)
    {
        // Проверка тега
        if (!string.IsNullOrEmpty(objectTag) && hitData.forwardHit.transform.tag != objectTag)
        {
            return false;
        }

        // Тег высоты
        float height = hitData.heightHit.point.y - player.position.y;
        if (height < minHeight || height > maxHeight)
        {
            return false;
        }

        if (rotateToObjects)
        {
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);
        }

        if (enableTargetMatching)
        {
            MatchPosition = hitData.heightHit.point;
        }

        return true;
    }

    public string AnimatorName => animatorName;
    public bool RotateToObjects => rotateToObjects;
    public float PostActionDelay => postActionDelay;
    
    public bool EnableTargetMatching => enableTargetMatching;
    public AvatarTarget MatchBodyPart => matchBodyPart;
    public float MatchStartTime => matchStartTime;
    public float MatchTargetTime => matchTargetTime;
    public Vector3 MatchPositionWeight => matchPositionWeight;
}