using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceGC.KS {
    public class ModelK11 : ModelGCBase {
        public InputDeviceKSPartUI inputDeviceKSPartUI {
            get {
                return inputDevicePartUIBase as InputDeviceKSPartUI;
            }
        }

        [Header("3DofBias")]
        public Vector3 modelPositionDeltaWithDevice = new Vector3(0, 0, 0f);

        [Header("Keys")]
        public MeshRenderer Upkey;
        public MeshRenderer Downkey;
        public MeshRenderer LeftKey;
        public MeshRenderer RightKey;
        public MeshRenderer TriggerKey;

        [Header("Joystick")]
        public Transform joystick;
        private Vector3 joystickInitLocalPosition;

        [Range(0,4)]
        public float rotationfactor = 2;
        public Vector2 joystickInitalValue = new Vector2(8, 8);
        public Vector3 joystickInitallocalEulerAngles;


        [Header("MaterialVisual")]
        public Material pressMaterial;
        public Material releaseMaterial;


        void UpdateTransform() {
            transform.localPosition = modelPositionDeltaWithDevice;
        }

        public override void OnSCStart() {
            base.OnSCStart();
            if (joystick)
                joystickInitLocalPosition = joystick.localPosition;
        }

        public override void OnSCLateUpdate() {
            base.OnSCLateUpdate();
            UpdateTransform();
            UpdateJoystickTransform();
        }

        ////[Range(0, 16)]
        //public int xx = 8;
        ////[Range(0, 16)]
        //public int yy = 8;
        Vector3 biasJoystick = new Vector3(0,0,0);
        public virtual void UpdateJoystickTransform() {

            if (joystick) {

                if(inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.JoystickX != joystickInitalValue.x || inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.JoystickY != joystickInitalValue.y) {
                    if (inputDeviceKSPartUI.inputDeviceKSPart.PartType == InputDevicePartType.KSLeft) {
                        biasJoystick.z = joystickInitLocalPosition.z + (inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.JoystickY - joystickInitalValue.y) * 0.0002f;
                        biasJoystick.x = joystickInitLocalPosition.x + (inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.JoystickX - joystickInitalValue.x) * 0.0002f;
                        biasJoystick.y = joystickInitLocalPosition.y;
                    } else if (inputDeviceKSPartUI.inputDeviceKSPart.PartType == InputDevicePartType.KSRight) {
                        biasJoystick.z = joystickInitLocalPosition.z + (joystickInitalValue.y - inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.JoystickY) * 0.0002f;
                        biasJoystick.x = joystickInitLocalPosition.x + (joystickInitalValue.x - inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.JoystickX) * 0.0002f;
                        biasJoystick.y = joystickInitLocalPosition.y;
                    }
                    joystick.localPosition = biasJoystick;
                }
            }
        }

        public override void SetHandleKeysColor() {
            if (releaseMaterial == null || pressMaterial == null) {
                return;
            }

            foreach (var item in inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.inputKeys.inputKeyPressDic) {

                if(TriggerKey && (item.Key == InputKeyCode.LTrigger || item.Key == InputKeyCode.RTrigger)) {
                    if (item.Value == InputKeyState.DOWN || item.Value == InputKeyState.LONG) {
                        TriggerKey.material = pressMaterial;
                    } else if (item.Value == InputKeyState.UP) {
                        TriggerKey.material = releaseMaterial;
                    }
                } else if (Upkey && (item.Key == InputKeyCode.UP || item.Key == InputKeyCode.Y)) {
                    if (item.Value == InputKeyState.DOWN || item.Value == InputKeyState.LONG) {
                        Upkey.material = pressMaterial;
                    } else if (item.Value == InputKeyState.UP) {
                        Upkey.material = releaseMaterial;
                    }
                } else if(RightKey && (item.Key == InputKeyCode.RIGHT || item.Key == InputKeyCode.B)) {
                    if (item.Value == InputKeyState.DOWN || item.Value == InputKeyState.LONG) {
                        RightKey.material = pressMaterial;
                    } else if (item.Value == InputKeyState.UP) {
                        RightKey.material = releaseMaterial;
                    }
                } else if (Downkey && (item.Key == InputKeyCode.DOWN || item.Key == InputKeyCode.A)) {
                    if (item.Value == InputKeyState.DOWN || item.Value == InputKeyState.LONG) {
                        Downkey.material = pressMaterial;
                    } else if (item.Value == InputKeyState.UP) {
                        Downkey.material = releaseMaterial;
                    }
                } else if(LeftKey && (item.Key == InputKeyCode.LEFT || item.Key == InputKeyCode.X)) {
                    if (item.Value == InputKeyState.DOWN || item.Value == InputKeyState.LONG) {
                        LeftKey.material = pressMaterial;
                    } else if (item.Value == InputKeyState.UP) {
                        LeftKey.material = releaseMaterial;
                    }
                }

            }
        }


    }
}
