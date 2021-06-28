using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SC.XR.Unity;
using SC.XR.Unity.Module_InputSystem.InputDeviceHand;

public class HandCursor : SCModuleMono
{
    FarPointer mfarPointer;
    public FarPointer farPointer {
        get {
            if (mfarPointer == null) {
                mfarPointer = GetComponentInParent<FarPointer>();
            }
            return mfarPointer;
        }
    }

    private SkinnedMeshRenderer skinnedMeshRenderer;
    //public float pressBlendShapeValue = 10;
    //public float pressEndBlendShapeValue = 20;
    //public float ReleaseBlendShapeValue = 100;
    private int BlendShapeCount = 0;

    private Color initColor;
    Vector3 initLocalScale = Vector3.one;
    Vector3 tempLocalScale;

    Vector3 initLocalPosition;
    Vector3 downOffset = new Vector3(0,0,0.025f);

    public override void OnSCAwake() {
        base.OnSCAwake();
        initLocalScale = transform.localScale;
        initLocalPosition = transform.localPosition;
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer) {
            initColor = skinnedMeshRenderer.material.color;
        }
        BlendShapeCount = skinnedMeshRenderer.sharedMesh.blendShapeCount;
    }

    public override void OnSCStart() {
        base.OnSCStart();
        InputDeviceHandPartEventBase.eventDelegate += CatchEvent;
        transform.localScale = initLocalScale;
        transform.localPosition = initLocalPosition;
        tempLocalScale = Vector3.zero; 
        if (BlendShapeCount != 0) {
            skinnedMeshRenderer.SetBlendShapeWeight(0, 100);
        }
    }

    public override void OnSCDisable() {
        base.OnSCDisable();
        InputDeviceHandPartEventBase.eventDelegate -= CatchEvent;
    }

    void CatchEvent(InputDeviceHandPart inputDeviceHandPart, HandEventType eventType, float eventPercent) {
        if (inputDeviceHandPart && farPointer.handDetector.inputDeviceHandPart == inputDeviceHandPart) {

            if (eventType == HandEventType.CatchDown || eventType == HandEventType.CatchDrag || eventType == HandEventType.CatchUp || eventType == HandEventType.CatchReady || eventType == HandEventType.CatchEnd) {

                tempLocalScale = initLocalScale * (1f - eventPercent * 0.2f);
                tempLocalScale.z = initLocalScale.z * (1f + eventPercent * 0.5f);
                transform.localScale = tempLocalScale;
                if (BlendShapeCount != 0) {
                    skinnedMeshRenderer.SetBlendShapeWeight(0, (1.1f - eventPercent) * 100);
                }

                if(eventType == HandEventType.CatchDown || eventType == HandEventType.CatchDrag) {
                    if (eventType == HandEventType.CatchDown) {
                        AudioSystem.getInstance.PlayAudioOneShot(gameObject, SCAudiosConfig.AudioType.Rotate_Stop);
                        if (skinnedMeshRenderer) {
                            skinnedMeshRenderer.material.color = Color.white;
                        }
                        if (BlendShapeCount != 0) {
                            skinnedMeshRenderer.SetBlendShapeWeight(0, 20);
                        }
                    }
                    tempLocalScale = initLocalScale * (0.5f);
                    tempLocalScale.z = initLocalScale.z * (1.8f);
                    transform.localScale = tempLocalScale;
                    transform.localPosition = Vector3.Lerp(transform.localPosition, initLocalPosition + downOffset, 0.5f);

                }  else if (eventType == HandEventType.CatchUp || eventType == HandEventType.CatchEnd) {
                    if (skinnedMeshRenderer) {
                        skinnedMeshRenderer.material.color = initColor;
                    }
                    transform.localScale = initLocalScale;
                    transform.localPosition = initLocalPosition;
                    if (BlendShapeCount != 0) {
                        skinnedMeshRenderer.SetBlendShapeWeight(0, 100);
                    }
                }

            }

        }

    }
}
