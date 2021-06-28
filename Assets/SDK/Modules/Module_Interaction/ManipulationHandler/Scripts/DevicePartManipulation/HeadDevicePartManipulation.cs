using SC.XR.Unity.Module_InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeadDevicePartManipulation : AbstractDevicePartManipulation
{
    private SCPointEventData eventData;
    private SCPose pointerPose;

    private Quaternion startRotation;
    private Quaternion localRotationInHead;

    public override void OneDevicePartInit(ManipulationHandler manipulationHandler, Dictionary<InputDevicePartType, SCPointEventData> eventDataDic, Transform targetTransform, MoveLogic moveLogic, RotateLogic rotateLogic, ScaleLogic scaleLogic)
    {
        base.OneDevicePartInit(manipulationHandler, eventDataDic, targetTransform, moveLogic, rotateLogic, scaleLogic);

        this.eventData = this.eventDataDic.Values.ToArray()[0];
        pointerPose = new SCPose(Vector3.zero, Quaternion.identity);
        Vector3 grabPosition = Vector3.zero;
        pointerPose.position = this.eventData.Position3D;
        pointerPose.rotation = Quaternion.identity;
        grabPosition = this.eventData.Position3D;

        startRotation = targetTransform.rotation;
        localRotationInHead = Quaternion.Inverse(API_GSXR_Slam.SlamManager.head.transform.rotation) * startRotation;

        //Head only need to setup movelogic
        moveLogic.Setup(pointerPose, grabPosition, targetTransform, targetTransform.localScale);
    }

    public override Tuple<Vector3, Quaternion, Vector3> OneDevicePartUpdate()
    {
        pointerPose.position = this.eventData.Position3D;
        pointerPose.rotation = startRotation;//Quaternion.identity;

        Quaternion rotation = targetTransform.rotation;
        if (manipulationHandler.headRotate)
        {
            rotation = API_GSXR_Slam.SlamManager.head.transform.rotation * localRotationInHead;
        }

        Vector3 position = moveLogic.Update(pointerPose, rotation, targetTransform.localScale, true);

        return new Tuple<Vector3, Quaternion, Vector3>(position, rotation, targetTransform.localScale);
    }

    public override void TwoDevicePartInit(ManipulationHandler manipulationHandler, Dictionary<InputDevicePartType, SCPointEventData> eventDataDic, Transform targetTransform, MoveLogic moveLogic, RotateLogic rotateLogic, ScaleLogic scaleLogic)
    {
        base.TwoDevicePartInit(manipulationHandler, eventDataDic, targetTransform, moveLogic, rotateLogic, scaleLogic);
        //Do nothing
    }

    public override Tuple<Vector3, Quaternion, Vector3> TwoDevicePartUpdate(Func<Vector3, Vector3> scaleConstraint)
    {
        //Do nothing
        return new Tuple<Vector3, Quaternion, Vector3>(Vector3.zero, Quaternion.identity, Vector3.one);
    }
}
