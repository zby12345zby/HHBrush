using System;
using UnityEngine;
using SC.XR.Unity.Module_InputSystem;
using SC.XR.Unity.Module_InputSystem.InputDeviceGC.KS;
using UnityEngine.EventSystems;

[Obsolete("Please Use API_GSXR_Module_InputSystem_KS")]
public class API_Module_InputSystem_KS {


    /// <summary>
    /// 获取InputSystem支持的游戏控制器输入设备,游戏控制器输入设备包含二个Part，名曰：KSRight,KSLeft,也就是第一个游戏控制器和第二个游戏控制器
    /// </summary>
    /// <returns>null表示不支持或者InputSystem未初始好</returns>
    public static InputDeviceKS KSDevice {
        get {
            if(Module_InputSystem.instance) {
                return Module_InputSystem.instance.GetInputDevice<InputDeviceKS>(InputDeviceType.KS);
            }
            return null;
        }
    }

    /// <summary>
    /// 游戏控制器输入设备连接的第一个游戏控制器
    /// </summary>
    public static InputDeviceKSPart KSRight {
        get {
            if (KSDevice && KSDevice.inputDevicePartList.Count > 0) {
                foreach (var part in KSDevice.inputDevicePartList) {
                    if (part.PartType == InputDevicePartType.KSRight) {
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
    public static InputDeviceKSPart KSLeft {
        get {
            if (KSDevice && KSDevice.inputDevicePartList.Count > 0) {
                foreach (var part in KSDevice.inputDevicePartList) {
                    if (part.PartType == InputDevicePartType.KSLeft) {
                        return part as InputDeviceKSPart;
                    }
                }
            }
            return null;
        }
    }

    public enum GCType { 
        Left,
        Right
    }


    /// <summary>
    /// KSRight/KSLeft的四元数，全局坐标
    /// </summary>
    /// <param name="type">右游戏控制器 KSRight /  左游戏控制器 KSLeft</param>
    /// <returns></returns>
    public static Quaternion KSRotation(GCType type = GCType.Right) {
        if(KSRight && type == GCType.Right) {
            return KSRight.inputDataKS.rotation;
        } else if(KSLeft && type == GCType.Left) {
            return KSLeft.inputDataKS.rotation;
        }
        return Quaternion.identity;
    }


    /// <summary>
    /// KSRight/KSLeft的位置信息，全局坐标
    /// </summary>
    /// <param name="type">右游戏控制器 KSRight /  左游戏控制器 BKSLeft</param>
    /// <returns></returns>
    public static Vector3 KSPosition(GCType type = GCType.Right) {
        if(KSRight && type == GCType.Right) {
            return KSRight.inputDataKS.position;
        } else if(KSLeft && type == GCType.Left) {
            return KSLeft.inputDataKS.position;
        }
        return Vector3.zero;
    }

    public static Transform KSTransform(GCType type = GCType.Right) {
        if (KSRight && type == GCType.Right) {
            return KSRight.inputDeviceKSPartUI.ModelGC.transform;
        } else if (KSLeft && type == GCType.Left) {
            return KSLeft.inputDeviceKSPartUI.ModelGC.transform;
        }
        return null;
    }

    public static string KSName(GCType type = GCType.Right)
    {
        if (KSRight && type == GCType.Right)
        {
            return KSRight.inputDataKS.GCName;
        }
        else if (KSLeft && type == GCType.Left)
        {
            return KSLeft.inputDataKS.GCName;
        }
        return "";
    }

    /// <summary>
    /// KSRight/KSLeft 检测到碰撞信息集合，包含碰撞到的物体，数据等
    /// </summary>
    /// <param name="type">右游戏控制器 KSRight /  左游戏控制器 KSLeft</param>
    /// <returns></returns>
    public static SCPointEventData KSPointerEventData(GCType type = GCType.Right)
    {
        if (KSRight && type == GCType.Right)
        {
            return KSRight.inputDataKS.SCPointEventData;
        }
        else if (KSLeft && type == GCType.Left)
        {
            return KSLeft.inputDataKS.SCPointEventData;
        }
        return null;
    }

    /// <summary>
    /// KSRight/KSLeft 碰撞到Collider，若不为null,可以通过BTHitInfo获取碰撞信息，
    /// </summary>
    /// <param name="type">右游戏控制器 KSRight /  左游戏控制器 KSLeft</param>
    /// <returns></returns>
    public static GameObject KSHitTarget(GCType type = GCType.Right)
    {
        if (KSPointerEventData(type) != null)
        {
            return KSPointerEventData(type).pointerCurrentRaycast.gameObject;
        }
        return null;
    }

    /// <summary>
    /// KSRight/KSLeft 碰撞信息
    /// </summary>
    /// <returns></returns>
    public static RaycastResult KSHitInfo(GCType type = GCType.Right)
    {
        if (KSPointerEventData(type) != null)
        {
            return KSPointerEventData(type).pointerCurrentRaycast;
        }
        return new RaycastResult();
    }

    /// <summary>
    /// KSRight/KSLeft 拖拽的物体
    /// </summary>
    public static GameObject KSDragTarget(GCType type = GCType.Right)
    {
        if (KSPointerEventData(type) != null)
        {
            return KSPointerEventData(type).pointerDrag;
        }
        return null;
    }


    /// <summary>
    /// KSRight/KSLeft某个按键是否按下，当前帧有效，下帧复位，参考Input.GetKeyDown
    /// <returns></returns>
    public static bool IsKSKeyDown(InputKeyCode inputKeyCode, GCType type = GCType.Right)
    {
        if (KSRight && type == GCType.Right)
        {
            return KSRight.inputDataKS.inputKeys.GetKeyDown(inputKeyCode);
        }
        else if (KSLeft && type == GCType.Left)
        {
            return KSLeft.inputDataKS.inputKeys.GetKeyDown(inputKeyCode);
        }
        return false;
    }


    /// <summary>
    /// KSRight/KSLeft某个按键是否按下后松开，当前帧有效，下帧复位，参考Input.GetKeyUp
    /// <returns></returns>
    public static bool IsKSKeyUp(InputKeyCode inputKeyCode, GCType type = GCType.Right)
    {
        if (KSRight && type == GCType.Right)
        {
            return KSRight.inputDataKS.inputKeys.GetKeyUp(inputKeyCode);
        }
        else if (KSLeft && type == GCType.Left)
        {
            return KSLeft.inputDataKS.inputKeys.GetKeyUp(inputKeyCode);
        }
        return false;
    }


    /// <summary>
    /// KSRight/KSLeft某个按键的状态信息，当前帧有效，下帧复位
    /// <returns></returns>
    public static InputKeyState KSKeyState(InputKeyCode inputKeyCode, GCType type = GCType.Right)
    {
        if (KSRight && type == GCType.Right)
        {
            return KSRight.inputDataKS.inputKeys.GetKeyState(inputKeyCode);
        }
        else if (KSLeft && type == GCType.Left)
        {
            return KSLeft.inputDataKS.inputKeys.GetKeyState(inputKeyCode);
        }
        return InputKeyState.Null;
    }


    /// <summary>
    /// KSRight/KSLeft某个按键的实时状态，参考Input.GetKey
    /// <returns></returns>
    public static InputKeyState KSKeyCurrentState(InputKeyCode inputKeyCode, GCType type = GCType.Right)
    {
        if (KSRight && type == GCType.Right)
        {
            return KSRight.inputDataKS.inputKeys.GetKeyCurrentState(inputKeyCode);
        }
        else if (KSLeft && type == GCType.Left)
        {
            return KSLeft.inputDataKS.inputKeys.GetKeyCurrentState(inputKeyCode);
        }
        return InputKeyState.Null;
    }

   
    /// <summary>
    /// KSRight/KSLeft发送一个按键，注意，发送按键至少需发送一个Down，然后再发送一个Up，此API模拟按键按下动作
    /// </summary>
    /// <param name="inputKeyCode">具体按键</param>
    /// <param name="inputKeyState">按键的状态</param>
    public static void KSKeyAddKey(InputKeyCode inputKeyCode, InputKeyState inputKeyState, GCType type = GCType.Right)
    {
        if (KSRight && type == GCType.Right)
        {
            KSRight.inputDataKS.inputKeys.InputDataAddKey(inputKeyCode, inputKeyState);
        }
        else if (KSLeft && type == GCType.Left)
        {
            KSLeft.inputDataKS.inputKeys.InputDataAddKey(inputKeyCode, inputKeyState);
        }
    }

   
    /// <summary>
    /// KSRight/KSLeft设置可检测Collider的范围半径 默认30米
    /// </summary>
    /// <param name="distance">单位米</param>
    public static void SetKSRayCastDistance(float distance, GCType type = GCType.Right)
    {
        if (KSRight && type == GCType.Right)
        {
            KSRight.gcDetector.gcPointer.MaxDetectDistance = distance;
        }
        else if (KSLeft && type == GCType.Left)
        {
            KSLeft.gcDetector.gcPointer.MaxDetectDistance = distance;
        }
    }


    /// <summary>
    /// KSRight/KSLeft设置可见光束的长度 默认3米
    /// </summary>
    /// <param name="distance">单位米</param>
    public static void SetKSEndPointerDistance(float distance, GCType type = GCType.Right)
    {
        if (KSRight && type == GCType.Right)
        {
            GetKSCursor(type).DefaultDistance = distance;
        }
        else if (KSLeft && type == GCType.Left)
        {
            GetKSCursor(type).DefaultDistance = distance;
        }
    }

    
    
    /// <summary>
    /// KSRight/KSLeft获取光束终点的光标Cursor对象
    /// </summary>
    public static DefaultCursor GetKSCursor(GCType type = GCType.Right)
    {
        if (KSRight && type == GCType.Right)
        {
            return KSRight.gcDetector.gcPointer.cursorBase as DefaultCursor;
        }
        else if (KSLeft && type == GCType.Left)
        {
            return KSLeft.gcDetector.gcPointer.cursorBase as DefaultCursor;
        }
        return null;
    }


    /// <summary>
    /// KSRight/KSLeft获取光束
    /// </summary>
    public static LineRenderer GetKSLine(GCType type = GCType.Right)
    {
        if (KSRight && type == GCType.Right)
        {
            return KSRight.detectorBase.pointerBase.lineBase.lineRenderer;
        }
        else if (KSLeft && type == GCType.Left)
        {
            return KSLeft.detectorBase.pointerBase.lineBase.lineRenderer;
        }
        return null;
    }

    
    /// <summary>
    /// 开启KSRight/KSLeft
    /// </summary>
    /// <param name="type"></param>
    public static void EnableKS(GCType type = GCType.Right)
    {
        if (KSDevice && type == GCType.Right)
        {
            KSDevice.SetActiveInputDevicePart(InputDevicePartType.GCOne, true);

        }
        else if (KSDevice && type == GCType.Left)
        {
            KSDevice.SetActiveInputDevicePart(InputDevicePartType.GCTwo, true);
        }
    }

   
    /// <summary>
    /// 关闭KSRight/KSLeft
    /// </summary>
    /// <param name="type"></param>
    public static void DisableKS(GCType type = GCType.Right)
    {
        if (KSDevice && type == GCType.Right)
        {
            KSDevice.SetActiveInputDevicePart(InputDevicePartType.GCOne, false);
        }
        else if (KSDevice && type == GCType.Left)
        {
            KSDevice.SetActiveInputDevicePart(InputDevicePartType.GCTwo, false);
        }
    }




}
