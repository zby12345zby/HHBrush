using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceGC.KS {
    public class InputDataGetKSPosture : InputDataGetGCPosture {
        public InputDataGetKS inputDataGetKS;

        public InputDataGetKSPosture(InputDataGetKS _inputDataGetKS) : base(_inputDataGetKS) {
            inputDataGetKS = _inputDataGetKS;
        }

        public Vector3 rightPositionDeltaWithHead = new Vector3(0.15f, -0.25f, 0.25f);
        public Vector3 leftPositionDeltaWithHead = new Vector3(-0.15f, -0.25f, 0.25f);

        private static readonly Matrix4x4 FLIP_Z = Matrix4x4.Scale(new Vector3(1, 1, -1));
        private Matrix4x4 mPoseMatrix1;
        float[] array = new float[16];

        public float PositionLerp = 1f;
        public float RotationLerp = 1f;


        Quaternion rotation;
        Vector3 position;
        Vector3 deltaEulerAngles;
        
        public override void OnSCStart() {
            base.OnSCStart();
            Update3DofDeltaEulerAngles();
        }

        public override void OnSCLateUpdate() {
            base.OnSCLateUpdate();
            UpdateTransform();
        }

        void UpdateTransform() {

            if(API_GSXR_Slam.SlamManager == null || API_GSXR_Slam.SlamManager.IsRunning == false) {
                return;
            }

            //DebugMy.Log("UpdateTransform:" + inputDataGetKS.inputDeviceKSPart.PartType+" "+Time.frameCount, this, true);

            if(Application.platform != RuntimePlatform.Android) {
                if(API_GSXR_Slam.SlamManager != null && API_GSXR_Slam.SlamManager.gameObject.activeSelf) {
                    //inputDataGetKS.inputDeviceKSPart.inputDataKS.rotation = API_GSXR_Slam.SlamManager.head.transform.rotation;
                    inputDataGetKS.inputDeviceKSPart.inputDataKS.rotation = Quaternion.Euler(API_GSXR_Slam.SlamManager.head.transform.eulerAngles + new Vector3(-10,0,0));
                }

                if(inputDataGetKS.inputDeviceKSPart.PartType == InputDevicePartType.KSLeft) {
                    if(API_GSXR_Slam.SlamManager != null && API_GSXR_Slam.SlamManager.gameObject.activeSelf) {
                        inputDataGetKS.inputDeviceKSPart.inputDataKS.position = API_GSXR_Slam.SlamManager.head.position + API_GSXR_Slam.SlamManager.head.rotation * leftPositionDeltaWithHead;
                    }
                } else if(inputDataGetKS.inputDeviceKSPart.PartType == InputDevicePartType.KSRight) {
                    if(API_GSXR_Slam.SlamManager != null && API_GSXR_Slam.SlamManager.gameObject.activeSelf) {
                        inputDataGetKS.inputDeviceKSPart.inputDataKS.position = API_GSXR_Slam.SlamManager.head.position + API_GSXR_Slam.SlamManager.head.rotation * rightPositionDeltaWithHead;
                    }
                } else {
                    inputDataGetKS.inputDeviceKSPart.inputDataKS.position = Vector3.zero;
                }

                return;
            }

            int result = 0;

            if (inputDataGetKS.inputDeviceKSPart.PartType == InputDevicePartType.KSLeft) {
                result = API_GSXR_Slam.SlamManager.plugin.GSXR_Get_ControllerPosture(array, 0);
            } else if (inputDataGetKS.inputDeviceKSPart.PartType == InputDevicePartType.KSRight) {
                result = API_GSXR_Slam.SlamManager.plugin.GSXR_Get_ControllerPosture(array, 1);
            }

            if (result != 0 || array == null) {
                DebugMy.Log(inputDataGetKS.inputDeviceKSPart.PartType + " GSXR_Get_ControllerPosture: Error <result != 0 || array == null> ", this, true);
                return;
            }

            rotation = new Quaternion(array[3], array[4], -array[5], array[6]);
            position = new Vector3(-array[0], -array[1], array[2]);

            if (inputDataGetKS.inputDeviceKSPart.inputDataKS.GCType == GCType.Nolo) {
                position = API_GSXR_Slam.SlamManager.head.TransformPoint(position);
                rotation = API_GSXR_Slam.SlamManager.head.rotation * rotation;
            }

            inputDataGetKS.inputDeviceKSPart.inputDataKS.rotation = Quaternion.Lerp(inputDataGetKS.inputDeviceKSPart.inputDataKS.rotation, rotation, RotationLerp);
            inputDataGetKS.inputDeviceKSPart.inputDataKS.position = Vector3.Lerp(inputDataGetKS.inputDeviceKSPart.inputDataKS.position, position, PositionLerp);


            if (inputDataGetKS.inputDeviceKSPart.inputDataKS.PostureType == PostureType._3Dof) {
                EffectByCalibrationKey();
                rotation = Quaternion.Euler(deltaEulerAngles + rotation.eulerAngles);
                inputDataGetKS.inputDeviceKSPart.inputDataKS.rotation = Quaternion.Lerp(inputDataGetKS.inputDeviceKSPart.inputDataKS.rotation, rotation, RotationLerp);
            }


        }

        void EffectByCalibrationKey() {
            InputKeyState inputKeyState;
            
            inputDataGetKS.inputDeviceKSPart.inputDataKS.inputKeys.inputKeyPressDic.TryGetValue(inputDataGetKS.inputDeviceKSPart.inputDataKS.CalibrationKeyAlias, out inputKeyState);

            if(inputKeyState == InputKeyState.DOWN || inputKeyState == InputKeyState.LONG) {
                DebugMy.Log(inputDataGetKS.inputDeviceKSPart.inputDataKS.CalibrationKeyAlias + " DOWN or LONG Key Reset", this,true);
                Update3DofDeltaEulerAngles();
            }
            //DebugMy.Log("OnUpdateRotation: " + inputDeviceHandShankPart.inputDataHandShank.rotation.eulerAngles, this);
        }

        void Update3DofDeltaEulerAngles() {
            if(API_GSXR_Slam.SlamManager && API_GSXR_Slam.SlamManager.gameObject.activeSelf) {
                deltaEulerAngles = API_GSXR_Slam.SlamManager.head.transform.eulerAngles - rotation.eulerAngles;
                deltaEulerAngles.x = 0;
            }
        }

        protected override void UpdatePosition() { 
        }

        protected override void UpdateRotation() { 
        }
    }
}
