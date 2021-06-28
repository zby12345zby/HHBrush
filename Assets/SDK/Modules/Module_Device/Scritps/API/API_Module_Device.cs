
using SC.XR.Unity.Module_Device;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class API_Module_Device {
    public static GSXRDevice Current {
        get {
            return Module_Device.getInstance.Current;
        }
    }

    public static bool IsGSXRAndroidDevice {
        get {
            if (API_GSXR_Slam.SlamManager.plugin is GSXRPluginAndroid ) {
                return true;
            }
            return false;
        }
    }

    public static bool IsGSXRWinDevice {
        get {
            if (API_GSXR_Slam.SlamManager.plugin is GSXRPluginWin) {
                return true;
            }
            return false;
        }
    }

    public static bool IsOtherDevice {
        get {
            if (API_GSXR_Slam.SlamManager.plugin is GSXRPluginOther) {
                return true;
            }
            return false;
        }
    }

}