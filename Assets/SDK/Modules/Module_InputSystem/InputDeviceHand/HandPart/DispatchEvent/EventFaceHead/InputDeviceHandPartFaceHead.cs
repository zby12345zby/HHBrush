using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceHand {

    public class InputDeviceHandPartFaceHead : InputDeviceHandPartEventBase {

        public float faceAngle = 60;

        public InputDeviceHandPartFaceHead(InputDevicePartDispatchEventHand inputDevicePartDispatchEventHand) : base(inputDevicePartDispatchEventHand) {
        }

        public override void OnSCStart() {
            base.OnSCStart();
            previousEvent = currentEvent = HandEventType.Null;
            EventPercent = 0;
        }


        protected override void OnUpdateEvent() {
            currentEvent = HandEventType.Null;
            if (API_GSXR_Slam.SlamManager && API_GSXR_Slam.SlamManager.head) {
                if ((Vector3.Angle(handInfo.normal, API_GSXR_Slam.SlamManager.head.up) <= 60 && Vector3.Angle(handInfo.normal, API_GSXR_Slam.SlamManager.head.right) >70 && Vector3.Angle(handInfo.normal, API_GSXR_Slam.SlamManager.head.right) <110)
                    ||
                    Vector3.Angle(handInfo.normal, API_GSXR_Slam.SlamManager.head.position - inputDeviceHandPart.inputDataHand.handInfo.centerPosition) < 50
                    ) {
                    if (previousEvent == HandEventType.PalmFaceHead || previousEvent == HandEventType.PalmFaceHeadStart) {
                        currentEvent = HandEventType.PalmFaceHead;
                    } else {
                        currentEvent = HandEventType.PalmFaceHeadStart;
                    }
                    EventPercent = 1;

                } else {
                    if (previousEvent == HandEventType.PalmFaceHead || previousEvent == HandEventType.PalmFaceHeadStart) {
                        currentEvent = HandEventType.PalmFaceHeadEnd;
                    }else {
                        currentEvent = HandEventType.Null;
                    }
                    EventPercent = 0;
                }
            }
            previousEvent = currentEvent;


            //if (currentEvent != HandEventType.Null) {
            //    DebugMy.Log(inputDeviceHandPart.PartType + "   Event -----> " + currentEvent + " " + EventPercent, this, true);
            //}
        }
        public override void OnSCDisable() {
            if (previousEvent == HandEventType.PalmFaceHead || previousEvent == HandEventType.PalmFaceHeadStart) {
                currentEvent = HandEventType.PalmFaceHeadEnd;
            }
            EventPercent = 0;
            base.OnSCDisable();
        }

    }
}
