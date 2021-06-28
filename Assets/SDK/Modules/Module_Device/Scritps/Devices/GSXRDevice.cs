
using SC.XR.Unity.Module_InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SC.XR.Unity.Module_Device {

    public class GSXRDevice {

        public GSXRDevice() {

            int fishEyeNum = 0;
            int fishEyeW = 0;
            int fishEyeH = 0;
            API_GSXR_Slam.SlamManager.plugin.GSXR_Get_FishEyeInfo(ref fishEyeNum, ref fishEyeW, ref fishEyeH);
            FishEyeNum = fishEyeNum;
            FishEyeResolution = new Vector2(fishEyeW, fishEyeH);

            SN = API_GSXR_Slam.SlamManager.plugin.SN;
            RELEASE_VERSION = API_GSXR_Slam.SlamManager.plugin.RELEASE_VERSION;

            DeviceName = API_GSXR_Slam.SlamManager.plugin.GSXR_Get_DeviceName();

            XRType = API_GSXR_Slam.SlamManager.plugin.GSXR_Get_XRType();

            IsSupportController = API_GSXR_Slam.SlamManager.plugin.GSXR_Is_SupportController();
            IsSupportHandTracking = API_GSXR_Slam.SlamManager.plugin.GSXR_Is_SupportHandTracking();
            IsSupportMap = API_GSXR_Slam.SlamManager.plugin.GSXR_Support_Map();
            IsSupportPanel = API_GSXR_Slam.SlamManager.plugin.GSXR_Support_Panel();
            IsSupportOpticsCalibration = API_GSXR_Slam.SlamManager.plugin.GSXR_Is_SupportOpticsCalibration();
            IsSupportEyeTracking = API_GSXR_Slam.SlamManager.plugin.GSXR_Support_EyeTracking();

        }

        public bool IsSupportController { get; private set; }
        public bool IsSupportHandTracking { get; private set; }
        public bool IsSupportMap { get; private set; }
        public bool IsSupportPanel { get; private set; }
        public bool IsSupportOpticsCalibration { get; private set; }
        public bool IsSupportEyeTracking { get; private set; }

        protected List<InputDeviceType> m_SupportInputDevices = null;
        public List<InputDeviceType> SupportInputDevices {
            get {
                if (m_SupportInputDevices == null) {
                    m_SupportInputDevices = new List<InputDeviceType>();

                    m_SupportInputDevices.Add(InputDeviceType.Head);

                    if (API_GSXR_Slam.SlamManager.plugin.GSXR_Is_SupportController() == true) {
                        m_SupportInputDevices.Add(InputDeviceType.KS);
                    }

                    if (API_GSXR_Slam.SlamManager.plugin.GSXR_Is_SupportHandTracking() == true) {
                        m_SupportInputDevices.Add(InputDeviceType.GGT26Dof);
                    }
                }

                return m_SupportInputDevices;
            }
        }

        public int FishEyeNum { get; private set; }
        public Vector2 FishEyeResolution { get; private set; }
        public string SN { get; private set; }
        public string RELEASE_VERSION { get; private set; }
        public XRType XRType { get; private set; }
        public string DeviceName { get; private set; }
        public int BatteryLevel {
            get => API_GSXR_Slam.SlamManager.plugin.BatteryLevel;
        }



        public virtual void ShowInfo() {
            DebugMy.Log("GSXRDevice:"
                        + "  XRType:" + XRType
                        + "  DeviceName:" + DeviceName
                        + "  SN:" + SN
                        + "  RELEASE_VERSION:" + RELEASE_VERSION

                        + "  IsSupportController:" + IsSupportController
                        + "  IsSupportHandTracking:" + IsSupportHandTracking
                        + "  IsSupportMap:" + IsSupportMap
                        + "  IsSupportPanel:" + IsSupportPanel
                        + "  IsSupportOpticsCalibration:" + IsSupportOpticsCalibration
                        + "  IsSupportEyeTracking:" + IsSupportEyeTracking

                        + "  BatteryLevel:" + BatteryLevel
                        + "  FishEyeNum:" + FishEyeNum
                        + "  FishEyeResolution:" + FishEyeResolution
                        , this, true);

        }

    }


}