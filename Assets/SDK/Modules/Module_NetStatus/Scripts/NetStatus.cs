using SC.XR.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace SC.XR.Unity
{
    public class NetStatus : MonoBehaviour
    {

        private static NetStatus mInstant;
        public  static NetStatus getInstance {
                get {
                
                return mInstant;
            }
        }
        AndroidJavaObject jo;


        string m_ReachabilityText = "";
        [HideInInspector]
       public  string netSpeedStr = "";
        string netIP;
        string netmac;
        string netStatus = "";
        WIFISTREGTH wifiStrength = WIFISTREGTH.WIFI_NULL;
        
        int strength = -2;
        WIFISTREGTH _WIFIStrength;

        private event Action<NetworkReachability, NetworkReachability> NetWorkChangeCallBack;

        private NetworkReachability preNetWorkStatus = NetworkReachability.NotReachable;

        private void Awake()
        {
            if (mInstant != null)
            {
                DestroyImmediate(gameObject);
                return;
            }
            mInstant = this;
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            jo = new AndroidJavaObject("com.example.netstatus.NetState");

            jo.Call("NetStart");
            StartCoroutine(GetNetSpeedAndstatus());
            SCGetNetStatus();
            preNetWorkStatus = Application.internetReachability;
        }
        void Update()
        {
            if (preNetWorkStatus != Application.internetReachability)
            {
                if (NetWorkChangeCallBack != null)
                {
                    NetWorkChangeCallBack(preNetWorkStatus, Application.internetReachability);
                }
                preNetWorkStatus = Application.internetReachability;
            }
        }



        private void OnDestroy()
        {
            StopCoroutine(GetNetSpeedAndstatus());
            jo = null;


            m_ReachabilityText = null;
            netmac = null;
            netIP = null;
            netSpeedStr = null;
            
        }


        /// <summary>
        /// 注册网络状态变化时触发事件，第一个参数为变化前的网络状态
        /// 第二个参数为变化后的网络状态
        /// </summary>
        /// <param name="callback"></param>
        public void RegisterNetWorkChangeCallBack(Action<NetworkReachability, NetworkReachability> callback)
        {
            NetWorkChangeCallBack += callback;
        }

        public string SCGetNetStatus()//获取网络状态
        {
            //Check if the device cannot reach the internet
            if (Application.internetReachability == NetworkReachability.NotReachable)//网络无法连接
            {
                //Change the Text
                m_ReachabilityText = "NoNetWork";

            }
            //Check if the device can reach the internet via a carrier data network
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)//网络可通过由运营商数据网络访问
            {
                m_ReachabilityText = "3G/4GNetWork";
            }
            //Check if the device can reach the internet via a LAN
            else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)//网络可通过WIFI或电缆访问
            {
                m_ReachabilityText = "WifiOrCableNetWork";
            }
            //Output the network reachability to the console window
            // Debug.Log("Internet : " + m_ReachabilityText);
            return m_ReachabilityText;
        }

     
        public WIFISTREGTH SCGetWifiStrength()
        {
            strength = jo.Call<int>("GetWifiStrength", AndroidPluginBase.CurrentActivity);
            if (strength == 0)
            {
                _WIFIStrength = WIFISTREGTH.WIFI_NULL;
            }
            else if (strength == 1)
            {
                _WIFIStrength = WIFISTREGTH.WIFI_LOW;
            }
            else if (strength == 2)
            {
                _WIFIStrength = WIFISTREGTH.WIFI_MIDDLE;
            }
            else if (strength == 3)
            {
                _WIFIStrength = WIFISTREGTH.WIFI_HIGH;
            }
            else if (strength == 4)
            {
                _WIFIStrength = WIFISTREGTH.WIFI_HIGH;
            }
            else if (strength == -1)
            {
                _WIFIStrength = WIFISTREGTH.WIFI_UNKNOWN;
            }
            return _WIFIStrength;
        }


        public string SCGetMacAddress()//获取Mac地址
        {
            string physicalAddress = "";
            NetworkInterface[] nice = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adaper in nice)
            {

                if (adaper.Description == "en0")
                {
                    physicalAddress = adaper.GetPhysicalAddress().ToString();
                    break;
                }
                else
                {
                    physicalAddress = adaper.GetPhysicalAddress().ToString();
                    if (physicalAddress != "")
                    {
                        break;
                    };
                }
            }
            return physicalAddress;
        }

        public string SCGetIP()//获取本机IP地址
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adater in adapters)
            {
                if (adater.Supports(NetworkInterfaceComponent.IPv4))
                {
                    UnicastIPAddressInformationCollection UniCast = adater.GetIPProperties().UnicastAddresses;
                    if (UniCast.Count > 0)
                    {
                        foreach (UnicastIPAddressInformation uni in UniCast)
                        {
                            if (uni.Address.AddressFamily == AddressFamily.InterNetwork)
                            {

                                return uni.Address.ToString();
                            }
                        }
                    }
                }
            }
            return null;
        }

        public string SCGetNetSpeed()
        {
            return jo.Call<string>("GetNetSpeed");
        }
        public string SCGetNetWorkInfo()//文本显示
        {
            return "本机IP地址为：" + netIP + "\n Mac地址为：" + netmac + "\n 当前网络状态为：" + netStatus
                 + "\n 当前网速为：" + netSpeedStr + "\n 当前Wifi信号强度为：" + wifiStrength.ToString();

        }

        private IEnumerator GetNetSpeedAndstatus()//每经过1秒执行一次
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1f);
                SCGetNetStatus();
                netIP = SCGetIP();
                netmac = SCGetMacAddress();
                netStatus = SCGetNetStatus();
                netSpeedStr = SCGetNetSpeed();
                wifiStrength = SCGetWifiStrength();
            }
        }



        public bool SCIsConnectWifi()
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
