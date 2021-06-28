using System.Collections;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceGC.KS {
    public class InputDeviceKSPartUI : InputDeviceGCPartUI {
        public InputDeviceKSPart inputDeviceKSPart {
            get {
                return inputDevicePartBase as InputDeviceKSPart;
            }
        }

        private ModelK102 modelK102 { get { return GetComponentInChildren<ModelK102>(true); } }
        private ModelK101 modelK101 { get { return GetComponentInChildren<ModelK101>(true); } }
        private ModelK11 modelK11 { get { return GetComponentInChildren<ModelK11>(true); } }
        private ModelK07 modelK07 { get { return GetComponentInChildren<ModelK07>(true); } }
        private ModelNolo modelNolo { get { return GetComponentInChildren<ModelNolo>(true); } }

        public override void OnSCAwake() {
            base.OnSCAwake();
            RemoveModule(modelBase);
            AddModule(modelK102);
            AddModule(modelK101);
            AddModule(modelK11);
            AddModule(modelK07);
            AddModule(modelNolo);
        }

        public override void OnSCStart() {
            base.OnSCStart();
            
            modelBase?.ModuleStop();

            if (inputDeviceKSPart.inputDataGC.GCType == GCType.K102 && modelK102) {
                modelBase = ModelGC = modelK102;
            } else if (inputDeviceKSPart.inputDataGC.GCType == GCType.K101 && modelK101) {
                modelBase = ModelGC = modelK101;
            } else if (inputDeviceKSPart.inputDataGC.GCType == GCType.K11 && modelK11) {
                modelBase = ModelGC = modelK11;
            } else if (inputDeviceKSPart.inputDataGC.GCType == GCType.K07 && modelK07) {
                modelBase = ModelGC = modelK07;
            } else if (inputDeviceKSPart.inputDataGC.GCType == GCType.Nolo && modelNolo) {
                modelBase = ModelGC = modelNolo;
            } else {
                DebugMy.Log("Error No find inputDeviceKSPart.inputDataGC.GCType Model Module,Use Default !", this, true);
                modelBase = ModelGC = modelK102;
            }

            AddModule(modelBase);
            modelBase?.ModuleStart();
            DebugMy.Log("Model Type:" + modelBase?.GetType(),this,true);
        }


        public override void OnSCDisable() {
            base.OnSCDisable();
            RemoveModule(modelBase);
            modelBase = ModelGC = null;
        }

    }
}