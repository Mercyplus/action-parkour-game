using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ObjectHitData
{
    public RaycastHit forwardHit;
    public RaycastHit heightHit;
    public bool forwardHitFound;
    public bool heightHitFound;
}

public class EnvironmentScanner : MonoBehaviour
{
    [SerializeField] private Vector3 forwardRayOffset = new Vector3(0, 2.5f, 0);
    [SerializeField] private float forwardRayLength = 0.8f;
    [SerializeField] private float heightRayLength = 5f;
    [SerializeField] private LayerMask objectsLayer;

    public ObjectHitData ObjectCheck()
    {
        var hitData = new ObjectHitData();

        Vector3 forwardOrigin = transform.position + forwardRayOffset;
        hitData.forwardHitFound = Physics.Raycast(forwardOrigin, // позиция игрока + позиция луча направленная вперед
                        transform.forward, // направление вперед
                        out hitData.forwardHit, // переменная, куда записываются данные от Raycast
                        forwardRayLength, // длина луча напрвленного вперед
                        objectsLayer // какой слой нужно проверить
                        );

        Debug.DrawLine(forwardOrigin, transform.forward * forwardRayLength, (hitData.forwardHitFound) ? Color.red : Color.white);

        if (hitData.forwardHitFound)
        {
            Vector3 heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLength;
            hitData.heightHitFound = Physics.Raycast(heightOrigin, // позиция игрока + позиция луча направленная вверх
                            Vector3.down, // направление вниз
                            out hitData.heightHit, // переменная, куда записываются данные от Raycast
                            heightRayLength, // длина луча напрвленного вверх
                            objectsLayer // какой слой нужно проверить
                            );

            Debug.DrawLine(heightOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? Color.red : Color.white);
        }

        return hitData;
    }
}