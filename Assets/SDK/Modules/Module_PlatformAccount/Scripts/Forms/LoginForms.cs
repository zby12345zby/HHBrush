namespace SC.XR.Unity.Module_PlatformAccount
{
    public class LoginForms : BaseUIForms
    {

        public static LoginForms Instance;

        private void Awake()
        {
            Instance = this;
        }

        public override void Init()
        {
            base.Init();
            apLoginForms.Init();
            smsLoginForms.Init();
            reLoginForms.Init();
        }

        public APLoginForms apLoginForms;
        public SMSLoginForms smsLoginForms;
        public ReLoginForms reLoginForms;

    }
}
