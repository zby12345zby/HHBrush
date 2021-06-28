using SC.XR.Unity;
using SC.XR.Unity.Module_Device;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SC.XR.Unity.Module_SDKSystem {

    public class Module_SDKSystem : SCModuleMono {

        public static Module_SDKSystem Instance { get; private set; }

        [SerializeField]
        private bool EnableInputSystem = true;

        bool isStart = false;

        public bool IsRunning {
            get; private set;
        }
        public bool Initialized {
            get; private set;
        }

        private Module_InputSystem.Module_InputSystem mInputSystem;
        public Module_InputSystem.Module_InputSystem InputSystem {
            get {
                if(EnableInputSystem && mInputSystem == null) {
                    mInputSystem = GetComponentInChildren<Module_InputSystem.Module_InputSystem>(true);
                }
                return mInputSystem;
            }
        }

        Coroutine waitSlamInit = null;
        Coroutine waitSlamRunning = null;

        private Coroutine updateWaitForEndOfFrame = null;

        #region MonoBehavior Driver

        void Awake() {
            DebugMy.Log("Awake", this, true);
            ModuleInit(false);
        }

        void OnEnable() {
            DebugMy.Log("OnEnable", this, true);
            if (updateWaitForEndOfFrame == null) {
                updateWaitForEndOfFrame = StartCoroutine(UpdateWaitForEndOfFrame());
            }
            if(isStart == true) {
                ModuleStart();
            }
        }

        void Start() {
            DebugMy.Log("Start", this, true);
            isStart = true;
            ModuleStart();
        }

        void Update() {
            ModuleUpdate();
        }

        void LateUpdate() {
            ModuleLateUpdate();
        }

        void OnApplicationPause(bool pause) {
            DebugMy.Log("OnApplicationPause:"+ pause,this,true);
            if(isStart) {
                if(pause) {
                    ModuleStop();
                } else {
                    ModuleStart();
                }
            }
        }

        IEnumerator UpdateWaitForEndOfFrame() {
            while(true) {
                yield return new WaitForEndOfFrame();
                if(InputSystem && InputSystem.IsModuleStarted) {
                    InputSystem.ModuleEndOfFrame();
                }
            }
        }


        void OnDisable() {
            DebugMy.Log("OnDisable", this, true);
            if (updateWaitForEndOfFrame != null) {
                StopCoroutine(updateWaitForEndOfFrame);
            }

            ModuleStop();
        }

        void OnDestroy() {
            DebugMy.Log("OnDestroy", this, true);
            ModuleDestroy();
            isStart = false;
        }

        #endregion


        #region Module Behavoir

        public override void OnSCAwake() {
            base.OnSCAwake();

            if(Instance != null) {
                DestroyImmediate(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Instance = this;
            Initialized = false;

            DebugMy.Log("OnSCAwake", this, true);
            DebugMy.Log("SDK Version:" + API_Module_SDKVersion.Version, this, true);


            if (API_Module_SDKConfiguration.HasKey("Module_InputSystem", "ShowDebugLog")) {
                DebugMy.isShowNormalLog = API_Module_SDKConfiguration.GetBool("Module_InputSystem", "ShowDebugLog", 0);
            }

            if (waitSlamInit == null) {
                waitSlamInit = StartCoroutine(WaitSlamInitAction());
            } else {
                DebugMy.LogError("waitSlamInit !=null",this);
            }

        }
        IEnumerator WaitSlamInitAction() {

            API_GSXR_Slam.SlamManager?.gameObject.SetActive(true);
            
            yield return new WaitUntil(() => API_GSXR_Slam.SlamManager!=null && API_GSXR_Slam.SlamManager.IsRunning);
            AddModule(InputSystem);
            Module_Device.Module_Device.getInstance.Current.ShowInfo();
            Initialized = true;
            DebugMy.Log("SDKSystem Module Initialized !", this, true);
            DebugMy.Log("BatteryLevel: " + API_Module_Device.Current.BatteryLevel + "% IsCharging: " + Module_BatteryStatus.getInstance.IsCharging, this, true);
        }


        public override void OnSCStart() {
            base.OnSCStart();

            if (waitSlamRunning == null) {
                waitSlamRunning = StartCoroutine(WaitSlamRunningAction());
            }
        }

        IEnumerator WaitSlamRunningAction() {
            
            yield return new WaitUntil(() => Initialized==true);

            API_GSXR_Slam.SlamManager?.gameObject.SetActive(true);

            yield return new WaitUntil(() => API_GSXR_Slam.SlamManager.IsRunning);
            InputSystem?.ModuleStart();

            if (InputSystem) {
                yield return new WaitUntil(() => InputSystem.IsRunning);
            }

            IsRunning = true;
            DebugMy.Log("SDKSystem Module IsRuning !", this, true);
        }

        public override void OnSCDisable() {
            base.OnSCDisable();

            if (waitSlamRunning != null) {
                StopCoroutine(waitSlamRunning);
                waitSlamRunning = null;
            }

            IsRunning = false;

            //不能操作 灭屏唤醒否则起不来
            //API_GSXR_Slam.SlamManager?.gameObject.SetActive(false);
        }

        public override void OnSCDestroy() {
            base.OnSCDestroy();
            if (Instance != this)
                return;

            Initialized = false;

            if (waitSlamInit != null) {
                StopCoroutine(waitSlamInit);
                waitSlamInit = null;
            }

            API_GSXR_Slam.SlamManager?.gameObject.SetActive(false);
        }

        #endregion

    }
}

