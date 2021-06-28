using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceHead {
    public class InputDataGetPosture : InputDataGetOneBase {

        InputDataGetHead inputDataGetHead;
        public InputDataGetPosture(InputDataGetHead _inputDataGetHead) : base(_inputDataGetHead) {
            inputDataGetHead = _inputDataGetHead;
        }

        public override void OnSCLateUpdate() {
            base.OnSCLateUpdate();
            OnUpdatePosition();
            OnUpdateRotation();
        }

        void OnUpdateRotation() {
            if(API_GSXR_Slam.SlamManager != null && API_GSXR_Slam.SlamManager.gameObject.activeSelf) {
                inputDataGetHead.inputDeviceHeadPart.inputDataHead.rotation = API_GSXR_Slam.SlamManager.head.rotation;
            } else if(Camera.main) {
                inputDataGetHead.inputDeviceHeadPart.inputDataHead.rotation = Camera.main.transform.rotation;
            }
        }

        void OnUpdatePosition() {
            if(API_GSXR_Slam.SlamManager != null && API_GSXR_Slam.SlamManager.gameObject.activeSelf) {
                inputDataGetHead.inputDeviceHeadPart.inputDataHead.position = API_GSXR_Slam.SlamManager.head.position;
            } else if(Camera.main) {
                inputDataGetHead.inputDeviceHeadPart.inputDataHead.position = Camera.main.transform.position;
            }
        }
    }
}
