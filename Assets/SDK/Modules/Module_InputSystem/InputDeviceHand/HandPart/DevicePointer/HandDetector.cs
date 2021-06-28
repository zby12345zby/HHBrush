using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceHand {
    public class HandDetector : DetectorBase {

        public InputDeviceHandPart inputDeviceHandPart {
            get {
                return inputDevicePartBase as InputDeviceHandPart;
            }
        }

        public FarPointer farPointer {
            get {
                return Transition<FarPointer>(pointerBase);
            }
        }

        [SerializeField]
        TouchPointer mTouchPointer;
        public TouchPointer touchPointer {
            get {
                if(mTouchPointer == null) {
                    mTouchPointer = GetComponentInChildren<TouchPointer>(true);
                }
                return mTouchPointer;
            }
            set {
                mTouchPointer = value;
            }
        }

        [SerializeField]
        GrabPointer mGrabPointer;
        public GrabPointer grabPointer {
            get {
                if(mGrabPointer == null) {
                    mGrabPointer = GetComponentInChildren<GrabPointer>(true);
                }
                return mGrabPointer;
            }
            set {
                mGrabPointer = value;
            }
        }

        [HideInInspector]
        public bool EnableHandTouchInteraction = true;
        [HideInInspector]
        public bool EnableHandGrabInteraction = true;


        public override void OnSCAwake() {
            base.OnSCAwake();
            if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "EnableHandTouchInteraction")) {
                EnableHandTouchInteraction = API_Module_SDKConfiguration.GetBool("Module_InputSystem", "EnableHandTouchInteraction", 1);
            }
            if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "EnableHandGrabInteraction")) {
                EnableHandGrabInteraction = API_Module_SDKConfiguration.GetBool("Module_InputSystem", "EnableHandGrabInteraction", 1);
            }
            if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "EnableHandFarInteraction")) {
                EnableFarInteraction = API_Module_SDKConfiguration.GetBool("Module_InputSystem", "EnableHandFarInteraction", 1);
            }

            PointerDic.Add(PointerType.Touch, touchPointer);
            PointerDic.Add(PointerType.Grab, grabPointer);
            AddModule(touchPointer);
            AddModule(grabPointer);
        }

        public override void OnSCStart() {
            base.OnSCStart();
            currentPointer = MutualOpenPointer(PointerType.Null);
        }

        public override void OnSCDestroy() {
            base.OnSCDestroy();
            touchPointer = null;
            grabPointer = null;
            currentPointer = null;
        }

        protected override PointerBase PointerActionALG() {

            if (touchPointer.IsFocusLocked || grabPointer.IsFocusLocked || farPointer.IsFocusLocked) {
                return currentPointer;
            } else {
                if (inputDeviceHandPart.inputDevicePartDispatchEventHand.inputDeviceHandPartFaceHead.currentEvent == HandEventType.PalmFaceHead) {
                    return MutualOpenPointer(PointerType.Null);
                } else {

                    if (EnableHandTouchInteraction && touchPointer.FindClosestTouchableForLayerMask()) {
                        if (touchPointer.IsNearObject) {
                            return MutualOpenPointer(PointerType.Touch);
                        } else if (EnableHandGrabInteraction && grabPointer.FindClosestGrabbableForLayerMask()) {
                            return MutualOpenPointer(PointerType.Grab);
                        }
                        return MutualOpenPointer(PointerType.Null);

                    } else if (EnableHandGrabInteraction && grabPointer.FindClosestGrabbableForLayerMask()) {
                        return MutualOpenPointer(PointerType.Grab);
                    } else if (EnableFarInteraction) {
                        return MutualOpenPointer(PointerType.Far);
                    } else {
                        return MutualOpenPointer(PointerType.Null);
                    }
                }
            }
        }

        PointerBase pointerTemp;
        PointerBase MutualOpenPointer(PointerType type) {
            pointerTemp = null;

            foreach (var pointerType in PointerDic.Keys) {
                if (pointerType == type) {
                    if (PointerDic[pointerType].IsModuleStarted == false) {
                        PointerDic[pointerType].ModuleStart();                        
                    }
                    pointerTemp = PointerDic[pointerType];
                } else if (PointerDic[pointerType].IsModuleStarted) {
                    PointerDic[pointerType].ModuleStop();
                }
            }
            return pointerTemp;
        }
    }
}
