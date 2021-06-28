
using SC.XR.Unity;
using SC.XR.Unity.Module_Device;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class API_Module_BatteryStatus {

    public static int BatteryLevel {
        get {
            return Module_BatteryStatus.getInstance.BatteryLevel;
        }
    }

    public static BATTERYSTATUS IsCharging {
        get {
            return Module_BatteryStatus.getInstance.IsCharging;
        }
    }

    public static void SetCallBack(Action<int, BATTERYHEALTH, int, BATTERYPLUGGED, BATTERYSTATUS, string, int> batteryStatusCallBack, Action batteryLowCallBack, Action batteryOKAYCallBack) {
        Module_BatteryStatus.getInstance.SetCallBack(batteryStatusCallBack, batteryLowCallBack, batteryOKAYCallBack);
    }

}