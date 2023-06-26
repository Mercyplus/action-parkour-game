using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct ObjectHitData
{
    public RaycastHit forwardHit;
    public RaycastHit heightHit;
    public bool forwardHitFound;
    public bool heightHitFound;
}

public struct LedgeData
{
    public float height;
    public float angle;
    public RaycastHit surfaceHit;
}

public class EnvironmentScanner : MonoBehaviour
{
    [SerializeField] private Vector3 forwardRayOffset = new Vector3(0, 2.5f, 0);
    [SerializeField] private float forwardRayLength = 0.8f;
    [SerializeField] private float heightRayLength = 5f;
    [SerializeField] private float ledgeRayLength = 10f;
    [SerializeField] private float climbLedgeRayLength = 1.5f;
    [SerializeField] private float ledgeHeightThreshold = 0.75f;
    [SerializeField] private LayerMask objectsLayer;
    [SerializeField] private LayerMask climbLedgeLayer;

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

        // Debug.DrawLine(forwardOrigin, transform.forward * forwardRayLength, (hitData.forwardHitFound) ? Color.red : Color.white);

        if (hitData.forwardHitFound)
        {
            Vector3 heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLength;
            hitData.heightHitFound = Physics.Raycast(heightOrigin, // позиция игрока + позиция луча направленная вверх
                            Vector3.down, // направление вниз
                            out hitData.heightHit, // переменная, куда записываются данные от Raycast
                            heightRayLength, // длина луча направленного вверх
                            objectsLayer // какой слой нужно проверить
                            );

            Debug.DrawLine(heightOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? Color.red : Color.white);
        }

        return hitData;
    }

    public bool ClimbLedgeCheck(Vector3 direction, out RaycastHit ledgeHit)
    {
        ledgeHit = new RaycastHit();
        float RaycastCount = 10f;

        if (direction == Vector3.zero) return false;

        var origin = transform.position + Vector3.up * 1.5f; // Насколько вверх идут полоски от Raycast
        var offset = new Vector3(0, 0.18f, 0);

        for (int i = 0; i < RaycastCount; i++)
        {
            Debug.DrawLine(origin + offset * i, direction);
            if (Physics.Raycast(origin + offset * i, direction, out RaycastHit hit, climbLedgeRayLength, climbLedgeLayer))
            {
                ledgeHit = hit;
                return true;
            }
        }

        return false;
    }

    public bool DropLedgeCheck(out RaycastHit ledgeHit)
    {
        float maxDistance = 3f;

        ledgeHit = new RaycastHit();
        var origin = transform.position + Vector3.down * 0.1f + transform.forward * 2f;

        if (Physics.Raycast(origin, -transform.forward, out RaycastHit hit, maxDistance, climbLedgeLayer))
        {
            ledgeHit = hit;
            return true;
        }

        return false;
    }

    public bool ObjectLedgeCheck(Vector3 moveDirection, out LedgeData ledgeData)
    {
        ledgeData = new LedgeData();

        if (moveDirection == Vector3.zero) return false;

        float originOffset = 0.5f;
        var origin = transform.position + moveDirection * originOffset + Vector3.up;

        List<RaycastHit> hits;
        if (PhysicsUtils.ThreeRaycasts(origin, Vector3.down, 0.25f, transform, 
            out hits, ledgeRayLength, objectsLayer, true))
        {
            List<RaycastHit> validHits = hits.Where(h => transform.position.y - h.point.y > ledgeHeightThreshold).ToList();

            if (validHits.Count > 0)
            {
                var surfaceRayOrigin = validHits[0].point;
                surfaceRayOrigin.y = transform.position.y - 0.1f;

                if (Physics.Raycast(surfaceRayOrigin, transform.position - surfaceRayOrigin, out RaycastHit surfaceHit, 2, objectsLayer))
                {
                    Debug.DrawLine(surfaceRayOrigin, transform.position, Color.cyan);
                    float height = transform.position.y - validHits[0].point.y;

                    ledgeData.angle = Vector3.Angle(transform.forward, surfaceHit.normal);
                    ledgeData.height = height;
                    ledgeData.surfaceHit = surfaceHit;

                    return true;
                }
            }
        }

        return false;
    }
}