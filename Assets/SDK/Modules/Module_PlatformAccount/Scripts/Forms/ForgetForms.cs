namespace SC.XR.Unity.Module_PlatformAccount
{
    public class ForgetForms : BaseUIForms
    {
        public static ForgetForms Instance;

        private void Awake()
        {
            Instance = this;
        }

        public override void Init()
        {
            base.Init();
            accountModeForms.Init();
            smsForgetForms.Init();
            emailForgetForms.Init();
            selectModeForms.Init();
            resetPasswordForms.Init();
            resetSuccessForms.Init();
        }

        public AccountModeForms accountModeForms;
        public SMSForgetForms smsForgetForms;
        public EmailForgetForms emailForgetForms;
        public SelectModeForms selectModeForms;
        public ResetPasswordForms resetPasswordForms;
        public ResetSuccessForms resetSuccessForms;

    }
}
