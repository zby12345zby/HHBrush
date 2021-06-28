using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public class ResetSuccessForms : BaseUIForms
    {
        public Button loginImmediatelyBtn;

        public Text content;

        public override void Display()
        {
            base.Display();

            loginImmediatelyBtn.onClick.AddListener(OnClickLoginImmediatelyBtn);

            StartCoroutine(BackToLogin());
        }

        public override void Hiding()
        {
            base.Hiding();

            loginImmediatelyBtn.onClick.RemoveListener(OnClickLoginImmediatelyBtn);
        }

        public override void Init()
        {
            base.Init();

            ClickableBtn(loginImmediatelyBtn);
        }

        #region Button
        public void OnClickLoginImmediatelyBtn()
        {
            LoginForms.Instance.apLoginForms.Display();
            Hiding();
        }
        #endregion

        #region Coroutine
        IEnumerator BackToLogin()
        {
            int time = 5;
            while (time>0)
            {
                content.text = time + "秒后将退出至登录界面，请重新登录";
                time--;
                yield return new WaitForSeconds(1);                
            }            
            LoginForms.Instance.apLoginForms.Display();
            Hiding();
        }
        #endregion

    }
}
