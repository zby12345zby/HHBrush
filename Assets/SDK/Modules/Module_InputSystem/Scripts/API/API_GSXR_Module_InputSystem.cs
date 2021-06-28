using System;
using UnityEngine;
using SC.XR.Unity.Module_InputSystem;
using System.Security.AccessControl;

public class API_GSXR_Module_InputSystem
{

    ///API-No.50
    /// <summary>
    /// 获取Module_InputSystem的单例
    /// </summary>
    /// <returns></returns>
    public static Module_InputSystem GSXR_Get_Instance()
    {
        return Module_InputSystem.instance;
    }

    ///API-No.51
    /// <summary>
    /// Module_InputSystem是否初始化完成
    /// </summary>
    /// <returns>true 表示初始化完成，反之</returns>
    public static bool GSXR_Is_ISInitialized()
    {
        if (Module_InputSystem.instance)
        {
            return Module_InputSystem.instance.initialize;
        }
        return false;
    }

    ///API-No.52
    /// <summary>
    /// 设置Module_InputSystem初始化完成时的回调
    /// </summary>
    /// <param name="action">委托的方法</param>
    public static void GSXR_Add_InitializedCallBack(Action action)
    {
        Module_InputSystem.instance.initializeCallBack += action;
    }

    ///API-No.53
    public static void GSXR_Remove_InitializedCallBack(Action action)
    {
        Module_InputSystem.instance.initializeCallBack -= action;
    }

    ///API-No.54
    /// <summary>
    /// 使能某个输入设备，支持的输入设备见InputDeviceType
    /// </summary>
    /// <param name="inputDevice">输入设备</param>
    public static void GSXR_Enable_InputDevice(InputDeviceType inputDevice)
    {
        if (Module_InputSystem.instance)
        {
            Module_InputSystem.instance.SetActiveInputDevice(inputDevice, true);
        }
    }

    ///API-No.55
    /// <summary>
    /// 关闭某个输入设备，支持的输入设备见InputDeviceType
    /// </summary>
    /// <param name="inputDevice">输入设备</param>
    public static void GSXR_Disable_InputDevice(InputDeviceType inputDevice)
    {
        if (Module_InputSystem.instance)
        {
            Module_InputSystem.instance.SetActiveInputDevice(inputDevice, false);
        }
    }

    ///API-No.56
    ///监听某个按键事件方式1
    ///usingusing SC.XR.Unity.Module_InputSystem;  然后继承PointerHandlers类并重写需要的方法
    ///支持的事件有：
    ///OnPointerExit, OnPointerEnter, OnPointerDown,OnPointerClick,OnPointerUp, OnDrag


    ///API-No.57
    ///监听某个按键事件方式2
    ///using SC.XR.Unity.Module_InputSystem;  然后继承PointerDelegate类然后重写对应的事件
    ///支持的事件有：
    ///KeyUpDelegateUnRegister KeyUpDelegateRegister KeyLongDelegateRegister 
    ///KeyLongDelegateUnRegister KeyDownDelegateUnRegister KeyDownDelegateRegister


    public static void GSXR_KeyDownDelegateRegister(AnyKeyEventDelegate keyDownEventDelegate)
    {
        DispatcherBase.KeyDownDelegateRegister(keyDownEventDelegate);
    }
    public static void GSXR_KeyDownDelegateUnRegister(AnyKeyEventDelegate keyDownEventDelegate)
    {
        DispatcherBase.KeyDownDelegateUnRegister(keyDownEventDelegate);
    }
    public static void GSXR_KeyUpDelegateRegister(AnyKeyEventDelegate keyUpEventDelegate)
    {
        DispatcherBase.KeyUpDelegateRegister(keyUpEventDelegate);
    }
    public static void GSXR_KeyUpDelegateUnRegister(AnyKeyEventDelegate keyUpEventDelegate)
    {
        DispatcherBase.KeyUpDelegateUnRegister(keyUpEventDelegate);
    }
    public static void GSXR_KeyLongDelegateRegister(AnyKeyEventDelegate keyLongEventDelegate)
    {
        DispatcherBase.KeyLongDelegateRegister(keyLongEventDelegate);
    }
    public static void GSXR_KeyLongDelegateUnRegister(AnyKeyEventDelegate keyLongEventDelegate)
    {
        DispatcherBase.KeyLongDelegateUnRegister(keyLongEventDelegate);
    }



    ///API-No.58
    /// <summary>
    /// 输入设备检测的目标，优先级为Head/BTRight/BTLeft/GTRight/GTLeft/GGTRight/GGLeft
    /// </summary>
    [Obsolete("This method will be removed in the new version, please use other interfaces to obtain")]
    public static GameObject GSXR_Target
    {
        get
        {
            if (API_GSXR_Module_InputSystem_Head.GSXR_Head != null)
            {
                return API_GSXR_Module_InputSystem_Head.GSXR_HeadHitTarget;
            }
            else if (API_GSXR_Module_InputSystem_BT3Dof.GSXR_BTRight != null)
            {
                return API_GSXR_Module_InputSystem_BT3Dof.GSXR_BTHitTarget(API_GSXR_Module_InputSystem_BT3Dof.BTType.Right);
            }
            else if (API_GSXR_Module_InputSystem_BT3Dof.GSXR_BTLeft != null)
            {
                return API_GSXR_Module_InputSystem_BT3Dof.GSXR_BTHitTarget(API_GSXR_Module_InputSystem_BT3Dof.BTType.Left);
            }
            else if (API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTRight != null)
            {
                return API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTHitTarget(API_GSXR_Module_InputSystem_GGT26Dof.GGestureType.Right);
            }
            else if (API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTLeft != null)
            {
                return API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTHitTarget(API_GSXR_Module_InputSystem_GGT26Dof.GGestureType.Left);
            }
            else if (API_GSXR_Module_InputSystem_KS.GSXR_KSRight != null)
            {
                return API_GSXR_Module_InputSystem_KS.GSXR_KSHitTarget(API_GSXR_Module_InputSystem_KS.GCType.Right);
            }
            else if (API_GSXR_Module_InputSystem_KS.GSXR_KSLeft != null)
            {
                return API_GSXR_Module_InputSystem_KS.GSXR_KSHitTarget(API_GSXR_Module_InputSystem_KS.GCType.Left);
            }
            return null;
        }
    }

    ///API-No.59
    /// <summary>
    /// 输入设备的发射射线起点，优先级为Head/BTRight/BTLeft/GTRight/GTLeft/GGTRight/GGLeft
    /// </summary>
    [Obsolete("This method will be removed in the new version, please use other interfaces to obtain")]
    public static GameObject GSXR_Gazer
    {
        get
        {
            if (API_GSXR_Module_InputSystem_Head.GSXR_Head != null)
            {
                return API_GSXR_Module_InputSystem_Head.GSXR_Head.detectorBase.pointerBase.gameObject;
            }
            else if (API_GSXR_Module_InputSystem_BT3Dof.GSXR_BTRight != null)
            {
                return API_GSXR_Module_InputSystem_BT3Dof.GSXR_BTRight.bT3DofDetector.pointerBase.gameObject;
            }
            else if (API_GSXR_Module_InputSystem_BT3Dof.GSXR_BTLeft != null)
            {
                return API_GSXR_Module_InputSystem_BT3Dof.GSXR_BTLeft.bT3DofDetector.pointerBase.gameObject;
            }
            else if (API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTRight != null)
            {
                return API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTRight.detectorBase.pointerBase.gameObject;
            }
            else if (API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTLeft != null)
            {
                return API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTLeft.detectorBase.pointerBase.gameObject;
            }
            else if (API_GSXR_Module_InputSystem_KS.GSXR_KSRight != null)
            {
                return API_GSXR_Module_InputSystem_KS.GSXR_KSRight.detectorBase.pointerBase.gameObject;
            }
            else if (API_GSXR_Module_InputSystem_KS.GSXR_KSLeft != null)
            {
                return API_GSXR_Module_InputSystem_KS.GSXR_KSLeft.detectorBase.pointerBase.gameObject;
            }
            return null;
        }
    }


    ///API-No.60
    /// <summary>
    /// 输入设备的发射射线方向，优先级为Head/BTRight/BTLeft/GTRight/GTLeft/GGTRight/GGLeft
    /// </summary>
    public static Vector3 GSXR_Normal
    {
        get
        {
            if (GSXR_Gazer != null)
            {
                return GSXR_Gazer.transform.forward;
            }
            return Vector3.zero;
        }
    }

    ///API-No.61
    /// <summary>
    /// 输入设备Cursor的位置，优先级为Head/BTRight/BTLeft/GTRight/GTLeft/GGTRight/GGLeft
    /// </summary>
    [Obsolete("This method will be removed in the new version, please use other interfaces to obtain")]
    public static Vector3 GSXR_Position
    {
        get
        {
            if (API_GSXR_Module_InputSystem_Head.GSXR_Head != null)
            {
                return API_GSXR_Module_InputSystem_Head.GSXR_Get_HeadCursor.transform.position;
            }
            else if (API_GSXR_Module_InputSystem_BT3Dof.GSXR_BTRight != null)
            {
                return API_GSXR_Module_InputSystem_BT3Dof.GSXR_Get_BTCursor(API_GSXR_Module_InputSystem_BT3Dof.BTType.Right).transform.position;
            }
            else if (API_GSXR_Module_InputSystem_BT3Dof.GSXR_BTLeft != null)
            {
                return API_GSXR_Module_InputSystem_BT3Dof.GSXR_Get_BTCursor(API_GSXR_Module_InputSystem_BT3Dof.BTType.Left).transform.position;
            }
            else if (API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTRight != null)
            {
                return API_GSXR_Module_InputSystem_GGT26Dof.GSXR_Get_GGTCursor(API_GSXR_Module_InputSystem_GGT26Dof.GGestureType.Right).transform.position;
            }
            else if (API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTLeft != null)
            {
                return API_GSXR_Module_InputSystem_GGT26Dof.GSXR_Get_GGTCursor(API_GSXR_Module_InputSystem_GGT26Dof.GGestureType.Left).transform.position;
            }
            else if (API_GSXR_Module_InputSystem_KS.GSXR_KSRight != null)
            {
                return API_GSXR_Module_InputSystem_KS.GSXR_Get_KSCursor(API_GSXR_Module_InputSystem_KS.GCType.Right).transform.position;
            }
            else if (API_GSXR_Module_InputSystem_KS.GSXR_KSLeft != null)
            {
                return API_GSXR_Module_InputSystem_KS.GSXR_Get_KSCursor(API_GSXR_Module_InputSystem_KS.GCType.Left).transform.position;
            }
            return Vector3.zero;
        }
    }

    ///API-No.62
    /// <summary>
    /// 获取当前的具体输入设备，优先级为Head/BTRight/BTLeft/GTRight/GTLeft/GGTRight/GGLeft
    /// </summary>
    [Obsolete("This method will be removed in the new version, please use other interfaces to obtain")]
    public static InputDevicePartBase GSXR_InputDeviceCurrent
    {
        get
        {
            if (API_GSXR_Module_InputSystem_Head.GSXR_Head != null)
            {
                return API_GSXR_Module_InputSystem_Head.GSXR_Head;
            }
            else if (API_GSXR_Module_InputSystem_BT3Dof.GSXR_BTRight != null)
            {
                return API_GSXR_Module_InputSystem_BT3Dof.GSXR_BTRight;
            }
            else if (API_GSXR_Module_InputSystem_BT3Dof.GSXR_BTLeft != null)
            {
                return API_GSXR_Module_InputSystem_BT3Dof.GSXR_BTLeft;
            }
            else if (API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTRight != null)
            {
                return API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTRight;
            }
            else if (API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTLeft != null)
            {
                return API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTLeft;
            }
            else if (API_GSXR_Module_InputSystem_KS.GSXR_KSRight != null)
            {
                return API_GSXR_Module_InputSystem_KS.GSXR_KSRight;
            }
            else if (API_GSXR_Module_InputSystem_KS.GSXR_KSLeft != null)
            {
                return API_GSXR_Module_InputSystem_KS.GSXR_KSLeft;
            }
            return null;
        }
    }

    public static bool GSXR_InputDeviceStatus(InputDeviceType deviceType)
    {
        if (Module_InputSystem.instance)
        {
            return Module_InputSystem.instance.GetInputDeviceStatus(deviceType);
        }
        return false;
    }

}
