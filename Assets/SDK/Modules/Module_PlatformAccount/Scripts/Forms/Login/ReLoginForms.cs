using LitJson;
using UnityEngine.UI;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public enum LoginType
    {
        APLogin,
        SMSLogin
    }

    public class ReLoginForms : BaseLoginForms
    {
        public LoginType loginType;

        public Button backLoginBtn;
        public Button sureLoginBtn;

        public string account;
        public string password;
        public string captcha;

        public string phone;
        public string diallingCode;
        public string smsCode;
        
        public override void Display()
        {
            base.Display();

            backLoginBtn.onClick.AddListener(OnClickBackLoginBtn);
            sureLoginBtn.onClick.AddListener(OnClickSureLoginBtn);           
        }

        public override void Hiding()
        {
            base.Hiding();

            backLoginBtn.onClick.RemoveListener(OnClickBackLoginBtn);
        }

        public override void Init()
        {
            base.Init();

            ClickableBtn(sureLoginBtn);

        }

        #region CallBack
        public void LoginCallBack(string json)
        {
            JsonData data = JsonMapper.ToObject(json);
            int code = (int)data["code"];
            if (code == (int)RemoteCode.success)
            {
                string token = data["data"]["token"].ToString();
                string unionId = data["data"]["unionId"].ToString();
                LoginMgr.Instance.userData = new UserData();
                LoginMgr.Instance.userData.token = token;
                LoginMgr.Instance.userData.unionId = unionId;
                LitJsonMgr.Instance.WriteJson<UserData>("UserData", LoginMgr.Instance.userData);
                SaveUserInfo();
                if (LoginForms.Instance.apLoginForms.rememberPwd.isOn)
                {
                    LitJsonMgr.Instance.WriteJson<LoginConfig>("LoginConfig", LoginMgr.Instance.loginConfig);
                }
                else
                {
                    LoginMgr.Instance.loginConfig.account = "";
                    LoginMgr.Instance.loginConfig.password = "";
                    LoginMgr.Instance.loginConfig.isRemeber = false;
                    LitJsonMgr.Instance.WriteJson<LoginConfig>("LoginConfig", LoginMgr.Instance.loginConfig);
                }
                Hiding();
            }
        }
        #endregion

        #region Button
        public void OnClickBackLoginBtn()
        {
            LoginForms.Instance.apLoginForms.Display();
            Hiding();
        }

        public void OnClickSureLoginBtn()
        {
            switch (loginType)
            {
                case LoginType.APLogin:
                    API_Module_PlatformAccount.AccountLogin(account, password,
                        LoginMgr.Instance.captchaData != null ? LoginMgr.Instance.captchaData.captchaId : null,
                        LoginMgr.Instance.captchaData != null ? captcha : null, LoginCallBack);
                    sureLoginBtn.interactable = false;
                    sureLoginBtn.interactable = true;
                    break;
                case LoginType.SMSLogin:
                    API_Module_PlatformAccount.PhoneLogin(phone, diallingCode, smsCode, LoginCallBack);
                    sureLoginBtn.interactable = false;
                    sureLoginBtn.interactable = true;
                    break;
            }
        }
        #endregion

    }
}
