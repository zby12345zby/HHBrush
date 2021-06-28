namespace SC.XR.Unity.Module_PlatformAccount
{
    public class ErrorForms : BaseUIForms
    {
        public static ErrorForms Instance;

        private void Awake()
        {
            Instance = this;
        }

        public override void Init()
        {
            base.Init();
            networkErrorForms.Init();
            reLoginErrorForms.Init();
            codeErrorForms.Init();
        }

        public NetworkErrorForms networkErrorForms;
        public ReLoginErrorForms reLoginErrorForms;
        public CodeErrorForms codeErrorForms;
    }
}
