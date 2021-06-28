using System.IO;
using UnityEngine;
using LitJson;
using System.Text;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public class LitJsonManager : LitJsonMgr
    {
        
        public static string LoginConfig = "";
        public static string ServerConfig = "";
        public static string RemoteConfig = "";
        public static string UserData = "";
        public override void Init()
        {
            LoginMgr.Instance.loginConfig = new LoginConfig();
            LoginMgr.Instance.serverConfig = new ServerConfig();
            LoginMgr.Instance.remoteConfig = new RemoteConfig();
            LoginMgr.Instance.userData = new UserData();
            WriteJson<LoginConfig>("LoginConfig", LoginMgr.Instance.loginConfig);
            WriteJson<ServerConfig>("ServerConfig", LoginMgr.Instance.serverConfig);
            WriteJson<RemoteConfig>("RemoteConfig", LoginMgr.Instance.remoteConfig);
            WriteJson<UserData>("UserData", LoginMgr.Instance.userData);
            PlayerPrefs.SetString(LoginConfig, JsonMapper.ToJson(LoginMgr.Instance.loginConfig));
            PlayerPrefs.SetString(ServerConfig, JsonMapper.ToJson(LoginMgr.Instance.serverConfig));
            PlayerPrefs.SetString(RemoteConfig, JsonMapper.ToJson(LoginMgr.Instance.remoteConfig));
            PlayerPrefs.SetString(UserData, JsonMapper.ToJson(LoginMgr.Instance.userData));
        }

        public override T ReadJson<T>(string name)
        {
            T t;

            if (!File.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            string filePath = DirectoryPath + name + ".txt";
            
            if ((PlayerPrefs.GetString(LoginConfig) == "")
                || (PlayerPrefs.GetString(ServerConfig) == "")
                || (PlayerPrefs.GetString(RemoteConfig) == "")
                || (PlayerPrefs.GetString(UserData) == ""))
            {
                Init();                
            }

            if (!File.Exists(filePath))
            {
                Init();
            }

            StreamReader sr = new StreamReader(filePath);
            JsonReader jr = new JsonReader(sr);
            t = JsonMapper.ToObject<T>(jr);
            sr.Close();

            return t;
        }

        public override void WriteJson<T>(string name, T t)
        {
            base.WriteJson<T>(name,t);

            string json = "";
            json = JsonMapper.ToJson(t);
            string filePath = DirectoryPath + name + ".txt";
            if (!File.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }
            if (!File.Exists(filePath))
            {
                FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate);

                StreamWriter sw = new StreamWriter(fileStream, Encoding.UTF8);

                sw.Write(json);

                sw.Close();

                fileStream.Close();
            }
            else
            {
                StreamWriter sw = new StreamWriter(filePath);

                sw.Write(json);

                sw.Close();
            }


        }

        public override void Save()
        {
            base.Save();

            PlayerPrefs.SetString(LoginConfig, JsonMapper.ToJson(LoginMgr.Instance.loginConfig));
            PlayerPrefs.SetString(ServerConfig, JsonMapper.ToJson(LoginMgr.Instance.serverConfig));
            PlayerPrefs.SetString(RemoteConfig, JsonMapper.ToJson(LoginMgr.Instance.remoteConfig));
            PlayerPrefs.SetString(UserData, JsonMapper.ToJson(LoginMgr.Instance.userData));
        }
    }
}
