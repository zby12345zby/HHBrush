using UnityEngine;


namespace SC.XR.Unity.Module_PlatformAccount
{
    public abstract class LitJsonMgr
    {

        private static LitJsonMgr instance;

        public static LitJsonMgr Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }
                else
                {
                    instance = new LitJsonManager();
                }
                return instance;
            }
        }


        /// <summary>
        /// Json保存目录
        /// </summary>
        public string DirectoryPath { get { return Application.persistentDataPath + "/PlatformAccount/"; } }

        /// <summary>
        /// 初始化Json
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// 读取Json
        /// </summary>
        public abstract T ReadJson<T>(string name) where T : class;

        /// <summary>
        /// 写入Json
        /// </summary>
        public virtual void WriteJson<T>(string name, T t) where T : class
        {
            Save();
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public virtual void Save() { }


    }
}
