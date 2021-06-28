using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public class BaseLoginForms : BaseCoroutineForms
    {
        public void SaveUserInfo()
        {
            LoginMgr.Instance.UserInfo(LoginMgr.Instance.userData.token, LoginMgr.Instance.userData.unionId, UserInfoCallBack);
        }

        public void UserInfoCallBack(string json)
        {
            JsonData data = JsonMapper.ToObject(json);
            int code = (int)data["code"];
            if (code == (int)RemoteCode.success)
            {
                LoginMgr.Instance.userData.uNumber = data["data"]["uNumber"].ToString();
                LoginMgr.Instance.userData.avatar = data["data"]["avatar"].ToString();
                LoginMgr.Instance.userData.nickName = data["data"]["nickName"].ToString();
                LoginMgr.Instance.userData.gender = (int)data["data"]["gender"];
                LoginMgr.Instance.userData.phone = data["data"]["phone"].ToString();
                LoginMgr.Instance.userData.email = data["data"]["email"].ToString();
                LoginMgr.Instance.userData.identityNum = data["data"]["identityNum"].ToString();
                LoginMgr.Instance.userData.address = data["data"]["address"].ToString();
                LoginMgr.Instance.userData.birthday = data["data"]["birthday"].ToString();
                LoginMgr.Instance.userData.country = data["data"]["country"].ToString();
                LoginMgr.Instance.userData.diallingCode = data["data"]["diallingCode"].ToString();
                LoginMgr.Instance.userData.language = data["data"]["language"].ToString();
                LitJsonMgr.Instance.WriteJson<UserData>("UserData", LoginMgr.Instance.userData);
            }
        }


    }
}
