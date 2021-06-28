using UnityEngine;

namespace SC.XR.Unity.Module_PlatformAccount
{
    /// <summary>
    /// 图形验证码数据
    /// </summary>
    public class CaptchaData
    {
        /// <summary>
        /// captchaImage上显示的数字，需要用户自行输入
        /// </summary>
        public string captcha;

        /// <summary>
        /// 获取验证码的Id
        /// </summary>
        public string captchaId;

        /// <summary>
        /// 获取验证码的图片
        /// </summary>
        public Texture2D captchaImage;
    }
}

