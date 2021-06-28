using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SC.XR.Unity {

    public class Module_BatteryStatus {
        private static Module_BatteryStatus instance;
        public static Module_BatteryStatus getInstance {
            get {
                if (instance == null) {
                    instance = new Module_BatteryStatus();
                }
                return instance;
            }
        }
        private AndroidJavaObject BatteryStatusObject;

        Module_BatteryStatus() {
            if (Application.platform == RuntimePlatform.Android) {
                BatteryStatusObject = new AndroidJavaObject("com.example.batterystatus.BatteryStatus", AndroidPluginBase.CurrentActivity);
            }
        }

        public void SetCallBack(Action<int, BATTERYHEALTH, int, BATTERYPLUGGED, BATTERYSTATUS, string, int> batteryStatusCallBack, Action batteryLowCallBack, Action batteryOKAYCallBack) {
            if (BatteryStatusObject!=null) {
                BatteryStatusObject.Call("SetCallBack", new BatteryListener(batteryStatusCallBack, batteryLowCallBack, batteryOKAYCallBack));
            }
        }

        public int BatteryLevel {
            get {
                if (BatteryStatusObject != null) {
                    return BatteryStatusObject.Call<int>("BatteryLevel");
                }
                return -1;
            }
        }

        public BATTERYSTATUS IsCharging {
            get {
                if (BatteryStatusObject != null) {
                    return (BATTERYSTATUS)BatteryStatusObject.Call<int>("IsCharging");
                }
                return BATTERYSTATUS.BATTERY_STATUS_UNKNOWN;
            }
        }

    }

}