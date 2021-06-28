namespace SC.XR.Unity.Module_PlatformAccount
{
    /// <summary>
    /// 服务器接口返回代号
    /// </summary>
    public enum RemoteCode
    {
        /// <summary>
        /// 请求成功
        /// </summary>
        success = 200,
        /// <summary>
        /// 账号或密码错误
        /// </summary>
        accountError = 5010,
        /// <summary>
        /// 账号未注册
        /// </summary>
        accountNotRegistered = 5013,

        accountHasNotLogin = 5031,
        /// <summary>
        /// 需要输入图形验证码
        /// </summary>
        needCaptcha = 5979,
        /// <summary>
        /// 图形验证码不正确
        /// </summary>
        captchaError = 5981,
        /// <summary>
        /// 手机验证码长度必须是6个字符
        /// </summary>
        smsLengthError = 5003,
        /// <summary>
        /// 验证码错误
        /// </summary>
        smsCodeError = 5006,
        /// <summary>
        /// 账号不存在
        /// </summary>
        accountNotExist = 5021,
        /// <summary>
        /// 短信验证码次数用完
        /// </summary>
        noSmsCodeNum = 5983
    }
    /// <summary>
    /// 接口配置
    /// </summary>
    public class RemoteConfig
    {

        /// <summary>
        /// 账号登录
        /// </summary>
        public string accountLogin = "/v1/accountLogin";
        /// <summary>
        /// 图形验证码获取
        /// </summary>
        public string captcha = "/v1/captcha";
        /// <summary>
        /// 手机登录
        /// </summary>
        public string phoneLogin = "/v1/phoneLogin";
        /// <summary>
        /// 获取手机登录验证码
        /// </summary>
        public string smsCodeLogin = "/v1/sms/code/login";
        /// <summary>
        /// 账号安全状态查询
        /// </summary>
        public string accountSafeState = "/v1/accountSafeState";
        /// <summary>
        /// 账号登录状态查询(未登录)
        /// </summary>
        public string accountLoginStatus = "/v1/accountLoginStatus";
        /// <summary>
        /// 手机验证码次数查询
        /// </summary>
        public string smsRestCodeTimes = "/v1/sms/restCodeTimes";
        /// <summary>
        /// 查询剩余邮件次数
        /// </summary>
        public string emailRestCodeTimes = "/v1/email/restCodeTimes";
        /// <summary>
        /// 获取忘记密码验证码
        /// </summary>
        public string smsCodeForget = "/v1/sms/code/forget";
        /// <summary>
        /// 获取忘记密码验证码
        /// </summary>
        public string smsVerifyForgetPassword = "/v1/sms/verify/forgetPassword";
        /// <summary>
        /// 发送忘记密码邮箱验证码
        /// </summary>
        public string emailCodeForget = "/v1/email/code/forget";
        /// <summary>
        /// 验证忘记密码邮箱验证码
        /// </summary>
        public string emailVerifyForgetPassword = "/v1/email/verify/forgetPassword";
        /// <summary>
        /// 忘记密码重设(手机验证码)
        /// </summary>
        public string forgetPasswordByPhone = "/v1/forgetPasswordByPhone";
        /// <summary>
        /// 忘记密码重设(邮箱验证码)
        /// </summary>
        public string forgetPasswordByMail = "/v1/forgetPasswordByMail";
        /// <summary>
        /// 账号合法状态查询(已登录)
        /// </summary>
        public string accountLoginValid = "/v1/accountLoginValid";
        /// <summary>
        /// 查询用户信息
        /// </summary>
        public string userInfo = "/v1/user/info";
        /// <summary>
        /// 账号预登陆
        /// </summary>
        public string accountPreLogin = "/v1/accountPreLogin";
        /// <summary>
        /// 手机预登陆
        /// </summary>
        public string phonePreLogin = "/v1/phonePreLogin";

    }
}
    
