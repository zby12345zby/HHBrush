using SC.XR.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class API_GSXR_Slam
{

    public static GSXRManager SlamManager
    {
        get
        {
            return GSXRManager.Instance;
        }
    }

    ///API-No.1
    /// <summary>
    /// 设置眼镜进入模式,运行过程中可修改
    /// </summary>
    public static void GSXR_Set_TrackMode(TrackMode mode)
    {
        if (API_GSXR_Slam.SlamManager != null)
        {

            int trackingMode = API_GSXR_Slam.SlamManager.plugin.GetTrackingMode();

            if (mode == TrackMode.Mode_6Dof)
            {
                trackingMode |= (int)GSXRPlugin.TrackingMode.kTrackingPosition;
            }
            else
            {
                trackingMode &= (int)(~(1 << 1));
            }
            API_GSXR_Slam.SlamManager.plugin.SetTrackingMode(trackingMode);
        }
    }

    ///API-No.2
    /// <summary>
    /// Slam系统是否在运行
    /// </summary>
    /// <returns>true表示在运行，false表示未运行（Pause时为false）</returns>
    public static bool GSXR_Is_SlamRunning() {
        if (API_GSXR_Slam.SlamManager != null) {
            return API_GSXR_Slam.SlamManager.status.running;
        }
        return false;
    }


    ///API-No.3
    /// <summary>
    /// Slam系统是否初始化完成
    /// </summary>
    /// <returns></returns>
    public static bool GSXR_Is_SlamInitialized() {
        if (API_GSXR_Slam.SlamManager != null) {
            return API_GSXR_Slam.SlamManager.status.initialized;
        }
        return false;
    }

    ///API-No.4
    /// <summary>
    /// 设置Slam初始化完成时的回调
    /// </summary>
    /// <param name="action"></param>
    public static void GSXR_Add_InitializedCallBack(Action action)
    {
        GSXRManager.SlamInitializedCallBack += action;
    }

    ///API-No.5
    public static void GSXR_Remove_InitializedCallBack(Action action)
    {
        GSXRManager.SlamInitializedCallBack -= action;
    }

    ///API-No.6
    /// <summary>
    /// 设置渲染帧率,只能在Start中调用
    /// </summary>
    /// <param name="frameRate">默认-1表示系统默认帧率,设置范围0-200</param>
    public static void GSXR_Set_RenderFrame(int frameRate = -1)
    {
        if (API_GSXR_Slam.SlamManager != null)
        {
            if (frameRate == -1)
            {
                API_GSXR_Slam.SlamManager.plugin.SetVSyncCount((int)(API_GSXR_Slam.SlamManager.settings.vSyncCount = GSXRManager.SlamSettings.eVSyncCount.k1));
                QualitySettings.vSyncCount = (int)(API_GSXR_Slam.SlamManager.settings.vSyncCount = GSXRManager.SlamSettings.eVSyncCount.k1);//Vsync
            }
            else
            {
                API_GSXR_Slam.SlamManager.plugin.SetVSyncCount((int)(API_GSXR_Slam.SlamManager.settings.vSyncCount = GSXRManager.SlamSettings.eVSyncCount.k0));
                QualitySettings.vSyncCount = (int)(API_GSXR_Slam.SlamManager.settings.vSyncCount = GSXRManager.SlamSettings.eVSyncCount.k0);//Don't sync
                Application.targetFrameRate = (frameRate >= 0 && frameRate < 200) ? frameRate : 75;
            }
        }
    }

    ///API-No.7
    /// <summary>
    /// 获取左右眼摄像头
    /// </summary>
    /// <returns>List[0]左眼 List[1]右眼，空表示系统未启动完成</returns>
    public static List<Camera> GSXR_Get_EyeCameras()
    {
        List<Camera> cameraList = new List<Camera>(2);
        if (API_GSXR_Slam.SlamManager != null && API_GSXR_Slam.SlamManager.status.running == true)
        {
            cameraList.Add(API_GSXR_Slam.SlamManager.leftCamera);
            cameraList.Add(API_GSXR_Slam.SlamManager.rightCamera);
        }
        return cameraList;
    }

    ///API-No.8
    /// <summary>
    /// 获取左右眼渲染的画面，为获取当前帧的渲染结果，当前帧结束时调用
    /// </summary>
    /// <returns>List[0]左眼 List[1]右眼，空表示系统未启动完成</returns>
    public static List<RenderTexture> GSXR_Get_RenderTexure()
    {
        List<Camera> cameraList = GSXR_Get_EyeCameras();
        List<RenderTexture> RTList = new List<RenderTexture>(2);
        foreach (var item in cameraList)
        {
            RTList.Add(item.targetTexture);
        }
        return RTList;
    }

    ///API-No.9
    /// <summary>
    /// 获取头部物体，如果想获取头部的旋转移动等数据，在LateUpdate方法里调用
    /// </summary>
    /// <returns>空表示系统未启动完成</returns>
    public static Transform GSXR_Get_Head()
    {
        if (API_GSXR_Slam.SlamManager != null && API_GSXR_Slam.SlamManager.status.running == true)
        {
            return API_GSXR_Slam.SlamManager.head;
        }
        return null;
    }

    ///API-No.10
    /// <summary>
    /// 设置瞳距，Awake时调用，Start后调用无效
    /// </summary>
    /// <param name="offset">瞳距的偏移量，单位米</param>
    public static void GSXR_Set_PD(float offset = 0)
    {
        if (API_GSXR_Slam.SlamManager != null) {
            API_GSXR_Slam.SlamManager.plugin.GSXR_SetMeshOffset(0, offset / 2);
            API_GSXR_Slam.SlamManager.plugin.GSXR_SetMeshOffset(2, offset / 2);
            //API_GSXR_Slam.SlamManager.leftCameraOffsetPostion += offset / 2 * Vector3.left;
            //API_GSXR_Slam.SlamManager.rightCameraOffsetPostion += offset / 2 * Vector3.right;
        }
    }


    ///API-No.11
    /// <summary>
    /// 获取瞳距，
    /// </summary>
    /// <param name="type">右眼类型：2，左眼类型：0</param>
    public static float GSXR_Get_PD(int type)
    {
        if (API_GSXR_Slam.SlamManager != null)
        {
           return API_GSXR_Slam.SlamManager.plugin.GSXR_GetMeshOffset(type)+0.064f;
        }
        return -1;
    }


    ///API-No.12
    /// <summary>
    /// 重定位,若无效果，表示系统初始化未完成,且只有在眼镜上有效
    /// </summary>
    public static void GSXR_RecenterTracking()
    {
        if (API_GSXR_Slam.SlamManager != null)
        {
            API_GSXR_Slam.SlamManager.RecenterTracking();
        }
    }

    ///API-No.13
    /// <summary>
    /// StartSlam
    /// </summary>
    public static void GSXR_Start_Slam()
    {
        if (API_GSXR_Slam.SlamManager != null)
        {
            API_GSXR_Slam.SlamManager.StartSlam();
        }
    }

    ///API-No.14
    /// <summary>
    /// StopSlam
    /// When a StartSlam is running (not completed), calling StopSlam will not work
    /// </summary>
    public static void GSXR_Stop_Slam()
    {
        if (API_GSXR_Slam.SlamManager != null)
        {
            API_GSXR_Slam.SlamManager.StopSlam();
        }
    }

    ///API-No.15
    /// <summary>
    /// ResetSlam
    /// </summary>
    public static void GSXR_Reset_Slam()
    {
        if (API_GSXR_Slam.SlamManager != null)
        {
            API_GSXR_Slam.SlamManager.ResetSlam();
        }
    }

    ///API-No.16
    /// <summary>
    /// IS Slam 6Dof DataLost
    /// </summary>
    public static bool GSXR_Is_SlamDataLost
    {
        get
        {
            if (API_GSXR_Slam.SlamManager != null)
            {
                return API_GSXR_Slam.SlamManager.IsTrackingValid;
            }
            return true;
        }
    }

    ///API-No.17
    /// <summary>
    /// Get FishEye Data
    /// </summary>
    public static int GSXR_Get_LatestFishEyeBinocularData(ref bool outBUdate, ref uint outCurrFrameIndex, ref ulong outFrameExposureNano, byte[] outLeftFrameData, byte[] outRightFrameData)
    {
        if (API_GSXR_Slam.SlamManager && API_GSXR_Slam.SlamManager.plugin != null)
        {
            PermissionRequest.getInstance.GetPerssion(UnityEngine.Android.Permission.Camera);
            return API_GSXR_Slam.SlamManager.plugin.GSXR_Get_LatestFishEyeBinocularData(ref outBUdate, ref outCurrFrameIndex, ref outFrameExposureNano, outLeftFrameData, outRightFrameData);
        }
        return 0;
    }


    static bool isOpen=false;

    public static bool GSXR_Is_EnablePointCloudData()
    {
        return isOpen;
    }
    public static void  GSXR_Set_PointCloudData(bool _isOpen)
    {
         isOpen = _isOpen;
    }

    public static int GSXR_Get_PointCloudData(ref int dataNum, ref ulong dataTimestamp, float[] dataArray)
    {
        if (isOpen == false) {
            dataNum = 0;
            return 0;
        }
        if (API_GSXR_Slam.SlamManager && API_GSXR_Slam.SlamManager.plugin != null)
        {
            return API_GSXR_Slam.SlamManager.plugin.GSXR_Get_PointCloudData(ref dataNum, ref dataTimestamp, dataArray);
        }
        return 0;
    }

    public static int GSXR_Get_OfflineMapRelocState()
    {
        if (API_GSXR_Slam.SlamManager && API_GSXR_Slam.SlamManager.plugin != null)
        {
            return API_GSXR_Slam.SlamManager.plugin.GSXR_Get_OfflineMapRelocState();
        }
        return 0;
    }

    public static int GSXR_ResaveMap(string path)
    {
        if (API_GSXR_Slam.SlamManager && API_GSXR_Slam.SlamManager.plugin != null)
        {
            return API_GSXR_Slam.SlamManager.plugin.GSXR_ResaveMap(path);
        }
        return 0;
    }

    public static void GSXR_SaveMap() 
    {
        if (API_GSXR_Slam.SlamManager && API_GSXR_Slam.SlamManager.plugin != null) {
            API_GSXR_Slam.SlamManager.plugin.GSXR_SaveMap();
        }
    }

    public static int GSXR_Get_Gnss(ref double dt, float[] gnss)
    {
        if (API_GSXR_Slam.SlamManager && API_GSXR_Slam.SlamManager.plugin != null)
        {
            return API_GSXR_Slam.SlamManager.plugin.GSXR_Get_Gnss(ref dt, gnss);
        }
        return 0;
    }

    public static int GSXR_Get_PanelNum()
    {
        if (API_GSXR_Slam.SlamManager && API_GSXR_Slam.SlamManager.plugin != null)
        {
            return API_GSXR_Slam.SlamManager.plugin.GSXR_Get_PanelNum();
        }
        return 0;
    }

    public static int GSXR_Get_PanelInfo(float[] info)
    {
        if (API_GSXR_Slam.SlamManager && API_GSXR_Slam.SlamManager.plugin != null)
        {
            return API_GSXR_Slam.SlamManager.plugin.GSXR_Get_PanelInfo(info);
        }
        return 0;
    }
}
