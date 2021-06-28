using System.Text.RegularExpressions;

namespace SC.XR.Unity.Module_PlatformAccount
{
    /// <summary>
    /// 正则表达式配置
    /// </summary>
    public static class RegexConfig
    {
        public static Regex format_phone = new Regex(@"^(\+?0?86?\-?)?(\d{7,})$");
        public static Regex format_bindingPhone = new Regex(@"^\+86(\d{7,})$");
        public static Regex format_email = new Regex(@"^.+@.+\..+$");
        public static Regex format_password = new Regex(@"^(?=.*\d)(?=.*[A-Za-z]).{8,}$");
        public static Regex except_regionCode = new Regex(@"[^(\+?0?86?\-?)](\d{7,})$");        
    }
}
