using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public abstract class BaseForgetForms :BaseCoroutineForms
    {
        [Header("DefaultMenu")]
        public GameObject defaultMenu;
        public Button sendCodeBtn;
        public Button selectModeBtn;
        
        [Header("CodeMenu")]
        public GameObject codeMenu;
        public SCInputField code;
        public Image codeFrame;
        public InputErrorForms codeError;

        public Button resendBtn;
        public Button codeErrorBtn;
        public Button nextStepBtn;
        public Button emptyCodeBtn;

        public Text defaultInfoText;
        public Text codeInfoText;
        public Text resendBtnText;

        public override void Display()
        {
            base.Display();

            //Default
            sendCodeBtn.onClick.AddListener(OnClickSendCodeBtn);
            selectModeBtn.onClick.AddListener(OnClickSelectModeBtn);
            //SMS
            resendBtn.onClick.AddListener(OnClickResendBtn);
            codeErrorBtn.onClick.AddListener(OnClickCodeErrorBtn);
            nextStepBtn.onClick.AddListener(OnClickNextStepBtn);
            emptyCodeBtn.onClick.AddListener(OnClickEmptyCodeBtn);

            code.onValueChanged.AddListener(OnCodeValueChanged);
            code.onEndEdit.AddListener(OnCodeEndEdit);
        }

        public override void Hiding()
        {
            base.Hiding();

            //Default
            sendCodeBtn.onClick.RemoveListener(OnClickSendCodeBtn);
            selectModeBtn.onClick.RemoveListener(OnClickSelectModeBtn);
            //SMS
            resendBtn.onClick.RemoveListener(OnClickResendBtn);
            codeErrorBtn.onClick.RemoveListener(OnClickCodeErrorBtn);
            nextStepBtn.onClick.RemoveListener(OnClickNextStepBtn);
            emptyCodeBtn.onClick.RemoveListener(OnClickEmptyCodeBtn);

            code.onValueChanged.RemoveListener(OnCodeValueChanged);
            code.onEndEdit.RemoveListener(OnCodeEndEdit);

            codeError.Hiding();
            OnFrameNormal(codeFrame);
        }


        public override void Init()
        {
            base.Init();
            
            ClickableBtn(sendCodeBtn);
            UnClickableBtn(resendBtn);
            UnClickableBtn(nextStepBtn);
            code.text = "";
            emptyCodeBtn.gameObject.SetActive(false);

            if (sendCodeCooling != null)
            {
                StopCoroutine(sendCodeCooling);
                ClickableBtn(resendBtn);
                resendBtnText.text = "获取验证码";
                isSendCodeCooling = false;
            }
        }

        #region Button
        public virtual void OnClickSendCodeBtn() { }

        public virtual void OnClickSelectModeBtn()
        {
            ForgetForms.Instance.selectModeForms.Display();
            ForgetForms.Instance.selectModeForms.lastUIForms = this;
            Hiding();
        }

        public virtual void OnClickResendBtn() { }

        public virtual void OnClickCodeErrorBtn()
        {
            ErrorForms.Instance.codeErrorForms.lastUIForms = this;
            ErrorForms.Instance.codeErrorForms.Display();
            Inactive();
        }

        public virtual void OnClickNextStepBtn() { }

        public virtual void OnClickEmptyCodeBtn() {
            code.text = "";
        }
        #endregion

        #region SCInputField
        public virtual void OnCodeSelect(BaseEventData baseEventData)
        {
            OnFrameSelect(codeFrame);
            codeError.Hiding();
        }

        protected virtual void OnCodeValueChanged(string user)
        {
            if (user != "")
            {
                emptyCodeBtn.gameObject.SetActive(true);
                ClickableBtn(nextStepBtn);
            }
            else
            {
                emptyCodeBtn.gameObject.SetActive(false);
                UnClickableBtn(nextStepBtn);
            }
        }
        protected virtual void OnCodeEndEdit(string user)
        {
            if (user == "")
            {
                OnFrameError(codeFrame);
                codeError.Display();
                codeError.errorInfo.text = HintConfig.input_captcha;
            }
            else
            {
                OnFrameNormal(codeFrame);
            }
        }
        #endregion

    }
}
