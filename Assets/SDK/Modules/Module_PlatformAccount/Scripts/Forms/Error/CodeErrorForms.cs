using UnityEngine.UI;

namespace SC.XR.Unity.Module_PlatformAccount
{

    public class CodeErrorForms : BaseUIForms
    {

        public Button backBtn;
        public BaseUIForms lastUIForms;

        public override void Display()
        {
            base.Display();

            backBtn.onClick.AddListener(OnClickBackBtn);
        }

        public override void Hiding()
        {
            base.Hiding();
            backBtn.onClick.RemoveListener(OnClickBackBtn);
        }

        public override void Init()
        {
            base.Init();

            ClickableBtn(backBtn);
        }

        #region Button
        public void OnClickBackBtn()
        {
            lastUIForms.Active();
            Hiding();
            backBtn.interactable = false;
            backBtn.interactable = true;
        }
        #endregion

    }
}
