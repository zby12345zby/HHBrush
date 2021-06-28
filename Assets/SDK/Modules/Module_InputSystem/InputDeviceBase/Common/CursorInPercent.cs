using SC.XR.Unity.Module_InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorInPercent :MonoBehaviour,IPointerExitHandler,IPointerEnterHandler  {
    [SerializeField]
    private BoxCollider boxCollider;


    // Half the size of the current bounds
    private Vector3 currentBoundsExtents;
    public Vector3 CurrentBoundsExtents {
        get {
            return currentBoundsExtents;
        }
    }

    // Half the size of the current bounds
    private Vector3 initBoundsExtents;
    public Vector3 InitBoundsExtents {
        get {
            return initBoundsExtents;
        }
    }

    // Half the size of the current bounds
    private Vector3 currentBoundsCenter;
    public Vector3 CurrentBoundsCenter {
        get {
            return currentBoundsCenter;
        }
    }

    public enum Face {
        XY,
        YZ,
        XZ,
    }

    public Face face = Face.XY;

    [Range(0, 1)]
    [SerializeField]
    private float SetPercent1Start0 = 0.3f;

    private float m_InPercentStart0;
    public float CurrentInPercentStart0 {
        get {
            return m_InPercentStart0;
        }
        private set {
            m_InPercentStart0 = value;
        }
    }

    protected Vector3 InitScale { get; private set; }

    protected Vector3 WorldCenter;
    public void Start() {
        if (boxCollider == null) {
            boxCollider = GetComponent<BoxCollider>();
        }

        if (boxCollider == null) {
            Debug.LogError("Error ! No BoxCollider , HandCursorEffect will no work !");
            return;
        }

        // Store current rotation then zero out the rotation so that the bounds
        // are computed when the object is in its 'axis aligned orientation'.
        Quaternion currentRotation = transform.rotation;
        transform.rotation = Quaternion.identity;
        Physics.SyncTransforms(); // Update collider bounds

        initBoundsExtents = boxCollider.bounds.extents;
        //currentBoundsCenter = boxCollider.bounds.center;
        currentBoundsCenter = boxCollider.center;

        WorldCenter = currentBoundsCenter;

        InitScale = boxCollider.transform.lossyScale;
        //Debug.Log(initBoundsExtents.x + " " + initBoundsExtents.y + " " + initBoundsExtents.z);

        // After bounds are computed, restore rotation...
        transform.rotation = currentRotation;
        Physics.SyncTransforms();

    }
    PointerEventData currentPointerEventData;

    Vector3 deltaV;
    float wPercent, hPercent;
    void LateUpdate() {
        if (boxCollider == null)
            return;

        if (currentPointerEventData == null)
            return;

        currentBoundsExtents.x = InitBoundsExtents.x * (boxCollider.transform.lossyScale.x / InitScale.x);
        currentBoundsExtents.y = InitBoundsExtents.y * (boxCollider.transform.lossyScale.y / InitScale.y);
        currentBoundsExtents.z = InitBoundsExtents.z * (boxCollider.transform.lossyScale.z / InitScale.z);

        SetPercent1Start0 = Mathf.Clamp(SetPercent1Start0, 0, 1);
        //WorldCenter = currentBoundsCenter;
        WorldCenter = transform.TransformPoint( currentBoundsCenter);
        //WorldCenter = boxCollider.bounds.center;
        //Debug.Log(WorldCenter);

        deltaV = currentPointerEventData.pointerCurrentRaycast.worldPosition - WorldCenter;

        if (face == Face.XY) {
            wPercent = Vector3.Dot(deltaV, transform.right) / CurrentBoundsExtents.x;
            hPercent = Vector3.Dot(deltaV, transform.up) / CurrentBoundsExtents.y;
        } else if (face == Face.YZ) {
            wPercent = Vector3.Dot(deltaV, transform.forward) / CurrentBoundsExtents.z;
            hPercent = Vector3.Dot(deltaV, transform.up) / CurrentBoundsExtents.y;
        } else if (face == Face.XZ) {
            wPercent = Vector3.Dot(deltaV, transform.forward) / CurrentBoundsExtents.z;
            hPercent = Vector3.Dot(deltaV, transform.right) / CurrentBoundsExtents.x;
        }

        wPercent = Mathf.Abs(wPercent);
        hPercent = Mathf.Abs(hPercent);
        wPercent = Mathf.Clamp01(wPercent);
        hPercent = Mathf.Clamp01(hPercent);

        float maxPercent = Mathf.Max(wPercent, hPercent);

        m_InPercentStart0 = (1 / SetPercent1Start0) * (1 - maxPercent);
        m_InPercentStart0 = Mathf.Clamp01(m_InPercentStart0);
        //Debug.Log(wPercent + "  " + hPercent + "  " + m_InPercentStart0+" "+ deltaV.x+" "+ deltaV.y+" "+ deltaV.z +"  "+ currentBoundsExtents.x + "  " + currentBoundsExtents.y + "  " + currentBoundsExtents.z);

    }

    public void OnPointerEnter(PointerEventData eventData) {
        currentPointerEventData = eventData;
    }

    public void OnPointerExit(PointerEventData eventData) {
        currentPointerEventData = null;
    }

}
