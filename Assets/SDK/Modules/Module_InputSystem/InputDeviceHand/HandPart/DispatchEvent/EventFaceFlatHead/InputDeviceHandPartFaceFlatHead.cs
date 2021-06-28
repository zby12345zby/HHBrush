using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceHand {

    public class InputDeviceHandPartFaceFlatHead : InputDeviceHandPartEventBase {

        public float faceAngle = 60;

        public InputDeviceHandPartFaceFlatHead(InputDevicePartDispatchEventHand inputDevicePartDispatchEventHand) : base(inputDevicePartDispatchEventHand) {
        }

        public override void OnSCStart() {
            base.OnSCStart();
            previousEvent = currentEvent = HandEventType.Null;
            EventPercent = 0;
        }
        public float forefingerAngle = 0;
        public float middleAngle = 0;
        public float ringAngle = 0;

        protected override void OnUpdateEvent() {
            currentEvent = HandEventType.Null;


            if (API_GSXR_Slam.SlamManager && API_GSXR_Slam.SlamManager.head) {
                if (Vector3.Angle(handInfo.normal, API_GSXR_Slam.SlamManager.head.up) <= 60
                    ||
                    Vector3.Angle(handInfo.normal, API_GSXR_Slam.SlamManager.head.position - inputDeviceHandPart.inputDataHand.handInfo.centerPosition) < 60
                    ) {

                    forefingerAngle = Vector3.Angle((handInfo.GetJoint(FINGER.forefinger, JOINT.Two).localPosition - handInfo.GetJoint(FINGER.forefinger, JOINT.Three).localPosition),
                        (handInfo.GetJoint(FINGER.forefinger, JOINT.Three).localPosition - handInfo.GetJoint(FINGER.forefinger, JOINT.Four).localPosition));

                    middleAngle = Vector3.Angle((handInfo.GetJoint(FINGER.middle, JOINT.Two).localPosition - handInfo.GetJoint(FINGER.middle, JOINT.Three).localPosition),
                        (handInfo.GetJoint(FINGER.middle, JOINT.Three).localPosition - handInfo.GetJoint(FINGER.middle, JOINT.Four).localPosition));

                    ringAngle = Vector3.Angle((handInfo.GetJoint(FINGER.ring, JOINT.Two).localPosition - handInfo.GetJoint(FINGER.ring, JOINT.Three).localPosition),
                        (handInfo.GetJoint(FINGER.ring, JOINT.Three).localPosition - handInfo.GetJoint(FINGER.ring, JOINT.Four).localPosition));
                    //DebugMy.Log(inputDeviceHandPart.PartType + "Event Angle  forefingerAngle:" + forefingerAngle + " " + middleAngle + " " + ringAngle, this, true);
                    if (forefingerAngle < 25 && middleAngle < 25 && ringAngle < 25) {

                        if (previousEvent == HandEventType.PalmFlatFaceHeadFlat || 
                            previousEvent == HandEventType.PalmFlatFaceHeadCatch  || 
                            previousEvent == HandEventType.PalmFlatFaceHeadStart ||
                            previousEvent == HandEventType.PalmFlatFaceHeadMiddelFlatAndCatch) {

                            currentEvent = HandEventType.PalmFlatFaceHeadFlat;
                        } else {
                            currentEvent = HandEventType.PalmFlatFaceHeadStart;
                        }
                    } else if (forefingerAngle > 60 && middleAngle > 60 && ringAngle > 60) {
                        if (previousEvent == HandEventType.PalmFlatFaceHeadFlat ||
                            previousEvent == HandEventType.PalmFlatFaceHeadCatch ||
                            previousEvent == HandEventType.PalmFlatFaceHeadStart ||
                            previousEvent == HandEventType.PalmFlatFaceHeadMiddelFlatAndCatch) {

                            currentEvent = HandEventType.PalmFlatFaceHeadCatch;
                        }
                    } else {
                        if (previousEvent == HandEventType.PalmFlatFaceHeadFlat ||
                            previousEvent == HandEventType.PalmFlatFaceHeadCatch ||
                            previousEvent == HandEventType.PalmFlatFaceHeadStart ||
                            previousEvent == HandEventType.PalmFlatFaceHeadMiddelFlatAndCatch) {

                            currentEvent = HandEventType.PalmFlatFaceHeadMiddelFlatAndCatch;
                        }
                    }

                } else {
                    if (previousEvent == HandEventType.PalmFlatFaceHeadFlat ||
                            previousEvent == HandEventType.PalmFlatFaceHeadCatch ||
                            previousEvent == HandEventType.PalmFlatFaceHeadStart ||
                            previousEvent == HandEventType.PalmFlatFaceHeadMiddelFlatAndCatch) {

                        currentEvent = HandEventType.PalmFlatFaceHeadEnd;
                    }else {
                        currentEvent = HandEventType.Null;
                    }
                }
            }
            previousEvent = currentEvent;

            //if (currentEvent != HandEventType.Null) {
            //    DebugMy.Log(inputDeviceHandPart.PartType + "   Event -----> " + currentEvent + " " + EventPercent, this, true);
            //}
        }
        public override void OnSCDisable() {
            if (previousEvent == HandEventType.PalmFlatFaceHeadFlat ||
                            previousEvent == HandEventType.PalmFlatFaceHeadCatch ||
                            previousEvent == HandEventType.PalmFlatFaceHeadStart ||
                            previousEvent == HandEventType.PalmFlatFaceHeadMiddelFlatAndCatch
                            ) {
                currentEvent = HandEventType.PalmFlatFaceHeadEnd;
            }
            EventPercent = 0;
            base.OnSCDisable();
        }

    }
}
