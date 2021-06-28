using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using SC.XR.Unity;
using SC.XR.Unity.Module_InputSystem.InputDeviceGC.KS;

class GSXRPluginWin : GSXRPlugin
{
	public static GSXRPluginWin Create()
	{
		return new GSXRPluginWin ();
	}

    private GSXRPluginWin() { }

    public override bool IsInitialized() { return slamManager != null; }

    public override bool IsRunning() { return eyes != null; }

    public override IEnumerator Initialize()
	{
        yield return base.Initialize();

        deviceInfo = GetDeviceInfo();

        yield break;
	}

	public override IEnumerator BeginVr(int cpuPerfLevel, int gpuPerfLevel)
	{
        yield return base.BeginVr(cpuPerfLevel, gpuPerfLevel);

		yield break;
    }
	
    public override void SetVSyncCount(int vSyncCount)
    {
        QualitySettings.vSyncCount = vSyncCount;
    }

    Vector2 mouseNDCRotate = Vector2.zero;
    Vector2 mouseNDCPosition = Vector2.zero;

    Vector2 mousePressPointTemp1 = Vector2.zero;
    Vector3 mousePressEuler = Vector3.zero;

    public override int GetHeadPose(ref HeadPose headPose, int frameIndex)
	{
        int poseStatus = 0;

		headPose.orientation = Quaternion.identity;
        headPose.position = Vector3.zero;
        //Debug.Log("Input.mousePosition:"+ Input.mousePosition+"  "+ Screen.width+" "+Screen.height);

        //if (Input.GetMouseButton(0))    // 0/Left mouse button
        //{
        //    poseStatus |= (int)TrackingMode.kTrackingOrientation;
        //    poseStatus |= (int)TrackingMode.kTrackingPosition;
        //}

        //if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))    // 1/Right mouse button
        //{
        //    mousePressPointTemp1 = Input.mousePosition;
        //    mousePressEuler = GSXRManager.Instance.head.eulerAngles;

        //} else if(Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
            
        //    mouseNDCRotate.x = 2 * ((Input.mousePosition.x - mousePressPointTemp1.x) / Screen.width) ;
        //    mouseNDCRotate.y = 2 * ((Input.mousePosition.y - mousePressPointTemp1.y) / Screen.height) ;
        //    poseStatus |= (int)TrackingMode.kTrackingOrientation;

        //}

        //if(Input.GetKey(KeyCode.W)) {

        //    mouseNDCPosition.y += Time.deltaTime * 0.2f;
        //    poseStatus |= (int)TrackingMode.kTrackingPosition;
        //} else if(Input.GetKey(KeyCode.S)) {
        //    mouseNDCPosition.y -= Time.deltaTime * 0.2f;
        //    poseStatus |= (int)TrackingMode.kTrackingPosition;
        //} else {
        //    mouseNDCPosition.y = 0;
        //}

        //if(Input.GetKey(KeyCode.A)) {
        //    mouseNDCPosition.x -= Time.deltaTime * 0.2f;
        //    poseStatus |= (int)TrackingMode.kTrackingPosition;
        //} else if(Input.GetKey(KeyCode.D)) {
        //    mouseNDCPosition.x += Time.deltaTime * 0.2f;
        //    poseStatus |= (int)TrackingMode.kTrackingPosition;
        //} else {
        //    mouseNDCPosition.x = 0;
        //}
        ////if(Input.mouseScrollDelta.y != 0) {
        ////    mouseNDCPosition.x = 0;
        ////    mouseNDCPosition.y = Input.mouseScrollDelta.y * 0.2f;
        ////    poseStatus |= (int)TrackingMode.kTrackingPosition;
        ////} else {
        ////    mouseNDCPosition = Vector2.zero;
        ////}

        /////复位
        //if(Input.GetKey(KeyCode.Escape) == true) {

        //    mouseNDCRotate = Vector2.zero;
        //    mouseNDCPosition = Vector2.zero;

        //    mousePressPointTemp1 = Vector2.zero;
        //    mousePressEuler = Vector3.zero;

        //    GSXRManager.Instance.head.position = Vector3.zero;

        //    poseStatus |= (int)TrackingMode.kTrackingOrientation;
        //    poseStatus |= (int)TrackingMode.kTrackingPosition;
        //}


        headPose.orientation.eulerAngles = mousePressEuler + new Vector3(-mouseNDCRotate.y * 45f, mouseNDCRotate.x * 90f, 0);

        headPose.position = new Vector3(mouseNDCPosition.x,0, mouseNDCPosition.y);
        headPose.position = GSXRManager.Instance.head.TransformPoint(headPose.position);

        return poseStatus;
    }

	public override DeviceInfo GetDeviceInfo()
	{
		DeviceInfo info 			= new DeviceInfo();

		info.displayWidthPixels 	= Screen.width;
		info.displayHeightPixels 	= Screen.height;
		info.displayRefreshRateHz 	= 60.0f;
		info.targetEyeWidthPixels 	= Screen.width / 2;
		info.targetEyeHeightPixels 	= Screen.height;
		info.targetFovXRad			= Mathf.Deg2Rad * 47;
		info.targetFovYRad			= Mathf.Deg2Rad * 20.1f;
		info.targetFrustumLeft.left 	= -0.02208847f;
		info.targetFrustumLeft.right    = 0.02208847f;
		info.targetFrustumLeft.top      = 0.0123837f;
		info.targetFrustumLeft.bottom   = -0.0123837f;
        info.targetFrustumLeft.near     = 0.0508f;
        info.targetFrustumLeft.far      = 100f;
		info.targetFrustumRight.left    = -0.02208847f;
		info.targetFrustumRight.right   = 0.02208847f;
		info.targetFrustumRight.top     = 0.0123837f;
		info.targetFrustumRight.bottom  = -0.0123837f;
        info.targetFrustumRight.near    = 0.0508f;
        info.targetFrustumRight.far     = 100f;
        return info;
	}

	public override void SubmitFrame(int frameIndex, float fieldOfView, int frameType)
	{
		RenderTexture.active = null;
		GL.Clear (false, true, Color.black);

		//float cameraFov = fieldOfView;
		//float fovMarginX = (cameraFov / deviceInfo.targetFovXRad) - 1;
		//float fovMarginY = (cameraFov / deviceInfo.targetFovYRad) - 1;
        //Rect textureRect = new Rect(fovMarginX * 0.5f, fovMarginY * 0.5f, 1 - fovMarginX, 1 - fovMarginY);
        Rect textureRect = new Rect(0, 0, 1, 1);

        Vector2 leftCenter = new Vector2(Screen.width * 0.25f, Screen.height * 0.5f);
		Vector2 rightCenter = new Vector2(Screen.width * 0.75f, Screen.height * 0.5f);
		Vector2 eyeExtent = new Vector3(Screen.width * 0.25f, Screen.height * 0.5f);
		eyeExtent.x -= 10.0f;
		eyeExtent.y -= 10.0f;

		Rect leftScreen = Rect.MinMaxRect(
            leftCenter.x - eyeExtent.x, 
            leftCenter.y - eyeExtent.y, 
            leftCenter.x + eyeExtent.x, 
            leftCenter.y + eyeExtent.y);
		Rect rightScreen = Rect.MinMaxRect(
            rightCenter.x - eyeExtent.x, 
            rightCenter.y - eyeExtent.y, 
            rightCenter.x + eyeExtent.x, 
            rightCenter.y + eyeExtent.y);

        if (eyes != null) for (int i = 0; i < eyes.Length; i++)
        {
            if (eyes[i].isActiveAndEnabled == false) continue;
            if (eyes[i].TexturePtr == null) continue;
            if (eyes[i].imageTransform != null && eyes[i].imageTransform.gameObject.activeSelf == false) continue;
            if (eyes[i].imageTransform != null && !eyes[i].imageTransform.IsChildOf(slamManager.transform)) continue;

            var eyeRectMin = eyes[i].clipLowerLeft; eyeRectMin /= eyeRectMin.w;
            var eyeRectMax = eyes[i].clipUpperRight; eyeRectMax /= eyeRectMax.w;

            if (eyes[i].Side == GSXREye.eSide.Left || eyes[i].Side == GSXREye.eSide.Both)
            {
                leftScreen = Rect.MinMaxRect(
                    leftCenter.x + eyeExtent.x * eyeRectMin.x, 
                    leftCenter.y + eyeExtent.y * eyeRectMin.y, 
                    leftCenter.x + eyeExtent.x * eyeRectMax.x, 
                    leftCenter.y + eyeExtent.y * eyeRectMax.y);
                Graphics.DrawTexture(leftScreen, eyes[i].TexturePtr, textureRect, 0, 0, 0, 0);
            }
            if (eyes[i].Side == GSXREye.eSide.Right || eyes[i].Side == GSXREye.eSide.Both)
            {
                rightScreen = Rect.MinMaxRect(
                    rightCenter.x + eyeExtent.x * eyeRectMin.x,
                    rightCenter.y + eyeExtent.y * eyeRectMin.y,
                    rightCenter.x + eyeExtent.x * eyeRectMax.x,
                    rightCenter.y + eyeExtent.y * eyeRectMax.y);
                Graphics.DrawTexture(rightScreen, eyes[i].TexturePtr, textureRect, 0, 0, 0, 0);
            }
        }

        if (overlays != null) for (int i = 0; i < overlays.Length; i++)
        {
            if (overlays[i].isActiveAndEnabled == false) continue;
            if (overlays[i].TexturePtr == null) continue;
            if (overlays[i].imageTransform != null && overlays[i].imageTransform.gameObject.activeSelf == false) continue;
            if (overlays[i].imageTransform != null && !overlays[i].imageTransform.IsChildOf(slamManager.transform)) continue;

            var eyeRectMin = overlays[i].clipLowerLeft; eyeRectMin /= eyeRectMin.w;
            var eyeRectMax = overlays[i].clipUpperRight; eyeRectMax /= eyeRectMax.w;

            textureRect.Set(overlays[i].uvLowerLeft.x, overlays[i].uvLowerLeft.y,
                overlays[i].uvUpperRight.x - overlays[i].uvLowerLeft.x,
                overlays[i].uvUpperRight.y - overlays[i].uvLowerLeft.y);

            if (overlays[i].Side == GSXROverlay.eSide.Left || overlays[i].Side == GSXROverlay.eSide.Both)
            {
                leftScreen = Rect.MinMaxRect(
                    leftCenter.x + eyeExtent.x * eyeRectMin.x,
                    leftCenter.y + eyeExtent.y * eyeRectMin.y,
                    leftCenter.x + eyeExtent.x * eyeRectMax.x,
                    leftCenter.y + eyeExtent.y * eyeRectMax.y);
                Graphics.DrawTexture(leftScreen, overlays[i].TexturePtr, textureRect, 0, 0, 0, 0);
            }
            if (overlays[i].Side == GSXROverlay.eSide.Right || overlays[i].Side == GSXROverlay.eSide.Both)
            {
                rightScreen = Rect.MinMaxRect(
                    rightCenter.x + eyeExtent.x * eyeRectMin.x,
                    rightCenter.y + eyeExtent.y * eyeRectMin.y,
                    rightCenter.x + eyeExtent.x * eyeRectMax.x,
                    rightCenter.y + eyeExtent.y * eyeRectMax.y);
                Graphics.DrawTexture(rightScreen, overlays[i].TexturePtr, textureRect, 0, 0, 0, 0);
            }
        }
 
	}



    public override void Shutdown()
	{
        base.Shutdown();
    }



    #region Controller

    public override bool GSXR_Is_SupportController() { return true; }
    public override int GSXR_Get_ControllerList(int lr) { return (int)KSID.K102; }

    #endregion Controller

    #region HandTracking
    
    public override bool GSXR_Is_SupportHandTracking() {return true;}

    float[] poseNull = new float[3] { 0, 0, 0 };
    public override void GSXR_Get_HandTrackingData(float[] mode, float[] pose) {

        pose = poseNull;
        #region PCgesture
        if (Input.GetKey(KeyCode.Alpha1) == true && Input.GetKey(KeyCode.Alpha2) == false) {
            ///左手抓取手型数据
            Array.Copy(DataLeftCatch, mode, DataLeftCatch.Length);
        } else if ((Input.GetKey(KeyCode.Alpha1) == false && Input.GetKey(KeyCode.Alpha2)) == true) {
            ///右手手抓取手型数据
            Array.Copy(DataRightCatch, mode, DataLeftCatch.Length);
        } else if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Alpha1) == true && Input.GetKey(KeyCode.Alpha2) == true) {
            ///双手抓取手型数据
            Array.Copy(DataBothCatch, mode, DataLeftCatch.Length);
        } else {
            ///正常张开手型数据
            Array.Copy(DataBothRelease, mode, DataLeftCatch.Length);

        }

        #endregion
        //if(Input.GetKey(KeyCode.W) == true) {
        //    temp +=Vector3.forward* Time.deltaTime * 0.1f;
        //}
        //if(Input.GetKey(KeyCode.A) == true) {
        //    temp += Vector3.left * Time.deltaTime * 0.1f;
        //}
        //if(Input.GetKey(KeyCode.D) == true) {
        //    temp += Vector3.right * Time.deltaTime * 0.1f;
        //}
        //if(Input.GetKey(KeyCode.S) == true) {
        //    temp += Vector3.back * Time.deltaTime * 0.1f;
        //}

        //pose[0] = temp.x;
        //pose[1] = temp.y;
        //pose[2] = temp.z;


    }
    float[] DataLeftCatch = new float[256] {
                    2f,

                    1f,
                    21f,
                    -0.05605574f  ,  -0.0273f  ,  0.314f,
                    -0.07f  ,  -0.0447f  ,  0.3f ,
                    -0.09515104f  , -0.05433898f  ,  0.2831757f,
                    -0.1463438f  ,  -0.06028023f  ,  0.2818089f,

                    -0.0519f  ,  -0.0216f  ,  0.3217f,
                    -0.06158409f  ,  -0.0002f  , 0.3183799f,
                    -0.07964747f  ,  0.0028f  ,  0.3071041f,
                    -0.09945723f  ,  -0.008320909f  ,  0.2969252f,

                    -0.06198902f  ,  -0.0038f  , 0.3519928f ,
                    -0.07581183f  ,  0.0061f  ,  0.3419357f ,
                    -0.09352214f  ,  0.0082f  ,  0.3285346f ,
                    -0.1137585f  ,  -0.002121262f  ,  0.3150989f,

                    -0.07718303f  ,  -0.0063f  ,  0.364556f,
                    -0.08849367f  ,  0.0018f  ,  0.3556355f,
                    -0.1056515f  ,  0.0004f  ,  0.3442511f,
                    -0.1236217f  ,  -0.008046876f  ,  0.331872f,

                    -0.09772705f  ,  -0.0135f  ,  0.3714658f,
                    -0.1064803f  ,  -0.0118f  ,  0.3659644f,
                    -0.1201454f  ,  -0.0166f  ,  0.3570033f ,
                    -0.1347919f  ,  -0.02225832f  ,  0.3441104f,
                    -0.1562702f  ,  -0.05975028f  ,  0.2943733f ,

                    2f,
                    21f,
                    0.01329147f  ,  -0.05185008f  ,  0.2831516f,
                    0.02629676f  ,  -0.05859572f  ,  0.2733593f ,
                    0.04144813f  ,  -0.06611566f  ,  0.2669144f,
                    0.09289996f  ,  -0.07803007f  ,  0.2608919f,

                    0.008512383f  ,  0.008053687f  ,  0.2999706f,
                    0.0184048f  ,  0.004125214f  ,  0.2896651f ,
                    0.0309000f  ,  -0.00680000f  ,  0.27900000f ,
                    0.05151144f  ,  -0.02237571f  ,  0.2674141f,

                    0.02381282f  ,  0.02699578f  ,  0.3218553f,
                    0.03562622f  ,  0.01996369f  ,  0.3106135f,
                    0.05122653f  ,  0.008665001f  ,  0.2939393f,
                    0.06585947f  ,  -0.01085604f  ,  0.2825926f,

                    0.04237749f  ,  0.01809751f  ,  0.3312246f ,
                    0.05178002f  ,  0.01224715f  ,  0.3221904f,
                    0.0659532f  ,  0.003831701f  ,  0.3083498f,
                    0.07914048f  ,  -0.01247946f  ,  0.296542f,

                    0.06218155f  ,  0.002945414f  ,  0.3410454f ,
                    0.07016665f  ,  -0.002003442f  ,  0.3339252f,
                    0.08204032f  ,  -0.01003692f  ,  0.322701f,
                    0.09209882f  ,  -0.02350484f  ,  0.3101943f ,
                    0.1055621f  ,  -0.07391939f  ,  0.2699004f ,

                    0,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,

                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0,0,0
            };
    float[] DataRightCatch = new float[256] {
                    2f,

                    1f,
                    21f,
                    -0.05605574f  ,  -0.04518472f  ,  0.2970701f,
                    -0.07650252f  ,  -0.04899639f  ,  0.2892286f ,
                    -0.09515104f  , -0.05433898f  ,  0.2831757f,
                    -0.1463438f  ,  -0.06028023f  ,  0.2818089f,

                    -0.04933745f  ,  0.01041036f  ,  0.3268114f,
                    -0.06158409f  ,  0.009036704f  ,  0.3183799f,
                    -0.07964747f  ,  0.0015f  ,  0.3071041f,
                    -0.09945723f  ,  -0.008320909f  ,  0.2969252f,

                    -0.06198902f  ,  0.02461821f  ,  0.3519928f ,
                    -0.07581183f  ,  0.02041251f  ,  0.3419357f ,
                    -0.09352214f  ,  0.01374155f  ,  0.3285346f ,
                    -0.1137585f  ,  -0.002121262f  ,  0.3150989f,

                    -0.07718303f  ,  0.01460867f  ,  0.364556f,
                    -0.08849367f  ,  0.01155128f  ,  0.3556355f,
                    -0.1056515f  ,  0.005635553f  ,  0.3442511f,
                    -0.1236217f  ,  -0.008046876f  ,  0.331872f,

                    -0.09772705f  ,  -0.003397862f  ,  0.3714658f,
                    -0.1064803f  ,  -0.005923742f  ,  0.3659644f,
                    -0.1201454f  ,  -0.01133183f  ,  0.3570033f ,
                    -0.1347919f  ,  -0.02225832f  ,  0.3441104f,
                    -0.1562702f  ,  -0.05975028f  ,  0.2943733f ,

                    2f,
                    21f,
                    0.0159f  ,  -0.0299f  ,  0.2831516f,
                    0.0348f  ,  -0.0511f  ,  0.2733593f ,
                    0.0491f  ,  -0.0602f  ,  0.2669144f,
                    0.09289996f  ,  -0.07803007f  ,  0.2608919f,

                    0.0085f  ,  -0.0223f  ,  0.2917f,
                    0.0184f  ,  -0.0117f  ,  0.28966f ,
                    0.0338265f  ,  -0.0106f  ,  0.2750752f ,
                    0.05151144f  ,  -0.02237571f  ,  0.2674141f,

                    0.02381282f  ,  -0.005f  ,  0.3218553f,
                    0.03562622f  ,  0.0052f  ,  0.3106135f,
                    0.05122653f  ,  0.0032f  ,  0.2939393f,
                    0.06585947f  ,  -0.01085604f  ,  0.2825926f,

                    0.04237749f  ,  0.002f  ,  0.3312246f ,
                    0.05178002f  ,  0.0005f  ,  0.3221904f,
                    0.0659532f  ,  -0.002f  ,  0.3083498f,
                    0.07914048f  ,  -0.01247946f  ,  0.296542f,

                    0.06218155f  ,  0.002945414f  ,  0.3410454f ,
                    0.07016665f  ,  -0.002003442f  ,  0.3339252f,
                    0.08204032f  ,  -0.01003692f  ,  0.322701f,
                    0.09209882f  ,  -0.02350484f  ,  0.3101943f ,
                    0.1055621f  ,  -0.07391939f  ,  0.2699004f ,

                    0,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,

                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0,0,0
            };
    float[] DataBothCatch = new float[256] {
                    2f,

                    1f,
                    21f,
                    -0.05605574f  ,  -0.0273f  ,  0.314f,
                    -0.07f  ,  -0.0447f  ,  0.3f ,
                    -0.09515104f  , -0.05433898f  ,  0.2831757f,
                    -0.1463438f  ,  -0.06028023f  ,  0.2818089f,

                    -0.0519f  ,  -0.0216f  ,  0.3217f,
                    -0.06158409f  ,  -0.0002f  , 0.3183799f,
                    -0.07964747f  ,  0.0028f  ,  0.3071041f,
                    -0.09945723f  ,  -0.008320909f  ,  0.2969252f,

                    -0.06198902f  ,  -0.0038f  , 0.3519928f ,
                    -0.07581183f  ,  0.0061f  ,  0.3419357f ,
                    -0.09352214f  ,  0.0082f  ,  0.3285346f ,
                    -0.1137585f  ,  -0.002121262f  ,  0.3150989f,

                    -0.07718303f  ,  -0.0063f  ,  0.364556f,
                    -0.08849367f  ,  0.0018f  ,  0.3556355f,
                    -0.1056515f  ,  0.0004f  ,  0.3442511f,
                    -0.1236217f  ,  -0.008046876f  ,  0.331872f,

                    -0.09772705f  ,  -0.0135f  ,  0.3714658f,
                    -0.1064803f  ,  -0.0118f  ,  0.3659644f,
                    -0.1201454f  ,  -0.0166f  ,  0.3570033f ,
                    -0.1347919f  ,  -0.02225832f  ,  0.3441104f,
                    -0.1562702f  ,  -0.05975028f  ,  0.2943733f ,

                    2f,
                    21f,
                    0.0159f  ,  -0.0299f  ,  0.2831516f,
                    0.0348f  ,  -0.0511f  ,  0.2733593f ,
                    0.0491f  ,  -0.0602f  ,  0.2669144f,
                    0.09289996f  ,  -0.07803007f  ,  0.2608919f,

                    0.0085f  ,  -0.0223f  ,  0.2917f,
                    0.0184f  ,  -0.0117f  ,  0.28966f ,
                    0.0338265f  ,  -0.0106f  ,  0.2750752f ,
                    0.05151144f  ,  -0.02237571f  ,  0.2674141f,

                    0.02381282f  ,  -0.005f  ,  0.3218553f,
                    0.03562622f  ,  0.0052f  ,  0.3106135f,
                    0.05122653f  ,  0.0032f  ,  0.2939393f,
                    0.06585947f  ,  -0.01085604f  ,  0.2825926f,

                    0.04237749f  ,  0.002f  ,  0.3312246f ,
                    0.05178002f  ,  0.0005f  ,  0.3221904f,
                    0.0659532f  ,  -0.002f  ,  0.3083498f,
                    0.07914048f  ,  -0.01247946f  ,  0.296542f,

                    0.06218155f  ,  0.002945414f  ,  0.3410454f ,
                    0.07016665f  ,  -0.002003442f  ,  0.3339252f,
                    0.08204032f  ,  -0.01003692f  ,  0.322701f,
                    0.09209882f  ,  -0.02350484f  ,  0.3101943f ,
                    0.1055621f  ,  -0.07391939f  ,  0.2699004f ,

                    0,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,

                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0,0,0
            };
    float[] DataBothRelease = new float[256] {
                    2f,

                    1f,
                    21f,
                    -0.05605574f  ,  -0.04518472f  ,  0.2970701f,
                    -0.07650252f  ,  -0.04899639f  ,  0.2892286f ,
                    -0.09515104f  , -0.05433898f  ,  0.2831757f,
                    -0.1463438f  ,  -0.06028023f  ,  0.2818089f,

                    -0.04933745f  ,  0.01041036f  ,  0.3268114f,
                    -0.06158409f  ,  0.009036704f  ,  0.3183799f,
                    -0.07964747f  ,  0.0015f  ,  0.3071041f,
                    -0.09945723f  ,  -0.008320909f  ,  0.2969252f,

                    -0.06198902f  ,  0.02461821f  ,  0.3519928f ,
                    -0.07581183f  ,  0.02041251f  ,  0.3419357f ,
                    -0.09352214f  ,  0.01374155f  ,  0.3285346f ,
                    -0.1137585f  ,  -0.002121262f  ,  0.3150989f,

                    -0.07718303f  ,  0.01460867f  ,  0.364556f,
                    -0.08849367f  ,  0.01155128f  ,  0.3556355f,
                    -0.1056515f  ,  0.005635553f  ,  0.3442511f,
                    -0.1236217f  ,  -0.008046876f  ,  0.331872f,

                    -0.09772705f  ,  -0.003397862f  ,  0.3714658f,
                    -0.1064803f  ,  -0.005923742f  ,  0.3659644f,
                    -0.1201454f  ,  -0.01133183f  ,  0.3570033f ,
                    -0.1347919f  ,  -0.02225832f  ,  0.3441104f,
                    -0.1562702f  ,  -0.05975028f  ,  0.2943733f ,

                    2f,
                    21f,
                    0.01329147f  ,  -0.05185008f  ,  0.2831516f,
                    0.02629676f  ,  -0.05859572f  ,  0.2733593f ,
                    0.04144813f  ,  -0.06611566f  ,  0.2669144f,
                    0.09289996f  ,  -0.07803007f  ,  0.2608919f,

                    0.008512383f  ,  0.008053687f  ,  0.2999706f,
                    0.0184048f  ,  0.004125214f  ,  0.2896651f ,
                    0.0309000f  ,  -0.00680000f  ,  0.27900000f ,
                    0.05151144f  ,  -0.02237571f  ,  0.2674141f,

                    0.02381282f  ,  0.02699578f  ,  0.3218553f,
                    0.03562622f  ,  0.01996369f  ,  0.3106135f,
                    0.05122653f  ,  0.008665001f  ,  0.2939393f,
                    0.06585947f  ,  -0.01085604f  ,  0.2825926f,

                    0.04237749f  ,  0.01809751f  ,  0.3312246f ,
                    0.05178002f  ,  0.01224715f  ,  0.3221904f,
                    0.0659532f  ,  0.003831701f  ,  0.3083498f,
                    0.07914048f  ,  -0.01247946f  ,  0.296542f,

                    0.06218155f  ,  0.002945414f  ,  0.3410454f ,
                    0.07016665f  ,  -0.002003442f  ,  0.3339252f,
                    0.08204032f  ,  -0.01003692f  ,  0.322701f,
                    0.09209882f  ,  -0.02350484f  ,  0.3101943f ,
                    0.1055621f  ,  -0.07391939f  ,  0.2699004f ,

                    0,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,

                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,
                    0,0,0,0
            };
    #endregion HandTracking

    #region Deflection
    #endregion Deflection

    #region PointCloud & Map
    #endregion  PointCloud & Map

    #region FishEye Data
    #endregion FishEye Data

    #region Optics Calibration
    #endregion Optics Calibration

    #region EyeTracking
    #endregion EyeTracking

    #region USBDisconnect
    #endregion

    #region luncher
    #endregion

    #region Device
    public override ulong GSXR_Get_SupportSlamMode() {
        return 6;
    }
    public override XRType GSXR_Get_XRType() { return XRType.VR; }

    public override string GSXR_Get_DeviceName() { return "Window AR"; }

    public override string SN => "000";

    public override string RELEASE_VERSION => "0.0.0";

    public override int BatteryLevel => 60;
    #endregion
}
