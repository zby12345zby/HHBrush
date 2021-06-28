using LitJson;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public class APLoginForms : BaseLoginForms
    {
        public SCInputField account;
        public SCInputField password;

        public Image accountFrame;
        public Image passwordFrame;
        public Image captchaFrame;

        public InputErrorForms accountError;
        public InputErrorForms passwordError;
        public InputErrorForms captchaError;
        public InputErrorForms registerError;

        public Button apLoginBtn;
        public Button smsLoginBtn;
        public Button forgetPwdBtn;
        public Button loginBtn;
        public Button emptyAccountBtn;
        public Button emptyPasswordBtn;
        public Button emptyCaptchaBtn;

        public GameObject captchaObj;
        public SCInputField captcha;

        public RawImage captchaTexture;
        public Button captchaBtn;

        public Toggle rememberPwd;

        public override void Display()
        {

            base.Display();
            
            apLoginBtn.onClick.AddListener(OnClickAPLoginBtn);
            smsLoginBtn.onClick.AddListener(OnClickSMSLoginBtn);
            forgetPwdBtn.onClick.AddListener(OnClickForgetPwdBtn);
            captchaBtn.onClick.AddListener(OnClickCaptchaBtn);
            loginBtn.onClick.AddListener(OnClickLoginBtn);
            emptyAccountBtn.onClick.AddListener(OnClickEmptyAccountBtn);
            emptyPasswordBtn.onClick.AddListener(OnClickEmptyPasswordBtn);
            emptyCaptchaBtn.onClick.AddListener(OnClickEmptyCaptchaBtn);
           
            account.onValueChanged.AddListener(OnUserValueChanged);
            password.onValueChanged.AddListener(OnPassValueChanged);
            captcha.onValueChanged.AddListener(OnCaptchaValueChanged);

            account.onEndEdit.AddListener(OnUserEndEdit);
            password.onEndEdit.AddListener(OnPassEndEdit);
            captcha.onEndEdit.AddListener(OnCaptchaEndEdit);
        }

        public override void Hiding()
        {
            base.Hiding();

            apLoginBtn.onClick.RemoveListener(OnClickAPLoginBtn);
            smsLoginBtn.onClick.RemoveListener(OnClickSMSLoginBtn);
            forgetPwdBtn.onClick.RemoveListener(OnClickForgetPwdBtn);
            captchaBtn.onClick.RemoveListener(OnClickCaptchaBtn);
            loginBtn.onClick.RemoveListener(OnClickLoginBtn);
            emptyAccountBtn.onClick.RemoveListener(OnClickEmptyAccountBtn);
            emptyPasswordBtn.onClick.RemoveListener(OnClickEmptyPasswordBtn);
            emptyCaptchaBtn.onClick.RemoveListener(OnClickEmptyCaptchaBtn);

            account.onValueChanged.RemoveListener(OnUserValueChanged);
            password.onValueChanged.RemoveListener(OnPassValueChanged);
            captcha.onValueChanged.RemoveListener(OnCaptchaValueChanged);

            account.onEndEdit.RemoveListener(OnUserEndEdit);
            password.onEndEdit.RemoveListener(OnPassEndEdit);
            captcha.onEndEdit.RemoveListener(OnCaptchaEndEdit);

            accountError.Hiding();
            passwordError.Hiding();
            captchaError.Hiding();
            OnFrameNormal(accountFrame);
            OnFrameNormal(passwordFrame);
            OnFrameNormal(captchaFrame);
        }

        public override void Init()
        {
            base.Init();

            UnClickableBtn(loginBtn);

            if (LoginMgr.Instance.loginConfig.isRemeber
                && LoginMgr.Instance.loginConfig.account != ""
                && LoginMgr.Instance.loginConfig.password != "")
            {
                rememberPwd.isOn = true;
                account.text = LoginMgr.Instance.loginConfig.account;
                password.text = LoginMgr.Instance.loginConfig.password;
                ClickableBtn(loginBtn);
            }
            else
            {
                rememberPwd.isOn = false;
                account.text = "";
                password.text = "";
                UnClickableBtn(loginBtn);
            }

            if (account.text == "") emptyAccountBtn.gameObject.SetActive(false); else emptyAccountBtn.gameObject.SetActive(true);
            if (password.text == "") emptyPasswordBtn.gameObject.SetActive(false); else emptyPasswordBtn.gameObject.SetActive(true);
            if (captcha.text == "") emptyCaptchaBtn.gameObject.SetActive(false); else emptyCaptchaBtn.gameObject.SetActive(true);

            captchaObj.SetActive(false);
        }
        

        #region CallBack
        public void AccountPreLoginCallBack(string json)
        {
            JsonData data = JsonMapper.ToObject(json);
            int code = (int)data["code"];
            if (code == (int)RemoteCode.success)
            {
                bool hasLogin = (bool)data["data"]["hasLogin"];
                if (hasLogin &&(account.text == LoginMgr.Instance.userData.email
                    ||RegexConfig.except_regionCode.Match(account.text).ToString() == LoginMgr.Instance.userData.phone))
                {
                    Hiding();
                    return;
                }
                if (!hasLogin)
                {
                    API_Module_PlatformAccount.AccountLogin(UserUtil.RegionCode(account.text), password.text,
                        LoginMgr.Instance.captchaData != null ? LoginMgr.Instance.captchaData.captchaId : null,
                        LoginMgr.Instance.captchaData != null ? captcha.text : null, AccountLoginCallBack);

                    Hiding();
                }
                else
                {
                    LoginForms.Instance.reLoginForms.Display();
                    LoginForms.Instance.reLoginForms.loginType = LoginType.APLogin;
                    LoginForms.Instance.reLoginForms.account = UserUtil.RegionCode(account.text);
                    LoginForms.Instance.reLoginForms.password = password.text;
                    LoginForms.Instance.reLoginForms.captcha = captcha.text;
                    Hiding();
                }
            }
            else if (code == (int)RemoteCode.accountNotRegistered)
            {
                registerError.Display();
                registerError.errorInfo.text = HintConfig.not_register_account;
                UnClickableBtn(loginBtn);
            }
            else if (code == (int)RemoteCode.needCaptcha)
            {
                if (!captchaObj.activeSelf)
                {
                    captchaObj.SetActive(true);
                    loginBtn.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 76);
                    rememberPwd.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 76);
                }
                API_Module_PlatformAccount.Captcha(CaptchaCallBack);
                passwordError.Display();
                passwordError.errorInfo.text = HintConfig.error_password_limit;

                UnClickableBtn(loginBtn);
            }
            else if (code == (int)RemoteCode.accountError)
            {
                passwordError.Display();
                passwordError.errorInfo.text = HintConfig.error_password;

                if (captchaObj.activeSelf) API_Module_PlatformAccount.Captcha(CaptchaCallBack);

                UnClickableBtn(loginBtn);
            }
            else if (code == (int)RemoteCode.captchaError)
            {
                captchaError.Display();
                captchaError.errorInfo.text = HintConfig.error_captcha;

                if (captchaObj.activeSelf) API_Module_PlatformAccount.Captcha(CaptchaCallBack);

                UnClickableBtn(loginBtn);
            }
            else if (code == (int)RemoteCode.smsLengthError)
            {
                passwordError.Display();
                passwordError.errorInfo.text = HintConfig.error_password;

                if (captchaObj.activeSelf) API_Module_PlatformAccount.Captcha(CaptchaCallBack);

                UnClickableBtn(loginBtn);
            }
            else if (code == (int)RemoteCode.accountNotExist)
            {
                registerError.Display();
                registerError.errorInfo.text = HintConfig.not_register_account;
                UnClickableBtn(loginBtn);
            }
        }

        public void AccountLoginCallBack(string json)
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
                if (rememberPwd.isOn)
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
            else if (code == (int)RemoteCode.needCaptcha)
            {
                if (!captchaObj.activeSelf)
                {
                    captchaObj.SetActive(true);
                    loginBtn.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 76);
                    rememberPwd.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 76);
                }
                API_Module_PlatformAccount.Captcha(CaptchaCallBack);
                passwordError.Display();
                passwordError.errorInfo.text = HintConfig.error_password_limit;

                UnClickableBtn(loginBtn);
            }
            else if (code == (int)RemoteCode.accountError)
            {
                passwordError.Display();
                passwordError.errorInfo.text = HintConfig.error_password;

                UnClickableBtn(loginBtn);
            }
            else if (code == (int)RemoteCode.captchaError)
            {
                captchaError.Display();
                captchaError.errorInfo.text = HintConfig.error_captcha;

                UnClickableBtn(loginBtn);
            }
            else if (code == (int)RemoteCode.smsLengthError)
            {
                passwordError.Display();
                passwordError.errorInfo.text = HintConfig.error_password;

                UnClickableBtn(loginBtn);
            }
        }

        private void CaptchaCallBack(string json)
        {
            JsonData data = JsonMapper.ToObject(json);
            string[] strs = data["data"]["bs64"].ToString().Split(',');
            byte[] bytes = Convert.FromBase64String(strs[strs.Length - 1]);
            Texture2D captcha2D = Texture2D.normalTexture;
            captcha2D.LoadImage(bytes);
            LoginMgr.Instance.captchaData = new CaptchaData();
            LoginMgr.Instance.captchaData.captchaId = data["data"]["captchaId"].ToString();
            LoginMgr.Instance.captchaData.captchaImage = captcha2D;
            captchaTexture.texture = captcha2D;
        }
        #endregion

        #region Button
        public void OnClickAPLoginBtn()
        {

        }

        public void OnClickSMSLoginBtn()
        {
            LoginForms.Instance.smsLoginForms.Display();
            Hiding();
        }

        public void OnClickForgetPwdBtn()
        {
            ForgetForms.Instance.accountModeForms.Display();
            Inactive();
        }

        public void OnClickCaptchaBtn()
        {
            API_Module_PlatformAccount.Captcha(CaptchaCallBack);
        }

        public void OnClickLoginBtn()
        {
            LoginMgr.Instance.loginConfig.account = account.text;
            LoginMgr.Instance.loginConfig.password = password.text;
            LoginMgr.Instance.loginConfig.isRemeber = rememberPwd.isOn;

            API_Module_PlatformAccount.AccountPreLogin(UserUtil.RegionCode(account.text), password.text,
                LoginMgr.Instance.captchaData != null ? LoginMgr.Instance.captchaData.captchaId : null,
                LoginMgr.Instance.captchaData != null ? captcha.text : null, AccountPreLoginCallBack);

            loginBtn.interactable = false;
            loginBtn.interactable = true;
        }

        public void OnClickEmptyAccountBtn()
        {
            account.text = "";
        }

        public void OnClickEmptyPasswordBtn()
        {
            password.text = "";
        }

        public void OnClickEmptyCaptchaBtn()
        {
            captcha.text = "";
        }
        #endregion

        #region SCInputField
        public void OnUserSelect(BaseEventData baseEventData)
        {
            OnFrameSelect(accountFrame);
            accountError.Hiding();
            registerError.Hiding();
        }

        public void OnPassSelect(BaseEventData baseEventData)
        {
            OnFrameSelect(passwordFrame);
            passwordError.Hiding();
        }

        public void OnCaptchaSelect(BaseEventData baseEventData)
        {
            OnFrameSelect(captchaFrame);
            captchaError.Hiding();
        }

        private void OnUserValueChanged(string user)
        {
            if (user != "")
            {
                emptyAccountBtn.gameObject.SetActive(true);
            }
            else
            {
                emptyAccountBtn.gameObject.SetActive(false);
            }

            if (user.Length != 0 && password.text.Length != 0 && (captchaObj.activeSelf ? captcha.text != null : true))
            {
                ClickableBtn(loginBtn);
            }
            else
            {
                UnClickableBtn(loginBtn);
            }
        }

        private void OnPassValueChanged(string pass)
        {
            if (pass != "")
            {
                emptyPasswordBtn.gameObject.SetActive(true);
            }
            else
            {
                emptyPasswordBtn.gameObject.SetActive(false);
            }

            if (pass.Length != 0 && account.text.Length != 0 && (captchaObj.activeSelf ? captcha.text != null : true))
            {
                ClickableBtn(loginBtn);
            }
            else
            {
                UnClickableBtn(loginBtn);
            }
        }

        private void OnCaptchaValueChanged(string captcha)
        {
            if (captcha != "")
            {
                emptyCaptchaBtn.gameObject.SetActive(true);
            }
            else
            {
                emptyCaptchaBtn.gameObject.SetActive(false);
            }

            if (captcha.Length != 0 && account.text.Length != 0 && password.text.Length != 0)
            {
                ClickableBtn(loginBtn);
            }
            else
            {
                UnClickableBtn(loginBtn);
            }
        }

        private void OnUserEndEdit(string user)
        {
            if (user == "")
            {
                OnFrameError(accountFrame);                
                accountError.errorInfo.text = HintConfig.input_account;
                accountError.Display();
            }
            else
            {
                OnFrameNormal(accountFrame);
                accountError.Hiding();
            }
        }

        private void OnPassEndEdit(string password)
        {
            if (password == "")
            {
                OnFrameError(passwordFrame);
                passwordError.Display();
                passwordError.errorInfo.text = HintConfig.input_password;
            }
            else
            {
                OnFrameNormal(passwordFrame);
            }
        }

        private void OnCaptchaEndEdit(string captcha)
        {
            if (captcha == "")
            {
                OnFrameError(captchaFrame);
                captchaError.Display();
                captchaError.errorInfo.text = HintConfig.input_captcha;
            }
            else
            {
                OnFrameNormal(captchaFrame);
            }
        }
        #endregion

    }
}
