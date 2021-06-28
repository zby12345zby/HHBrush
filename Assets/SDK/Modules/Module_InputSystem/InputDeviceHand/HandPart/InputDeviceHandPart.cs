using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceHand {

    public class InputDeviceHandPart : InputDevicePartBase {

        public InputDeviceHand inputDeviceHand {
            get { return Transition<InputDeviceHand>(inputDeviceBase); }
        }

        public InputDeviceHandPartUI inputDeviceHandPartUI {
            get { return Transition<InputDeviceHandPartUI>(inputDevicePartUIBase); }
        }
        public HandDetector gGT26DofDetector {
            get { return Transition<HandDetector>(detectorBase); }
        }


        public InputDataHand inputDataHand {get; set;}

        public InputDataGetHand inputDataGetHand { get; set; }


        public InputDeviceHandPartStatus inputDevicePartStatusHand { get; set; }


        public InputDevicePartDispatchEventHand inputDevicePartDispatchEventHand { get; set; }


        public override void OnSCStart() {

            if (inputDataBase != null)
                inputDataBase.ModuleStart();

            if (inputDevicePartStatusBase != null)
                inputDevicePartStatusBase.ModuleStart();

        }

        protected override void SubModuleStatusChange() {

            if (inputDataBase.isVaild == false) {
                if (inputDataGetBase != null && inputDataGetBase.IsModuleStarted)
                    inputDataGetBase.ModuleStop();

                if (inputDevicePartUIBase != null && inputDevicePartUIBase.IsModuleStarted)
                    inputDevicePartUIBase.ModuleStop();

                if (detectorBase != null && detectorBase.IsModuleStarted)
                    detectorBase.ModuleStop();

                if (inputDevicePartDispatchEventBase != null && inputDevicePartDispatchEventBase.IsModuleStarted)
                    inputDevicePartDispatchEventBase.ModuleStop();

            } else {

                if (inputDataGetBase != null && !inputDataGetBase.IsModuleStarted)
                    inputDataGetBase.ModuleStart();

                if (inputDevicePartUIBase != null && !inputDevicePartUIBase.IsModuleStarted)
                    inputDevicePartUIBase.ModuleStart();

                if (detectorBase != null && !detectorBase.IsModuleStarted)
                    detectorBase.ModuleStart();

                if (inputDevicePartDispatchEventBase != null && !inputDevicePartDispatchEventBase.IsModuleStarted)
                    inputDevicePartDispatchEventBase.ModuleStart();

            }
        }

        protected override void ModuleCreater() {
            inputDevicePartDispatchEventBase = inputDevicePartDispatchEventHand = new InputDevicePartDispatchEventHand(this);
        }
    }
}
