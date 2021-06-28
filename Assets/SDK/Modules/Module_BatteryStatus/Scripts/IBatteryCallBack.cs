using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBatteryCallBack {
    void BATTERY_CHANGED(int voltage, int health, int level, int pluged, int status, string technology, int temperature);
    void BATTERY_LOW();
    void BATTERY_OKAY();
}
