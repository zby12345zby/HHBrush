using System;
using UnityEngine;
using UnityEngine.UI;

namespace SC.XR.Unity.Module_PlatformAccount
{    
    public class API_Module_PlatformAccount : MonoBehaviour
    {
        /// <summary>
        /// 登录成功时的回调注册
        /// </summary>
        public static void RegistLoginCallback(Action callback)
        {
            LoginMgr.Instance.LoginCallBack += callback;
        }
        
        /// <summary>
        /// 登录成功时的回调注销
        /// </summary>
        public static void UnRegistLoginCallback(Action callback)
        {
            LoginMgr.Instance.LoginCallBack -= callback;
        }

        /// <summary>
        /// 当前用户状态(未登录，已登录)
        /// </summary>
        /// <param name="onCheckComplete"></param>
        public static void CheckUserState(Action<string> callback)
        {
            AccountLoginValid(LoginMgr.Instance.userData.unionId, LoginMgr.Instance.userData.token, callback);
        }


        //----------------------------------------------------Login----------------------------------------------------------

        /// <summary>
        /// 账号预登录
        /// </summary>
        public static void AccountPreLogin(string account, string password, string captchaId = null, string captchaValue = null, Action<string> callback = null)
        {
            LoginMgr.Instance.AccountPreLogin(account, password, captchaId, captchaValue, callback);
        }
              
        /// <summary>
        /// 账号登录
        /// </summary>
        public static void AccountLogin(string account, string password, string captchaId = null, string captchaValue = null, Action<string> callback = null)
        {
            LoginMgr.Instance.AccountLogin(account, password, captchaId, captchaValue, callback);
        }

        /// <summary>
        /// 图形验证码获取
        /// </summary>
        /// <param name="captchaTexture">验证码图片</param>
        public static void Captcha(Action<string> callback = null)
        {
            LoginMgr.Instance.Captcha(callback);
        }
        
        /// <summary>
        /// 手机预登录
        /// </summary>
        public static void PhonePreLogin(string phone, string diallingCode, string smsCode,Action<string> callback = null)
        {
            LoginMgr.Instance.PhonePreLogin(phone,diallingCode,smsCode, callback);
        }
        
        /// <summary>
        /// 手机登录
        /// </summary>
        public static void PhoneLogin(string phone, string diallingCode, string smsCode, Action<string> callback = null)
        {
            LoginMgr.Instance.PhoneLogin(phone, diallingCode, smsCode, callback);
        }

        /// <summary>
        /// 获取登录验证码
        /// </summary>
        public static void SMSLoginCode(string phone, string diallingCode, Action<string> callback = null)
        {
            LoginMgr.Instance.SMSLoginCode(phone, diallingCode, callback);
        }

        /// <summary>
        /// 账号合法状态查询(已登录)
        /// </summary>
        public static void AccountLoginValid(string unionId, string authorization, Action<string> callback = null)
        {
            LoginMgr.Instance.AccountLoginValid(unionId,authorization, callback);
        }

        //----------------------------------------------------Forget---------------------------------------------------------

        /// <summary>
        /// 账号信息查询(未登录)
        /// </summary>
        public static void AccountSafeState(string account, Action<string> callback = null)
        {
            LoginMgr.Instance.AccountSafeState(account, callback);
        }

        /// <summary>
        /// 获取忘记密码手机验证码
        /// </summary>
        public static void SMSCodeForget(string phone, string diallingCode, Action<string> callback = null)
        {
            LoginMgr.Instance.SMSCodeForget(phone,diallingCode, callback);
        }

        /// <summary>
        /// 验证忘记密码手机验证码
        /// </summary>
        public static void SMSVerifyForgetPassword(string phone, string diallingCode, string smsCode, Action<string> callback = null)
        {
            LoginMgr.Instance.SMSVerifyForgetPassword(phone, diallingCode, smsCode, callback);
        }

        /// <summary>
        /// 发送忘记密码邮箱验证码
        /// </summary>
        public static void EmailCodeForget(string email, Action<string> callback = null)
        {
            LoginMgr.Instance.EmailCodeForget(email, callback);
        }

        /// <summary>
        /// 验证忘记密码邮箱验证码
        /// </summary>
        public static void EmailVerifyForgetPassword(string email, string emailCode, Action<string> callback = null)
        {
            LoginMgr.Instance.EmailVerifyForgetPassword(email, emailCode, callback);
        }

        /// <summary>
        /// 忘记密码重设（手机验证码）
        /// </summary>
        public static void ForgetPasswordByPhone(string password, string confirmPassword, string codeToken, Action<string> callback = null)
        {
            LoginMgr.Instance.ForgetPasswordByPhone(password, confirmPassword, codeToken, callback);
        }

        /// <summary>
        /// 忘记密码重设（邮箱验证码）
        /// </summary>
        public static void ForgetPasswordByMail(string password, string confirmPassword, string codeToken, Action<string> callback = null)
        {
            LoginMgr.Instance.ForgetPasswordByMail(password, confirmPassword, codeToken, callback);
        }

        //----------------------------------------------------Times---------------------------------------------------------

        /// <summary>
        /// 手机验证码次数查询
        /// </summary>
        public static void SMSRestCodeTimes(string phone, string diallingCode, Action<string> callback = null)
        {
            LoginMgr.Instance.SMSRestCodeTimes(phone, diallingCode, callback);
        }

        /// <summary>
        /// 查询剩余邮件次数
        /// </summary>
        public static void EmailRestCodeTimes(string email, Action<string> callback = null)
        {
            LoginMgr.Instance.EmailRestCodeTimes( email, callback);
        }

    }
}
