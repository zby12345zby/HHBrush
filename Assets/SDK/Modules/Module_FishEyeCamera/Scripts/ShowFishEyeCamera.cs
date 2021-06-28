using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SC.XR.Unity
{
    public class ShowFishEyeCamera : MonoBehaviour
    {
        public RawImage _showLeftImage;
        public RawImage _showRightImage;
        Texture2D textureTemp;
        bool isPreview = false;


        int imageWidth;
        int imageHeight;
        bool outBUdate = true;
        uint outCurrFrameIndex = 0;
        ulong outFrameExposureNano = 0;
        byte[] outLeftFrameData;
        byte[] outRightFrameData;
        TextureFormat textureFormat = TextureFormat.Alpha8;

        void Awake()
        {
            API_GSXR_Slam.GSXR_Add_InitializedCallBack(Init);
        }

        private void LateUpdate()
        {
            if (isPreview)
            {
                ShowCamera();
            }
            else
            {
                _showLeftImage.texture = null;
                _showRightImage.texture = null;

            }
        }

        void OnDestroy() {
            API_GSXR_Slam.GSXR_Remove_InitializedCallBack(Init);
            textureTemp = null;
            outCurrFrameIndex = 0;
            outFrameExposureNano = 0;
            outLeftFrameData = null;
            outRightFrameData = null;
        }
        public void PreBtn()
        {
            isPreview = !isPreview;
            Debug.Log("LGS:是否预览：" + isPreview);
        }


        void Init()
        {

            imageWidth = (int)API_Module_Device.Current.FishEyeResolution.x;
            imageHeight = (int)API_Module_Device.Current.FishEyeResolution.y;
            outBUdate = true;
            outCurrFrameIndex = 0;
            outFrameExposureNano = 0;
            outLeftFrameData = new byte[imageWidth * imageHeight];
            outRightFrameData = new byte[imageWidth * imageHeight];
            textureFormat = TextureFormat.Alpha8;

            textureTemp = new Texture2D(imageWidth, imageHeight, textureFormat, false);
        }
        public void ShowCamera()
        {
            // if (!API_GSXR_Slam.SlamManager.Initialized) return;
            if (Application.platform == RuntimePlatform.Android)
            {

                API_GSXR_Slam.GSXR_Get_LatestFishEyeBinocularData(ref outBUdate, ref outCurrFrameIndex, ref outFrameExposureNano, outLeftFrameData, outRightFrameData);

                Debug.Log("LGS:outBUdate=>" + outBUdate + " outCurrFrameIndex:" + outCurrFrameIndex + "  outFrameExposureNano" + outFrameExposureNano);
                if (outBUdate)
                {
                    _showLeftImage.texture = GetTexture(outLeftFrameData);
                    _showLeftImage.rectTransform.sizeDelta = new Vector2(imageWidth, imageHeight);

                    _showRightImage.texture = GetTexture(outRightFrameData);
                    _showRightImage.rectTransform.sizeDelta = new Vector2(imageWidth, imageHeight);

                }
                else
                {
                    Debug.Log("Error: Please Check Slamconfig prop: gUseXXXCamera = true");
                }
            }
        }
    
        public Texture2D GetTexture(byte[] outFrameData)
        {
            
            textureTemp.LoadRawTextureData(outFrameData);
            textureTemp.Apply();
            return textureTemp;
        }

    }
}

