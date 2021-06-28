using LitJson;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public class SMSForgetForms : BaseForgetForms
    {
        [HideInInspector]
        public string phone;
        public string diallingCode = "86";

        public override void Display()
        {            
            base.Display();

            diallingCode = LoginMgr.Instance.accountInfoData.diallingCode;
            phone = LoginMgr.Instance.accountInfoData.phone;

            defaultInfoText.text = "点击发送按钮，将发送一条含有验证码的短信至手机：\n" + "+86 " + phone;          
        }

        #region CallBack
        public void SMSCodeForgetCallBack(string json)
        {
            API_Module_PlatformAccount.SMSRestCodeTimes(phone, diallingCode, SMSRestCodeTimesCallBack);
        }

        public void SMSRestCodeTimesCallBack(string json)
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
                    codeError.errorInfo.text = "今日验证码发送次数仅剩: " + rest + "次";
                }
                if (rest == 0)
                {
                    codeError.Display();
                    codeError.errorInfo.text =HintConfig.error_forget_code_limit;
                }
            }

            codeInfoText.text = "短信验证码已经发送至手机：<color=#9f1010>+86 " + phone + "</color>";
            defaultMenu.SetActive(false);
            codeMenu.SetActive(true);
            sendCodeBtn.interactable = false;
            sendCodeBtn.interactable = true;
            CoolingBtn(resendBtn);

        }

        public void SMSVerifyForgetPasswordCallBack(string json)
        {
            JsonData data = JsonMapper.ToObject(json);
            if ((int)data["code"] == (int)RemoteCode.success)
            {
                ForgetForms.Instance.resetPasswordForms.resetType = ResetType.Phone;
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
            API_Module_PlatformAccount.SMSCodeForget(phone, diallingCode, SMSCodeForgetCallBack);
        }

        public override void OnClickResendBtn()
        {
            API_Module_PlatformAccount.SMSCodeForget(phone, diallingCode, SMSCodeForgetCallBack);
        }

        public override void OnClickNextStepBtn()
        {
            API_Module_PlatformAccount.SMSVerifyForgetPassword(phone, diallingCode, code.text, SMSVerifyForgetPasswordCallBack);
        }
        #endregion
        
    }
}
