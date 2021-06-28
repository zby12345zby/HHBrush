using System.Text.RegularExpressions;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public enum AccountType
    {
        NotAccount,
        Phone,
        Email
    }

    public class UserUtil
    {
        
        public static string RegionCode(string account)
        {
            switch (PhoneOrEmail(account))
            {
                case AccountType.NotAccount:
                    break;
                case AccountType.Phone:
                    return RegexConfig.except_regionCode.Match(account).ToString();
                case AccountType.Email:
                    return account;
            }
            return "";
        }

        public static AccountType PhoneOrEmail(string account)
        {
            if (RegexConfig.format_phone.IsMatch(account))
            {
                return AccountType.Phone;
            }
            else if(RegexConfig.format_email.IsMatch(account))
            {
                return AccountType.Email;
            }
            return AccountType.NotAccount;            
        }

        public static bool AccountFormat(string account)
        {
            if (RegexConfig.format_bindingPhone.IsMatch(account)
                ||RegexConfig.format_email.IsMatch(account))
            {
                return true;
            }
            return false;
        }

        public static bool PasswordFormat(string password)
        {
            if (RegexConfig.format_password.IsMatch(password))
            {
                return true;
            }
            return false;
        }

    }
}
