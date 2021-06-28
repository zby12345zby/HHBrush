using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceHand {

    public class InputDeviceHandPartCatchEvent : InputDeviceHandPartEventBase {

        public InputDeviceHandPartCatchEvent(InputDevicePartDispatchEventHand inputDevicePartDispatchEventHand) : base(inputDevicePartDispatchEventHand) {
        }

        enum XDirection {
            Release = -1,
            Catch = 1,
        }


        List<Vector3> HandTrendList = new List<Vector3>(new Vector3[6]);

        protected int currentNum = 0;
        protected float timer = 0;
        private float catchDeltaTime = 0.07f;

        bool isForefingerCatch = false;
        bool isDistanceNear = false;

        float angle;
        float distance = 0;
        bool isDown = false;

        public override void OnSCStart() {
            base.OnSCStart();
            previousEvent = currentEvent = HandEventType.Null; 
            isDown = false;
            timer = 0; 
            isDistanceNear = false;
            isDistanceNear = false;
        }
        protected override void OnUpdateEvent() {


            angle = Vector3.Angle(
                (handInfo.GetJoint(FINGER.forefinger, JOINT.Two).localPosition - handInfo.GetJoint(FINGER.forefinger, JOINT.Three).localPosition),
                (handInfo.GetJoint(FINGER.forefinger, JOINT.Three).localPosition - handInfo.GetJoint(FINGER.forefinger, JOINT.Four).localPosition));
            if(angle > 28) {
                isForefingerCatch = true;
            } else if(angle < 20) {
                isForefingerCatch = false;
            }
            //DebugMy.Log(inputDevice26Dof.PartType+"   "+FINGER.forefinger + "  Angle :" + angle + ">28 ?", this);

            distance = Vector3.Distance(handInfo.GetJoint(FINGER.forefinger, JOINT.One).localPosition, handInfo.GetJoint(FINGER.thumb, JOINT.One).localPosition);
            if(distance < 0.035f) {
                isDistanceNear = true;
            } else if(distance < 0.050f) {
                isDistanceNear = false;
            }

            if(isDistanceNear == false) {
                distance = Vector3.Distance(handInfo.GetJoint(FINGER.forefinger, JOINT.Two).localPosition, handInfo.GetJoint(FINGER.thumb, JOINT.Two).localPosition);
                if(distance < 0.035f) {
                    isDistanceNear = true;
                }
            }

            if (isDistanceNear && isForefingerCatch) {
                isDown = true;
            } else {
                isDown = false;
                timer = 0;
            }
            if (isDown == true) {
                timer += Time.deltaTime;
                //DebugMy.Log(inputDeviceHandPart.PartType + "   timer: " + timer, this, true);
            }

            EventPercent = (Mathf.Clamp(1 - distance / 0.035f / 2, 0, 0.5f) + Mathf.Clamp(angle / 28 / 2, 0, 0.5f)) * Mathf.Clamp(timer / catchDeltaTime, 0.2f, 1);

            if (EventPercent >= 1) {
                if (previousEvent == HandEventType.CatchDown || previousEvent == HandEventType.CatchDrag) {
                    currentEvent = HandEventType.CatchDrag;
                } else {
                    currentEvent = HandEventType.CatchDown;
                }
            } else {
                if (previousEvent == HandEventType.CatchDown || previousEvent == HandEventType.CatchDrag) {
                    currentEvent = HandEventType.CatchUp;
                } else if (EventPercent > 0.2f) {
                    currentEvent = HandEventType.CatchReady;
                } else {
                    if (currentEvent == HandEventType.CatchReady || currentEvent == HandEventType.CatchUp) {
                        currentEvent = HandEventType.CatchEnd;
                        EventPercent = 0;
                    } else {
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
            if (previousEvent == HandEventType.CatchDown || previousEvent == HandEventType.CatchDrag) {
                currentEvent = HandEventType.CatchUp;
            }
            base.OnSCDisable();
        }

    }
}
