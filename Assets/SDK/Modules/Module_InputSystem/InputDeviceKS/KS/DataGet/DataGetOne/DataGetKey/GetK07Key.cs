using AOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceGC.KS {
    public class GetK07Key : SCModule {

        public InputDataGetKSKey inputDataGetKSKey;
        public GetK07Key(InputDataGetKSKey inputDataGetKSKey) {
            this.inputDataGetKSKey = inputDataGetKSKey;
        }

        public override void OnSCAwake() {
            base.OnSCAwake();
            if (inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.PartType == InputDevicePartType.KSLeft) {
                if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "K07_Left_EnterKeyAlias")) {
                    InputKeyCode keyparse;
                    inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataGC.EnterKeyAlias = System.Enum.TryParse<InputKeyCode>(API_Module_SDKConfiguration.GetString("Module_InputSystem", "K07_Left_EnterKeyAlias", "LTrigger"), false, out keyparse) ? keyparse : InputKeyCode.Trigger;
                } else {
                    inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataGC.EnterKeyAlias = InputKeyCode.Trigger;
                }
                if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "K07_Left_AnotherEnterKeyAlias")) {
                    InputKeyCode keyparse;
                    inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataGC.AnotherEnterKeyAlias = System.Enum.TryParse<InputKeyCode>(API_Module_SDKConfiguration.GetString("Module_InputSystem", "K07_Left_AnotherEnterKeyAlias", "NULL"), false, out keyparse) ? keyparse : InputKeyCode.NULL;
                }
            } else if (inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.PartType == InputDevicePartType.KSRight) {
                if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "K07_Right_EnterKeyAlias")) {
                    InputKeyCode keyparse;
                    inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataGC.EnterKeyAlias = System.Enum.TryParse<InputKeyCode>(API_Module_SDKConfiguration.GetString("Module_InputSystem", "K07_Right_EnterKeyAlias", "RTrigger"), false, out keyparse) ? keyparse : InputKeyCode.Trigger;
                } else {
                    inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataGC.EnterKeyAlias = InputKeyCode.Trigger;
                }
                if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "K07_Right_AnotherEnterKeyAlias")) {
                    InputKeyCode keyparse;
                    inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataGC.AnotherEnterKeyAlias = System.Enum.TryParse<InputKeyCode>(API_Module_SDKConfiguration.GetString("Module_InputSystem", "K07_Right_AnotherEnterKeyAlias", "NULL"), false, out keyparse) ? keyparse : InputKeyCode.NULL;
                }
            }
            DebugMy.Log(inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.PartType + " inputDataGetKSKey.EnterKeyAlias:" + inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataGC.EnterKeyAlias
                + " inputDataGetKSKey.AnotherEnterKeyAlias:" + inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataGC.AnotherEnterKeyAlias
                , this, true);

            if (inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.PartType == InputDevicePartType.KSLeft) {
                if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "K07_Left_CalibrationKeyAlias")) {
                    InputKeyCode keyparse;
                    inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataGC.CalibrationKeyAlias = System.Enum.TryParse<InputKeyCode>(API_Module_SDKConfiguration.GetString("Module_InputSystem", "K07_Left_CalibrationKeyAlias", "DOWN"), false, out keyparse) ? keyparse : InputKeyCode.Function;
                } else {
                    inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataGC.CalibrationKeyAlias = InputKeyCode.Function;
                }
            } else if (inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.PartType == InputDevicePartType.KSRight) {
                if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "K07_Right_CalibrationKeyAlias")) {
                    InputKeyCode keyparse;
                    inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataGC.CalibrationKeyAlias = System.Enum.TryParse<InputKeyCode>(API_Module_SDKConfiguration.GetString("Module_InputSystem", "K07_Right_CalibrationKeyAlias", "A"), false, out keyparse) ? keyparse : InputKeyCode.Function;
                } else {
                    inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataGC.CalibrationKeyAlias = InputKeyCode.Function;
                }
            }
            DebugMy.Log(inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.PartType + " inputDataGetKSKey.CalibrationKeyAlias:" + inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataGC.CalibrationKeyAlias, this, true);
        }

        public override void OnSCLateUpdate() {
            base.OnSCLateUpdate();

            ProcessKeyList();
        }

        public override void OnSCDestroy() {
            base.OnSCDestroy();
            inputDataGetKSKey = null;
        }


        protected virtual void ProcessKeyList() {
            //DebugMy.Log("InputDataGC.GCData.GCKeyList.Count:"+ InputDataGC.GCData.GCKeyList.Count+" "+ inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.PartType,this,true);

            if (InputDataGC.GCData.GCKeyList.Count > 0) {
                InputKeyCode inputKeyCode;
                InputKeyState inputKeyState;

                //DebugMy.Log("InputDataGC.GCData.GCKeyList[0].deivceID:" + InputDataGC.GCData.GCKeyList[0].deivceID+" "+ InputDataGC.GCData.GCKeyList[0].keycode +" "+ InputDataGC.GCData.GCKeyList[0].keyevent, this, true);
                if ((inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.PartType == InputDevicePartType.KSLeft && InputDataGC.GCData.GCKeyList[0].deivceID == 0)
                    ||
                   (inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.PartType == InputDevicePartType.KSRight && InputDataGC.GCData.GCKeyList[0].deivceID == 1)) {

                    switch ((KSKeyCode)InputDataGC.GCData.GCKeyList[0].keycode) {
                        case KSKeyCode.K07_BACK:
                            inputKeyCode = InputKeyCode.Back;
                            break;
                        case KSKeyCode.K07_TIGGER:
                            inputKeyCode = InputKeyCode.Trigger;
                            break;
                        case KSKeyCode.K07_FUNCTION:
                            inputKeyCode = InputKeyCode.Function;
                            break;
                        case KSKeyCode.K07_TP:
                            inputKeyCode = InputKeyCode.Tp;
                            break;
                        case KSKeyCode.K07_VOLUMEDOWN:
                            inputKeyCode = InputKeyCode.VolumeDown;
                            break;
                        case KSKeyCode.K07_VOLUMEUP:
                            inputKeyCode = InputKeyCode.VolumeUp;
                            break;
                        default:
                            inputKeyCode = InputKeyCode.OTHER;
                            break;
                    }

                    switch ((KSKeyState)InputDataGC.GCData.GCKeyList[0].keyevent) {
                        case KSKeyState.UP:
                            inputKeyState = InputKeyState.UP;
                            break;
                        case KSKeyState.DOWN:
                            inputKeyState = InputKeyState.DOWN;
                            break;
                        case KSKeyState.LONG:
                            inputKeyState = InputKeyState.LONG;
                            break;
                        default:
                            inputKeyState = InputKeyState.Null;
                            break;
                    }

                    inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataKS.inputKeys.InputDataAddKey(inputKeyCode, inputKeyState);
                    DebugMy.Log(inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.PartType + " ProcessKeyList:" + inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.PartType + " Add Key:" + inputKeyCode + "  State:" + inputKeyState, this, true);

                    ///Enter别名
                    if (inputKeyCode == inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataKS.EnterKeyAlias || inputKeyCode == inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataKS.AnotherEnterKeyAlias) {
                        inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataKS.inputKeys.InputDataAddKey(InputKeyCode.Enter, inputKeyState);
                        DebugMy.Log(inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.PartType + " ProcessKeyList:" + inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.PartType + " Add (Alias:" + inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataKS.EnterKeyAlias + ") Key:" + InputKeyCode.Enter + "  State:" + inputKeyState, this, true);
                    }

                    if (inputKeyCode == inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataKS.CancelKeyAlias) {
                        inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataKS.inputKeys.InputDataAddKey(InputKeyCode.Cancel, inputKeyState);
                        DebugMy.Log(inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.PartType + " ProcessKeyList:" + inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.PartType + " Add (Alias:" + inputDataGetKSKey.inputDataGetKS.inputDeviceKSPart.inputDataKS.CancelKeyAlias + ") Key:" + InputKeyCode.Cancel + "  State:" + inputKeyState, this, true);
                    }

                    InputDataGC.GCData.GCKeyList.RemoveAt(0);
                } else {
                    DebugMy.Log("Delect illegality Key:" + +InputDataGC.GCData.GCKeyList[0].deivceID + " " + InputDataGC.GCData.GCKeyList[0].keycode + " " + InputDataGC.GCData.GCKeyList[0].keyevent, this, true);
                    InputDataGC.GCData.GCKeyList.RemoveAt(0);
                }

            }
        }


    }
}
