
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SC.XR.Unity.Module_Device {
    public class Module_Device {

        private static Module_Device instance;
        public static Module_Device getInstance {
            get {
                if (instance == null) {
                    instance = new Module_Device();
                }
                return instance;
            }
        }

        private GSXRDevice _current;
        public GSXRDevice Current {
            get {
                if (_current == null) {
                    _current = new GSXRDevice();
                }
                return _current;
            }
        }

    }


}