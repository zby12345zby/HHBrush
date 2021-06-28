using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceGC.KS {
    public class ModelK101 : ModelGCBase {
        public InputDeviceKSPartUI inputDeviceKSPartUI {
            get {
                return inputDevicePartUIBase as InputDeviceKSPartUI;
            }
        }
        [Header("3DofBias")]
        public Vector3 modelPositionDeltaWithDevice = new Vector3(0, 0, 0f);

        [Header("Keys")]
        public MeshRenderer axkey;
        public MeshRenderer bykey;
        public MeshRenderer functionKey;

        [Header("Joystick")]
        public Transform joystick;
        public MeshRenderer joystickKey;
        [Range(-4,4)]
        public float rotationXfactor = 2;
        [Range(-4, 4)]
        public float rotationZfactor = 2;
        public Vector2 joystickInitalValue = new Vector2(8, 8);
        public Vector3 joystickInitallocalEulerAngles;


        [Header("HallForward")]
        public MeshRenderer hallFoward;
        public Transform HallForward;
        public int HallForwardReleaseValue = 10;
        public int HallForwardPressValue = 0;
        public Vector3 HallForwardReleaseLocalPosition;
        public Vector3 HallForwardPressLocalPosition;

        [Header("HallInside")]
        public MeshRenderer hallInside;
        public Transform HallInside;
        public int HallInsideReleaseValue = 10;
        public int HallInsidePressValue = 0;
        public Vector3 HallInsideReleaseLocalPosition;
        public Vector3 HallInsidePressLocalPosition;

        [Header("MaterialVisual")]
        public Material pressMaterial;
        public Material releaseMaterial;


        void UpdateTransform() {
            transform.localPosition = modelPositionDeltaWithDevice;
        }

        public override void OnSCStart() {
            base.OnSCStart();

        }

        public override void OnSCLateUpdate() {
            base.OnSCLateUpdate();
            UpdateTransform();
            UpdateJoystickTransform();
            UpdateHallVisual();
        }
        public virtual void UpdateHallVisual() {
            if(HallForward) {
                HallForward.localPosition = (HallForwardPressLocalPosition - HallForwardReleaseLocalPosition) / (HallForwardPressValue - HallForwardReleaseValue) * inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.HallFoward + HallForwardPressLocalPosition;
                if(inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.HallFoward < (HallForwardReleaseValue - HallForwardPressValue) / 2) {
                    hallFoward.material = pressMaterial;
                } else {
                    hallFoward.material = releaseMaterial;
                }
            }
            if(HallInside) {
                HallInside.localPosition = (HallInsidePressLocalPosition - HallInsideReleaseLocalPosition) / (HallInsidePressValue - HallInsideReleaseValue) * inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.HallInside + HallInsidePressLocalPosition;
                if(inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.HallInside < (HallInsideReleaseValue - HallInsidePressValue) / 2) {
                    hallInside.material = pressMaterial;
                } else {
                    hallInside.material = releaseMaterial;
                }
            }
        }

        //[Range(0, 16)]
        //public int xx = 8;
        //[Range(0, 16)]
        //public int yy = 8;
        Vector3 biasJoystick = new Vector3(0,0,0);
        public virtual void UpdateJoystickTransform() {

            if (joystick) {
                //inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.JoystickX = xx;
                //inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.JoystickY = yy;

                if (inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.JoystickX != joystickInitalValue.x || inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.JoystickY != joystickInitalValue.y) {
                    biasJoystick.z = Mathf.Clamp(rotationZfactor * (inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.JoystickX - joystickInitalValue.x), -30, 30);
                    biasJoystick.x = Mathf.Clamp(rotationXfactor * (inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.JoystickY - joystickInitalValue.y), -30, 30);
                    joystick.localEulerAngles = joystickInitallocalEulerAngles + biasJoystick;
                }
            }
        }
        

        public override void SetHandleKeysColor() {
            if (releaseMaterial == null || pressMaterial == null) {
                return;
            }

            foreach (var item in inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.inputKeys.inputKeyPressDic) {

                if (item.Key == InputKeyCode.Trigger) {
                    //triggerKey.material = releaseMaterial;
                    //if (item.Value == InputKeyState.DOWN || item.Value == InputKeyState.LONG) {
                    //    triggerKey.material = pressMaterial;
                    //}
                } else if (functionKey && (item.Key == InputKeyCode.RFunction || item.Key == InputKeyCode.LFunction)) {
                    if (item.Value == InputKeyState.DOWN || item.Value == InputKeyState.LONG) {
                        functionKey.material = pressMaterial;
                    } else if (item.Value == InputKeyState.UP) {
                        functionKey.material = releaseMaterial;
                    }
                } else if (axkey && (item.Key == InputKeyCode.X || item.Key == InputKeyCode.A)) {
                    if (item.Value == InputKeyState.DOWN || item.Value == InputKeyState.LONG) {
                        axkey.material = pressMaterial;
                    } else if (item.Value == InputKeyState.UP) {
                        axkey.material = releaseMaterial;
                    }
                } else if(bykey && (item.Key == InputKeyCode.Y || item.Key == InputKeyCode.B)) {
                    if(item.Value == InputKeyState.DOWN || item.Value == InputKeyState.LONG) {
                        bykey.material = pressMaterial;
                    } else if (item.Value == InputKeyState.UP) {
                        bykey.material = releaseMaterial;
                    }
                } else if(joystickKey &&( item.Key == InputKeyCode.LjoystickKey || item.Key == InputKeyCode.RjoystickKey)) {
                    if(item.Value == InputKeyState.DOWN || item.Value == InputKeyState.LONG) {
                        joystickKey.material = pressMaterial;
                    } else if (item.Value == InputKeyState.UP) {
                        joystickKey.material = releaseMaterial;
                    }
                }

            }
        }


    }
}
