using LitJson;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public class WebRequestManager : WebRequestMgr
    {

        public override void PostData(string url, string headerJson, string formJson, Action<string> callback = null)
        {
            if (!isRequesting)
            {
                isRequesting = true;

                JsonData header = JsonMapper.ToObject(headerJson);
                JsonData form = JsonMapper.ToObject(formJson);

                StartCoroutine(PostsDataQueue(url, header, form, callback));
            }                    
        }

        public override void PostData(string url, JsonData header, JsonData form, Action<string> callback = null)
        {
            if (!isRequesting)
            {
                isRequesting = true;

                StartCoroutine(PostsDataQueue(url, header, form, callback));
            }
        }

        public override void GetData(string url, string headerJson, string formJson, Action<string> callback = null)
        {
            if (!isRequesting)
            {
                isRequesting = true;

                JsonData header = JsonMapper.ToObject(headerJson);
                JsonData form = JsonMapper.ToObject(formJson);

                StartCoroutine(GetsDataQueue(url, header, form, callback));
            }
        }

        public override void GetData(string url, JsonData header, JsonData form, Action<string> callback = null)
        {
            if (!isRequesting)
            {
                isRequesting = true;

                StartCoroutine(GetsDataQueue(url, header, form, callback));
            }           
        }

        protected override IEnumerator GetsDataQueue(string url, JsonData header, JsonData form, Action<string> callback = null)
        {

            if (url != null) Debug.Log("url :" + url);
            if (header != null) Debug.Log("header :" + JsonMapper.ToJson(header));
            if (form != null) Debug.Log("form :" + JsonMapper.ToJson(form));


            using (UnityWebRequest web = UnityWebRequest.Get(url))
            {
                foreach (var key in header.Keys)
                {
                    web.SetRequestHeader(key, (string)header[key]);
                }
                //把客户端数据传往服务器：给服务器上传我们的数据；
                web.uploadHandler = new UploadHandlerRaw(Encoding.Default.GetBytes(form != null ? JsonMapper.ToJson(form) : "null"));
                //服务器数据---》客户端 获取服务器返回的数据
                web.downloadHandler = new DownloadHandlerBuffer();
                //发送一个请求：SendWebRequest异步，你发完后，不用等底层完整的发出去，你就可以做别的；防止卡主游戏线程
                yield return web.SendWebRequest();
                if (web.isNetworkError || web.isHttpError)
                {
                    isRequesting = false;
                    Debug.LogWarning("RequestData :" + url + web.error);
                    NetworkErrorCallBack?.Invoke();
                    yield break;
                }
                else
                {
                    isRequesting = false;
                    try
                    {
                        Debug.Log("RequestData :" + web.downloadHandler.text);
                        callback?.Invoke(web.downloadHandler.text);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Exception message :" + e.Message);
                        NetworkErrorCallBack?.Invoke();
                    }
                    
                }
            }
        }

        protected override IEnumerator PostsDataQueue(string url, JsonData header, JsonData form, Action<string> callback = null)
        {

            if (url != null) Debug.Log("url :" + url);
            if (header != null) Debug.Log("header :" + JsonMapper.ToJson(header));
            if (form != null) Debug.Log("form :" + JsonMapper.ToJson(form));

            
            using (UnityWebRequest web = UnityWebRequest.Post(url, ""))
            {
                foreach (var key in header.Keys)
                {
                    web.SetRequestHeader(key, (string)header[key]);
                }
                //把客户端数据传往服务器：给服务器上传我们的数据；
                web.uploadHandler = new UploadHandlerRaw(Encoding.Default.GetBytes(form != null ? JsonMapper.ToJson(form) : "null"));
                //服务器数据---》客户端 获取服务器返回的数据
                web.downloadHandler = new DownloadHandlerBuffer();
                //发送一个请求：SendWebRequest异步，你发完后，不用等底层完整的发出去，你就可以做别的；防止卡主游戏线程
                yield return web.SendWebRequest();
                if (web.isNetworkError || web.isHttpError)
                {
                    isRequesting = false;
                    Debug.LogWarning("RequestData :" + url + web.error);
                    NetworkErrorCallBack?.Invoke();
                    yield break;
                }
                else
                {
                    isRequesting = false;
                    try
                    {
                        Debug.Log("RequestData :" + web.downloadHandler.text);
                        callback?.Invoke(web.downloadHandler.text);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Exception message :" + e.Message);
                        NetworkErrorCallBack?.Invoke();
                    }
                }
            }
        }
        
    }
}
