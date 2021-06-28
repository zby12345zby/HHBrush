using System;
using UnityEngine;
using SC.XR.Unity.Module_InputSystem;
using SC.XR.Unity.Module_InputSystem.InputDeviceGC.BT3Dof;
using UnityEngine.EventSystems;

public class API_GSXR_Module_InputSystem_BT3Dof
{

    ///API-No.150
    /// <summary>
    /// 获取InputSystem支持的手柄输入设备,手柄输入设备包含二个Part，名曰：BTRight,BTLeft,也就是第一个手柄和第二个手柄
    /// </summary>
    /// <returns>null表示不支持或者InputSystem未初始好</returns>
    public static InputDeviceBT3Dof GSXR_BTDevice
    {
        get
        {
            if (Module_InputSystem.instance)
            {
                return Module_InputSystem.instance.GetInputDevice<InputDeviceBT3Dof>(InputDeviceType.BT3Dof);
            }
            return null;
        }
    }

    ///API-No.151
    /// <summary>
    /// 手柄输入设备连接的第一个手柄
    /// </summary>
    public static InputDeviceBT3DofPart GSXR_BTRight
    {
        get
        {
            if (GSXR_BTDevice && GSXR_BTDevice.inputDevicePartList[0])
            {
                //return BTDevice.inputDeviceHandShankPartList[0].inputDataBase.isVaild ? BTDevice.inputDeviceHandShankPartList[0] : null;
                return GSXR_BTDevice.inputDevicePartList[0] as InputDeviceBT3DofPart;
            }
            return null;
        }
    }

    ///API-No.152
    /// <summary>
    /// 手柄输入设备连接的第二个手柄
    /// </summary>
    public static InputDeviceBT3DofPart GSXR_BTLeft
    {
        get
        {
            if (GSXR_BTDevice && GSXR_BTDevice.inputDevicePartList[1])
            {
                //return BTDevice.inputDeviceHandShankPartList[1].inputDataBase.isVaild ? BTDevice.inputDeviceHandShankPartList[1] : null;
                return GSXR_BTDevice.inputDevicePartList[1] as InputDeviceBT3DofPart;
            }
            return null;
        }
    }

    public enum BTType
    {
        Left,
        Right
    }

    ///API-No.153
    /// <summary>
    /// BTRight/BTLeft的四元数，全局坐标
    /// </summary>
    /// <param name="type">右手柄 BTRight /  左手柄 BTLeft</param>
    /// <returns></returns>
    public static Quaternion GSXR_BTRotation(BTType type = BTType.Right)
    {
        if (GSXR_BTRight && type == BTType.Right)
        {
            return GSXR_BTRight.inputDataBT3Dof.rotation;
        }
        else if (GSXR_BTLeft && type == BTType.Left)
        {
            return GSXR_BTLeft.inputDataBT3Dof.rotation;
        }
        return Quaternion.identity;
    }

    ///API-No.154
    /// <summary>
    /// BTRight/BTLeft的位置信息，全局坐标
    /// </summary>
    /// <param name="type">右手柄 BTRight /  左手柄 BTLeft</param>
    /// <returns></returns>
    public static Vector3 BTPosition(BTType type = BTType.Right)
    {
        if (GSXR_BTRight && type == BTType.Right)
        {
            return GSXR_BTRight.inputDataBT3Dof.position;
        }
        else if (GSXR_BTLeft && type == BTType.Left)
        {
            return GSXR_BTLeft.inputDataBT3Dof.position;
        }
        return Vector3.zero;
    }

    ///API-No.155
    /// <summary>
    /// BTRight/BTLeft 的触摸板是否触摸
    /// </summary>
    /// <param name="type">右手柄 BTRight /  左手柄 BTLeft</param>
    /// <returns>ture表示被触摸，反之</returns>
    public static bool GSXR_Is_BTTpTouch(BTType type = BTType.Right)
    {
        if (GSXR_BTRight && type == BTType.Right)
        {
            return GSXR_BTRight.inputDataBT3Dof.isTpTouch;
        }
        else if (GSXR_BTLeft && type == BTType.Left)
        {
            return GSXR_BTLeft.inputDataBT3Dof.isTpTouch;
        }
        return false;
    }

    ///API-No.156
    /// <summary>
    /// BTRight/BTLeft 的触摸板触摸位置数据
    /// </summary>
    /// <param name="type">右手柄 BTRight /  左手柄 BTLeft</param>
    /// <returns></returns>
    public static Vector2 GSXR_BTTpTouchInfo(BTType type = BTType.Right)
    {
        if (GSXR_BTRight && type == BTType.Right)
        {
            return GSXR_BTRight.inputDataBT3Dof.tpPosition;
        }
        else if (GSXR_BTLeft && type == BTType.Left)
        {
            return GSXR_BTLeft.inputDataBT3Dof.tpPosition;
        }
        return Vector2.zero;
    }

    ///API-No.157
    /// <summary>
    /// BTRight/BTLeft 的触摸板的手柄名称
    /// </summary>
    /// <param name="type">右手柄 BTRight /  左手柄 BTLeft</param>
    /// <returns></returns>
    public static string GSXR_BTName(BTType type = BTType.Right)
    {
        if (GSXR_BTRight && type == BTType.Right)
        {
            return GSXR_BTRight.inputDataBT3Dof.GCName;
        }
        else if (GSXR_BTLeft && type == BTType.Left)
        {
            return GSXR_BTLeft.inputDataBT3Dof.GCName;
        }
        return "";
    }

    ///API-No.158
    /// <summary>
    /// BTRight/BTLeft 检测到碰撞信息集合，包含碰撞到的物体，数据等
    /// </summary>
    /// <param name="type">右手柄 BTRight /  左手柄 BTLeft</param>
    /// <returns></returns>
    public static SCPointEventData GSXR_BTPointerEventData(BTType type = BTType.Right)
    {
        if (GSXR_BTRight && type == BTType.Right)
        {
            return GSXR_BTRight.inputDataBT3Dof.SCPointEventData;
        }
        else if (GSXR_BTLeft && type == BTType.Left)
        {
            return GSXR_BTLeft.inputDataBT3Dof.SCPointEventData;
        }
        return null;
    }

    ///API-No.159
    /// <summary>
    /// BTRight/BTLeft 碰撞到Collider，若不为null,可以通过BTHitInfo获取碰撞信息，
    /// </summary>
    /// <param name="type">右手柄 BTRight /  左手柄 BTLeft</param>
    /// <returns></returns>
    public static GameObject GSXR_BTHitTarget(BTType type = BTType.Right)
    {
        if (GSXR_BTPointerEventData(type) != null)
        {
            return GSXR_BTPointerEventData(type).pointerCurrentRaycast.gameObject;
        }
        return null;
    }

    ///API-No.160
    /// <summary>
    /// BTRight/BTLeft 碰撞信息
    /// </summary>
    /// <returns></returns>
    public static RaycastResult GSXR_BTHitInfo(BTType type = BTType.Right)
    {
        if (GSXR_BTPointerEventData(type) != null)
        {
            return GSXR_BTPointerEventData(type).pointerCurrentRaycast;
        }
        return new RaycastResult();
    }

    ///API-No.161
    /// <summary>
    /// BTRight/BTLeft 拖拽的物体
    /// </summary>
    public static GameObject GSXR_BTDragTarget(BTType type = BTType.Right)
    {
        if (GSXR_BTPointerEventData(type) != null)
        {
            return GSXR_BTPointerEventData(type).pointerDrag;
        }
        return null;
    }

    ///API-No.162
    /// <summary>
    /// BTRight/BTLeft某个按键是否按下，当前帧有效，下帧复位，参考Input.GetKeyDown
    /// </summary>
    /// <param name="inputKeyCode">具体按键，BTRight/BTLeft支持Enter/Back/Function</param>
    /// <returns></returns>
    public static bool GSXR_Is_BTKeyDown(InputKeyCode inputKeyCode, BTType type = BTType.Right)
    {
        if (GSXR_BTRight && type == BTType.Right)
        {
            return GSXR_BTRight.inputDataBT3Dof.inputKeys.GetKeyDown(inputKeyCode);
        }
        else if (GSXR_BTLeft && type == BTType.Left)
        {
            return GSXR_BTLeft.inputDataBT3Dof.inputKeys.GetKeyDown(inputKeyCode);
        }
        return false;
    }

    ///API-No.163
    /// <summary>
    /// BTRight/BTLeft某个按键是否按下后松开，当前帧有效，下帧复位，参考Input.GetKeyUp
    /// </summary>
    /// <param name="inputKeyCode">具体按键，BTRight/BTLeft支持Enter/Back/Function</param>
    /// <returns></returns>
    public static bool GSXR_Is_BTKeyUp(InputKeyCode inputKeyCode, BTType type = BTType.Right)
    {
        if (GSXR_BTRight && type == BTType.Right)
        {
            return GSXR_BTRight.inputDataBT3Dof.inputKeys.GetKeyUp(inputKeyCode);
        }
        else if (GSXR_BTLeft && type == BTType.Left)
        {
            return GSXR_BTLeft.inputDataBT3Dof.inputKeys.GetKeyUp(inputKeyCode);
        }
        return false;
    }

    ///API-No.164
    /// <summary>
    /// BTRight/BTLeft某个按键的状态信息，当前帧有效，下帧复位
    /// </summary>
    /// <param name="inputKeyCode">具体按键，BTRight/BTLeft支持Enter/Back/Function</param>
    /// <returns></returns>
    public static InputKeyState GSXR_BTKeyState(InputKeyCode inputKeyCode, BTType type = BTType.Right)
    {
        if (GSXR_BTRight && type == BTType.Right)
        {
            return GSXR_BTRight.inputDataBT3Dof.inputKeys.GetKeyState(inputKeyCode);
        }
        else if (GSXR_BTLeft && type == BTType.Left)
        {
            return GSXR_BTLeft.inputDataBT3Dof.inputKeys.GetKeyState(inputKeyCode);
        }
        return InputKeyState.Null;
    }

    ///API-No.165
    /// <summary>
    /// BTRight/BTLeft某个按键的实时状态，参考Input.GetKey
    /// </summary>
    /// <param name="inputKeyCode">具体按键，BTRight/BTLeft支持Enter/Back/Function</param>
    /// <returns></returns>
    public static InputKeyState GSXR_BTKeyCurrentState(InputKeyCode inputKeyCode, BTType type = BTType.Right)
    {
        if (GSXR_BTRight && type == BTType.Right)
        {
            return GSXR_BTRight.inputDataBT3Dof.inputKeys.GetKeyCurrentState(inputKeyCode);
        }
        else if (GSXR_BTLeft && type == BTType.Left)
        {
            return GSXR_BTLeft.inputDataBT3Dof.inputKeys.GetKeyCurrentState(inputKeyCode);
        }
        return InputKeyState.Null;
    }

    ///API-No.166
    /// <summary>
    /// BTRight/BTLeft发送一个按键，注意，发送按键至少需发送一个Down，然后再发送一个Up，此API模拟按键按下动作
    /// </summary>
    /// <param name="inputKeyCode">具体按键</param>
    /// <param name="inputKeyState">按键的状态</param>
    public static void GSXR_BTKeyAddKey(InputKeyCode inputKeyCode, InputKeyState inputKeyState, BTType type = BTType.Right)
    {
        if (GSXR_BTRight && type == BTType.Right)
        {
            GSXR_BTRight.inputDataBT3Dof.inputKeys.InputDataAddKey(inputKeyCode, inputKeyState);
        }
        else if (GSXR_BTLeft && type == BTType.Left)
        {
            GSXR_BTLeft.inputDataBT3Dof.inputKeys.InputDataAddKey(inputKeyCode, inputKeyState);
        }
    }

    ///API-No.167
    /// <summary>
    /// BTRight/BTLeft设置可检测Collider的范围半径 默认30米
    /// </summary>
    /// <param name="distance">单位米</param>
    public static void GSXR_Set_BTRayCastDistance(float distance, BTType type = BTType.Right)
    {
        if (GSXR_BTRight && type == BTType.Right)
        {
            GSXR_BTRight.bT3DofDetector.bT3DofPointer.MaxDetectDistance = distance;
        }
        else if (GSXR_BTLeft && type == BTType.Left)
        {
            GSXR_BTLeft.bT3DofDetector.bT3DofPointer.MaxDetectDistance = distance;
        }
    }

    ///API-No.168
    /// <summary>
    /// BTRight/BTLeft设置可见光束的长度 默认3米
    /// </summary>
    /// <param name="distance">单位米</param>
    public static void GSXR_Set_BTEndPointerDistance(float distance, BTType type = BTType.Right)
    {
        if (GSXR_BTRight && type == BTType.Right)
        {
            GSXR_Get_BTCursor(type).DefaultDistance = distance;
        }
        else if (GSXR_BTLeft && type == BTType.Left)
        {
            GSXR_Get_BTCursor(type).DefaultDistance = distance;
        }
    }

    ///API-No.169
    /// <summary>
    /// BTRight/BTLeft获取光束终点的光标Cursor对象
    /// </summary>
    public static DefaultCursor GSXR_Get_BTCursor(BTType type = BTType.Right)
    {
        if (GSXR_BTRight && type == BTType.Right)
        {
            return GSXR_BTRight.detectorBase.pointerBase.cursorBase as DefaultCursor;
        }
        else if (GSXR_BTLeft && type == BTType.Left)
        {
            return GSXR_BTLeft.detectorBase.pointerBase.cursorBase as DefaultCursor;
        }
        return null;
    }

    ///API-No.170
    /// <summary>
    /// BTRight/BTLeft获取光束
    /// </summary>
    public static LineRenderer GSXR_Get_BTLine(BTType type = BTType.Right)
    {
        if (GSXR_BTRight && type == BTType.Right)
        {
            return GSXR_BTRight.detectorBase.pointerBase.lineBase.lineRenderer;
        }
        else if (GSXR_BTLeft && type == BTType.Left)
        {
            return GSXR_BTLeft.detectorBase.pointerBase.lineBase.lineRenderer;
        }
        return null;
    }

    ///API-No.171
    /// <summary>
    /// 开启BTRight/BTLeft
    /// </summary>
    /// <param name="type"></param>
    public static void GSXR_Enable_BT(BTType type = BTType.Right)
    {
        if (GSXR_BTDevice && type == BTType.Right)
        {
            //BTDevice.OneGCActive = true;
            GSXR_BTDevice.SetActiveInputDevicePart(InputDevicePartType.GCOne, true);

        }
        else if (GSXR_BTDevice && type == BTType.Left)
        {
            //BTDevice.TwoGCActive = true;
            GSXR_BTDevice.SetActiveInputDevicePart(InputDevicePartType.GCTwo, true);
        }
    }

    ///API-No.172
    /// <summary>
    /// 关闭BTRight/BTLeft
    /// </summary>
    /// <param name="type"></param>
    public static void GSXR_Disable_BT(BTType type = BTType.Right)
    {
        if (GSXR_BTDevice && type == BTType.Right)
        {
            // BTDevice.OneGCActive = false;
            GSXR_BTDevice.SetActiveInputDevicePart(InputDevicePartType.GCOne, false);
        }
        else if (GSXR_BTDevice && type == BTType.Left)
        {
            //BTDevice.TwoGCActive = false;
            GSXR_BTDevice.SetActiveInputDevicePart(InputDevicePartType.GCTwo, false);
        }
    }

    static int[] temp = new int[3] { 0, 0, 0 };
    ///API-No.173
    /// <summary>
    /// 获取BTRight/BTLeft的ACC数据
    /// </summary>
    /// <param name="type"></param>
    public static int[] GSXR_Get_Acc(BTType type = BTType.Right)
    {

        if (GSXR_BTDevice && type == BTType.Right)
        {
            return GSXR_BTRight.inputDataGetBT3Dof.inputDataGetBT3DofIMU.GetAcc();
        }
        else if (GSXR_BTDevice && type == BTType.Left)
        {
            return GSXR_BTLeft.inputDataGetBT3Dof.inputDataGetBT3DofIMU.GetAcc();
        }
        return temp;
    }

    ///API-No.174
    /// <summary>
    /// 获取BTRight/BTLeft的Gyro数据
    /// </summary>
    /// <param name="type"></param>
    public static int[] GSXR_Get_Gyro(BTType type = BTType.Right)
    {
        if (GSXR_BTDevice && type == BTType.Right)
        {
            return GSXR_BTRight.inputDataGetBT3Dof.inputDataGetBT3DofIMU.GetGyro();
        }
        else if (GSXR_BTDevice && type == BTType.Left)
        {
            return GSXR_BTLeft.inputDataGetBT3Dof.inputDataGetBT3DofIMU.GetGyro();
        }
        return temp;
    }


}
