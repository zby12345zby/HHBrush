using LitJson;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public enum ResetType
    {
        Phone,
        Mail
    }

    public class ResetPasswordForms : BaseUIForms
    {
        public SCInputField password;
        public SCInputField confirmPassword;

        public Image passwordFrame;
        public Image confirmPasswordFrame;

        public InputErrorForms passwordError;
        public InputErrorForms confirmPasswordError;

        public Button emptyPassword;
        public Button emptyConfirmPassword;

        public RectTransform hint;

        public Button submitBtn;

        public ResetType resetType;

        public override void Display()
        {
            base.Display();

            submitBtn.onClick.AddListener(OnClickSubmitBtn);

            password.onValueChanged.AddListener(OnPassValueChanged);
            confirmPassword.onValueChanged.AddListener(OnConfirmPassValueChanged);

            password.onEndEdit.AddListener(OnPassEndEdit);
            confirmPassword.onEndEdit.AddListener(OnConfirmPassEndEdit);

            emptyPassword.onClick.AddListener(EmptyPassword);
            emptyConfirmPassword.onClick.AddListener(EmptyConfirmPassword);

        }

        public override void Hiding()
        {
            base.Hiding();

            submitBtn.onClick.RemoveListener(OnClickSubmitBtn);

            password.onValueChanged.RemoveListener(OnPassValueChanged);
            confirmPassword.onValueChanged.RemoveListener(OnConfirmPassValueChanged);

            password.onEndEdit.RemoveListener(OnPassEndEdit);
            confirmPassword.onEndEdit.RemoveListener(OnConfirmPassEndEdit);

            emptyPassword.onClick.RemoveListener(EmptyPassword);
            emptyConfirmPassword.onClick.RemoveListener(EmptyConfirmPassword);

            passwordError.Hiding();
            confirmPasswordError.Hiding();

        }

        public override void Init()
        {
            base.Init();

            password.text = "";
            confirmPassword.text = "";
            OnFrameNormal(passwordFrame);
            OnFrameNormal(confirmPasswordFrame);            
            emptyPassword.gameObject.SetActive(false);
            emptyConfirmPassword.gameObject.SetActive(false);
            UnClickableBtn(submitBtn);
        }

        #region CallBack
        public void ResetSucessCallBack(string json)
        {
            JsonData data = JsonMapper.ToObject(json);
            if ((int)data["code"] == (int)RemoteCode.success)
            {
                ForgetForms.Instance.resetSuccessForms.Display();
                Hiding();
            }
        }
        #endregion

        #region Button
        public void OnClickSubmitBtn()
        {
            switch (resetType)
            {
                case ResetType.Phone:
                    API_Module_PlatformAccount.ForgetPasswordByPhone(password.text, confirmPassword.text, LoginMgr.Instance.forgetData.codeToken, ResetSucessCallBack);
                    break;
                case ResetType.Mail:
                    API_Module_PlatformAccount.ForgetPasswordByMail(password.text, confirmPassword.text, LoginMgr.Instance.forgetData.codeToken, ResetSucessCallBack);
                    break;
            }

        }
        #endregion

        #region SCInputField       
        public void OnPassSelect(BaseEventData baseEventData)
        {
            OnFrameSelect(passwordFrame);
            passwordError.Hiding();
        }

        public void OnConfirmPassSelect(BaseEventData baseEventData)
        {
            OnFrameSelect(confirmPasswordFrame);
            confirmPasswordError.Hiding();
            hint.anchoredPosition = new Vector2(-23.0f, -81.0f);
        }

        private void OnPassValueChanged(string pass)
        {
            if (pass!="")
            {
                emptyPassword.gameObject.SetActive(true);
            }
            if (RegexConfig.format_password.IsMatch(pass) 
                && RegexConfig.format_password.IsMatch(confirmPassword.text)
                && pass == confirmPassword.text)
            {
                ClickableBtn(submitBtn);
            }
            else
            {
                UnClickableBtn(submitBtn);
            }
        }

        private void OnConfirmPassValueChanged(string confirm)
        {
            if (confirm!="")
            {
                emptyConfirmPassword.gameObject.SetActive(true);
            }
            if (RegexConfig.format_password.IsMatch(confirm) 
                && RegexConfig.format_password.IsMatch(password.text)
                && confirm == password.text)
            {
                ClickableBtn(submitBtn);
            }
            else
            {
                UnClickableBtn(submitBtn);
            }
        }

        private void OnPassEndEdit(string pass)
        {
            if (pass == "")
            {
                OnFrameError(passwordFrame);
                passwordError.Display();
                passwordError.errorInfo.text = HintConfig.input_password;
            }
            else if (confirmPassword.text != "" && pass != confirmPassword.text)
            {
                OnFrameNormal(passwordFrame);
                OnFrameError(confirmPasswordFrame);
                confirmPasswordError.Display();
                confirmPasswordError.errorInfo.text = HintConfig.error_confirm_password;
                hint.anchoredPosition = new Vector2(-23.0f, -104.3f);
            }
            else if (!UserUtil.PasswordFormat(pass))
            {
                OnFrameError(passwordFrame);
                passwordError.Display();
                passwordError.errorInfo.text = HintConfig.error_password_format;
            }
            else if(confirmPassword.text != "" && pass == confirmPassword.text)
            {
                OnFrameNormal(passwordFrame);
                OnFrameNormal(confirmPasswordFrame);
                confirmPasswordError.Hiding();
            }            
            else
            {
                OnFrameNormal(passwordFrame);
            }
        }

        private void OnConfirmPassEndEdit(string confirm)
        {
            if (confirm == "")
            {
                OnFrameError(confirmPasswordFrame);
                confirmPasswordError.Display();
                confirmPasswordError.errorInfo.text = HintConfig.input_confirm_password;
                hint.anchoredPosition = new Vector2(-23.0f, -104.3f);
            }
            else if (!UserUtil.PasswordFormat(confirm))
            {
                OnFrameError(confirmPasswordFrame);
                confirmPasswordError.Display();
                confirmPasswordError.errorInfo.text = HintConfig.error_confirm_password_format;
                hint.anchoredPosition = new Vector2(-23.0f, -104.3f);
            }
            else if (password.text != "" && confirm != password.text)
            {
                OnFrameError(confirmPasswordFrame);
                confirmPasswordError.Display();
                confirmPasswordError.errorInfo.text = HintConfig.error_confirm_password;
                hint.anchoredPosition = new Vector2(-23.0f, -104.3f);
            }            
            else
            {                
                OnFrameNormal(confirmPasswordFrame);
                confirmPasswordError.Hiding();
            }
        }

        private void EmptyPassword()
        {
            password.text = "";
        }

        private void EmptyConfirmPassword()
        {
            confirmPassword.text = "";
        }
        #endregion

    }
}
