using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SafetyAreaInfo
{
    public SafetyAreaInfo()
    {
        safetyAreaName = string.Empty;
        showAreaWhenBowHead = false;
        originAlphaParam = 0f;
        originSafetyAreaColorIndex = 0;
        originHeight = 0f;
    }

    public string safetyAreaName;
    public Vector3 position;
    public List<Vector3> vertices;
    public List<int> triangles;
    public List<Vector2> uv;
    public float originHeight;
    public float perimeter;

    //playarea
    public List<Color> colors;
    //stationaryArea
    public float radius;

    public bool showAreaWhenBowHead;
    public float originAlphaParam;
    public int originSafetyAreaColorIndex;
}
