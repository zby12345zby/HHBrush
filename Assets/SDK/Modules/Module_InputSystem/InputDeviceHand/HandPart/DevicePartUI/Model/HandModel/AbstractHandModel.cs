using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceHand {

    [Serializable]
    public class FingerUI {
        public Transform[] jointGameObject;
    }

    public abstract class AbstractHandModel : SCModuleMono, IHandModel {

        /// <summary>
        /// Father Module
        /// </summary>
        ModelHand _modelHand;
        public ModelHand modelHand {
            get {
                if (_modelHand == null) {
                    _modelHand = GetComponentInParent<ModelHand>();
                }
                return _modelHand;
            }
        }

        [SerializeField]
        private FingerUI[] mFingerUI;
        public FingerUI[] fingerUI => mFingerUI;


        [SerializeField]
        private Transform mhandJointContainer;
        public Transform handJointContainer => mhandJointContainer;


        public abstract HandModelType handModelType { get; }

        public abstract void UpdateTransform();
        public abstract Transform GetJointTransform(FINGER finger, JOINT joint);

        public override void OnSCLateUpdate() {
            base.OnSCLateUpdate();
            UpdateTransform();
        }

        private bool mShowHandRender;
        public virtual bool ShowHandRender {
            get {
                return mShowHandRender;
            }
            set {
                mShowHandRender = value;
                HandRenderSet(value);
            }
        }
        public abstract void HandRenderSet(bool isOn);
        public override void OnSCAwake() {
            base.OnSCAwake();
            if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "ShowHandRender")) {
                try {
                    ShowHandRender = API_Module_SDKConfiguration.GetBool("Module_InputSystem", "ShowHandRender", 0);
                    DebugMy.Log("ShowHandRender From SDKConfig:" + ShowHandRender, this, true);
                } catch (Exception e) {
                    Debug.Log(e);
                }
            }
            if (API_Module_Device.Current.XRType == XRType.VR) {
                DebugMy.Log("ShowHandRender Must True For VR Device !", this, true);
                ShowHandRender = true;
            }
        }
    }
}