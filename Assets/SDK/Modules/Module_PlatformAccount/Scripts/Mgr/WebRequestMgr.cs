using LitJson;
using System;
using System.Collections;
using UnityEngine;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public abstract class WebRequestMgr : MonoBehaviour
    {

        public static WebRequestMgr Instance;

        private void Awake()
        {
            Instance = this as WebRequestManager;
        }

        /// <summary>
        /// 是否正在请求
        /// </summary>
        public bool isRequesting = false;

        /// <summary>
        /// 网络错误回调
        /// </summary>
        public Action NetworkErrorCallBack;

        /// <summary>
        /// 通用数据Post请求接口
        /// </summary>
        public abstract void PostData(string url, string headerJson, string formJson, Action<string> callback = null);

        public abstract void PostData(string url, JsonData header, JsonData form, Action<string> callback = null);

        /// <summary>
        /// 通用数据Get请求接口
        /// </summary>
        public abstract void GetData(string url, string headerJson, string formJson, Action<string> callback = null);

        public abstract void GetData(string url, JsonData header, JsonData form, Action<string> callback = null);

        /// <summary>
        /// Get请求队列
        /// </summary>
        protected abstract IEnumerator GetsDataQueue(string url, JsonData header, JsonData form, Action<string> callback = null);

        /// <summary>
        /// Post请求队列
        /// </summary>
        protected abstract IEnumerator PostsDataQueue(string url, JsonData header, JsonData form, Action<string> callback = null);

    }
}
