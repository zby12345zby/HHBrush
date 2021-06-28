using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[Serializable]
public class SpatialGameObjectData
{
    [SerializeField]
    private string prefabName;
    [SerializeField]
    private Vector3 localPosition;
    [SerializeField]
    private Quaternion localRotation;
    [SerializeField]
    private Vector3 localScale;

    public SpatialGameObjectData()
    { 
        
    }

    public void SetPrefabName(string prefabName)
    {
        this.prefabName = prefabName;
    }

    public string GetPrefabName()
    {
        return this.prefabName;
    }

    public void SetTransformData(Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
    {
        this.localPosition = localPosition;
        this.localRotation = localRotation;
        this.localScale = localScale;
    }

    public void GetTransformData(ref Vector3 localPosition, ref Quaternion localRotation, ref Vector3 localScale)
    {
        localPosition = this.localPosition;
        localRotation = this.localRotation;
        localScale = this.localScale;
    }
}
