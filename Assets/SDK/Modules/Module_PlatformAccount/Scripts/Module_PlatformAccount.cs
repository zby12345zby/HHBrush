using LitJson;
using UnityEngine;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public class Module_PlatformAccount : MonoBehaviour
    {
        public static Module_PlatformAccount Instance;
        
        public bool checkLoginState = false;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            WebRequestMgr.Instance.NetworkErrorCallBack += NetWorkErrorCallBack;
        }

        private void Start()
        {
            Init();
            if (checkLoginState)
            {
                API_Module_PlatformAccount.CheckUserState(CheckUserStateCallBack);
            }
            else
            {
                LoginForms.Instance.apLoginForms.Display();
            }                     
        }

        private void OnDisable()
        {
            WebRequestMgr.Instance.NetworkErrorCallBack -= NetWorkErrorCallBack;
        }

        public void Init()
        {
            InitData();
            LoginForms.Instance.Init();
            ForgetForms.Instance.Init();
            ErrorForms.Instance.Init();
        }

        public void InitData()
        {
            LoginMgr.Instance.loginConfig = LitJsonMgr.Instance.ReadJson<LoginConfig>("LoginConfig");
            LoginMgr.Instance.serverConfig = LitJsonMgr.Instance.ReadJson<ServerConfig>("ServerConfig");
            LoginMgr.Instance.remoteConfig = LitJsonMgr.Instance.ReadJson<RemoteConfig>("RemoteConfig");
            LoginMgr.Instance.userData = LitJsonMgr.Instance.ReadJson<UserData>("UserData");
        }

        public void NetWorkErrorCallBack()
        {
            foreach (var item in LoginForms.Instance.GetComponentsInChildren<BaseUIForms>())
            {
                if (item != LoginForms.Instance)
                {
                    item.Hiding();
                }
            }

            foreach (var item in ForgetForms.Instance.GetComponentsInChildren<BaseUIForms>())
            {
                if (item != ForgetForms.Instance)
                {
                    item.Hiding();
                }
            }

            foreach (var item in ErrorForms.Instance.GetComponentsInChildren<BaseUIForms>())
            {
                if (item != ErrorForms.Instance)
                {
                    item.Hiding();
                }
            }
            ErrorForms.Instance.networkErrorForms.Display();

        }

        public void CheckUserStateCallBack(string json)
        {
            JsonData data = JsonMapper.ToObject(json);
            int code = (int)data["code"];
            if (code == (int)RemoteCode.success)
            {
                bool valid = (bool)data["data"]["valid"];
                if (valid)
                {
                    //用户已登录
                    LoginMgr.Instance.LoginCallBack?.Invoke();
                }
                else
                {
                    //用户未登录或已过期
                    LoginForms.Instance.apLoginForms.Display();

                }
            }
            else
            {
                LoginForms.Instance.apLoginForms.Display();
            }
        }

        

    }
}
