using System;
using System.Collections.Generic;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem {

    public class InputDataGetKey : InputDataGetOneBase {

        public InputDataGetKey(InputDataGetBase _inputDataGet) : base(_inputDataGet) {
        }

        public override void OnSCLateUpdate() {
            base.OnSCLateUpdate();
            OnUpdateKey();
        }

        public virtual void OnUpdateKey() {

            if(API_GSXR_Slam.SlamManager == null)
                return;

            if(API_GSXR_Slam.SlamManager.plugin.HeadSetEnterKeyDown()) {
                inputDataGetBase.inputDevicePartBase.inputDataBase.inputKeys.InputDataAddKey(InputKeyCode.Enter, InputKeyState.DOWN);
            } else if(API_GSXR_Slam.SlamManager.plugin.HeadSetEnterKeyUp()) {
                inputDataGetBase.inputDevicePartBase.inputDataBase.inputKeys.InputDataAddKey(InputKeyCode.Enter, InputKeyState.UP);
            }

            if(API_GSXR_Slam.SlamManager.plugin.HeadSetBackKeyDown()) {
                inputDataGetBase.inputDevicePartBase.inputDataBase.inputKeys.InputDataAddKey(InputKeyCode.Back, InputKeyState.DOWN);
            } else if(API_GSXR_Slam.SlamManager.plugin.HeadSetBackKeyUp()) {
                inputDataGetBase.inputDevicePartBase.inputDataBase.inputKeys.InputDataAddKey(InputKeyCode.Back, InputKeyState.UP);
            }
        }
    }
}
