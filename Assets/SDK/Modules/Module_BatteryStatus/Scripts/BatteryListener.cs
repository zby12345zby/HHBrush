using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryListener : AndroidJavaProxy, IBatteryCallBack {

    Action<int, BATTERYHEALTH, int, BATTERYPLUGGED, BATTERYSTATUS, string, int> batteryStatusCallBack;
    Action batteryLowCallBack;
    Action batteryOKAYCallBack;

    public BatteryListener(Action<int, BATTERYHEALTH, int, BATTERYPLUGGED, BATTERYSTATUS, string, int> batteryStatusCallBack, Action batteryLowCallBack = null, Action batteryOKAYCallBack = null) : base("com.example.batterystatus.IBatteyStatus") {
        this.batteryStatusCallBack = batteryStatusCallBack;
        this.batteryLowCallBack = batteryLowCallBack;
        this.batteryOKAYCallBack = batteryOKAYCallBack;
    }

    public void BATTERY_CHANGED(int voltage, int health, int level, int pluged, int status, string technology, int temperature) {
        if (batteryStatusCallBack != null) {
            batteryStatusCallBack(voltage, (BATTERYHEALTH)health, level, (BATTERYPLUGGED)pluged, (BATTERYSTATUS)status, technology, temperature);
        }
    }

    public void BATTERY_LOW() {
        if (batteryLowCallBack != null) {
            batteryLowCallBack();
        }

    }

    public void BATTERY_OKAY() {
        if (batteryOKAYCallBack != null) {
            batteryOKAYCallBack();
        }
    }
}
