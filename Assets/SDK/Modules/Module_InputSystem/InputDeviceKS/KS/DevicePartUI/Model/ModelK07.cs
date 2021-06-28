using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceGC.KS {
    public class ModelK07 : ModelGCBase {
        public InputDeviceKSPartUI inputDeviceKSPartUI {
            get {
                return inputDevicePartUIBase as InputDeviceKSPartUI;
            }
        }

        public MeshRenderer backKey;
        public MeshRenderer functionKey;
        public MeshRenderer triggerKey;
        public TpInfo tpInfo;
        public MeshRenderer volumeDown;
        public MeshRenderer volumeUp;
        public MeshRenderer tpKey;

        [Serializable]
        public class TpInfo {
            public Transform tpPosition;
        }


        public Material pressMaterial;
        public Material releaseMaterial;

        public Vector3 modelPositionDeltaWithDevice = new Vector3(0, 0, 0.20f);

        void UpdateTransform() {
            transform.localPosition = modelPositionDeltaWithDevice;
        }

        public override void OnSCLateUpdate() {
            base.OnSCLateUpdate();
            UpdateTransform();
        }

        public override void SetTpPosition() {
            if (inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.touchPanelAction ==  TouchPanelAction.DOWN || inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.touchPanelAction == TouchPanelAction.MOVE) {
                //tpPosition.gameObject.activeSelf ? tpPosition.gameObject.SetActive(true) : null;
                if (!tpInfo.tpPosition.gameObject.activeSelf) {
                    tpInfo.tpPosition.gameObject.SetActive(true);
                }
                tpInfo.tpPosition.localPosition = new Vector3(
                    (inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.tpPosition.x - 125) * 0.00012f,
                    tpInfo.tpPosition.localPosition.y,
                    (inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.tpPosition.y - 125) * 0.00012f);

                //if (touchDircionCoroutine == null) {
                //    ////Debug.Log("wangcq327 --- StartCoroutine device " +inputDeviceType);
                //    //touchDircionCoroutine = StartCoroutine("TouchEvent", (inputDeviceType == InputDeviceType.HandShankMain) ? 0 : 1);
                //}
            } else {
                if (tpInfo.tpPosition.gameObject.activeSelf) {
                    tpInfo.tpPosition.gameObject.SetActive(false);
                }
                //if (touchDircionCoroutine != null) {
                //    StopCoroutine("TouchEvent");
                //    touchDircionCoroutine = null;
                //}
            }
        }


        public override void SetHandleKeysColor() {
            if (releaseMaterial == null || pressMaterial == null) {
                return;
            }

            foreach (var item in inputDeviceKSPartUI.inputDeviceKSPart.inputDataKS.inputKeys.inputKeyDic) {

                if (item.Key == InputKeyCode.Trigger) {
                    triggerKey.material = releaseMaterial;
                    if (item.Value == InputKeyState.DOWN || item.Value == InputKeyState.LONG) {
                        triggerKey.material = pressMaterial;
                    }
                } else if (item.Key == InputKeyCode.Function) {
                    functionKey.material = releaseMaterial;
                    if (item.Value == InputKeyState.DOWN || item.Value == InputKeyState.LONG) {
                        functionKey.material = pressMaterial;
                    }
                } else if (item.Key == InputKeyCode.Back) {
                    backKey.material = releaseMaterial;
                    if (item.Value == InputKeyState.DOWN || item.Value == InputKeyState.LONG) {
                        backKey.material = pressMaterial;
                    }
                }else if (item.Key == InputKeyCode.Tp) {
                    tpKey.material = releaseMaterial;
                    if (item.Value == InputKeyState.DOWN || item.Value == InputKeyState.LONG) {
                        tpKey.material = pressMaterial;
                    }
                } else if (item.Key == InputKeyCode.VolumeDown) {
                    volumeDown.material = releaseMaterial;
                    if (item.Value == InputKeyState.DOWN || item.Value == InputKeyState.LONG) {
                        volumeDown.material = pressMaterial;
                    }
                } else if (item.Key == InputKeyCode.VolumeUp) {
                    volumeUp.material = releaseMaterial;
                    if (item.Value == InputKeyState.DOWN || item.Value == InputKeyState.LONG) {
                        volumeUp.material = pressMaterial;
                    }
                }

            }
        }



    }
}
