using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static SC.XR.Unity.Module_InputSystem.InputDeviceHand.ModelHand;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceHand {
    public class InputDataGetHandPosture : InputDataGetOneBase {
        public InputDataGetHand inputDataGetHand;
        public InputDataGetHandPosture(InputDataGetHand _inputDataGetHand) : base(_inputDataGetHand) {
            inputDataGetHand = _inputDataGetHand;
        }

        protected ModelHand modelHand {
            get {
                return inputDataGetHand.inputDeviceHandPart.inputDeviceHandPartUI.modelHand;
            }
        }

        Vector3 v1, v2, v3;
        public Vector3 GranPointerOffset;
        public Vector3 TouchdetectCenterOffset;
        public override void OnSCStart() {
            base.OnSCStart();
            if (inputDataGetHand.inputDeviceHandPart.PartType == InputDevicePartType.HandLeft) {
                GranPointerOffset = new Vector3(0.05f, 0.06f, 0.06f);
                TouchdetectCenterOffset = new Vector3(0.00f, 0.1f, 0.1f);
            } else {
                GranPointerOffset = new Vector3(-0.05f, 0.06f, 0.06f);
                TouchdetectCenterOffset = new Vector3(-0.00f, 0.1f, 0.1f);
            }
        }

        public override void OnSCLateUpdate() {
            base.OnSCLateUpdate();
            OnUpdateHandTransform();
            OnUpdateHandNormal();
            OnUpdateHandRight();
            OnUpdateHandUp();
            OnUpdateHandCenter();
            OnUpdateHandPosture();
            OnUpdateHandKeyPosition();
        }

        Vector3 winPositionOffset = new Vector3(0,-0.03f,0.18f);
        protected virtual void OnUpdateHandTransform() {

            if (API_Module_Device.IsGSXRAndroidDevice) {
                inputDataGetHand.inputDeviceHandPart.inputDataHand.position = Vector3.zero;
                inputDataGetHand.inputDeviceHandPart.inputDataHand.rotation = Quaternion.identity;
            } else {
                if (API_GSXR_Slam.SlamManager != null && API_GSXR_Slam.SlamManager.gameObject.activeSelf) {
                    inputDataGetHand.inputDeviceHandPart.inputDataHand.position = API_GSXR_Slam.SlamManager.head.position + API_GSXR_Slam.SlamManager.head.rotation * (winPositionOffset);
                    inputDataGetHand.inputDeviceHandPart.inputDataHand.rotation = API_GSXR_Slam.SlamManager.head.rotation;
                }
            }

        }

        protected virtual void OnUpdateHandRight() {
            v1 = v2 = v3 = Vector3.zero;
            if (API_Module_Device.IsGSXRAndroidDevice) {
                v1 = inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.GetJoint(FINGER.forefinger, JOINT.Four).localPosition;
                v2 = inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.GetJoint(FINGER.ring, JOINT.Four).localPosition;
            } else {
                if (modelHand.ActiveHandModel != null) {
                    v1 = modelHand.ActiveHandModel.GetJointTransform(FINGER.forefinger, JOINT.Four).transform.position;
                    v2 = modelHand.ActiveHandModel.GetJointTransform(FINGER.ring, JOINT.Four).transform.position;
                }
            }
            inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.right = (v2 - v1).normalized;

        }
        protected virtual void OnUpdateHandUp() {
            if (inputDataGetHand.inputDeviceHandPart.PartType == InputDevicePartType.HandLeft) {
                inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.up =
                    Vector3.Cross(inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.right, inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.normal).normalized;
            } else if (inputDataGetHand.inputDeviceHandPart.PartType == InputDevicePartType.HandRight) {
                inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.up =
                    Vector3.Cross(inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.normal, inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.right).normalized;
            }
        }
        protected virtual void OnUpdateHandNormal() {
            v1 = v2 = v3 = Vector3.zero;

            if (API_Module_Device.IsGSXRAndroidDevice) {
                v1 = inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.GetJoint(FINGER.ring, JOINT.Four).localPosition - inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.GetJoint(FINGER.forefinger, JOINT.Four).localPosition;
                v2 = inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.GetJoint(FINGER.small, JOINT.Five).localPosition - inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.GetJoint(FINGER.forefinger, JOINT.Four).localPosition;
            } else {
                if (modelHand.ActiveHandModel != null) {
                    v1 = modelHand.ActiveHandModel.GetJointTransform(FINGER.ring, JOINT.Four).position - modelHand.ActiveHandModel.GetJointTransform(FINGER.forefinger, JOINT.Four).transform.position;
                    v2 = modelHand.ActiveHandModel.GetJointTransform(FINGER.small, JOINT.Five).position - modelHand.ActiveHandModel.GetJointTransform(FINGER.forefinger, JOINT.Four).position;
                }
            }

            if (inputDataGetHand.inputDeviceHandPart.PartType == InputDevicePartType.HandLeft) {
                inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.normal = Vector3.Cross(v1, v2).normalized;
            } else if (inputDataGetHand.inputDeviceHandPart.PartType == InputDevicePartType.HandRight) {
                inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.normal = Vector3.Cross(v2, v1).normalized;
            }

        }
        protected virtual void OnUpdateHandCenter() {
            v1 = v2 = v3 = Vector3.zero;
            if (API_Module_Device.IsGSXRAndroidDevice) {
                v1 = inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.GetJoint(FINGER.forefinger, JOINT.Four).localPosition;
                v2 = inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.GetJoint(FINGER.ring, JOINT.Four).localPosition;
                v3 = inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.GetJoint(FINGER.small, JOINT.Five).localPosition;
            } else {
                if (modelHand.ActiveHandModel != null) {
                    v1 = modelHand.ActiveHandModel.GetJointTransform(FINGER.forefinger, JOINT.Four).position;
                    v2 = modelHand.ActiveHandModel.GetJointTransform(FINGER.ring, JOINT.Four).position;
                    v3 = modelHand.ActiveHandModel.GetJointTransform(FINGER.small, JOINT.Five).position;
                }
            }
            inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.centerPosition = (v1 + v2 + v3) / 3f;
            inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.centerRotation = Quaternion.LookRotation(inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.normal, inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.up);

        }
        protected virtual void OnUpdateHandPosture() {
            inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.position = inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.centerPosition;
            inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.rotation = inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.centerRotation;
        }

        protected virtual void OnUpdateHandKeyPosition() {

            inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.MainGrabPosition = inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.centerPosition +
                inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.centerRotation * GranPointerOffset;
            inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.MainGrabRotation = inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.centerRotation;


            inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.MainTouchDetectCenterPosition = inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.centerPosition +
                inputDataGetHand.inputDeviceHandPart.inputDataHand.handInfo.centerRotation * TouchdetectCenterOffset;
        }

    }
}
