using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCPolygonUtils
{
    public static bool AreVerticesEqual(List<Vector3> firstList, List<Vector3> secondList)
    {
        if (firstList.Count != secondList.Count)
        {
            return false;
        }

        for (int i = 0; i < firstList.Count; i++)
        {
            if (firstList[i] != secondList[i])
            {
                return false;
            }
        }

        return true;
    }

    public static Vector3 GetPolygonGravityPoint(Vector3[] points)
    {
        var sum_x = 0.0f;
        var sum_z = 0.0f;
        var sum_area = 0.0f;
        var p1 = points[1];
        for (var i = 2; i < points.Length; i++)
        {
            var p2 = points[i];
            var area = PolygonArea(points[0], p1, p2);
            sum_area += area;
            sum_x += (points[0].x + p1.x + p2.x) * area;
            sum_z += (points[0].z + p1.z + p2.z) * area;
            p1 = p2;
        }
        var xx = sum_x / sum_area / 3;
        var zz = sum_z / sum_area / 3;
        return new Vector3(xx, p1.y, zz);
    }

    private static float PolygonArea(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        var area = 0.0f;
        area = p0.x * p1.z + p1.x * p2.z + p2.x * p0.z - p1.x * p0.z - p2.x * p1.z - p0.x * p2.z;
        return area / 2.0f;
    }
}
