using System;
using System.Collections;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceHand {
    public class InputDeviceHandPartUI : InputDevicePartUIBase {
        public InputDeviceHandPart inputDeviceHandPart {
            get {
                return inputDevicePartBase as InputDeviceHandPart;
            }
        }

        public ModelHand modelHand {
            get {
                return modelBase as ModelHand;
            }
        }

        HandMenu _handMenu;
        public HandMenu handMenu {
            get {
                if(_handMenu == null) {
                    _handMenu = GetComponentInChildren<HandMenu>(true);
                }
                return _handMenu;
            }
        }

        public override void OnSCAwake() {
            base.OnSCAwake();
            //InputDeviceHandPartEventBase.eventDelegate += HandEventDelegate;
            AddModule(handMenu);

        }

        public override void OnSCDestroy() {
            base.OnSCDestroy();
            //InputDeviceHandPartEventBase.eventDelegate -= HandEventDelegate;
        }

        //void HandEventDelegate(InputDeviceHandPart inputDeviceHandPart, HandEventType eventType, float EventPercent) {
        //    if (this.inputDeviceHandPart == inputDeviceHandPart && inputDeviceHandPart && API_GSXR_Slam.SlamManager) {

        //        if (eventType == HandEventType.PalmFlatFaceHeadStart) {
        //            if (handMenu && handMenu.IsModuleStarted == false) {
        //                handMenu.ModuleStart();
        //            }
        //        } else if(eventType == HandEventType.PalmFlatFaceHeadEnd) {
        //            if (handMenu && handMenu.IsModuleStarted) {
        //                handMenu.ModuleStop();
        //            }
        //        }
        //    }
            
        //}

    }
}