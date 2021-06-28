using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceGC.KS {
    public class ModelK102 : ModelK101 {

        Vector3 initLocaleulerAngle;
        public Vector3 DefauleAngleOffset;
        public override void OnSCAwake() {
            base.OnSCAwake();
            if (StartPoint) {
                initLocaleulerAngle = StartPoint.localEulerAngles;
                DefauleAngleOffset = new Vector3(43,0,0);
            }
        }

        public override void OnSCStart() {
            base.OnSCStart(); 
            
            if (StartPoint) {
                InitStartPointRotation();
            }
        }

        public override void OnSCDestroy() {
            base.OnSCDestroy();
            StartPoint.localEulerAngles = initLocaleulerAngle;
        }


        protected void InitStartPointRotation() {
            if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "K102PointerAngleOffsetX")) {
                DefauleAngleOffset.x = Mathf.Clamp(API_Module_SDKConfiguration.GetFloat("Module_InputSystem", "K102PointerAngleOffsetX", 0), -60, 60);
            }
            if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "K102PointerAngleOffsetY")) {
                DefauleAngleOffset.y = Mathf.Clamp(API_Module_SDKConfiguration.GetFloat("Module_InputSystem", "K102PointerAngleOffsetY", 0), -60, 60);
            }
            if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "K102PointerAngleOffsetZ")) {
                DefauleAngleOffset.z = Mathf.Clamp(API_Module_SDKConfiguration.GetFloat("Module_InputSystem", "K102PointerAngleOffsetZ", 0), -60, 60);
            }

            DebugMy.Log("StartPoint initLocaleulerAngle:" + initLocaleulerAngle + "   DefauleAngleOffset:" + DefauleAngleOffset, this, true);

            if (Application.isEditor) {
                DefauleAngleOffset = new Vector3(0, 0, 0);
                DebugMy.Log("isEditor Set DefauleAngleOffset:" + DefauleAngleOffset, this, true);
            }
            StartPoint.localEulerAngles = initLocaleulerAngle + DefauleAngleOffset;

        }

    }
}
