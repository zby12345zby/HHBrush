using AOT;
using SC.XR.Unity.Module_Device;
using SC.XR.Unity.Module_InputSystem.InputDeviceGC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceHand.GGT26Dof
{
    public class InputDeviceGGT26Dof : InputDeviceHand
    {


        public override InputDeviceType inputDeviceType
        {
            get
            {
                return InputDeviceType.GGT26Dof;
            }
        }

        [HideInInspector]
        public float LowPowerPercent = 15;
        private Coroutine lowPowerCoroutine = null;

        bool isHandTrackStart = false;
        Coroutine startHand;

        [MonoPInvokeCallback(typeof(Action))]
        public static void GestureModelDataChangeCallback()
        {
            //Debug.Log("[04]GestureModelDataChangeCallback start");    

            //InputDeviceGGT26DofPart inputDeviceGGT26DofPart = (inputDevicePartList[0] as InputDeviceGGT26DofPart);
            //if (inputDeviceGGT26DofPart != null) {
            //    inputDeviceGGT26DofPart.inputDataGetGGT26Dof.inputDataGetHandsData.OnUpdateInputDataAndStore();
            //}
        }


        protected override void InputDeviceStart(){
            StartHandTrack();
        }

        public override void OnSCUpdate() {
            base.OnSCUpdate();

            if (isHandTrackStart) {
                API_GSXR_Slam.SlamManager.plugin.GSXR_Get_HandTrackingData(InputDataGGT26Dof.handsInfo.originDataMode, InputDataGGT26Dof.handsInfo.originDataPose);
            }
            
        }

        public override void OnSCLateUpdate()
        {
            base.OnSCLateUpdate();
            if (lowPowerTrigger)
            {
                DebugMy.Log("ScHANDTRACK_lowPowerTrigger", this, true);
                lowPowerTrigger = false;
                if (lowPowerCoroutine == null)
                {
                    lowPowerCoroutine = StartCoroutine(LowPowerFunction());
                }
            }
        }

        IEnumerator LowPowerFunction(float loopTime = 3)
        {
            StopHandTrack();

            SetActiveInputDevicePart(InputDevicePartType.HandLeft, false);
            SetActiveInputDevicePart(InputDevicePartType.HandRight, false);

            while (loopTime-- > 0)
            {
                if (inputDeviceUI as InputDeviceHandUI)
                {
                    (inputDeviceUI as InputDeviceHandUI).SetActiveUI(HandUIType.LOWPOWER, true);
                }
                yield return null;
                yield return new WaitForSeconds(12);
            }

            lowPowerCoroutine = null;
        }

        IEnumerator StartGreyHand()
        {
            yield return new WaitUntil(() => API_GSXR_Slam.SlamManager != null);
            yield return new WaitUntil(() => API_GSXR_Slam.SlamManager.IsRunning == true);


            //if (API_GSXR_Slam.SlamManager.plugin.GSXR_Is_SupportHandTracking() == false) {
            //    DebugMy.Log("Not Support HandTracking",this,true);
            //    yield break;
            //}


            if (API_Module_Device.Current.BatteryLevel < LowPowerPercent) {
                if (lowPowerCoroutine == null) {
                    lowPowerCoroutine = StartCoroutine(LowPowerFunction());
                }
                DebugMy.Log("BatteryLevel < " + LowPowerPercent + " StopHandTrack", this, true);
                yield break;
            }

            PermissionRequest.getInstance.GetPerssion(UnityEngine.Android.Permission.ExternalStorageRead);
            PermissionRequest.getInstance.GetPerssion(UnityEngine.Android.Permission.ExternalStorageWrite);

            if (isHandTrackStart == false) {
                isHandTrackStart = true;
                DebugMy.Log("GSXR_StartHandTracking", this, true);

                if (Application.platform == RuntimePlatform.Android) {
                    try {
                        API_GSXR_Slam.SlamManager.plugin.GSXR_StartHandTracking(LowPowerWarningCallback);
                        API_GSXR_Slam.SlamManager.plugin.GSXR_Set_HandTrackingCallBack(GestureChangeCallback);
                        API_GSXR_Slam.SlamManager.plugin.GSXR_Set_HandTrackingModelDataCallBack(GestureModelDataChangeCallback);
                    } catch (Exception e) {
                        Debug.Log(e);
                    }
                }
            }
            startHand = null;

            base.InputDeviceStart();

        }

        public override void OnSCDisable()
        {
            base.OnSCDisable();
            StopHandTrack();
        }

        private void StartHandTrack() {
            if (startHand == null) {
                startHand = StartCoroutine(StartGreyHand());
            }
        }

        private void StopHandTrack() {

            if (startHand != null) {
                StopCoroutine(startHand);
                startHand = null;
            }
            if (isHandTrackStart) {
                isHandTrackStart = false;
                DebugMy.Log("GSXR_StopHandTracking", this, true);
                if (Application.platform == RuntimePlatform.Android) {
                    try {
                        API_GSXR_Slam.SlamManager.plugin.GSXR_StopHandTracking();
                    } catch (Exception e) {
                        Debug.Log(e);
                    }
                }
            }
        }

        //{1, "THUMB"},
        //{2, "ONE"},
        //{3, "TWO"},
        //{4, "THREE"},
        //{5, "FOUR"},
        //{6, "FIVE"},
        //{7, "OK"},
        //{8, "DIRECTION"},
        [MonoPInvokeCallback(typeof(Action<int>))]
        public static void GestureChangeCallback(int gesture)
        {
            Debug.Log("GestureChangeCallback, gesture id:" + gesture);
        }

        static bool lowPowerTrigger = false;
        [MonoPInvokeCallback(typeof(Action<int>))]
        public static void LowPowerWarningCallback(int power)
        {
            Debug.Log("HandGesture Cannot work in low power state:" + power);
            lowPowerTrigger = true;
        }

    }
}
