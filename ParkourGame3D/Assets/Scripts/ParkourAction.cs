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

    // Проверяет возможность столкновения с объектом
    public virtual bool CheckIfPossible(ObjectHitData hitData, Transform player)
    {
        // Проверяем соответствие тега объекта
        if (CheckTag(hitData))
        {
            // Проверяем соответствие высоты объекта
            if (CheckHeight(hitData, player))
            {
                // Если включено вращение к объектам, вычисляем целевую ориентацию
                if (rotateToObjects)
                {
                    TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);
                }

                // Если включено совпадение позиций, вычисляем координаты
                if (enableTargetMatching)
                {
                    MatchPosition = hitData.heightHit.point;
                }

                // Если объект соответствует всем условиям, возвращаем true
                return true;
            }
        }

        // Если объект не соответствует условиям, возвращаем false
        return false;
    }

    // Проверяет соответствие тега объекта
    private bool CheckTag(ObjectHitData hitData)
    {
        // Если тег объекта задан и не совпадает с заданным тегом, возвращаем false
        if (!string.IsNullOrEmpty(objectTag) && hitData.forwardHit.transform.tag != objectTag)
        {
            return false;
        }

        // Иначе возвращаем true
        return true;
    }

    // Проверяет соответствие высоты объекта
    private bool CheckHeight(ObjectHitData hitData, Transform player)
    {
        // Вычисляем высоту объекта относительно игрока
        float height = hitData.heightHit.point.y - player.position.y;

        // Если высота объекта меньше заданной минимальной высоты или больше заданной максимальной высоты, возвращаем false
        if (height < minHeight || height > maxHeight)
        {
            return false;
        }

        // Иначе возвращаем true
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