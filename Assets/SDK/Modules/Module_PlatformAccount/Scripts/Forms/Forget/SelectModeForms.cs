using UnityEngine.UI;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public class SelectModeForms : BaseUIForms
    {

        public Toggle phoneVerify;
        public Toggle emailVerify;

        public Button nextStepBtn;
        public Button backBtn;

        public BaseUIForms lastUIForms;

        public override void Display()
        {
            base.Display();

            phoneVerify.onValueChanged.AddListener(OnPhoneVerifyToggle);
            emailVerify.onValueChanged.AddListener(OnEmailVerifyToggle);

            nextStepBtn.onClick.AddListener(OnClickNextStepBtn);
            backBtn.onClick.AddListener(OnClickBackBtn);
        }

        public override void Hiding()
        {
            base.Hiding();

            phoneVerify.onValueChanged.RemoveListener(OnPhoneVerifyToggle);
            emailVerify.onValueChanged.RemoveListener(OnEmailVerifyToggle);

            nextStepBtn.onClick.RemoveListener(OnClickNextStepBtn);
            backBtn.onClick.RemoveListener(OnClickBackBtn);
        }

        public override void Init()
        {
            base.Init();

            ClickableBtn(nextStepBtn);
        }

        #region Button
        public void OnClickNextStepBtn()
        {
            if (phoneVerify.isOn)
            {
                ForgetForms.Instance.smsForgetForms.Display();
                ForgetForms.Instance.smsForgetForms.defaultMenu.SetActive(true);
                ForgetForms.Instance.smsForgetForms.codeMenu.SetActive(false);
                Hiding();
            }
            else if (emailVerify.isOn)
            {
                ForgetForms.Instance.emailForgetForms.Display();
                ForgetForms.Instance.smsForgetForms.defaultMenu.SetActive(true);
                ForgetForms.Instance.smsForgetForms.codeMenu.SetActive(false);
                Hiding();
            }
        }


        public void OnClickBackBtn()
        {
            lastUIForms.Display();
            Hiding();
        }
        #endregion

        #region Toggle
        private void OnPhoneVerifyToggle(bool isOn)
        {
            if (isOn) emailVerify.isOn = !isOn;
            if (!emailVerify.isOn) phoneVerify.isOn = true;
        }


        private void OnEmailVerifyToggle(bool isOn)
        {
            if (isOn) phoneVerify.isOn = !isOn;
            if (!phoneVerify.isOn) emailVerify.isOn = true;
        }
        #endregion

    }
}
