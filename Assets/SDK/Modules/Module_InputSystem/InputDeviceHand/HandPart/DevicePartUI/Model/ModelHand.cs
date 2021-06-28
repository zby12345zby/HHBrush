using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceHand
{

    public class ModelHand : ModelBase {
        public InputDeviceHandPartUI inputDeviceHandPartUI {
            get {
                return inputDevicePartUIBase as InputDeviceHandPartUI;
            }
        }

        [Tooltip("All Vaild HandModel")]
        [SerializeField]
        private List<AbstractHandModel> mAllHandModelList;
        protected List<AbstractHandModel> AllHandModelList {
            get {
                if (mAllHandModelList.Count == 0) {
                    mAllHandModelList = new List<AbstractHandModel>(GetComponentsInChildren<AbstractHandModel>(true)) ;
                }
                return mAllHandModelList;
            }
        }

        public AbstractHandModel ActiveHandModel { get; private set; }


        private HandModelType currentHandModelType;

        public FingerUI[] fingerUI {
            get {
                if (ActiveHandModel != null) {
                    return ActiveHandModel.fingerUI;
                }
                return null;
            }
        }

        public override void OnSCAwake() {
            base.OnSCAwake();
            foreach (var handmodel in AllHandModelList) {
                AddModule(handmodel);
            }
            if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "HandModelType")) {
                System.Enum.TryParse<HandModelType>(API_Module_SDKConfiguration.GetString("Module_InputSystem", "HandModelType", "EffectHand"), false, out currentHandModelType);
            }
            DebugMy.Log("HandModelType:" + currentHandModelType, this, true);
        }

        public override void OnSCStart() {
            base.OnSCStart();
            HandModelChange(currentHandModelType);
        }

        public override void OnSCDisable() {
            base.OnSCDisable();
            ActiveHandModel = null;
        }

        public void HandModelChange(HandModelType type) {
            if (ActiveHandModel == null || type != ActiveHandModel.handModelType) {
                if (ActiveHandModel) {
                    ActiveHandModel.ModuleStop();
                }
                foreach (var handmodel in AllHandModelList) {
                    if (type == handmodel.handModelType) {
                        handmodel.ModuleStart();
                        ActiveHandModel = handmodel;
                        currentHandModelType = type;
                    }
                }
            }
        }

        //void OnDrawGizmos()
        //{
        //    if (Application.isPlaying)
        //    {
        //        Gizmos.color = Color.black * 0.3f;
        //        Gizmos.DrawSphere(inputDeviceHandPartUI.inputDeviceHandPart.inputDataHand.handInfo.centerPosition, 0.01f);

        //        Gizmos.color = Color.yellow * 1f;
        //        Gizmos.DrawSphere(inputDeviceHandPartUI.inputDeviceHandPart.inputDataHand.handInfo.centerPosition + inputDeviceHandPartUI.inputDeviceHandPart.inputDataHand.handInfo.normal * 0.05f, 0.01f);

        //        Gizmos.color = Color.blue * 1f;
        //        Gizmos.DrawSphere(inputDeviceHandPartUI.inputDeviceHandPart.inputDataHand.handInfo.centerPosition + inputDeviceHandPartUI.inputDeviceHandPart.inputDataHand.handInfo.up * 0.05f, 0.01f);

        //        Gizmos.color = Color.black * 0.2f;
        //        Gizmos.DrawSphere(inputDeviceHandPartUI.inputDeviceHandPart.inputDataHand.handInfo.centerPosition + inputDeviceHandPartUI.inputDeviceHandPart.inputDataHand.handInfo.right * 0.05f, 0.01f);
        //    }
        //}
    }
}