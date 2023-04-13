using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUtils
{
    /* метод для выполнения трех лучей (направленных лучей) из точки origin и в направлении direction, 
     с заданным расстоянием spacing между ними, и проверкой столкновений с объектами в слое layer.
     метод возвращает true, если было найдено хотя бы одно столкновение и сохраняет в hits информацию 
     обо всех трех лучах в виде списка столкновений. 
     Параметры hits, distance, layer, debugDraw являются необязательными и могут быть пропущены при вызове метода.
     Параметр transform используется для определения вектора смещения влево/вправо от origin при проведении 
     лучей с расстоянием spacing между ними. */

    public static bool ThreeRaycasts(Vector3 origin, Vector3 direction, float spacing, Transform transform,
        out List<RaycastHit> hits, float distance, LayerMask layer, bool debugDraw=false)
    {
        // выполняется проверка наличия столкновения луча, направленного вниз от центра origin до расстояния distance.
        bool centerHitFound = Physics.Raycast(origin, Vector3.down, out RaycastHit centerHit, distance, layer);
        // выполняется проверка наличия столкновения луча, направленного вниз от точки origin с учетом смещения transform.right на spacing влево.
        bool leftHitFound = Physics.Raycast(origin - transform.right * spacing, Vector3.down, out RaycastHit leftHit, distance, layer);
        // выполняется проверка наличия столкновения луча, направленного вниз от точки origin с учетом смещения transform.right на spacing вправо.
        bool rightHitFound = Physics.Raycast(origin + transform.right * spacing, Vector3.down, out RaycastHit rightHit, distance, layer);

        // все найденные столкновения сохраняются в список hits в порядке центр, лево, право.
        hits = new List<RaycastHit>() { centerHit, leftHit, rightHit };

        bool hitFound = centerHitFound || leftHitFound || rightHitFound;

        // если хотя бы одно столкновение было найдено, и флаг debugDraw установлен в true,
        // то выполняется отрисовка линий между точками начала теста origin и точками столкновений: центральным (centerHit), 
        // левым (leftHit) и правым (rightHit). Цвет линий задается в виде красного цвета.
        if (hitFound && debugDraw)
        {
            Debug.DrawLine(origin, centerHit.point, Color.red);
            Debug.DrawLine(origin - transform.right * spacing, leftHit.point, Color.red);
            Debug.DrawLine(origin + transform.right * spacing, rightHit.point, Color.red);
        }

        // метод возвращает true, если хотя бы одно столкновение было найдено
        return hitFound;
    }
}