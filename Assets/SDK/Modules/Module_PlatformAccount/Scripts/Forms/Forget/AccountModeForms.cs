using LitJson;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public class AccountModeForms : BaseUIForms
    {        
               
        public SCInputField bindingAccount;

        public Button nextStepBtn;
        public Button backLoginBtn;
        public Button emptyBindingAccountBtn;

        public Image accountFrame;
        public Text hint;

        public InputErrorForms accountError;

        AccountType accountType;

        public override void Display()
        {
            base.Display();

            nextStepBtn.onClick.AddListener(OnClickNextStepBtn);
            backLoginBtn.onClick.AddListener(OnClickBackLoginBtn);
            emptyBindingAccountBtn.onClick.AddListener(OnClickEmptyBindingAccountBtn);

            bindingAccount.onValueChanged.AddListener(OnUserValueChanged);
            bindingAccount.onEndEdit.AddListener(OnUserEndEdit);                                           
        }

        public override void Hiding()
        {
            base.Hiding();

            nextStepBtn.onClick.RemoveListener(OnClickNextStepBtn);
            backLoginBtn.onClick.RemoveListener(OnClickBackLoginBtn);
            emptyBindingAccountBtn.onClick.RemoveListener(OnClickEmptyBindingAccountBtn);

            bindingAccount.onValueChanged.RemoveListener(OnUserValueChanged);
            bindingAccount.onEndEdit.RemoveListener(OnUserEndEdit);

            accountError.Hiding();
        }

        public override void Init()
        {
            base.Init();

            bindingAccount.text = "";
            bindingAccount.textComponent.color = ColorConfig.text_normal;
            bindingAccount.placeholder.color = ColorConfig.placeholder_normal;
            hint.GetComponent<RectTransform>().anchoredPosition = new Vector2(-4.6f, -33.5f);
            UnClickableBtn(nextStepBtn);

            OnFrameNormal(accountFrame);
        }

        #region CallBack
        public void AccountSafeStateCallBack(string json)
        {
            JsonData data = new JsonData();
            data = JsonMapper.ToObject(json);
            if ((int)data["code"] == (int)RemoteCode.success)
            {
                AccountInfoData info = new AccountInfoData();
                info.phone = data["data"]["phone"].ToString();
                info.email = data["data"]["email"].ToString();
                info.diallingCode = data["data"]["diallingCode"].ToString();
                LoginMgr.Instance.accountInfoData = info;
                switch (accountType)
                {
                    case AccountType.NotAccount:
                        break;
                    case AccountType.Phone:
                        ForgetForms.Instance.smsForgetForms.Display();
                        ForgetForms.Instance.smsForgetForms.defaultMenu.SetActive(true);
                        ForgetForms.Instance.smsForgetForms.codeMenu.SetActive(false);
                        Hiding();
                        break;
                    case AccountType.Email:
                        ForgetForms.Instance.emailForgetForms.Display();
                        ForgetForms.Instance.smsForgetForms.defaultMenu.SetActive(true);
                        ForgetForms.Instance.smsForgetForms.codeMenu.SetActive(false);
                        Hiding();
                        break;
                }
            }
            else if ((int)data["code"] == (int)RemoteCode.accountNotExist)
            {
                OnFrameError(accountFrame);
                bindingAccount.textComponent.color = ColorConfig.text_error;
                bindingAccount.placeholder.color = ColorConfig.placeholder_error;
                accountError.Display();
                accountError.errorInfo.text = HintConfig.not_exist_user;
                hint.GetComponent<RectTransform>().anchoredPosition = new Vector2(-4.6f, -46.2f);
            }
        }
        #endregion

        #region Button
        public void OnClickNextStepBtn()
        {
            if (!UserUtil.AccountFormat(bindingAccount.text))
            {
                OnFrameError(accountFrame);
                bindingAccount.textComponent.color = ColorConfig.text_error;
                bindingAccount.placeholder.color = ColorConfig.placeholder_error;
                accountError.Display();
                accountError.errorInfo.text = HintConfig.error_account_format;
                hint.GetComponent<RectTransform>().anchoredPosition = new Vector2(-4.6f, -46.2f);
                return;
            }

            API_Module_PlatformAccount.AccountSafeState(UserUtil.RegionCode(bindingAccount.text), AccountSafeStateCallBack);
            accountType = UserUtil.PhoneOrEmail(bindingAccount.text);

            nextStepBtn.interactable = false;
            nextStepBtn.interactable = true;
        }

        public void OnClickBackLoginBtn()
        {
            LoginForms.Instance.apLoginForms.Active();
            Hiding();
        }

        public void OnClickEmptyBindingAccountBtn()
        {
            bindingAccount.text = "";
        }

        #endregion

        #region SCInputField
        public void OnUserSelect(BaseEventData baseEventData)
        {
            OnFrameSelect(accountFrame);
            bindingAccount.textComponent.color = ColorConfig.text_normal;
            bindingAccount.placeholder.color = ColorConfig.placeholder_normal;
            hint.GetComponent<RectTransform>().anchoredPosition = new Vector2(-4.6f, -33.5f);
            accountError.Hiding();
        }

        private void OnUserValueChanged(string user)
        {
            if (user != "")
            {
                emptyBindingAccountBtn.gameObject.SetActive(true);
                ClickableBtn(nextStepBtn);
            }
            else
            {
                emptyBindingAccountBtn.gameObject.SetActive(false);
                UnClickableBtn(nextStepBtn);
            }
        }

        private void OnUserEndEdit(string user)
        {
            if (user == "")
            {
                OnFrameError(accountFrame);
                bindingAccount.textComponent.color = ColorConfig.text_error;
                bindingAccount.placeholder.color = ColorConfig.placeholder_error;
                accountError.Display();
                accountError.errorInfo.text = HintConfig.input_account;
                hint.GetComponent<RectTransform>().anchoredPosition = new Vector2(-4.6f, -46.2f);
            }
            else
            {
                OnFrameNormal(accountFrame);
                bindingAccount.textComponent.color = ColorConfig.text_normal;
                bindingAccount.placeholder.color = ColorConfig.placeholder_normal;
            }
        }
        #endregion

    }
}