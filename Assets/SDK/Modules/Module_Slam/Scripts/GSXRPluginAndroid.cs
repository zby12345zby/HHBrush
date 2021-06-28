using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using SC.XR.Unity;

class GSXRPluginAndroid : GSXRPlugin
{

    private const string DLLName = "gsxrplugin";

    [DllImport(DLLName)]
	private static extern IntPtr GetRenderEventFunc();

    [DllImport(DLLName)]
    private static extern bool GSXRIsInitialized();

    [DllImport(DLLName)]
    private static extern bool GSXRIsRunning();

    [DllImport(DLLName)]
    private static extern bool GSXRCanBeginVR();

    [DllImport(DLLName)]
	private static extern void GSXRInitializeEventData(IntPtr activity);

	[DllImport(DLLName)]
	private static extern void GSXRSubmitFrameEventData(int frameIndex, float fieldOfView, int frameType);

    [DllImport(DLLName)]
    private static extern void GSXRSetupLayerCoords(int layerIndex, float[] lowerLeft, float[] lowerRight, float[] upperLeft, float[] upperRight);
    [DllImport(DLLName)]
    private static extern void GSXRSetupLayerData(int layerIndex, int sideMask, int textureId, int textureType, int layerFlags);

    [DllImport(DLLName)]
	private static extern void GSXRSetTrackingModeEventData(int mode);

	[DllImport(DLLName)]
	private static extern void GSXRSetPerformanceLevelsEventData(int newCpuPerfLevel, int newGpuPerfLevel);

    [DllImport(DLLName)]
    private static extern void GSXRSetEyeEventData(int sideMask, int layerMask);

    [DllImport(DLLName)]
	private static extern void GSXRSetColorSpace(int colorSpace);

    [DllImport(DLLName)]
    private static extern void GSXRSetFrameOption(uint frameOption);

    [DllImport(DLLName)]
    private static extern void GSXRUnsetFrameOption(uint frameOption);

    [DllImport(DLLName)]
    private static extern void GSXRSetVSyncCount(int vSyncCount);

    [DllImport(DLLName)]
    private static extern int GSXRGetPredictedPose(ref float rx,
                                                   ref float ry,
                                                   ref float rz,
                                                   ref float rw,
                                                   ref float px,
                                                   ref float py,
                                                   ref float pz,
                                                   int frameIndex,
                                                   bool isMultiThreadedRender);


    [DllImport(DLLName)]
    private static extern bool GSXRRecenterTrackingPose();

    [DllImport(DLLName)]
    private static extern int GSXRGetTrackingMode();

    [DllImport(DLLName)]
    private static extern void GSXRGetDeviceInfo(ref int displayWidthPixels,
	                                            ref int displayHeightPixels,
	                                            ref float displayRefreshRateHz,
	                                            ref int targetEyeWidthPixels,
	                                            ref int targetEyeHeightPixels,
	                                            ref float targetFovXRad,
	                                       		ref float targetFovYRad,
                                                ref float leftFrustumLeft, ref float leftFrustumRight, ref float leftFrustumBottom, ref float leftFrustumTop, ref float leftFrustumNear, ref float leftEyeFrustumFar,
                                                ref float rightFrustumLeft, ref float rightFrustumRight, ref float rightFrustumBottom, ref float rightFrustumTop, ref float rightFrustumNear, ref float rightFrustumFar,
                                                ref float targetfrustumConvergence, ref float targetFrustumPitch);

    [DllImport(DLLName)]
    private static extern void GSXRSetFrameOffset(float[] delta);

    [DllImport(DLLName)]
    private static extern void GSXRSetFoveationParameters(int textureId, int previousId, float focalPointX, float focalPointY, float foveationGainX, float foveationGainY, float foveationArea, float foveationMinimum);

    [DllImport(DLLName)]
    private static extern bool GSXRPollEvent(ref int eventType, ref uint deviceId, ref float eventTimeStamp, int eventDataCount, uint[] eventData);
    
	//---------------------------------------------------------------------------------------------
	// Conotroller Apis
	//---------------------------------------------------------------------------------------------
    [DllImport(DLLName)]
    private static extern int GSXRControllerStartTracking(string desc);
    
    [DllImport(DLLName)]
    private static extern void GSXRControllerStopTracking(int handle);
    
    [DllImport(DLLName)]
	private static extern void GSXRControllerGetState(int handle, int space, ref GSXRControllerState state);

	[DllImport(DLLName)]
	private static extern void GSXRControllerSendMessage(int handle, int what, int arg1, int arg2);

	[DllImport(DLLName)]
	private static extern int GSXRControllerQuery(int handle, int what, IntPtr mem, int size);

    private enum RenderEvent
	{
		Initialize,
		BeginVr,
		EndVr,
		BeginEye,
		EndEye,
		SubmitFrame,
        Foveation,
		Shutdown,
		RecenterTracking,
		SetTrackingMode,
		SetPerformanceLevels
	};

	public static GSXRPluginAndroid Create()
	{
		if(Application.isEditor)
		{
			Debug.LogError("GSXRPlugin not supported in unity editor!");
			throw new InvalidOperationException();
		}

		return new GSXRPluginAndroid();
	}


	private GSXRPluginAndroid() {}

	private void IssueEvent(RenderEvent e)
	{
		// Queue a specific callback to be called on the render thread
		GL.IssuePluginEvent(GetRenderEventFunc(), (int)e);
	}

    public override bool IsInitialized() { return GSXRIsInitialized(); }

    public override bool IsRunning() { return GSXRIsRunning(); }

    public override IEnumerator Initialize()
	{
        //yield return new WaitUntil(() => GSXRIsInitialized() == false);  // Wait for shutdown

        yield return base.Initialize();

        if(Application.platform == RuntimePlatform.Android) {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            GSXRInitializeEventData(activity.GetRawObject()); 
        }
        IssueEvent(RenderEvent.Initialize);
		yield return new WaitUntil (() => GSXRIsInitialized () == true);

        yield return null;  // delay one frame - fix for re-init w multi-threaded rendering

        deviceInfo = GetDeviceInfo();
    }

	public override IEnumerator BeginVr(int cpuPerfLevel, int gpuPerfLevel)
	{
        //yield return new WaitUntil(() => GSXRIsRunning() == false);  // Wait for EndVr

        yield return base.BeginVr(cpuPerfLevel, gpuPerfLevel);

        // float[6]: x, y, z, w, u, v
        float[] lowerLeft = { -1f, -1f, 0f, 1f, 0f, 0f };
        float[] upperLeft = { -1f,  1f, 0f, 1f, 0f, 1f };
        float[] upperRight = { 1f,  1f, 0f, 1f, 1f, 1f };
        float[] lowerRight = { 1f, -1f, 0f, 1f, 1f, 0f };
        GSXRSetupLayerCoords(-1, lowerLeft, lowerRight, upperLeft, upperRight);    // Layers/All

        GSXRSetPerformanceLevelsEventData(cpuPerfLevel, gpuPerfLevel);
		
		ColorSpace space = QualitySettings.activeColorSpace;
		if(space == ColorSpace.Gamma)
		{
			// 0 == kColorSpaceLinear from slamApi.h
			GSXRSetColorSpace(0);   //Unity will be supplying gamma space eye buffers into warp so we want
								   //to setup a linear color space display surface so no further gamma 
								   //correction is performed
		}
		else
		{
            // 1 == kColorSpaceSRGB from slamApi.h
            GSXRSetColorSpace(1);	//Unity will be supplying linear space eye buffers into warp so we want
									//to setup an sRGB color space display surface to properly convert
									//incoming linear values into sRGB
		}
        
		yield return new WaitUntil(() => GSXRCanBeginVR() == true);
        IssueEvent (RenderEvent.BeginVr);
        //yield return new WaitUntil(() => GSXRIsRunning() == true);
    }

    public override void EndVr()
	{
        base.EndVr();
        Debug.Log("EndVR...."+Time.frameCount);
		IssueEvent (RenderEvent.EndVr);
	}

	public override void BeginEye(int sideMask, float[] frameDelta)
	{
        GSXRSetFrameOffset(frameDelta);    // Enabled for foveation head orientation delta
        GSXRSetEyeEventData(sideMask, 0);
        IssueEvent (RenderEvent.BeginEye);
	}

	public override void EndEye(int sideMask, int layerMask)
	{
        GSXRSetEyeEventData(sideMask, layerMask);
        IssueEvent(RenderEvent.EndEye);
	}

    public override void SetTrackingMode(int modeMask)
    {
        GSXRSetTrackingModeEventData(modeMask);
		IssueEvent (RenderEvent.SetTrackingMode);
    }

	public override void SetFoveationParameters(int textureId, int previousId, float focalPointX, float focalPointY, float foveationGainX, float foveationGainY, float foveationArea, float foveationMinimum)
	{
		GSXRSetFoveationParameters(textureId, previousId, focalPointX, focalPointY, foveationGainX, foveationGainY, foveationArea, foveationMinimum);
	}

    public override void ApplyFoveation()
    {
        IssueEvent(RenderEvent.Foveation);
    }

    public override int GetTrackingMode()
    {
        return GSXRGetTrackingMode();
    }

    public override void SetPerformanceLevels(int newCpuPerfLevel, int newGpuPerfLevel)
    {
        GSXRSetPerformanceLevelsEventData((int)newCpuPerfLevel, (int)newGpuPerfLevel);
		IssueEvent (RenderEvent.SetPerformanceLevels);
    }

    public override void SetFrameOption(FrameOption frameOption)
    {
        GSXRSetFrameOption((uint)frameOption);
    }

    public override void UnsetFrameOption(FrameOption frameOption)
    {
        GSXRUnsetFrameOption((uint)frameOption);
    }

    public override void SetVSyncCount(int vSyncCount)
    {
        GSXRSetVSyncCount(vSyncCount);
    }

    public override bool RecenterTracking()
	{
        //IssueEvent (RenderEvent.RecenterTracking);
        return GSXRRecenterTrackingPose();
	}

    public override int GetPredictedPose(ref Quaternion orientation, ref Vector3 position, int frameIndex)
    {
        orientation.z = -orientation.z;
        position.x = -position.x;
        position.y = -position.y;

        int rv = GSXRGetPredictedPose(ref orientation.x, ref orientation.y, ref orientation.z, ref orientation.w,
                            ref position.x, ref position.y, ref position.z, frameIndex, SystemInfo.graphicsMultiThreaded);

        orientation.z = -orientation.z;
        position.x = -position.x;
        position.y = -position.y;

        return rv;
    }

    private float[] leftViewData = new float[16];
    private float[] rightViewData = new float[16];

    private float[] TData = new float[3];
    private float[] RData = new float[4];

    public override int GetHeadPose(ref HeadPose headPose, int frameIndex)
    {
        int rv = 0;


        if (!GSXR_Is_SupportOpticsCalibration()) {
            headPose.orientation.z = -headPose.orientation.z;
            headPose.position.x = -headPose.position.x;
            headPose.position.y = -headPose.position.y;

            rv = GSXRGetPredictedPose(ref headPose.orientation.x, ref headPose.orientation.y, ref headPose.orientation.z, ref headPose.orientation.w,
                                ref headPose.position.x, ref headPose.position.y, ref headPose.position.z, frameIndex, SystemInfo.graphicsMultiThreaded);
        } else {
            rv = GSXR_Get_LatestEyeMatrices(leftViewData, rightViewData, TData, RData, frameIndex, SystemInfo.graphicsMultiThreaded);

            leftViewMatrix.SetColumn(0, new Vector4(leftViewData[0], leftViewData[1], leftViewData[2], leftViewData[3]));
            leftViewMatrix.SetColumn(1, new Vector4(leftViewData[4], leftViewData[5], leftViewData[6], leftViewData[7]));
            leftViewMatrix.SetColumn(2, new Vector4(leftViewData[8], leftViewData[9], leftViewData[10], leftViewData[11]));
            leftViewMatrix.SetColumn(3, new Vector4(leftViewData[12], leftViewData[13], leftViewData[14], leftViewData[15]));

            rightViewMatrix.SetColumn(0, new Vector4(rightViewData[0], rightViewData[1], rightViewData[2], rightViewData[3]));
            rightViewMatrix.SetColumn(1, new Vector4(rightViewData[4], rightViewData[5], rightViewData[6], rightViewData[7]));
            rightViewMatrix.SetColumn(2, new Vector4(rightViewData[8], rightViewData[9], rightViewData[10], rightViewData[11]));
            rightViewMatrix.SetColumn(3, new Vector4(rightViewData[12], rightViewData[13], rightViewData[14], rightViewData[15]));

            //        Debug.Log($"leftViewMatrix [{leftViewMatrix.m00}, {leftViewMatrix.m01}, {leftViewMatrix.m02}, {leftViewMatrix.m03};" +
            //$"{leftViewMatrix.m10}, {leftViewMatrix.m11}, {leftViewMatrix.m12}, {leftViewMatrix.m13};" +
            //$"{leftViewMatrix.m20}, {leftViewMatrix.m21}, {leftViewMatrix.m22}, {leftViewMatrix.m23};" +
            //$"{leftViewMatrix.m30}, {leftViewMatrix.m31}, {leftViewMatrix.m32}, {leftViewMatrix.m33}]");

            //        Debug.Log($"rightViewMatrix [{rightViewMatrix.m00}, {rightViewMatrix.m01}, {rightViewMatrix.m02}, {rightViewMatrix.m03};" +
            //$"{rightViewMatrix.m10}, {rightViewMatrix.m11}, {rightViewMatrix.m12}, {rightViewMatrix.m13};" +
            //$"{rightViewMatrix.m20}, {rightViewMatrix.m21}, {rightViewMatrix.m22}, {rightViewMatrix.m23};" +
            //$"{rightViewMatrix.m30}, {rightViewMatrix.m31}, {rightViewMatrix.m32}, {rightViewMatrix.m33}]");

            headPose.orientation.x = RData[0];
            headPose.orientation.y = RData[1];
            headPose.orientation.z = RData[2];
            headPose.orientation.w = RData[3];

            headPose.position.x = TData[0];
            headPose.position.y = TData[1];
            headPose.position.z = TData[2];
        }


        headPose.orientation.z = -headPose.orientation.z;
        headPose.position.x = -headPose.position.x;
        headPose.position.y = -headPose.position.y;

        return rv;
    }

	
    public override DeviceInfo GetDeviceInfo()
	{
		DeviceInfo info = new DeviceInfo();

		GSXRGetDeviceInfo (ref info.displayWidthPixels,
		                  ref info.displayHeightPixels,
		                  ref info.displayRefreshRateHz,
		                  ref info.targetEyeWidthPixels,
		                  ref info.targetEyeHeightPixels,
		                  ref info.targetFovXRad,
		                  ref info.targetFovYRad,
                          ref info.targetFrustumLeft.left, ref info.targetFrustumLeft.right, ref info.targetFrustumLeft.bottom, ref info.targetFrustumLeft.top, ref info.targetFrustumLeft.near, ref info.targetFrustumLeft.far,
                          ref info.targetFrustumRight.left, ref info.targetFrustumRight.right, ref info.targetFrustumRight.bottom, ref info.targetFrustumRight.top, ref info.targetFrustumRight.near, ref info.targetFrustumRight.far,
                          ref info.targetFrustumConvergence, ref info.targetFrustumPitch);

		return info;
	}

    public override void SubmitFrame(int frameIndex, float fieldOfView, int frameType)
	{
        int i;
        int layerCount = 0;
        if (eyes != null) for (i = 0; i < eyes.Length; i++)
        {
            var eye = eyes[i];
            if (eyes[i].isActiveAndEnabled == false || eye.TextureId == 0 || eye.Side == 0) continue;
            if (eye.imageTransform != null && eye.imageTransform.gameObject.activeSelf == false) continue;
            GSXRSetupLayerData(layerCount, (int)eye.Side, eye.TextureId, eye.ImageType == GSXREye.eType.EglTexture ? 2 : 0, eye.layerDepth > 0 ? 0x0 : 0x2);
            float[] lowerLeft = { eye.clipLowerLeft.x, eye.clipLowerLeft.y, eye.clipLowerLeft.z, eye.clipLowerLeft.w, eye.uvLowerLeft.x, eye.uvLowerLeft.y };
            float[] upperLeft = { eye.clipUpperLeft.x, eye.clipUpperLeft.y, eye.clipUpperLeft.z, eye.clipUpperLeft.w, eye.uvUpperLeft.x, eye.uvUpperLeft.y };
            float[] upperRight = { eye.clipUpperRight.x, eye.clipUpperRight.y, eye.clipUpperRight.z, eye.clipUpperRight.w, eye.uvUpperRight.x, eye.uvUpperRight.y };
            float[] lowerRight = { eye.clipLowerRight.x, eye.clipLowerRight.y, eye.clipLowerRight.z, eye.clipLowerRight.w, eye.uvLowerRight.x, eye.uvLowerRight.y };
            GSXRSetupLayerCoords(layerCount, lowerLeft, lowerRight, upperLeft, upperRight);
            layerCount++;
        }

        if (overlays != null) for (i = 0; i < overlays.Length; i++)
        {
            var overlay = overlays[i];
            if (overlay.isActiveAndEnabled == false || overlay.TextureId == 0 || overlay.Side == 0) continue;
            if (overlay.imageTransform != null && overlay.imageTransform.gameObject.activeSelf == false) continue;
            GSXRSetupLayerData(layerCount, (int)overlay.Side, overlay.TextureId, overlay.ImageType == GSXROverlay.eType.EglTexture ? 2 : 0, 0x1);
            float[] lowerLeft = { overlay.clipLowerLeft.x, overlay.clipLowerLeft.y, overlay.clipLowerLeft.z, overlay.clipLowerLeft.w, overlay.uvLowerLeft.x, overlay.uvLowerLeft.y };
            float[] upperLeft = { overlay.clipUpperLeft.x, overlay.clipUpperLeft.y, overlay.clipUpperLeft.z, overlay.clipUpperLeft.w, overlay.uvUpperLeft.x, overlay.uvUpperLeft.y };
            float[] upperRight = { overlay.clipUpperRight.x, overlay.clipUpperRight.y, overlay.clipUpperRight.z, overlay.clipUpperRight.w, overlay.uvUpperRight.x, overlay.uvUpperRight.y };
            float[] lowerRight = { overlay.clipLowerRight.x, overlay.clipLowerRight.y, overlay.clipLowerRight.z, overlay.clipLowerRight.w, overlay.uvLowerRight.x, overlay.uvLowerRight.y };
            GSXRSetupLayerCoords(layerCount, lowerLeft, lowerRight, upperLeft, upperRight);
            layerCount++;
        }

        for (i = layerCount; i < GSXRManager.RenderLayersMax; i++)
        {
            GSXRSetupLayerData(i, 0, 0, 0, 0);
        }

        GSXRSubmitFrameEventData(frameIndex, fieldOfView, frameType);
		IssueEvent (RenderEvent.SubmitFrame);
	}

	public override void Shutdown()
	{
        IssueEvent (RenderEvent.Shutdown);

        base.Shutdown();
	}

    public override bool PollEvent(ref GSXRManager.SlamEvent frameEvent)
    {
        uint[] dataBuffer = new uint[2];
        int dataCount = Marshal.SizeOf(frameEvent.eventData) / sizeof(uint);
		int eventType = 0;
        bool isEvent = GSXRPollEvent(ref eventType, ref frameEvent.deviceId, ref frameEvent.eventTimeStamp, dataCount, dataBuffer);
		frameEvent.eventType = (GSXRManager.slamEventType)(eventType);
        switch (frameEvent.eventType)
        {
            case GSXRManager.slamEventType.kEventThermal:
                //Debug.LogFormat("PollEvent: data {0} {1}", dataBuffer[0], dataBuffer[1]);
                frameEvent.eventData.thermal.zone = (GSXRManager.slamThermalZone)dataBuffer[0];
                frameEvent.eventData.thermal.level = (GSXRManager.slamThermalLevel)dataBuffer[1];
                break;
        }
        return isEvent;
    }

	//---------------------------------------------------------------------------------------------
	//Controller Apis
	//---------------------------------------------------------------------------------------------

	/// <summary>
	/// Controllers the start tracking.
	/// </summary>
	/// <returns>The start tracking.</returns>
	/// <param name="desc">Desc.</param>
	//---------------------------------------------------------------------------------------------
    public override int ControllerStartTracking(string desc)
    {
        return GSXRControllerStartTracking(desc);
    }
    
	/// <summary>
	/// Controllers the stop tracking.
	/// </summary>
	/// <param name="handle">Handle.</param>
	//---------------------------------------------------------------------------------------------
	public override void ControllerStopTracking(int handle)
    {
        GSXRControllerStopTracking(handle);
    }

	/// <summary>
	/// Dumps the state.
	/// </summary>
	/// <param name="state">State.</param>
	//---------------------------------------------------------------------------------------------
	private void dumpState(GSXRControllerState state)
	{
		String s = "{" + state.rotation + "}\n";
		s += "[" + state.position + "]\n";
		s += "<" + state.timestamp + ">\n";

		Debug.Log (s);
	}
    
	/// <summary>
	/// Controllers the state of the get.
	/// </summary>
	/// <returns>The get state.</returns>
	/// <param name="handle">Handle.</param>
	//---------------------------------------------------------------------------------------------
	public override GSXRControllerState ControllerGetState(int handle, int space)
    {
		GSXRControllerState state = new GSXRControllerState();
		GSXRControllerGetState (handle, space, ref state);
		//dumpState (state);
 		return state;
    }

	/// <summary>
	/// Controllers the send event.
	/// </summary>
	/// <param name="handle">Handle.</param>
	/// <param name="what">What.</param>
	/// <param name="arg1">Arg1.</param>
	/// <param name="arg2">Arg2.</param>
	//---------------------------------------------------------------------------------------------
	public override void ControllerSendMessage(int handle, GSXRController.slamControllerMessageType what, int arg1, int arg2)
	{
		GSXRControllerSendMessage (handle, (int)what, arg1, arg2);
	}

	/// <summary>
	/// Controllers the query.
	/// </summary>
	/// <returns>The query.</returns>
	/// <param name="handle">Handle.</param>
	/// <param name="what">What.</param>
	/// <param name="mem">Mem.</param>
	/// <param name="size">Size.</param>
	//---------------------------------------------------------------------------------------------
	public override object ControllerQuery(int handle, GSXRController.slamControllerQueryType what)
	{
		int memorySize = 0;
		IntPtr memory = IntPtr.Zero;
		object result = null;

		System.Type typeOfObject = null;

		switch(what)
		{
			case GSXRController.slamControllerQueryType.kControllerBatteryRemaining:
				{
					typeOfObject = typeof(int);
					memorySize = System.Runtime.InteropServices.Marshal.SizeOf (typeOfObject);
					memory = System.Runtime.InteropServices.Marshal.AllocHGlobal (memorySize);	
				}
				break;
			case GSXRController.slamControllerQueryType.kControllerControllerCaps:
				{
                    typeOfObject = typeof(SlamControllerCaps);
					memorySize = System.Runtime.InteropServices.Marshal.SizeOf (typeOfObject);
					memory = System.Runtime.InteropServices.Marshal.AllocHGlobal (memorySize);	
				}
				break;				
		}

		int writtenBytes = GSXRControllerQuery (handle, (int)what, memory, memorySize);

		if (memorySize == writtenBytes) {
			result = System.Runtime.InteropServices.Marshal.PtrToStructure (memory, typeOfObject);
		}
			
		if (memory != IntPtr.Zero) {
			Marshal.FreeHGlobal (memory);
		}
			
		return result;
	}



    #region Controller

    [DllImport(DLLName)]
    public static extern bool GSXRIsSupportController();

    [DllImport(DLLName)]
    public static extern int GSXRControllerMode();

    [DllImport(DLLName)]
    public static extern int GSXRGetControllerNum();

    [DllImport(DLLName)]
    public static extern void GSXRSetControllerKeyEventCallback(OnKeyEvent _event);

    [DllImport(DLLName)]
    public static extern void GSXR_Unity_SetControllerTouchPanelCallback(OnTouchPanelEvent _event);

    [DllImport(DLLName)]
    public static extern void GSXRSetControllerKeyTouchEventCallback(OnKeyTouchEvent _event);

    [DllImport(DLLName)]
    public static extern void GSXRSetControllerRockerCallback(OnTouchEvent _event);

    [DllImport(DLLName)]
    public static extern void GSXRSetControllerTriggerCallback(OnHallEvent _event);

    [DllImport(DLLName)]
    public static extern void GSXRSetControllerChargingEventCallback(OnChargingEvent _event);

    [DllImport(DLLName)]
    public static extern void GSXRSetControllerBatteryEventCallback(OnBatteryEvent _event);

    [DllImport(DLLName)]
    public static extern void GSXRSetControllerConnectEventCallback(OnConnectEvent _event);

    [DllImport(DLLName)]
    public static extern int GSXRGetControllerBattery(int lr);

    [DllImport(DLLName)]
    public static extern int GSXRGetControllerVersion(int lr);

    [DllImport(DLLName)]
    public static extern int GSXRGetControllerList(int lr);

    [DllImport(DLLName)]
    public static extern bool GSXRIsControllerConnect(int lr);

    [DllImport(DLLName)]
    public static extern void GSXRSetControllerLed(int enable);

    [DllImport(DLLName)]
    public static extern void GSXRSetControllerVibrate(int value);

    [DllImport(DLLName)]
    public static extern int GSXRGetControllerPosture(float[] outOrientationArray, int lr);

    public override bool GSXR_Is_SupportController() { return GSXRIsSupportController(); }

    public override int GSXR_Get_ControllerMode() { return GSXRControllerMode(); }

    public override int GSXR_Get_ControllerNum() { return GSXRGetControllerNum(); }

    public override void GSXR_Set_ControllerKeyEventCallback(OnKeyEvent _event) { GSXRSetControllerKeyEventCallback(_event); }

    public override void GSXR_Set_ControllerTouchPanelCallback(OnTouchPanelEvent _event) { GSXR_Unity_SetControllerTouchPanelCallback(_event); }

    public override void GSXR_Set_ControllerKeyTouchEventCallback(OnKeyTouchEvent _event) { GSXRSetControllerKeyTouchEventCallback(_event); }

    public override void GSXR_Set_ControllerRockerCallback(OnTouchEvent _event) { GSXRSetControllerRockerCallback(_event); }

    public override void GSXR_Set_ControllerHallCallback(OnHallEvent _event) { GSXRSetControllerTriggerCallback(_event); }

    public override void GSXR_Set_ControllerChargingEventCallback(OnChargingEvent _event) { GSXRSetControllerChargingEventCallback(_event); }

    public override void GSXR_Set_ControllerBatteryEventCallback(OnBatteryEvent _event) { GSXRSetControllerBatteryEventCallback(_event); }

    public override void GSXR_Set_ControllerConnectEventCallback(OnConnectEvent _event) { GSXRSetControllerConnectEventCallback(_event); }


    public override int GSXR_Get_ControllerBattery(int lr) { return GSXRGetControllerBattery(lr); }

    public override int GSXR_Get_ControllerVersion(int lr) { return GSXRGetControllerVersion(lr); }

    public override int GSXR_Get_ControllerList(int lr) { return GSXRGetControllerList(lr); }

    public override bool GSXR_Is_ControllerConnect(int lr) { return GSXRIsControllerConnect(lr); }
	
    public override void GSXR_Set_ControllerLed(int lr) { GSXRSetControllerLed(lr); }
	
    public override void GSXR_Set_ControllerVibrate(int lr) { GSXRSetControllerVibrate(lr); }
	
    public override int GSXR_Get_ControllerPosture(float[] outOrientationArray, int lr) {return GSXRGetControllerPosture(outOrientationArray, lr);}

    #endregion Controller

    #region HandTracking
    [DllImport(DLLName)]
    private static extern bool GSXRIsSupportHandTracking();

    [DllImport(DLLName)]
    protected static extern void GSXRStartGesture(Action<int> func);

    [DllImport(DLLName)]
    public static extern void GSXRStopGesture();

    [DllImport(DLLName)]
    private static extern void GSXRSetGestureCallback(Action<int> func);

    [DllImport(DLLName)]
    private static extern int GSXRGetHandTrackingData(float[] model, float[] pose);

    //for test
    [DllImport(DLLName)]
    private static extern int GSXRSetGestureData(float[] model, float[] pose);

    [DllImport(DLLName)]
    private static extern int ScHANDTRACK_GetGestureWithIdx(ref ulong index, float[] model, float[] pose);

    [DllImport(DLLName)]
    private static extern int GSXRSetGestureModelCallback(Action func);

    [DllImport(DLLName)]
    protected static extern void GSXRSetHandTrackingLowPowerWarningCallback(Action<int> func);

    public override bool GSXR_Is_SupportHandTracking() {
        return GSXRIsSupportHandTracking();
    }
    public override void GSXR_StartHandTracking(Action<int> func) {
        GSXRStartGesture(func);
    }

    public override void GSXR_StopHandTracking() {
        GSXRStopGesture();
    }

    public override void GSXR_Get_HandTrackingData(float[] mode, float[] pose) {
        GSXRGetHandTrackingData(mode, pose);
    }
    public override int GSXR_Set_HandTrackingData(float[] mode, float[] pose) {
        return GSXRSetGestureData(mode, pose);
    }

    public override int GSXR_Get_HandTrackingGestureIdx(ref ulong index, float[] model, float[] pose) {
        return ScHANDTRACK_GetGestureWithIdx(ref index, model, pose);
    }
    public override void GSXR_Set_HandTrackingCallBack(Action<int> callback) {
        GSXRSetGestureCallback(callback);
    }
    public override int GSXR_Set_HandTrackingModelDataCallBack(Action callback) {
        return GSXRSetGestureModelCallback(callback);
    }
    public override void GSXR_Set_HandTrackingLowPowerWarningCallback(Action<int> func) {
        GSXRSetHandTrackingLowPowerWarningCallback(func);
    }

    #endregion HandTracking

    #region Deflection
    [DllImport(DLLName)]
    private static extern float GSXRFetchDeflection();

    [DllImport(DLLName)]
    private static extern void GSXRUpdateRelativeDeflection(int deflection);

    public override float GSXR_Get_Deflection() {
        return GSXRFetchDeflection();
    }
    public override void GSXR_Set_RelativeDeflection(int deflection) {
        GSXRUpdateRelativeDeflection(deflection);
    }
    #endregion Deflection

    #region PointCloud & Map

    [DllImport(DLLName)]
    private static extern bool GSXRSupportMap();

    [DllImport(DLLName)]
    private static extern bool GSXRSupportPanel();

    [DllImport(DLLName)]
    private static extern int GSXRGetPointCloudData(ref int dataNum,ref ulong dataTimestamp,float[] dataArray);

    [DllImport(DLLName)]
    private static extern int GSXRGetOfflineMapRelocState();

    /// <summary>
    /// 将地图信息保存在path里，是ply格式文件，存的是点云信息
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    [DllImport(DLLName)]
    private static extern int GSXRResaveMap(string path);

    [DllImport(DLLName)]
    private static extern void GSXRSaveMap();

    [DllImport(DLLName)]
    private static extern int ScGetGnss(ref double dt, float[] gnss);

    /// <summary>
    /// Get the identified plane
    /// </summary>
    /// <returns></returns>
    [DllImport(DLLName)]
    private static extern int GSXRGetPanelNum();

    /// <summary>
    /// Get the plane Infomations
    /// 1：id 
    /// 2：nPoints 
    /// 3: 20个点的坐标 :如果没20个点，后面为空 
    /// 4: createPoint 平面顶点
    /// 5: normal 法向量
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    [DllImport(DLLName)]
    private static extern int GSXRGetPlaneData(float[] info);

    public override bool GSXR_Support_Map() {
        return GSXRSupportMap();
    }

    public override bool GSXR_Support_Panel() {
        return GSXRSupportPanel();
    }

    public override int GSXR_Get_PointCloudData(ref int dataNum, ref ulong dataTimestamp, float[] dataArray) {
        return GSXRGetPointCloudData(ref dataNum, ref dataTimestamp, dataArray);
    }

    public override int GSXR_Get_OfflineMapRelocState() {
        return GSXRGetOfflineMapRelocState();
    }

    public override int GSXR_ResaveMap(string path) {
        return GSXRResaveMap(path);
    }
    public override void GSXR_SaveMap() {
        GSXRSaveMap();
    }

    public override int GSXR_Get_Gnss(ref double dt, float[] gnss) {
        return ScGetGnss(ref dt, gnss);
    }

    public override int GSXR_Get_PanelNum() {
        return GSXRGetPanelNum();
    }

    public override int GSXR_Get_PanelInfo(float[] info) {
        return GSXRGetPlaneData(info);
    }

    #endregion  PointCloud & Map

    #region FishEye Data
    [DllImport(DLLName)]
    public static extern int GSXRGetFishEyeInfo(ref int num, ref int outWidth, ref int outHeight);

    [DllImport(DLLName)]
    public static extern void GSXRGetLatestCameraFrameData(ref bool outBUdate, ref uint outCurrFrameIndex, ref ulong outFrameExposureNano,
                byte[] outFrameData, float[] outTRDataArray);

    [DllImport(DLLName)]
    public static extern void GSXRGetLatestCameraFrameDataNoTransform(ref bool outBUdate, ref uint outCurrFrameIndex, ref ulong outFrameExposureNano,
                byte[] outFrameData, float[] outTRDataArray);

    [DllImport(DLLName)]
    private static extern int GSXRGetLatestCameraBinocularData(ref bool outBUdate, ref uint outCurrFrameIndex, ref ulong outFrameExposureNano, 
                byte[] outLeftFrameData, byte[] outRightFrameData);


    public override int GSXR_Get_FishEyeInfo(ref int num, ref int outWidth, ref int outHeight) {
        return GSXRGetFishEyeInfo(ref num, ref outWidth, ref outHeight);
    }

    public override void GSXR_Get_LatestFishEyeFrameData(ref bool outBUdate, ref uint outCurrFrameIndex, ref ulong outFrameExposureNano, byte[] outFrameData, float[] outTRDataArray) {
        GSXRGetLatestCameraFrameData(ref outBUdate, ref outCurrFrameIndex, ref outFrameExposureNano, outFrameData, outTRDataArray);
        if (outBUdate == false) {
            Debug.LogError("Error: Please Check Slamconfig prop: gUseXXXCamera = true");
        }
    }

    public override void GSXR_Get_LatestFishEyeFrameDataNoTransform(ref bool outBUdate, ref uint outCurrFrameIndex, ref ulong outFrameExposureNano,byte[] outFrameData, float[] outTRDataArray) {
        GSXRGetLatestCameraFrameDataNoTransform(ref outBUdate, ref outCurrFrameIndex, ref outFrameExposureNano, outFrameData, outTRDataArray);
        if (outBUdate == false) {
            Debug.LogError("Error: Please Check Slamconfig prop: gUseXXXCamera = true");
        }
    }

    public override int GSXR_Get_LatestFishEyeBinocularData(ref bool outBUdate, ref uint outCurrFrameIndex, ref ulong outFrameExposureNano, byte[] outLeftFrameData, byte[] outRightFrameData) {
        return GSXRGetLatestCameraBinocularData(ref outBUdate, ref outCurrFrameIndex, ref outFrameExposureNano, outLeftFrameData, outRightFrameData);
    }
    #endregion FishEye Data

    #region Optics Calibration

    [DllImport(DLLName)]
    private static extern bool GSXRIsSupportOpticsCalibration();

    public override bool GSXR_Is_SupportOpticsCalibration() {
        return GSXRIsSupportOpticsCalibration();
    }

    [DllImport(DLLName)]
    private static extern int GSXRGetTransformMatrix(ref bool outBLoaded, float[] outTransformArray);

    public override int GSXR_Get_TransformMatrix(ref bool outBLoaded, float[] outTransformArray) {
        return GSXRGetTransformMatrix(ref outBLoaded, outTransformArray);
    }

    [DllImport(DLLName)]
    private static extern int GSXRGetLatestEyeMatrices(float[] outLeftEyeMatrix,
                                               float[] outRightEyeMatrix,
                                               float[] outT,
                                               float[] outR,
                                               int frameIndex,
                                               bool isMultiThreadedRender);

    public override int GSXR_Get_LatestEyeMatrices(float[] outLeftEyeMatrix, float[] outRightEyeMatrix, float[] outT, float[] outR, int frameIndex, bool isMultiThreadedRender) {
        return GSXRGetLatestEyeMatrices(outLeftEyeMatrix, outRightEyeMatrix, outT, outR, frameIndex, isMultiThreadedRender);
    }

    #endregion Optics Calibration

    #region EyeTracking
    [DllImport(DLLName)]
    private static extern bool GSXRSupportEyeTracking();
    public override bool GSXR_Support_EyeTracking() { return GSXRSupportEyeTracking(); }

    [DllImport(DLLName)]
    private static extern int GSXRStartEyeTracking();
    public override int GSXR_Start_EyeTracking() { return GSXRStartEyeTracking(); }

    [DllImport(DLLName)]
    private static extern int GSXRStopEyeTracking();
    public override int GSXR_Stop_EyeTracking() { return GSXRStopEyeTracking(); }

    [DllImport(DLLName)]
    private static extern int GSXRGetEyeFocus(ref float outFocusX, ref float outFocusY);
    public override int GSXR_Get_EyeFocus(ref float outFocusX, ref float outFocusY) { return GSXRGetEyeFocus(ref outFocusX,ref outFocusY); }


    [DllImport(DLLName)]
    private static extern int GSXRGetEyePose(ref int leftStatus,
                                        ref int rightStatus,
                                        ref int combinedStatus,
                                        ref float leftOpenness,
                                        ref float rightOpenness,
                                        ref float leftDirectionX,
                                        ref float leftDirectionY,
                                        ref float leftDirectionZ,
                                        ref float leftPositionX,
                                        ref float leftPositionY,
                                        ref float leftPositionZ,
                                        ref float rightDirectionX,
                                        ref float rightDirectionY,
                                        ref float rightDirectionZ,
                                        ref float rightPositionX,
                                        ref float rightPositionY,
                                        ref float rightPositionZ,
                                        ref float combinedDirectionX,
                                        ref float combinedDirectionY,
                                        ref float combinedDirectionZ,
                                        ref float combinedPositionX,
                                        ref float combinedPositionY,
                                        ref float combinedPositionZ,
                                        int frameIndex);

    public override int GSXR_Get_EyePose(ref EyePose eyePose, int frameIndex = -1) {
        int rv = GSXRGetEyePose(
            ref eyePose.leftStatus, ref eyePose.rightStatus, ref eyePose.combinedStatus,
            ref eyePose.leftOpenness, ref eyePose.rightOpenness,
            ref eyePose.leftDirection.x, ref eyePose.leftDirection.y, ref eyePose.leftDirection.z,
            ref eyePose.leftPosition.x, ref eyePose.leftPosition.y, ref eyePose.leftPosition.z,
            ref eyePose.rightDirection.x, ref eyePose.rightDirection.y, ref eyePose.rightDirection.z,
            ref eyePose.rightPosition.x, ref eyePose.rightPosition.y, ref eyePose.rightPosition.z,
            ref eyePose.combinedDirection.x, ref eyePose.combinedDirection.y, ref eyePose.combinedDirection.z,
            ref eyePose.combinedPosition.x, ref eyePose.combinedPosition.y, ref eyePose.combinedPosition.z,
            frameIndex);

        return rv;
    }


    #endregion EyeTracking

    #region USBDisconnect

    [DllImport(DLLName)]
    public static extern void GSXRSetAppExit(Action callBack);
    public override void GSXR_Set_GlassDisconnectedCallBack(Action callBack) {
        GSXRSetAppExit(callBack);
    }
    #endregion

    #region luncher
    [DllImport(DLLName)]
    public static extern int ScInitLayer();

    [DllImport(DLLName)]
    public static extern int ScStartLayerRendering();

    [DllImport(DLLName)]
    public static extern int ScGetAllLayersData(ref GSXRManager.SCAllLayers outAllLayers);

    [DllImport(DLLName)]
    public static extern int ScEndLayerRendering(ref GSXRManager.SCAllLayers allLayers);

    [DllImport(DLLName)]
    public static extern int ScDestroyLayer();

    [DllImport(DLLName)]
    public static extern int ScUpdateModelMatrix(UInt32 layerId, float[] modelMatrixArray);

    [DllImport(DLLName)]
    public static extern int ScSendActionBarCMD(UInt32 layerId, int cmd);

    [DllImport(DLLName)]
    public static extern int ScSetForcedDisplaySize(int width, int height);

    [DllImport(DLLName)]
    public static extern int ScInjectMotionEvent(UInt32 layerId, int displayID, int action, float x, float y);

    public override int initLayer() {
        return ScInitLayer();
    }
    public override int startLayerRendering() {
        return ScStartLayerRendering();
    }
    public override int getAllLayersData(ref GSXRManager.SCAllLayers outAllLayers) {
        return ScGetAllLayersData(ref outAllLayers);
    }
    public override int endLayerRendering(ref GSXRManager.SCAllLayers allLayers) {
        return ScEndLayerRendering(ref allLayers);
    }
    public override int destroyLayer() {
        return ScDestroyLayer();
    }

    public override int updateModelMatrix(UInt32 layerId, float[] modelMatrixArray) {
        return ScUpdateModelMatrix(layerId, modelMatrixArray);
    }

    public override int sendActionBarCMD(UInt32 layerId, int cmd) {
        return ScSendActionBarCMD(layerId, cmd);
    }

    public override int setForcedDisplaySize(int width, int height)
    {
        return ScSetForcedDisplaySize(width, height);
    }

    public override int injectMotionEvent(UInt32 layerId, int displayID, int action, float x, float y) {
        return ScInjectMotionEvent(layerId, displayID, action, x, y);
    }
    #endregion

    #region Device

    [DllImport(DLLName)]
    private static extern int GSXR_Unity_SetMeshOffset(int type, float value);

    [DllImport(DLLName)]
    private static extern float GSXR_Unity_GetMeshOffset(int type);

    public override int GSXR_SetMeshOffset(int type, float value) {
        return GSXR_Unity_SetMeshOffset(type, value);
    }

    public override float GSXR_GetMeshOffset(int type) {
        return GSXR_Unity_GetMeshOffset(type);
    }

    [DllImport(DLLName)]
    private static extern int GSXR_Unity_Set6Dof(bool is6dof);

    public override int GSXR_Set_6Dof(bool is6dof) {
        return GSXR_Unity_Set6Dof(is6dof);
    }

    [DllImport(DLLName)]
    private static extern ulong GSXRGetSupportSlamMode();
    public override ulong GSXR_Get_SupportSlamMode() {
        return GSXRGetSupportSlamMode();
    }

    [DllImport(DLLName)]
    private static extern void GSXR_Unity_GetVersion(ref string version);

    public override string GSXR_Get_Version()
    {
        //string version = "0.0.0";///must init for malloc memory
        //GSXR_Unity_GetVersion(ref version);
        return "0.0.0";
    }

    [DllImport(DLLName)]
    private static extern int GSXRGetXRType();

    [DllImport(DLLName)]
    private static extern void GSXR_Unity_GetDeviceName(ref string deviceName);
    //public override string GSXR_Get_DeviceName() {
    //    string devicename = "0000000000000000000000000000";//must init for malloc memory
    //    //GSXR_Unity_GetDeviceName(ref devicename);
    //    return devicename;
    //}

    public override XRType GSXR_Get_XRType() { return (XRType)GSXRGetXRType(); }
    #endregion
}
