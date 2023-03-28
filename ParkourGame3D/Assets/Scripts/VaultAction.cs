using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Parkour System/New vault action")]
public class VaultAction : ParkourAction
{
    // Проверка возможности выполнения действия "vault"
    public override bool CheckIfPossible(ObjectHitData hitData, Transform player)
    {
        // Проверка базовых условий
        if (!base.CheckIfPossible(hitData, player))
        {
            return false;
        }

        // Получение точки столкновения в локальной системе координат
        var localHitPoint = hitData.forwardHit.transform.InverseTransformPoint(hitData.forwardHit.point);

        // Определение зеркальности анимации
        SetMirrorAnimation(localHitPoint);

        // Возврат результата базового метода
        return base.CheckIfPossible(hitData, player);
    }

    // Определение зеркальности анимации
    private void SetMirrorAnimation(Vector3 hitPoint)
    {
        // Определение целевой части тела для анимации
        matchBodyPart = (hitPoint.z < 0 && hitPoint.x < 0 || hitPoint.z > 0 && hitPoint.x > 0) 
            ? AvatarTarget.RightHand 
            : AvatarTarget.LeftHand;
        
        // Установка зеркальности анимации
        MirrorAnimation = !matchBodyPart.Equals(AvatarTarget.LeftHand);
    }
}