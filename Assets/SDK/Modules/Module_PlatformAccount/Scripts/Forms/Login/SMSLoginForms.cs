using LitJson;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public class SMSLoginForms : BaseLoginForms
    {

        public SCInputField phone;
        public SCInputField smsCode;

        public Image phoneFrame;
        public Image smsCodeFrame;

        public InputErrorForms phoneError;
        public InputErrorForms smsCodeError;
        public InputErrorForms registerError;

        public Button smsLoginBtn;
        public Button apLoginBtn;

        public Button loginBtn;
        public Button sendSMSCodeBtn;

        public Button emptyAccountBtn;
        public Button emptyCaptchaBtn;
        public Button codeErrorBtn;

        public Text sendSMSCodeBtnText;

        public string diallingCode = "86";

        public override void Display()
        {
            base.Display();
            
            apLoginBtn.onClick.AddListener(OnClickAPLoginBtn);
            smsLoginBtn.onClick.AddListener(OnClickSMSLoginBtn);
            loginBtn.onClick.AddListener(OnClickLoginBtn);
            sendSMSCodeBtn.onClick.AddListener(OnClickSendSMSCodeBtn);
            codeErrorBtn.onClick.AddListener(OnClickCodeErrorBtn);
            emptyAccountBtn.onClick.AddListener(OnClickEmptyAccountBtn);
            emptyCaptchaBtn.onClick.AddListener(OnClickEmptyCaptchaBtn);

            phone.onValueChanged.AddListener(OnUserValueChanged);
            smsCode.onValueChanged.AddListener(OnCaptchaValueChanged);

            phone.onEndEdit.AddListener(OnUserEndEdit);
            smsCode.onEndEdit.AddListener(OnCaptchaEndEdit);                     
        }

        public override void Hiding()
        {
            base.Hiding();

            apLoginBtn.onClick.RemoveListener(OnClickAPLoginBtn);
            smsLoginBtn.onClick.RemoveListener(OnClickSMSLoginBtn);
            loginBtn.onClick.RemoveListener(OnClickLoginBtn);
            sendSMSCodeBtn.onClick.RemoveListener(OnClickSendSMSCodeBtn);
            codeErrorBtn.onClick.AddListener(OnClickCodeErrorBtn);
            emptyAccountBtn.onClick.RemoveListener(OnClickEmptyAccountBtn);
            emptyCaptchaBtn.onClick.RemoveListener(OnClickEmptyCaptchaBtn);

            phone.onValueChanged.RemoveListener(OnUserValueChanged);
            smsCode.onValueChanged.RemoveListener(OnCaptchaValueChanged);

            phone.onEndEdit.RemoveListener(OnUserEndEdit);
            smsCode.onEndEdit.RemoveListener(OnCaptchaEndEdit);

            phoneError.Hiding();
            smsCodeError.Hiding();
            OnFrameNormal(phoneFrame);
            OnFrameNormal(smsCodeFrame);
        }

        public override void Init()
        {
            base.Init();
            UnClickableBtn(loginBtn);
            UnClickableBtn(sendSMSCodeBtn);
            sendSMSCodeBtnText.text = "发送验证码";
            phone.text = "";
            smsCode.text = "";
            emptyAccountBtn.gameObject.SetActive(false);
            emptyCaptchaBtn.gameObject.SetActive(false);
            if (sendCodeCooling != null)
            {
                StopCoroutine(sendCodeCooling);
                sendCodeCooling = null;                
                isSendCodeCooling = false;
            }
        }

        #region CallBack
        public void SMSLoginCodeCallBack(string json)
        {
            JsonData data = JsonMapper.ToObject(json);
            int code = (int)data["code"];
            if (code == (int)RemoteCode.success)
            {
                int rest = (int)data["data"]["rest"];
                int total = (int)data["data"]["total"];

                sendCodeCooling = StartCoroutine(SendCodeCooling(rest, total, "已发送", "发送验证码", sendSMSCodeBtn, sendSMSCodeBtnText));

                if (rest <= 2)
                {
                    smsCodeError.Display();
                    smsCodeError.errorInfo.text = "今日验证码发送次数仅剩: " + rest + "次";
                }
                if (rest == 0)
                {
                    smsCodeError.Display();
                    smsCodeError.errorInfo.text = "今日验证码发送次数已用完";
                }
            }
            else if (code == (int)RemoteCode.noSmsCodeNum)
            {
                smsCodeError.Display();
                smsCodeError.errorInfo.text = "今日验证码发送次数已用完";
            }

        }

        public void PhonePreLoginCallBack(string json)
        {
            JsonData data = JsonMapper.ToObject(json);
            int code = (int)data["code"];
            if (code == (int)RemoteCode.success)
            {
                bool hasLogin = (bool)data["data"]["hasLogin"];
                if (hasLogin && RegexConfig.except_regionCode.Match(phone.text).ToString() == LoginMgr.Instance.userData.phone)
                {
                    Hiding();
                    return;
                }
                if (!hasLogin)
                {
                    API_Module_PlatformAccount.PhoneLogin(phone.text, diallingCode, smsCode.text, PhoneLoginCallBack);
                    Hiding();
                }
                else
                {
                    LoginForms.Instance.reLoginForms.Display();
                    LoginForms.Instance.reLoginForms.loginType = LoginType.SMSLogin;
                    LoginForms.Instance.reLoginForms.phone = phone.text;
                    LoginForms.Instance.reLoginForms.diallingCode = diallingCode;
                    LoginForms.Instance.reLoginForms.smsCode = smsCode.text;
                    Hiding();
                }
            }
            else if (code == (int)RemoteCode.accountNotRegistered)
            {
                registerError.Display();
                registerError.errorInfo.text = HintConfig.not_register_phone;
            }
            else if (code == (int)RemoteCode.smsLengthError ||
                code == (int)RemoteCode.smsCodeError)
            {
                smsCodeError.Display();
                smsCodeError.errorInfo.text = HintConfig.error_login_code;
            }
        }

        public void PhoneLoginCallBack(string json)
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
                Hiding();
            }
            else if (code == (int)RemoteCode.smsLengthError ||
                code == (int)RemoteCode.smsCodeError)
            {
                smsCodeError.Display();
                smsCodeError.errorInfo.text = HintConfig.error_login_code;
            }
        }
        #endregion

        #region Button
        public void OnClickAPLoginBtn()
        {
            LoginForms.Instance.apLoginForms.Display();
            Hiding();
        }


        public void OnClickSMSLoginBtn()
        {

        }

        public void OnClickLoginBtn()
        {
            API_Module_PlatformAccount.PhonePreLogin(phone.text, diallingCode, smsCode.text, PhonePreLoginCallBack);
            loginBtn.interactable = false;
            loginBtn.interactable = true;
        }

        public void OnClickSendSMSCodeBtn()
        {
            API_Module_PlatformAccount.SMSLoginCode(phone.text, diallingCode, SMSLoginCodeCallBack);
            sendSMSCodeBtn.interactable = false;
            sendSMSCodeBtn.interactable = true;
            CoolingBtn(sendSMSCodeBtn);
        }

        public void OnClickCodeErrorBtn()
        {
            ErrorForms.Instance.codeErrorForms.lastUIForms = this;
            ErrorForms.Instance.codeErrorForms.Display();
            Inactive();
        }

        public void OnClickEmptyAccountBtn()
        {
            phone.text = "";
        }

        public void OnClickEmptyCaptchaBtn()
        {
            smsCode.text = "";
        }
        #endregion

        #region SCInputField
        public void OnUserSelect(BaseEventData baseEventData)
        {
            OnFrameSelect(phoneFrame);
            phoneError.Hiding();
            registerError.Hiding();
        }

        public void OnSMSCodeSelect(BaseEventData baseEventData)
        {
            OnFrameSelect(smsCodeFrame);
            smsCodeError.Hiding();
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

            if (user.Length < 7.0)
            {
                UnClickableBtn(loginBtn);
                UnClickableBtn(sendSMSCodeBtn);
            }
            else
            {
                if(!isSendCodeCooling)ClickableBtn(sendSMSCodeBtn);
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

            if (captcha.Length != 0 && phone.text.Length >= 7)
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
                OnFrameError(phoneFrame);
                phoneError.Display();
                phoneError.errorInfo.text = HintConfig.input_phone;
            }
            else if (user.Length < 7.0)
            {
                OnFrameError(phoneFrame);
                phoneError.Display();
                phoneError.errorInfo.text = HintConfig.error_phone;
            }
            else
            {
                OnFrameNormal(phoneFrame);
                phoneError.Hiding();
            }
        }

        private void OnCaptchaEndEdit(string captcha)
        {
            if (captcha == "")
            {
                OnFrameError(smsCodeFrame);
                smsCodeError.Display();
                smsCodeError.errorInfo.text = HintConfig.input_captcha;
            }
            else
            {
                OnFrameNormal(smsCodeFrame);
            }
        }
        #endregion

    }
}
