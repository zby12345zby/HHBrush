using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceHand {

    public class FarPointer : PointerBase {
        public HandDetector handDetector {
            get {
                return detectorBase as HandDetector;
            }
        }

        HandCursor _handCursor;
        public HandCursor handCursor {
            get {
                if (_handCursor == null) {
                    _handCursor = GetComponentInChildren<HandCursor>(true);
                }
                return _handCursor;
            }
        }

        public override PointerType PointerType { get => PointerType.Far; }


        public Action<bool> TargetDetectModelChange;

        private Vector3 RotationOffset;
        private float slowness = 0;

        public bool ShowHandCursor = false;
        public bool ShowHandLine = true;
        public bool OutCameraShowHandLine = false;

        private Vector3 LeftOffset = new Vector3(0.035f,-0.02f,0.06f);
        private Vector3 RightOffset = new Vector3(-0.035f, -0.01f, 0.08f);
        public override void OnSCAwake() {
            base.OnSCAwake();
            AddModule(handCursor);

            RotationOffset = Vector3.zero;
            if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "FarPointerRotationOffsetX")) {
                RotationOffset.x = API_Module_SDKConfiguration.GetFloat("Module_InputSystem", "FarPointerRotationOffsetX", 0);
            }
            if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "FarPointerRotationOffsetY")) {
                RotationOffset.y = API_Module_SDKConfiguration.GetFloat("Module_InputSystem", "FarPointerRotationOffsetY", 0);
            }
            if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "FarPointerRotationOffsetZ")) {
                RotationOffset.z = API_Module_SDKConfiguration.GetFloat("Module_InputSystem", "FarPointerRotationOffsetZ", 0);
            }
            DebugMy.Log("FarPointerRotationOffset:" + RotationOffset, this, true);

            if (API_Module_Device.IsGSXRAndroidDevice == false) {
                RotationOffset.x += 10;
            } else {
                //if (handDetector.inputDeviceHandPart.PartType == InputDevicePartType.HandLeft) {
                //    RotationOffset.x = -20;
                //} else if (handDetector.inputDeviceHandPart.PartType == InputDevicePartType.HandRight) {
                //    RotationOffset.x = -10;
                //}
            }
        }

        public override void OnSCStart() {
            base.OnSCStart();
            
            //lineBase?.ModuleStop();

            InputDeviceHandPartEventBase.eventDelegate += CatchEvent;
            //if (handDetector.inputDeviceHandPart.PartType == InputDevicePartType.HandRight) {
            //    //lineBase?.ModuleStop();
            //}
            if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "ShowHandLine")) {
                ShowHandLine = API_Module_SDKConfiguration.GetBool("Module_InputSystem", "ShowHandLine", 0);
                DebugMy.Log("ShowHandLine:" + ShowHandLine, this, true);
            }
            if (ShowHandLine==false) {
                lineBase?.ModuleStop();
            }

            if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "ShowHandCursor")) {
                ShowHandCursor = API_Module_SDKConfiguration.GetBool("Module_InputSystem", "ShowHandCursor", 0);
                DebugMy.Log("ShowHandCursor:" + ShowHandCursor, this, true);
            }

            if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "OutCameraShowHandLine")) {
                OutCameraShowHandLine = API_Module_SDKConfiguration.GetBool("Module_InputSystem", "OutCameraShowHandLine", 0);
                DebugMy.Log("OutCameraShowHandLine:" + OutCameraShowHandLine, this, true);
            }

            if (ShowHandCursor) {
                if (handCursor != null && !handCursor.IsModuleStarted)
                    handCursor.ModuleStart();
            }
        }

        Vector3 up, forward = Vector3.forward;
        protected override void UpdateTransform() {

            //if (HandRayOffset) {
            //    transform.position = handDetector.inputDeviceHandPart.inputDataHand.handInfo.MainGrabPosition;
            //} else {
            //    transform.position = handDetector.inputDeviceHandPart.inputDeviceHandPartUI.modelHand.ActiveHandModel.GetJointTransform(FINGER.forefinger, JOINT.Four).position;
            //}
            transform.position = handDetector.inputDeviceHandPart.inputDeviceHandPartUI.modelHand.ActiveHandModel.GetJointTransform(FINGER.forefinger, JOINT.Four).position;

            Quaternion temprotation = Quaternion.identity;
            if (handDetector.inputDeviceHandPart.PartType == InputDevicePartType.HandLeft) {
                temprotation = Quaternion.LookRotation(transform.position - API_GSXR_Slam.SlamManager.shoulder.Left.position, API_GSXR_Slam.SlamManager.shoulder.Left.up);
            } else if (handDetector.inputDeviceHandPart.PartType == InputDevicePartType.HandRight) {
                temprotation = Quaternion.LookRotation(transform.position - API_GSXR_Slam.SlamManager.shoulder.Right.position, API_GSXR_Slam.SlamManager.shoulder.Right.up);
            }

            //if (handDetector.inputDevicePartBase.inputDataBase.SCPointEventData.pointerCurrentRaycast.gameObject != null && slowness > 0) {
            //    forward = Vector3.Lerp(forward, handDetector.inputDevicePartBase.inputDataBase.SCPointEventData.pointerCurrentRaycast.worldPosition - transform.position,0.5f);
            //} else {
            //if (handDetector.inputDeviceHandPart.PartType == InputDevicePartType.HandLeft) {
            //    forward = transform.position - API_GSXR_Slam.SlamManager.shoulder.Left.position;
            //    up = Vector3.Cross(handDetector.inputDeviceHandPart.inputDataHand.handInfo.right, forward);
            //} else if (handDetector.inputDeviceHandPart.PartType == InputDevicePartType.HandRight) {
            //    forward = transform.position - API_GSXR_Slam.SlamManager.shoulder.Right.position;
            //    up = Vector3.Cross(forward, handDetector.inputDeviceHandPart.inputDataHand.handInfo.right);
            //}
            //}

            //if (handDetector.inputDeviceHandPart.PartType == InputDevicePartType.HandLeft) {
            //    forward = transform.position - API_GSXR_Slam.SlamManager.shoulder.Left.position;
            //    up = Vector3.Cross(handDetector.inputDeviceHandPart.inputDataHand.handInfo.right, forward);
            //} else if (handDetector.inputDeviceHandPart.PartType == InputDevicePartType.HandRight) {
            //    forward = transform.position - API_GSXR_Slam.SlamManager.shoulder.Right.position;
            //    up = Vector3.Cross(forward, handDetector.inputDeviceHandPart.inputDataHand.handInfo.right);
            //}
            //temprotation = Quaternion.LookRotation(forward, up);

            transform.rotation = temprotation;
            transform.Rotate(RotationOffset);

            if (handDetector.inputDeviceHandPart.PartType == InputDevicePartType.HandLeft) {
                transform.position = transform.position + transform.rotation * LeftOffset;
            } else if (handDetector.inputDeviceHandPart.PartType == InputDevicePartType.HandRight) {
                transform.position = transform.position + transform.rotation * RightOffset;
            }
        }

        protected override void DoTargetDetect() {
            SCInputModule.Instance.ProcessCS(handDetector.inputDevicePartBase.inputDataBase.SCPointEventData, transform, LayerMask, MaxDetectDistance);
            IsFocusLocked = handDetector.inputDevicePartBase.inputDataBase.SCPointEventData.DownPressGameObject != null;

            if (handDetector.inputDevicePartBase.inputDataBase.SCPointEventData.DownPressGameObject || handDetector.inputDevicePartBase.inputDataBase.SCPointEventData.pointerCurrentRaycast.gameObject != null) {
                if (cursorBase && !cursorBase.IsModuleStarted) {
                    cursorBase?.ModuleStart();
                }
            } else {
                if (cursorBase && cursorBase.IsModuleStarted) {
                    cursorBase?.ModuleStop();
                }
            }
        }

        public override void OnSCDisable() {
            base.OnSCDisable();
            IsFocusLocked = false;
            InputDeviceHandPartEventBase.eventDelegate -= CatchEvent;
        }
        protected override void UpdateSubVisual() {
            //base.UpdateSubVisual();

            if (OutCameraShowHandLine && lineBase) {
                if (API_GSXR_Slam.SlamManager != null && API_GSXR_Slam.SlamManager.IsRunning) {
                    if (!IsInCamera(API_GSXR_Slam.SlamManager.leftCamera, cursorBase.transform.position) &&
                        !IsInCamera(API_GSXR_Slam.SlamManager.rightCamera, cursorBase.transform.position)) {
                        if (lineBase != null && !lineBase.IsModuleStarted) {
                            lineBase.ModuleStart();
                        }
                    } else {
                        if (lineBase != null && lineBase.IsModuleStarted) {
                            lineBase.ModuleStop();
                        }
                    }
                }
            }
        }

        Vector3 viewPos;
        bool IsInCamera(Camera camera,Vector3 worldPosition) {
            viewPos = camera.WorldToViewportPoint(worldPosition);
            if (viewPos.z < 0) 
                return false;
            if (viewPos.z > camera.farClipPlane)
                return false;
            if (viewPos.x < 0 || viewPos.y < 0 || viewPos.x > 1 || viewPos.y > 1) 
                return false;
            return true;
        }

        void CatchEvent(InputDeviceHandPart inputDeviceHandPart, HandEventType eventType, float eventPercent) {
            //if (handDetector && handDetector.inputDeviceHandPart == inputDeviceHandPart) {

            //    if (eventType == HandEventType.CatchReady) {
            //        slowness = 0f;
            //        if (eventPercent > 0.2f) {
            //            slowness = 1f;
            //        }
            //    } else if (eventType == HandEventType.CatchDrag || eventType == HandEventType.CatchDown || eventType == HandEventType.CatchUp ) {
            //        slowness = 0f;
            //    }

            //}

        }
    }
}
