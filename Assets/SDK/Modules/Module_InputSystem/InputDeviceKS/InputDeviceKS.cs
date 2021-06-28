using AOT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceGC.KS {
    public class InputDeviceKS : InputDeviceGC {

        public override InputDeviceType inputDeviceType {
            get {
                return InputDeviceType.KS;
            }
        }

        private Coroutine callback=null;


        [Header("Enable GameController")]
        public bool LeftActive = true;
        public bool RightActive = true;
        protected override void InputDeviceStart() {
            int num = API_GSXR_Slam.SlamManager.plugin.GSXR_Get_ControllerNum();
            DebugMy.Log("GSXR_Get_ControllerNum:"+num,this,true);
            if (num == 2) {
                SetActiveInputDevicePart(InputDevicePartType.KSLeft, LeftActive);
                SetActiveInputDevicePart(InputDevicePartType.KSRight, RightActive);
            } else if (num == 1) {
                SetActiveInputDevicePart(InputDevicePartType.KSRight, RightActive);
            }
            if (callback == null) {
                callback = StartCoroutine(StartSetCallBack());
            }
        }

        public override void OnSCDisable() {
            base.OnSCDisable();
            if (callback != null) { StopCoroutine(StartSetCallBack()); callback = null; }
            SetCallBack(true);
        }
        IEnumerator StartSetCallBack() {
            yield return new WaitUntil(() => (API_GSXR_Slam.SlamManager.plugin != null && API_GSXR_Slam.SlamManager.IsRunning == true));
            SetCallBack();
            callback = null;
        }


        void SetCallBack(bool isSetNull = false) {
            if (API_GSXR_Slam.SlamManager.plugin == null)
                return;

            if (isSetNull == false) {
                DebugMy.Log("Controller SetCallBack", this, true);
                try {
                    API_GSXR_Slam.SlamManager.plugin.GSXR_Set_ControllerKeyEventCallback(KeyEvent);
                    API_GSXR_Slam.SlamManager.plugin.GSXR_Set_ControllerKeyTouchEventCallback(KeyTouchEvent);
                    API_GSXR_Slam.SlamManager.plugin.GSXR_Set_ControllerRockerCallback(JoystickEvent);
                    API_GSXR_Slam.SlamManager.plugin.GSXR_Set_ControllerHallCallback(HallEvent);
                    API_GSXR_Slam.SlamManager.plugin.GSXR_Set_ControllerChargingEventCallback(ChargingEvent);
                    API_GSXR_Slam.SlamManager.plugin.GSXR_Set_ControllerBatteryEventCallback(BatteryEvent);
                    API_GSXR_Slam.SlamManager.plugin.GSXR_Set_ControllerConnectEventCallback(ConnectEvent);
                    API_GSXR_Slam.SlamManager.plugin.GSXR_Set_ControllerTouchPanelCallback(OnTouchPanelEvent);
                } catch (Exception e) {
                    Debug.Log(e);
                }
            } else {
                DebugMy.Log("Controller SetCallBack Null", this, true);
                try {
                    API_GSXR_Slam.SlamManager.plugin.GSXR_Set_ControllerKeyEventCallback(null);
                    API_GSXR_Slam.SlamManager.plugin.GSXR_Set_ControllerKeyTouchEventCallback(null);
                    API_GSXR_Slam.SlamManager.plugin.GSXR_Set_ControllerRockerCallback(null);
                    API_GSXR_Slam.SlamManager.plugin.GSXR_Set_ControllerHallCallback(null);
                    API_GSXR_Slam.SlamManager.plugin.GSXR_Set_ControllerChargingEventCallback(null);
                    API_GSXR_Slam.SlamManager.plugin.GSXR_Set_ControllerBatteryEventCallback(null);
                    API_GSXR_Slam.SlamManager.plugin.GSXR_Set_ControllerConnectEventCallback(null);
                    API_GSXR_Slam.SlamManager.plugin.GSXR_Set_ControllerTouchPanelCallback(null);
                } catch (Exception e) {
                    Debug.Log(e);
                }
            }
        }

        [MonoPInvokeCallback(typeof(Action<int,int, int>))]
        public static void KeyEvent(int keycode, int action, int lr) {
            Debug.Log("KS -- key event: " + keycode + " " + action + " " + lr);
            InputDataGC.GCData.GCKeyList.Add(new GCKeyData() { keycode = keycode, keyevent = action, deivceID = lr });
        }


        [MonoPInvokeCallback(typeof(Action<int,bool,int>))]
        static void KeyTouchEvent(int keycode, bool touch, int lr) {
            Debug.Log("KS -- KeyTouchEvent:" + keycode + " " + touch + " " + lr);
            InputDataKS.KeyTouchDataList.Add(new InputDataKS.KeyTouchData() { keycode = keycode, touch = touch, deviceID = lr });
        }

        [MonoPInvokeCallback(typeof(Action<int,int, int>))]
        static void JoystickEvent(int touch_x, int touch_y, int lr) {
            Debug.Log("KS -- JoystickEvent:" + touch_x +" "+ touch_y + " " + lr);
            InputDataKS.TempJoystickDataList.Add(new InputDataKS.JoystickData() { JoystickX = touch_x, JoystickY = touch_y, deviceID = lr });
        }

        [MonoPInvokeCallback(typeof(Action<int,int, int>))]
        static void HallEvent(int hall_x, int hall_y, int lr) {
            Debug.Log("KS -- HallEvent:" + hall_x + " " + hall_y + " " + lr);
            InputDataKS.TempHallDataList.Add(new InputDataKS.HallData() { HallInside = hall_x, HallFoward = hall_y, deviceID = lr });
        }

        [MonoPInvokeCallback(typeof(Action<bool, int>))]
        static void ChargingEvent(bool isCharging, int lr) {
            Debug.Log("KS -- ChargingEvent:" + isCharging + " " + lr);
        }

        [MonoPInvokeCallback(typeof(Action<int, int>))]
        static void BatteryEvent(int battery, int lr) {
            Debug.Log("KS -- BatteryEvent:" + battery + " " + lr);
        }

        [MonoPInvokeCallback(typeof(Action<bool,int>))]
        static void ConnectEvent(bool isConnected, int lr) {
            Debug.Log("KS -- ConnectEvent:" + isConnected + " " + lr);
            InputDataKS.StatusDataList.Add(new InputDataKS.StatusData() { isConnected = isConnected, deviceID = lr });
        }

        [MonoPInvokeCallback(typeof(Action<float, float, int, int>))]
        static void OnTouchPanelEvent(float x, float y, int touch, int lr) {
            Debug.Log("KS -- OnTouchPanelEvent:" + x + " " + y + " " + touch + " " + lr);
            InputDataKS.TouchPanelDataList.Add(new InputDataKS.TouchPanelData() { x = x, y = y, deviceID = lr });
        }
    }
}
