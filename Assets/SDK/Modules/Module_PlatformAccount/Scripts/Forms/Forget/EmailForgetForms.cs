using LitJson;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public class EmailForgetForms : BaseForgetForms
    {
        [HideInInspector]
        public string email;
        
        public override void Display()
        {
            base.Display();

            email = LoginMgr.Instance.accountInfoData.email;

            defaultInfoText.text = "点击发送按钮，将发送一条含有验证码的短信至邮箱：\n" + email;
        }

        #region CallBack
        public void EmailCodeForgetCallBack(string json)
        {
            API_Module_PlatformAccount.EmailRestCodeTimes(email, EmailRestCodeTimesCallBack);
        }

        public void EmailRestCodeTimesCallBack(string json)
        {
            JsonData data = JsonMapper.ToObject(json);
            int code = (int)data["code"];
            if (code == (int)RemoteCode.success)
            {
                int rest = (int)data["data"]["rest"];
                int total = (int)data["data"]["total"];

                sendCodeCooling = StartCoroutine(SendCodeCooling(rest, total, "重新发送", "获取验证码", resendBtn, resendBtnText));

                if (rest <= 2)
                {
                    codeError.Display();
                    codeError.errorInfo.text = HintConfig.error_forget_code_leave + rest + "次";
                }
                if (rest == 0)
                {
                    codeError.Display();
                    codeError.errorInfo.text = HintConfig.error_forget_code_limit;
                }
            }

            codeInfoText.text = "邮箱验证码已经发送至邮箱：<color=#9f1010>" + email + "</color>";
            defaultMenu.SetActive(false);
            codeMenu.SetActive(true);
            CoolingBtn(resendBtn);

        }

        public void EmailVerifyForgetPasswordCallBack(string json)
        {
            JsonData data = JsonMapper.ToObject(json);
            if ((int)data["code"] == (int)RemoteCode.success)
            {
                ForgetForms.Instance.resetPasswordForms.resetType = ResetType.Mail;
                ForgetForms.Instance.resetPasswordForms.Display();
                Hiding();
            }
            else if ((int)data["code"] == (int)RemoteCode.smsCodeError ||
                (int)data["code"] == (int)RemoteCode.smsLengthError)
            {
                OnFrameError(codeFrame);
                codeError.Display();
                codeError.errorInfo.text = HintConfig.error_forget_code;
            }
        }

        #endregion

        #region Button
        public override void OnClickSendCodeBtn()
        {
            API_Module_PlatformAccount.EmailCodeForget(email, EmailCodeForgetCallBack);
        }

        public override void OnClickResendBtn()
        {
            API_Module_PlatformAccount.EmailCodeForget(email, EmailCodeForgetCallBack);
        }

        public override void OnClickNextStepBtn()
        {
            API_Module_PlatformAccount.EmailVerifyForgetPassword(email, code.text, EmailVerifyForgetPasswordCallBack);
            nextStepBtn.interactable = false;
            nextStepBtn.interactable = true;
        }
        #endregion
        
    }
}
