using SC.XR.Unity.Module_InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static API_Module_InputSystem_KS;
using UnityEngine.EventSystems;

namespace TiltBrush
{
    public class MRControllerInfo : ControllerInfo
    {
        GCType _gcType;

        private readonly float kThumbstickDeadzone = 0.075f;
        private const float kWmrThumbstickDeadzone = 0.1f;

        public MRControllerInfo(BaseControllerBehavior behavior, bool isLeftHand)
          : base(behavior)
        {
            if (isLeftHand)
            {
                _gcType = GCType.Left;
            }
            else
            {
                _gcType = GCType.Right;
            }

        }

        public override bool IsTrackedObjectValid { get { return true; } set { } }


        /// <summary>
        /// 二级触发器按下的值
        /// </summary>
        /// <returns></returns>
        public override float GetGripValue()
        {
            return IsKSKey(InputKeyCode.LHallInside, _gcType) ? 1f : 0f;
        }


        /// <summary>
        /// LogicPen才会用到，所以在这里暂且不用设置
        /// </summary>
        /// <returns></returns>
        public override Vector2 GetPadValue()
        {
            return Vector2.zero;
        }


        /// <summary>
        /// 源代码用于GVR设备，因此在这里无需设置
        /// </summary>
        /// <returns></returns>
        public override Vector2 GetPadValueDelta()
        {
            return Vector2.zero;
        }

        public override float GetScrollXDelta()
        {
            Vector2 _temp = GetKSJoyStickValue(_gcType);
            if (_temp.x < 8)
            {
                _temp.x = -Mathf.InverseLerp(0, 7, _temp.x);
            }
            else if (_temp.x == 8)
            {
                _temp.x = 0;
            }
            else
            {
                _temp.x = Mathf.InverseLerp(9, 15, _temp.x);
            }
            return _temp.x;
        }

        public override float GetScrollYDelta()
        {
            if (_temp.y < 8)
            {
                _temp.y = -Mathf.InverseLerp(0, 7, _temp.y);
            }
            else if (_temp.y == 8)
            {
                _temp.y = 0;
            }
            else
            {
                _temp.y = Mathf.InverseLerp(9, 15, _temp.y);
            }
            return _temp.y;
        }


        /// <summary>
        /// 返回摇杆的触发值()
        /// </summary>
        /// <returns></returns>
        public override Vector2 GetThumbStickValue()
        {
            Vector2 _temp = GetKSJoyStickValue(_gcType);
            if (_temp.x < 8)
            {
                _temp.x = -Mathf.InverseLerp(0, 7, _temp.x);
            }
            else if (_temp.x == 8)
            {
                _temp.x = 0;
            }
            else
            {
                _temp.x = Mathf.InverseLerp(9, 15, _temp.x);
            }


            if (_temp.y < 8)
            {
                _temp.y = -Mathf.InverseLerp(0, 7, _temp.y);
            }
            else if (_temp.y == 8)
            {
                _temp.y = 0;
            }
            else
            {
                _temp.y = Mathf.InverseLerp(9, 15, _temp.y);
            }

            return new Vector2(DeadStickMap(_temp.x), DeadStickMap(_temp.y));
        }

        /// <summary>
        /// 一级触发按键按下的比率
        /// </summary>
        /// <returns></returns>
        public override float GetTriggerRatio()
        {
            //if (_gcType == GCType.Right)
            //{
            //    return IsKSKey(InputKeyCode.A, _gcType) ? 1f : 0;
            //}
            //else
            //{
            //    return IsKSKey(InputKeyCode.Trigger, _gcType) ? 1f : 0;
            //}
            return ViewpointScript.m_Instance.IsMREnter ? 1f : 0;
        }

        /// <summary>
        /// 一级触发按键按下的值
        /// </summary>
        /// <returns></returns>
        public override float GetTriggerValue()
        {
            if (_gcType == GCType.Right)
            {
                return IsKSKey(InputKeyCode.A, _gcType) ? 1f : 0;
            }
            else
            {
                return IsKSKey(InputKeyCode.Trigger, _gcType) ? 1f : 0;
            }
        }


        bool _isThumbstick = true;
        private bool GetVrInputForFrame(VrInput input, bool currentFrame)
        {
            switch (input)
            {
                case VrInput.Button01:
                    //if (currentFrame)
                    //{
                    //    if (_gcType == GCType.Left)
                    //    {
                    //        return IsKSKey(InputKeyCode.X, _gcType);

                    //    }
                    //    else
                    //    {
                    //        //return IsKSKey(InputKeyCode.A, _gcType);
                    //        return false;
                    //    }
                    //}
                    //else
                    //{
                    //    if (_gcType == GCType.Left)
                    //    {
                    //        return IsKSKeyDown(InputKeyCode.X, _gcType);
                    //    }
                    //    else
                    //    {
                    //        //return IsKSKeyDown(InputKeyCode.A, _gcType);
                    //        return false;
                    //    }
                    //}
                    return false;


                case VrInput.Button02:
                    //if (currentFrame)
                    //{
                    //    if (_gcType == GCType.Left)
                    //    {
                    //        return IsKSKey(InputKeyCode.Y, _gcType);
                    //    }
                    //    else
                    //    {
                    //        return IsKSKey(InputKeyCode.B, _gcType);
                    //    }
                    //}
                    //else
                    //{
                    //    if (_gcType == GCType.Left)
                    //    {
                    //        return IsKSKeyDown(InputKeyCode.Y, _gcType);

                    //    }
                    //    else
                    //    {
                    //        return IsKSKeyDown(InputKeyCode.B, _gcType);
                    //    }
                    //}
                    return false;


                case VrInput.Button03:
                    //if (currentFrame)
                    //{
                    //    if (_gcType == GCType.Left)
                    //    {
                    //        return IsKSKey(InputKeyCode.Y, _gcType);
                    //    }
                    //    else
                    //    {
                    //        return IsKSKey(InputKeyCode.B, _gcType);
                    //    }
                    //}
                    //else
                    //{
                    //    if (_gcType == GCType.Left)
                    //    {
                    //        return IsKSKeyDown(InputKeyCode.Y, _gcType);

                    //    }
                    //    else
                    //    {
                    //        return IsKSKeyDown(InputKeyCode.B, _gcType);
                    //    }
                    //}
                    return false;


                case VrInput.Button04:
                    //if (currentFrame)
                    //{
                    //    if (_gcType == GCType.Left)
                    //    {
                    //        return IsKSKey(InputKeyCode.X, _gcType);
                    //    }
                    //    else
                    //    {
                    //        //return IsKSKey(InputKeyCode.A, _gcType);
                    //        return false;
                    //    }
                    //}
                    //else
                    //{
                    //    if (_gcType == GCType.Left)
                    //    {
                    //        return IsKSKeyDown(InputKeyCode.X, _gcType);
                    //    }
                    //    else
                    //    {
                    //        //return IsKSKeyDown(InputKeyCode.A, _gcType);
                    //        return false;
                    //    }
                    //}
                    return false;


                case VrInput.Button05:
                    //if (currentFrame)
                    //{
                    //    if (_gcType == GCType.Left)
                    //    {
                    //        return IsKSKey(InputKeyCode.Y, _gcType);
                    //    }
                    //    else
                    //    {
                    //        return IsKSKey(InputKeyCode.B, _gcType);
                    //    }
                    //}
                    //else
                    //{
                    //    if (_gcType == GCType.Left)
                    //    {
                    //        return IsKSKeyDown(InputKeyCode.Y, _gcType);

                    //    }
                    //    else
                    //    {
                    //        return IsKSKeyDown(InputKeyCode.B, _gcType);
                    //    }
                    //}
                    return false;

                case VrInput.Button06:
                    //if (currentFrame)
                    //{
                    //    if (_gcType == GCType.Left)
                    //    {
                    //        return IsKSKey(InputKeyCode.X, _gcType);
                    //    }
                    //    else
                    //    {
                    //        //return IsKSKey(InputKeyCode.A, _gcType);
                    //        return false;
                    //    }
                    //}
                    //else
                    //{
                    //    if (_gcType == GCType.Left)
                    //    {
                    //        return IsKSKeyDown(InputKeyCode.X, _gcType);
                    //    }
                    //    else
                    //    {
                    //        //return IsKSKeyDown(InputKeyCode.A, _gcType);
                    //        return false;
                    //    }
                    //}
                    return false;


                case VrInput.Directional:
                    if (_isThumbstick)
                    {
                        goto case VrInput.Thumbstick;
                    }
                    else
                    {
                        goto case VrInput.Touchpad;
                    }

                case VrInput.Trigger:
                    //if (currentFrame)
                    //{
                    //    if (_gcType == GCType.Left)
                    //    {
                    //        return IsKSKey(InputKeyCode.Trigger, _gcType);
                    //    }
                    //    else
                    //    {
                    //        return IsKSKey(InputKeyCode.A, _gcType);
                    //    }
                    //}
                    //else
                    //{
                    //    if (_gcType == GCType.Left)
                    //    {
                    //        return IsKSKeyDown(InputKeyCode.Trigger, _gcType);
                    //    }
                    //    else
                    //    {
                    //        return IsKSKeyDown(InputKeyCode.A, _gcType);
                    //    }
                    //}

                    return ViewpointScript.m_Instance.IsMREnter;



                case VrInput.Grip:
                    //if (currentFrame)
                    //{
                    //    if (_gcType == GCType.Left)
                    //    {
                    //        return IsKSKey(InputKeyCode.LHallInside, _gcType);
                    //    }
                    //    else
                    //    {
                    //        return IsKSKey(InputKeyCode.RHallInside, _gcType);
                    //    }
                    //}
                    //else
                    //{
                    //    if (_gcType == GCType.Left)
                    //    {
                    //        return IsKSKeyDown(InputKeyCode.LHallInside, _gcType);

                    //    }
                    //    else
                    //    {
                    //        return IsKSKeyDown(InputKeyCode.RHallInside, _gcType);

                    //    }
                    //}
                    return false;


                case VrInput.Any:
                    return false;

                case VrInput.Thumbstick:
                    //if (currentFrame)
                    //{
                    //    return GetKSJoyStickValue().sqrMagnitude > 0;
                    //}
                    //else
                    //{
                    //    if (_gcType == GCType.Left)
                    //    {
                    //        return IsKSKeyDown(InputKeyCode.LjoystickKey, _gcType);
                    //    }
                    //    else
                    //    {
                    //        return IsKSKeyDown(InputKeyCode.RjoystickKey, _gcType);
                    //    }
                    //}
                    return false;
                case VrInput.Touchpad:
                    return false;
            }
            return false;
        }


        public override bool GetVrInput(VrInput input)
        {
            return GetVrInputForFrame(input, currentFrame: true);
        }


        public override bool GetVrInputDown(VrInput input)
        {
            return GetVrInputForFrame(input, currentFrame: false);
        }


        public override bool GetVrInputTouch(VrInput input)
        {
            //throw new System.NotImplementedException();
            return false;
        }


        public override void TriggerControllerHaptics(float seconds)
        {
            //throw new System.NotImplementedException();

        }


        private float DeadStickMap(float x)
        {
            float deadZone = kWmrThumbstickDeadzone;
            float sign = Mathf.Sign(x);
            x = Mathf.Clamp01(Mathf.Abs(x) - deadZone);
            x /= (1f - deadZone);
            return x * sign;
        }




        public static bool IsKSKeyDown(InputKeyCode inputKeyCode, GCType type)
        {
            if (API_Module_InputSystem_KS.KSRight && type == GCType.Right)
            {
                return API_Module_InputSystem_KS.KSRight.inputDataKS.inputKeys.GetKeyDown(inputKeyCode);
            }
            else if (API_Module_InputSystem_KS.KSLeft && type == GCType.Left)
            {
                return API_Module_InputSystem_KS.KSLeft.inputDataKS.inputKeys.GetKeyDown(inputKeyCode);
                //return false;
            }
            return false;
        }

        public static bool IsKSKey(InputKeyCode inputKeyCode, GCType type)
        {
            if (API_Module_InputSystem_KS.KSRight && type == GCType.Right)
            {
                var _right = API_Module_InputSystem_KS.KSRight.inputDataKS.inputKeys.GetKeyCurrentState(inputKeyCode);
                switch (_right)
                {
                    case InputKeyState.DOWN:
                        return true;
                    case InputKeyState.LONG:
                        return true;
                    case InputKeyState.UP:
                        return false;
                    case InputKeyState.Null:
                        return false;
                }
            }
            else if (API_Module_InputSystem_KS.KSLeft && type == GCType.Left)
            {
                var _left = API_Module_InputSystem_KS.KSLeft.inputDataKS.inputKeys.GetKeyCurrentState(inputKeyCode);
                switch (_left)
                {
                    case InputKeyState.DOWN:
                        return true;
                    case InputKeyState.LONG:
                        return true;
                    case InputKeyState.UP:
                        return false;
                    case InputKeyState.Null:
                        return false;
                }
            }
            return false;
        }


        static Vector2 _temp = Vector2.zero;
        public static Vector2 GetKSJoyStickValue(GCType type = GCType.Right)
        {
            if (API_Module_InputSystem_KS.KSRight && type == GCType.Right)
            {
                _temp.x = KSRight.inputDataKS.JoystickX;
                _temp.y = KSRight.inputDataKS.JoystickY;
            }
            else if (API_Module_InputSystem_KS.KSLeft && type == GCType.Left)
            {
                _temp.x = KSLeft.inputDataKS.JoystickX;
                _temp.y = KSLeft.inputDataKS.JoystickY;
            }
            return _temp;
        }
    }
}
