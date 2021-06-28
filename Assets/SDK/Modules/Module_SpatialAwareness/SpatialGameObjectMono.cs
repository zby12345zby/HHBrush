using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialGameObjectMono : MonoBehaviour
{
    private SpatialGameObjectData data;

    public void SetDataEntity(SpatialGameObjectData data)
    {
        this.data = data;
    }

    public void Update()
    {
        data.SetTransformData(this.transform.localPosition, this.transform.localRotation, this.transform.localScale);
    }
}
