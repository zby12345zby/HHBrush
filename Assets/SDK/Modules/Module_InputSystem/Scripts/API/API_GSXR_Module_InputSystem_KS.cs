using System;
using UnityEngine;
using SC.XR.Unity.Module_InputSystem;
using SC.XR.Unity.Module_InputSystem.InputDeviceGC.KS;
using UnityEngine.EventSystems;

public class API_GSXR_Module_InputSystem_KS
{


    /// <summary>
    /// 获取InputSystem支持的游戏控制器输入设备,游戏控制器输入设备包含二个Part，名曰：KSRight,KSLeft,也就是第一个游戏控制器和第二个游戏控制器
    /// </summary>
    /// <returns>null表示不支持或者InputSystem未初始好</returns>
    public static InputDeviceKS GSXR_KSDevice
    {
        get
        {
            if (Module_InputSystem.instance)
            {
                return Module_InputSystem.instance.GetInputDevice<InputDeviceKS>(InputDeviceType.KS);
            }
            return null;
        }
    }

    /// <summary>
    /// 游戏控制器输入设备连接的第一个游戏控制器
    /// </summary>
    public static InputDeviceKSPart GSXR_KSRight
    {
        get
        {
            if (GSXR_KSDevice && GSXR_KSDevice.inputDevicePartList.Count > 0)
            {
                foreach (var part in GSXR_KSDevice.inputDevicePartList)
                {
                    if (part.PartType == InputDevicePartType.KSRight)
                    {
                        return part as InputDeviceKSPart;
                    }
                }
            }

            return null;
        }
    }


    /// <summary>
    /// 游戏控制器输入设备连接的第二个游戏控制器
    /// </summary>
    public static InputDeviceKSPart GSXR_KSLeft
    {
        get
        {
            if (GSXR_KSDevice && GSXR_KSDevice.inputDevicePartList.Count > 0)
            {
                foreach (var part in GSXR_KSDevice.inputDevicePartList)
                {
                    if (part.PartType == InputDevicePartType.KSLeft)
                    {
                        return part as InputDeviceKSPart;
                    }
                }
            }
            return null;
        }
    }

    public enum GCType
    {
        Left,
        Right
    }


    /// <summary>
    /// KSRight/KSLeft的四元数，全局坐标
    /// </summary>
    /// <param name="type">右游戏控制器 KSRight /  左游戏控制器 KSLeft</param>
    /// <returns></returns>
    public static Quaternion GSXR_KSRotation(GCType type = GCType.Right)
    {
        if (GSXR_KSRight && type == GCType.Right)
        {
            return GSXR_KSRight.inputDataKS.rotation;
        }
        else if (GSXR_KSLeft && type == GCType.Left)
        {
            return GSXR_KSLeft.inputDataKS.rotation;
        }
        return Quaternion.identity;
    }


    /// <summary>
    /// KSRight/KSLeft的位置信息，全局坐标
    /// </summary>
    /// <param name="type">右游戏控制器 KSRight /  左游戏控制器 BKSLeft</param>
    /// <returns></returns>
    public static Vector3 GSXR_KSPosition(GCType type = GCType.Right)
    {
        if (GSXR_KSRight && type == GCType.Right)
        {
            return GSXR_KSRight.inputDataKS.position;
        }
        else if (GSXR_KSLeft && type == GCType.Left)
        {
            return GSXR_KSLeft.inputDataKS.position;
        }
        return Vector3.zero;
    }

    public static Transform KSTransform(GCType type = GCType.Right)
    {
        if (GSXR_KSRight && type == GCType.Right)
        {
            return GSXR_KSRight.inputDeviceKSPartUI.ModelGC.transform;
        }
        else if (GSXR_KSLeft && type == GCType.Left)
        {
            return GSXR_KSLeft.inputDeviceKSPartUI.ModelGC.transform;
        }
        return null;
    }

    public static string GSXR_KSName(GCType type = GCType.Right)
    {
        if (GSXR_KSRight && type == GCType.Right)
        {
            return GSXR_KSRight.inputDataKS.GCName;
        }
        else if (GSXR_KSLeft && type == GCType.Left)
        {
            return GSXR_KSLeft.inputDataKS.GCName;
        }
        return "";
    }

    /// <summary>
    /// KSRight/KSLeft 检测到碰撞信息集合，包含碰撞到的物体，数据等
    /// </summary>
    /// <param name="type">右游戏控制器 KSRight /  左游戏控制器 KSLeft</param>
    /// <returns></returns>
    public static SCPointEventData GSXR_KSPointerEventData(GCType type = GCType.Right)
    {
        if (GSXR_KSRight && type == GCType.Right)
        {
            return GSXR_KSRight.inputDataKS.SCPointEventData;
        }
        else if (GSXR_KSLeft && type == GCType.Left)
        {
            return GSXR_KSLeft.inputDataKS.SCPointEventData;
        }
        return null;
    }

    /// <summary>
    /// KSRight/KSLeft 碰撞到Collider，若不为null,可以通过BTHitInfo获取碰撞信息，
    /// </summary>
    /// <param name="type">右游戏控制器 KSRight /  左游戏控制器 KSLeft</param>
    /// <returns></returns>
    public static GameObject GSXR_KSHitTarget(GCType type = GCType.Right)
    {
        if (GSXR_KSPointerEventData(type) != null)
        {
            return GSXR_KSPointerEventData(type).pointerCurrentRaycast.gameObject;
        }
        return null;
    }

    /// <summary>
    /// KSRight/KSLeft 碰撞信息
    /// </summary>
    /// <returns></returns>
    public static RaycastResult GSXR_KSHitInfo(GCType type = GCType.Right)
    {
        if (GSXR_KSPointerEventData(type) != null)
        {
            return GSXR_KSPointerEventData(type).pointerCurrentRaycast;
        }
        return new RaycastResult();
    }

    /// <summary>
    /// KSRight/KSLeft 拖拽的物体
    /// </summary>
    public static GameObject GSXR_KSDragTarget(GCType type = GCType.Right)
    {
        if (GSXR_KSPointerEventData(type) != null)
        {
            return GSXR_KSPointerEventData(type).pointerDrag;
        }
        return null;
    }


    /// <summary>
    /// KSRight/KSLeft某个按键是否按下，当前帧有效，下帧复位，参考Input.GetKeyDown
    /// <returns></returns>
    public static bool GSXR_Is_KSKeyDown(InputKeyCode inputKeyCode, GCType type = GCType.Right)
    {
        if (GSXR_KSRight && type == GCType.Right)
        {
            return GSXR_KSRight.inputDataKS.inputKeys.GetKeyDown(inputKeyCode);
        }
        else if (GSXR_KSLeft && type == GCType.Left)
        {
            return GSXR_KSLeft.inputDataKS.inputKeys.GetKeyDown(inputKeyCode);
        }
        return false;
    }


    /// <summary>
    /// KSRight/KSLeft某个按键是否按下后松开，当前帧有效，下帧复位，参考Input.GetKeyUp
    /// <returns></returns>
    public static bool GSXR_Is_KSKeyUp(InputKeyCode inputKeyCode, GCType type = GCType.Right)
    {
        if (GSXR_KSRight && type == GCType.Right)
        {
            return GSXR_KSRight.inputDataKS.inputKeys.GetKeyUp(inputKeyCode);
        }
        else if (GSXR_KSLeft && type == GCType.Left)
        {
            return GSXR_KSLeft.inputDataKS.inputKeys.GetKeyUp(inputKeyCode);
        }
        return false;
    }


    /// <summary>
    /// KSRight/KSLeft某个按键的状态信息，当前帧有效，下帧复位
    /// <returns></returns>
    public static InputKeyState GSXR_KSKeyState(InputKeyCode inputKeyCode, GCType type = GCType.Right)
    {
        if (GSXR_KSRight && type == GCType.Right)
        {
            return GSXR_KSRight.inputDataKS.inputKeys.GetKeyState(inputKeyCode);
        }
        else if (GSXR_KSLeft && type == GCType.Left)
        {
            return GSXR_KSLeft.inputDataKS.inputKeys.GetKeyState(inputKeyCode);
        }
        return InputKeyState.Null;
    }


    /// <summary>
    /// KSRight/KSLeft某个按键的实时状态，参考Input.GetKey
    /// <returns></returns>
    public static InputKeyState GSXR_KSKeyCurrentState(InputKeyCode inputKeyCode, GCType type = GCType.Right)
    {
        if (GSXR_KSRight && type == GCType.Right)
        {
            return GSXR_KSRight.inputDataKS.inputKeys.GetKeyCurrentState(inputKeyCode);
        }
        else if (GSXR_KSLeft && type == GCType.Left)
        {
            return GSXR_KSLeft.inputDataKS.inputKeys.GetKeyCurrentState(inputKeyCode);
        }
        return InputKeyState.Null;
    }


    /// <summary>
    /// KSRight/KSLeft发送一个按键，注意，发送按键至少需发送一个Down，然后再发送一个Up，此API模拟按键按下动作
    /// </summary>
    /// <param name="inputKeyCode">具体按键</param>
    /// <param name="inputKeyState">按键的状态</param>
    public static void GSXR_KSKeyAddKey(InputKeyCode inputKeyCode, InputKeyState inputKeyState, GCType type = GCType.Right)
    {
        if (GSXR_KSRight && type == GCType.Right)
        {
            GSXR_KSRight.inputDataKS.inputKeys.InputDataAddKey(inputKeyCode, inputKeyState);
        }
        else if (GSXR_KSLeft && type == GCType.Left)
        {
            GSXR_KSLeft.inputDataKS.inputKeys.InputDataAddKey(inputKeyCode, inputKeyState);
        }
    }


    /// <summary>
    /// KSRight/KSLeft设置可检测Collider的范围半径 默认30米
    /// </summary>
    /// <param name="distance">单位米</param>
    public static void GSXR_Set_KSRayCastDistance(float distance, GCType type = GCType.Right)
    {
        if (GSXR_KSRight && type == GCType.Right)
        {
            GSXR_KSRight.gcDetector.gcPointer.MaxDetectDistance = distance;
        }
        else if (GSXR_KSLeft && type == GCType.Left)
        {
            GSXR_KSLeft.gcDetector.gcPointer.MaxDetectDistance = distance;
        }
    }


    /// <summary>
    /// KSRight/KSLeft设置可见光束的长度 默认3米
    /// </summary>
    /// <param name="distance">单位米</param>
    public static void GSXR_Set_KSEndPointerDistance(float distance, GCType type = GCType.Right)
    {
        if (GSXR_KSRight && type == GCType.Right)
        {
            GSXR_Get_KSCursor(type).DefaultDistance = distance;
        }
        else if (GSXR_KSLeft && type == GCType.Left)
        {
            GSXR_Get_KSCursor(type).DefaultDistance = distance;
        }
    }



    /// <summary>
    /// KSRight/KSLeft获取光束终点的光标Cursor对象
    /// </summary>
    public static DefaultCursor GSXR_Get_KSCursor(GCType type = GCType.Right)
    {
        if (GSXR_KSRight && type == GCType.Right)
        {
            return GSXR_KSRight.gcDetector.gcPointer.cursorBase as DefaultCursor;
        }
        else if (GSXR_KSLeft && type == GCType.Left)
        {
            return GSXR_KSLeft.gcDetector.gcPointer.cursorBase as DefaultCursor;
        }
        return null;
    }


    /// <summary>
    /// KSRight/KSLeft获取光束
    /// </summary>
    public static LineRenderer GSXR_Get_KSLine(GCType type = GCType.Right)
    {
        if (GSXR_KSRight && type == GCType.Right)
        {
            return GSXR_KSRight.detectorBase.pointerBase.lineBase.lineRenderer;
        }
        else if (GSXR_KSLeft && type == GCType.Left)
        {
            return GSXR_KSLeft.detectorBase.pointerBase.lineBase.lineRenderer;
        }
        return null;
    }


    /// <summary>
    /// 开启KSRight/KSLeft
    /// </summary>
    /// <param name="type"></param>
    public static void GSXR_Enable_KS(GCType type = GCType.Right)
    {
        if (GSXR_KSDevice && type == GCType.Right)
        {
            GSXR_KSDevice.SetActiveInputDevicePart(InputDevicePartType.GCOne, true);

        }
        else if (GSXR_KSDevice && type == GCType.Left)
        {
            GSXR_KSDevice.SetActiveInputDevicePart(InputDevicePartType.GCTwo, true);
        }
    }


    /// <summary>
    /// 关闭KSRight/KSLeft
    /// </summary>
    /// <param name="type"></param>
    public static void GSXR_Disable_KS(GCType type = GCType.Right)
    {
        if (GSXR_KSDevice && type == GCType.Right)
        {
            GSXR_KSDevice.SetActiveInputDevicePart(InputDevicePartType.GCOne, false);
        }
        else if (GSXR_KSDevice && type == GCType.Left)
        {
            GSXR_KSDevice.SetActiveInputDevicePart(InputDevicePartType.GCTwo, false);
        }
    }


    public static int GSXR_Battery(GCType type = GCType.Right)
    {
        if (GSXR_KSDevice && type == GCType.Right)
        {
            return GSXR_KSRight.inputDataKS.BatteryPower;
        }
        else if (GSXR_KSDevice && type == GCType.Left)
        {
            return GSXR_KSLeft.inputDataKS.BatteryPower;
        }
        return -1;
    }


    public static bool GSXR_Is_Connected(GCType type = GCType.Right)
    {
        if (GSXR_KSRight && type == GCType.Right)
        {
            return GSXR_KSRight.inputDataKS.isConnected;
        }
        else if (GSXR_KSDevice && type == GCType.Left)
        {
            return GSXR_KSLeft.inputDataKS.isConnected;
        }
        return false;
    }


    public static int GSXR_Get_ControllerVersion(GCType type = GCType.Right)
    {
        if (GSXR_KSRight && type == GCType.Right)
        {
            return GSXR_KSRight.inputDataKS.SoftVesion;
        }
        else if (GSXR_KSLeft && type == GCType.Left)
        {
            return GSXR_KSLeft.inputDataKS.SoftVesion;
        }
        return -1;
    }


   static Vector2 JoystickValue=Vector2.zero;

    public static Vector2 GSXR_Get_JoystickValue(GCType type = GCType.Right)
    {
        if (GSXR_KSRight && type == GCType.Right)
        {
            JoystickValue.x= GSXR_KSRight.inputDataKS.JoystickX;
            JoystickValue.y = GSXR_KSRight.inputDataKS.JoystickY;
            return JoystickValue;
        }
        else if (GSXR_KSLeft && type == GCType.Left)
        {
            JoystickValue.x = GSXR_KSLeft.inputDataKS.JoystickX;
            JoystickValue.y = GSXR_KSLeft.inputDataKS.JoystickY;
            return JoystickValue;
        }
        return Vector2.zero;
    }

    public static string GSX_Get_PostureType(GCType type = GCType.Right)
    {
        if (GSXR_KSRight && type == GCType.Right)
        {
            return GSXR_KSRight.inputDataKS.PostureType.ToString();
        }
        else if (GSXR_KSLeft && type == GCType.Left)
        {
            return GSXR_KSLeft.inputDataKS.PostureType.ToString();
        }
        return "";
    }

}
