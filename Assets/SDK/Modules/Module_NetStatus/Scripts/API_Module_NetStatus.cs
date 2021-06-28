using SC.XR.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class API_Module_NetStatus
    {
        public static bool IsConnect()
        {
            return NetStatus.getInstance.SCIsConnectWifi();
        }

        public static string GetNetSpeedValue()
        {
            return NetStatus.getInstance.netSpeedStr;
        }

        public static WIFISTREGTH GetNetStrength()
        {
            return NetStatus.getInstance.SCGetWifiStrength();
        }
        public static void SetCallBack(Action<NetworkReachability, NetworkReachability> NetWorkChange)
        {
            NetStatus.getInstance.RegisterNetWorkChangeCallBack(NetWorkChange);
        }
    }

