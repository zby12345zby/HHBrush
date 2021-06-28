using UnityEngine.UI;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public class NetworkErrorForms : BaseUIForms
    {

        public Button backLoginBtn;

        public override void Display()
        {
            base.Display();

            backLoginBtn.onClick.AddListener(OnClickBackLoginBtn);
        }

        public override void Hiding()
        {
            base.Hiding();

            backLoginBtn.onClick.RemoveListener(OnClickBackLoginBtn);
        }

        public override void Init()
        {
            base.Init();

            ClickableBtn(backLoginBtn);

        }

        #region Button
        public void OnClickBackLoginBtn()
        {
            LoginForms.Instance.apLoginForms.Display();
            Hiding();
        }
        #endregion

    }
}
