using AOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceGC.KS {
    public class InputDataGetKSTouch : InputDataGetGCTouch {

        public InputDataGetKS inputDataGetKS;

        public InputDataGetKSTouch(InputDataGetKS _inputDataGetKS) : base(_inputDataGetKS) {
            inputDataGetKS = _inputDataGetKS;
        }

        public override void UpdateTpPosition() {
            if (InputDataKS.TouchPanelDataList.Count > 0) {

                if ((inputDataGetKS.inputDeviceKSPart.PartType == InputDevicePartType.KSLeft && InputDataKS.TouchPanelDataList[0].deviceID == 0)
                    ||
                   (inputDataGetKS.inputDeviceKSPart.PartType == InputDevicePartType.KSRight && InputDataKS.TouchPanelDataList[0].deviceID == 1)) {

                    TpPosition.x = InputDataKS.TouchPanelDataList[0].x;
                    TpPosition.y = InputDataKS.TouchPanelDataList[0].y;
                    inputDataGetKS.inputDeviceKSPart.inputDataKS.touchPanelAction = (TouchPanelAction)InputDataKS.TouchPanelDataList[0].action;

                    InputDataKS.TouchPanelDataList.RemoveAt(0);

                    DebugMy.Log(inputDataGetKS.inputDeviceKSPart.PartType + " TouchPanelData:" + TpPosition.x + " " + TpPosition.y +" "+ inputDataGetKS.inputDeviceKSPart.inputDataKS.touchPanelAction, this, true);

                }

            }
        }
    }
}
