using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem {
    public abstract class DetectorBase : SCModuleMono {

        public PointerBase currentPointer;
        [HideInInspector]
        public bool EnableFarInteraction = true;

        InputDevicePartBase _inputDevicePartBase;
        public InputDevicePartBase inputDevicePartBase {
            get {
                if(_inputDevicePartBase == null) {
                    _inputDevicePartBase = GetComponentInParent<InputDevicePartBase>();
                }
                return _inputDevicePartBase;
            }
            set {
                _inputDevicePartBase = value;
            }
        }

        [SerializeField]
        PointerBase mPointerBase;
        public PointerBase pointerBase {
            get {
                if(mPointerBase == null) {
                    mPointerBase = GetComponentInChildren<PointerBase>(true);
                }
                return mPointerBase;
            }
            set {
                mPointerBase = value;
            }
        }

        public Dictionary<PointerType, PointerBase> PointerDic = new Dictionary<PointerType, PointerBase>();

        public override void OnSCAwake() {
            base.OnSCAwake();
            if (PointerDic == null) {
                PointerDic = new Dictionary<PointerType, PointerBase>();
            }
            PointerDic.Add(PointerType.Far, pointerBase);
            AddModule(pointerBase);
        }


        public override void OnSCStart() {
            base.OnSCStart();
            if (EnableFarInteraction) {
                pointerBase?.ModuleStart();
            }
        }

        public override void OnSCLateUpdate() {
            base.OnSCLateUpdate();
            currentPointer = PointerActionALG(); 
        }

        public override void OnSCDisable() {
            base.OnSCDisable();
            currentPointer = null;
        }

        public override void OnSCDestroy() {
            base.OnSCDestroy();
            pointerBase = null;
        }
        protected virtual PointerBase PointerActionALG() {
            if (pointerBase.IsModuleStarted) {
                return pointerBase;
            }
            return null;
        }
    }
}
