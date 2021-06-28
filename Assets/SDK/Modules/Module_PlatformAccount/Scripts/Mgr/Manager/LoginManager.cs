using LitJson;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public class LoginManager : LoginMgr
    {
                      
        //----------------------------------------------------Login----------------------------------------------------------
        
        public override void AccountPreLogin(string account, string password, string captchaId = null, string captchaValue = null,Action<string> callback = null)
        {
            if (WebRequestMgr.Instance.isRequesting) return;

            //url
            string url = LoginMgr.Instance.serverConfig.url + LoginMgr.Instance.remoteConfig.accountPreLogin;
            
            //header
            JsonData header = new JsonData();
            header["Content-Type"] = "application/json";

            //form
            JsonData form = new JsonData();
            form["account"] = account;
            form["password"] = password;
            string sn = API_Module_Device.Current.SN;
            string version = API_Module_Device.Current.RELEASE_VERSION;
            string model = API_Module_Device.Current.DeviceName;
            string source = API_Module_Device.Current.XRType.ToString();
            form["deviceId"] = sn;
            form["deviceType"] = model;
            form["deviceOS"] = version;
            form["source"] = source;
            if (captchaData != null)
            {
                form["captchaId"] = captchaId;
                form["captchaValue"] = captchaValue;
            }
            //Post
            WebRequestMgr.Instance.PostData(url,header,form,callback);            
        }
        public override void AccountLogin(string account,string password,string captchaId = null,string captchaValue = null, Action<string> callback = null)
        {
            if (WebRequestMgr.Instance.isRequesting) return;
            //url
            string url = LoginMgr.Instance.serverConfig.url + LoginMgr.Instance.remoteConfig.accountLogin;

            //header
            JsonData header = new JsonData();
            header["Content-Type"] = "application/json";

            //form
            JsonData form = new JsonData();
            form["account"] = account;
            form["password"] = password;
            string sn = API_Module_Device.Current.SN;
            Debug.Log("sn:"+sn);
            string version = API_Module_Device.Current.RELEASE_VERSION;
            string model = API_Module_Device.Current.DeviceName;
            string source = API_Module_Device.Current.XRType.ToString();
            form["deviceId"] = sn;
            form["deviceType"] = model;
            form["deviceOS"] = version;               
            form["source"] = source;
            if (captchaData!=null)
            {
                form["captchaId"] = captchaId;
                form["captchaValue"] = captchaValue;
            }
            //Post
            WebRequestMgr.Instance.PostData(
                url,
                header,
                form,
                (string json) =>
                {
                    callback?.Invoke(json);
                    LoginCallBack?.Invoke();
                });
            
        }
        public override void Captcha(Action<string> callback = null)
        {
            if (WebRequestMgr.Instance.isRequesting) return;

            //url
            string url = LoginMgr.Instance.serverConfig.url + LoginMgr.Instance.remoteConfig.captcha;

            //header
            JsonData header = new JsonData();

            //form
            JsonData form = null;

            //Post
            WebRequestMgr.Instance.GetData(
                url,
                header,
                form,
                (string json) =>
                {
                    callback?.Invoke(json);                    
                });
           
        }
        public override void PhonePreLogin(string phone, string diallingCode, string smsCode, Action<string> callback = null)
        {
            if (WebRequestMgr.Instance.isRequesting) return;
            //url
            string url = LoginMgr.Instance.serverConfig.url + LoginMgr.Instance.remoteConfig.phonePreLogin;

            //header
            JsonData header = new JsonData();
            header["Content-Type"] = "application/json";

            //form
            JsonData form = new JsonData();
            form["phone"] = phone;
            form["diallingCode"] = diallingCode;
            form["smsCode"] = smsCode;
            string sn = API_Module_Device.Current.SN;
            string version = API_Module_Device.Current.RELEASE_VERSION;
            string model = API_Module_Device.Current.DeviceName;
            string source = API_Module_Device.Current.XRType.ToString();
            form["deviceId"] = sn;
            form["deviceType"] = model;
            form["deviceOS"] = version;
            form["source"] = source;
            //Post
            WebRequestMgr.Instance.PostData(
                url,
                header,
                form,
                callback);
            
        }
        public override void PhoneLogin(string phone, string diallingCode, string smsCode, Action<string> callback = null)
        {
            if (WebRequestMgr.Instance.isRequesting) return;
            //url
            string url = LoginMgr.Instance.serverConfig.url + LoginMgr.Instance.remoteConfig.phoneLogin;
            
            //header
            JsonData header = new JsonData();
            header["Content-Type"] = "application/json";

            //form
            JsonData form = new JsonData();
            form["phone"] = phone;
            form["diallingCode"] = diallingCode;
            form["smsCode"] = smsCode;
            string sn = API_Module_Device.Current.SN;
            string version = API_Module_Device.Current.RELEASE_VERSION;
            string model = API_Module_Device.Current.DeviceName;
            string source = API_Module_Device.Current.XRType.ToString();
            form["deviceId"] = sn;
            form["deviceType"] = model;
            form["deviceOS"] = version;
            form["source"] = source;

            //Post
            WebRequestMgr.Instance.PostData(
                url,
                header,
                form,
                (string json) =>
                {
                    callback?.Invoke(json);
                    LoginCallBack?.Invoke();
                });                
        }
        public override void SMSLoginCode(string phone, string diallingCode, Action<string> callback = null)
        {
            if (WebRequestMgr.Instance.isRequesting) return;

            //query
            string query = "?" + "phone=" + phone + "&diallingCode=" + diallingCode;

            //url
            string url = LoginMgr.Instance.serverConfig.url + LoginMgr.Instance.remoteConfig.smsCodeLogin + query;

            //header
            JsonData header = new JsonData();

            //form
            JsonData form = null;

            //Get
            WebRequestMgr.Instance.GetData(
                url,
                header,
                form,
                (string json) =>
                {
                    callback?.Invoke(json);
                });
            
        }
        public override void AccountLoginValid(string unionId, string authorization, Action<string> callback = null)
        {
            if (WebRequestMgr.Instance.isRequesting) return;
            //url
            string url = LoginMgr.Instance.serverConfig.url + LoginMgr.Instance.remoteConfig.accountLoginValid;

            //header
            JsonData header = new JsonData();
            header["UnionId"] = unionId;
            header["Authorization"] = authorization;

            //form
            JsonData form = null;

            //Get
            WebRequestMgr.Instance.GetData(
                url,
                header,
                form,
                (string json) =>
                {
                    callback?.Invoke(json);
                });            
        }
        public override void UserInfo(string authorization, string unionId, Action<string> callback = null)
        {
            if (WebRequestMgr.Instance.isRequesting) return;
            //url
            string url = LoginMgr.Instance.serverConfig.url + LoginMgr.Instance.remoteConfig.userInfo;

            //header
            JsonData header = new JsonData();
            header["Authorization"] = authorization;
            header["UnionId"] = unionId;

            //form
            JsonData form = null;

            //Get
            WebRequestMgr.Instance.GetData(
                url,
                header,
                form,
                (string json) =>
                {
                    callback?.Invoke(json);
                });
        }


        //----------------------------------------------------Forget---------------------------------------------------------

        public override void AccountSafeState(string account, Action<string> callback = null)
        {
            if (WebRequestMgr.Instance.isRequesting) return;

            //query
            string query = "?" + "account=" + account;

            //url
            string url = LoginMgr.Instance.serverConfig.url + LoginMgr.Instance.remoteConfig.accountSafeState + query;

            //header
            JsonData header = new JsonData();

            //form
            JsonData form = null;

            //Get
            WebRequestMgr.Instance.GetData(
                url,
                header,
                form,
                callback);
            
        }        
        public override void SMSCodeForget(string phone,string diallingCode, Action<string> callback = null)
        {
            if (WebRequestMgr.Instance.isRequesting) return;

            //query
            string query = "?" + "phone=" + phone + "&" + "diallingCode=" + diallingCode;

            //url
            string url = LoginMgr.Instance.serverConfig.url + LoginMgr.Instance.remoteConfig.smsCodeForget + query;

            //header
            JsonData header = new JsonData();

            //form
            JsonData form = null;

            //Get
            WebRequestMgr.Instance.GetData(
                url,
                header,
                form,
                callback);
            
        }
        public override void SMSVerifyForgetPassword(string phone, string diallingCode,string smsCode, Action<string> callback = null)
        {
            if (WebRequestMgr.Instance.isRequesting) return;
            //url
            string url = LoginMgr.Instance.serverConfig.url + LoginMgr.Instance.remoteConfig.smsVerifyForgetPassword;

            //header
            JsonData header = new JsonData();
            header["Content-Type"] = "application/json";

            //form
            JsonData form = new JsonData();
            form["phone"] = phone;
            form["diallingCode"] = diallingCode;
            form["smsCode"] = smsCode;

            //Post
            WebRequestMgr.Instance.PostData(
                url,
                header,
                form,
                (string json) =>
                {
                    SetForgetData(json);
                    callback?.Invoke(json);
                });
            
        }
        public override void EmailCodeForget(string email, Action<string> callback = null)
        {
            if (WebRequestMgr.Instance.isRequesting) return;

            //query
            string query = "?" + "email=" + email;

            //url
            string url = LoginMgr.Instance.serverConfig.url + LoginMgr.Instance.remoteConfig.emailCodeForget + query;

            //header
            JsonData header = new JsonData();
            header["Content-Type"] = "application/json";

            //form
            JsonData form = null;
            //form["email"] = email;

            //Get
            WebRequestMgr.Instance.GetData(
                url,
                header,
                form,
                callback);
            
        }        
        public override void EmailVerifyForgetPassword(string email,string emailCode, Action<string> callback = null)
        {
            if (WebRequestMgr.Instance.isRequesting) return;

            //url
            string url = LoginMgr.Instance.serverConfig.url + LoginMgr.Instance.remoteConfig.emailVerifyForgetPassword;

            //header
            JsonData header = new JsonData();
            header["Content-Type"] = "application/json";

            //form
            JsonData form = new JsonData();
            form["email"] = email;
            form["emailCode"] = emailCode;

            //Post
            WebRequestMgr.Instance.PostData(
                url,
                header,
                form,
                (string json) =>
                {
                    SetForgetData(json);
                    callback?.Invoke(json);
                });
            
        }       
        public override void ForgetPasswordByPhone(string password, string confirmPassword,string codeToken, Action<string> callback = null)
        {
            if (WebRequestMgr.Instance.isRequesting) return;
            //url
            string url = LoginMgr.Instance.serverConfig.url + LoginMgr.Instance.remoteConfig.forgetPasswordByPhone;

            //header
            JsonData header = new JsonData();
            header["Content-Type"] = "application/json";

            //form
            JsonData form = new JsonData();
            form["password"] = password;
            form["confirmPassword"] = confirmPassword;
            form["codeToken"] = codeToken;

            //Post
            WebRequestMgr.Instance.PostData(
                url,
                header,
                form,
                callback);
            
        }        
        public override void ForgetPasswordByMail(string password, string confirmPassword, string codeToken, Action<string> callback = null)
        {
            if (WebRequestMgr.Instance.isRequesting) return;
            //url
            string url = LoginMgr.Instance.serverConfig.url + LoginMgr.Instance.remoteConfig.forgetPasswordByMail;

            //header
            JsonData header = new JsonData();
            header["Content-Type"] = "application/json";

            //form
            JsonData form = new JsonData();
            form["password"] = password;
            form["confirmPassword"] = confirmPassword;
            form["codeToken"] = codeToken;

            //Post
            WebRequestMgr.Instance.PostData(
                url,
                header,
                form,
                callback);
            
        }
        //----------------------------------------------------Times---------------------------------------------------------

        public override void SMSRestCodeTimes(string phone, string diallingCode, Action<string> callback = null)
        {
            if (WebRequestMgr.Instance.isRequesting) return;

            //query
            string query = "?" + "phone=" + phone + "&diallingCode=" + diallingCode;

            //url
            string url = LoginMgr.Instance.serverConfig.url + LoginMgr.Instance.remoteConfig.smsRestCodeTimes + query;

            //header
            JsonData header = new JsonData();

            //form
            JsonData form = null;

            //Get
            WebRequestMgr.Instance.GetData(
                url,
                header,
                form,
                callback);
            
        }
        public override void EmailRestCodeTimes(string email, Action<string> callback = null)
        {
            if (WebRequestMgr.Instance.isRequesting) return;

            //query
            string query = "?" + "email=" + email;

            //url
            string url = LoginMgr.Instance.serverConfig.url + LoginMgr.Instance.remoteConfig.emailRestCodeTimes + query;

            //header
            JsonData header = new JsonData();

            //form
            JsonData form = null;

            //Get
            WebRequestMgr.Instance.GetData(
                url,
                header,
                form,
                callback);
            
        }

        /// <summary>
        /// 获取修改密码codeToken
        /// </summary>
        private void SetForgetData(string json)
        {
            JsonData data = JsonMapper.ToObject(json);
            if ((int)data["code"] == (int)RemoteCode.success)
            {
                string codeToken = data["data"]["codeToken"].ToString();
                forgetData = new ForgetData();
                forgetData.codeToken = codeToken;
            }
        }
    }
}

