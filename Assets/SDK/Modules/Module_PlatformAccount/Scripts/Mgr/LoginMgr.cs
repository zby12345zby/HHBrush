using System;


namespace SC.XR.Unity.Module_PlatformAccount
{
    public abstract class LoginMgr
    {

        private static LoginMgr instance;

        public static LoginMgr Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }
                else
                {
                    instance = new LoginManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// 登录配置信息，账号密码缓存
        /// </summary>
        public LoginConfig loginConfig = new LoginConfig();

        /// <summary>
        /// 服务器的配置信息
        /// </summary>
        public ServerConfig serverConfig = new ServerConfig();

        /// <summary>
        /// 通讯接口
        /// </summary>
        public RemoteConfig remoteConfig = new RemoteConfig();

        /// <summary>
        /// 账号信息
        /// </summary>
        public AccountInfoData accountInfoData = new AccountInfoData();

        /// <summary>
        /// 验证码数据
        /// </summary>
        public CaptchaData captchaData;
        /// <summary>
        /// 忘记密码数据
        /// </summary>
        public ForgetData forgetData = new ForgetData();
        /// <summary>
        /// 用户数据
        /// </summary>
        public UserData userData = new UserData();

        /// <summary>
        /// 登录成功时的回调注册
        /// </summary>
        public Action LoginCallBack;


        //----------------------------------------------------Login----------------------------------------------------------

        /// <summary>
        /// 账号预登录
        /// </summary>
        public abstract void AccountPreLogin(string account, string password, string captchaId = null, string captchaValue = null, Action<string> callback = null);

        /// <summary>
        /// 账号登录
        /// </summary>
        public abstract void AccountLogin(string account, string password, string captchaId = null, string captchaValue = null, Action<string> callback = null);

        /// <summary>
        /// 图形验证码获取
        /// </summary>
        public abstract void Captcha(Action<string> callback = null);

        /// <summary>
        /// 手机预登录
        /// </summary>
        public abstract void PhonePreLogin(string phone, string diallingCode, string smsCode, Action<string> callback = null);

        /// <summary>
        /// 手机登录
        /// </summary>
        public abstract void PhoneLogin(string phone, string diallingCode, string smsCode, Action<string> callback = null);

        /// <summary>
        /// 获取登录验证码
        /// </summary>
        public abstract void SMSLoginCode(string phone, string diallingCode, Action<string> callback = null);

        /// <summary>
        /// 账号合法状态查询(已登录)
        /// </summary>
        public abstract void AccountLoginValid(string unionId, string authorization, Action<string> callback = null);

        /// <summary>
        /// 查询用户信息
        /// </summary>
        public abstract void UserInfo(string authorization,string unionId,Action<string> callback = null);

        //----------------------------------------------------Forget---------------------------------------------------------
        /// <summary>
        /// 账号信息查询(未登录)
        /// </summary>
        public abstract void AccountSafeState(string account, Action<string> callback = null);

        /// <summary>
        /// 获取忘记密码验证码
        /// </summary>
        public abstract void SMSCodeForget(string phone, string diallingCode, Action<string> callback = null);

        /// <summary>
        /// 验证忘记密码验证码
        /// </summary>
        public abstract void SMSVerifyForgetPassword(string phone, string diallingCode, string smsCode, Action<string> callback = null);

        /// <summary>
        /// 发送忘记密码邮箱验证码
        /// </summary>
        public abstract void EmailCodeForget(string email, Action<string> callback = null);

        /// <summary>
        /// 验证忘记密码邮箱验证码
        /// </summary>
        public abstract void EmailVerifyForgetPassword(string email, string emailCode, Action<string> callback = null);

        /// <summary>
        /// 忘记密码重设（手机验证码）
        /// </summary>
        public abstract void ForgetPasswordByPhone(string password, string confirmPassword, string codeToken, Action<string> callback = null);

        /// <summary>
        /// 忘记密码重设（邮箱验证码）
        /// </summary>
        public abstract void ForgetPasswordByMail(string password, string confirmPassword, string codeToken, Action<string> callback = null);

        //----------------------------------------------------Times---------------------------------------------------------

        /// <summary>
        /// 手机验证码次数查询
        /// </summary>
        public abstract void SMSRestCodeTimes(string phone, string diallingCode, Action<string> callback = null);

        /// <summary>
        /// 查询剩余邮件次数
        /// </summary>
        public abstract void EmailRestCodeTimes(string email, Action<string> callback = null);

    }
}
