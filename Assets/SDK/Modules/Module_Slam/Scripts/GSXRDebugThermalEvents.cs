using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GSXRDebugThermalEvents : MonoBehaviour, GSXRManager.SlamEventListener
{
    enum slamThermalLevel
    {
        kSafe,
        kLevel1,
        kLevel2,
        kLevel3,
        kCritical,
        kNumThermalLevels
    };

    public class BaseFsa
    {
        public List<string>[] actions = {
            null, //new List<string>(), // State0
            null, //new List<string>(), // State1
            null, //new List<string>(), // State2
            null, //new List<string>(), // State3
            null, //new List<string>(), // State4
        };

        private int _state = 0;
        public int state
        {
            get { return _state; }
            set { OnExit(_state); OnEnter(value); _state = value; }
        }

        public void OnEnter(int state) { Apply(actions[state]); }
        public void OnExit(int state) { }

        public void Apply(List<string> actions)
        {
            if (actions == null || actions.Count == 0) return;

            Debug.LogFormat("Actions: {0} {1} {2}", (actions.Count > 0 ? actions[0] : null), (actions.Count > 1 ? actions[1] : null), (actions.Count > 2 ? actions[2] : null));
            for(int i=0; i<actions.Count; i++)
            {
                string[] tokens = actions[i].Split(':');

                switch (tokens[0])
                {
                    case "FullFPS":
                    case "Vsync0":
                        GSXROverrideSettings.VSyncCount = GSXROverrideSettings.eVSyncCount.k1;
                        break;
                    case "HalfFPS":
                    case "Vsync1":
                        GSXROverrideSettings.VSyncCount = GSXROverrideSettings.eVSyncCount.k2;
                        break;

                    case "MSAA4":
                        GSXROverrideSettings.EyeAntiAliasing = GSXROverrideSettings.eAntiAliasing.k4;
                        break;
                    case "MSAA2":
                        GSXROverrideSettings.EyeAntiAliasing = GSXROverrideSettings.eAntiAliasing.k2;
                        break;
                    case "MSAA1":
                        GSXROverrideSettings.EyeAntiAliasing = GSXROverrideSettings.eAntiAliasing.k1;
                        break;

                    case "Render":
                        GSXROverrideSettings.EyeResolutionScaleFactor = tokens.Length > 1 ? float.Parse(tokens[1]) / 100f : 1.0f;
                        break;
                    case "Render100":
                        GSXROverrideSettings.EyeResolutionScaleFactor = 1.0f;
                        break;
                    case "Render90":
                        GSXROverrideSettings.EyeResolutionScaleFactor = 0.90f;
                        break;
                    case "Render80":
                        GSXROverrideSettings.EyeResolutionScaleFactor = 0.80f;
                        break;
                    case "Render70":
                        GSXROverrideSettings.EyeResolutionScaleFactor = 0.70f;
                        break;
                    case "Render60":
                        GSXROverrideSettings.EyeResolutionScaleFactor = 0.60f;
                        break;
                    case "Render50":
                        GSXROverrideSettings.EyeResolutionScaleFactor = 0.50f;
                        break;
                    case "Render40":
                        GSXROverrideSettings.EyeResolutionScaleFactor = 0.40f;
                        break;
                    case "Render30":
                        GSXROverrideSettings.EyeResolutionScaleFactor = 0.30f;
                        break;
                    case "Render20":
                        GSXROverrideSettings.EyeResolutionScaleFactor = 0.20f;
                        break;
                    case "Render10":
                        GSXROverrideSettings.EyeResolutionScaleFactor = 0.10f;
                        break;

                    case "TextureFull":
                        GSXROverrideSettings.MasterTextureLimit = GSXROverrideSettings.eMasterTextureLimit.k0;
                        break;
                    case "TextureHalf":
                        GSXROverrideSettings.MasterTextureLimit = GSXROverrideSettings.eMasterTextureLimit.k1;
                        break;
                    case "TextureForth":
                        GSXROverrideSettings.MasterTextureLimit = GSXROverrideSettings.eMasterTextureLimit.k2;
                        break;
                    case "TextureEighth":
                        GSXROverrideSettings.MasterTextureLimit = GSXROverrideSettings.eMasterTextureLimit.k3;
                        break;

                    case "Fovea0":
                        GSXROverrideSettings.FoveateArea = 0;
                        GSXROverrideSettings.FoveateGain = Vector2.one;
                        break;
                    case "Fovea1":
                        GSXROverrideSettings.FoveateArea = 2;
                        GSXROverrideSettings.FoveateGain = 2*Vector2.one;
                        break;
                    case "Fovea2":
                        GSXROverrideSettings.FoveateArea = 2;
                        GSXROverrideSettings.FoveateGain = 4 * Vector2.one;
                        break;
                    case "Fovea3":
                        GSXROverrideSettings.FoveateArea = 1;
                        GSXROverrideSettings.FoveateGain = 4 * Vector2.one;
                        break;
                    case "Fovea4":
                        GSXROverrideSettings.FoveateArea = 0;
                        GSXROverrideSettings.FoveateGain = 4 * Vector2.one;
                        break;
                    case "Fovea5":
                        GSXROverrideSettings.FoveateArea = 0;
                        GSXROverrideSettings.FoveateGain = 6 * Vector2.one;
                        break;

                    case "CpuPerfMax":
                        GSXROverrideSettings.CpuPerfLevel = GSXROverrideSettings.ePerfLevel.Maximum;
                        break;
                    case "CpuPerfMed":
                        GSXROverrideSettings.CpuPerfLevel = GSXROverrideSettings.ePerfLevel.Medium;
                        break;
                    case "CpuPerfMin":
                        GSXROverrideSettings.CpuPerfLevel = GSXROverrideSettings.ePerfLevel.Minimum;
                        break;

                    case "GpuPerfMax":
                        GSXROverrideSettings.GpuPerfLevel = GSXROverrideSettings.ePerfLevel.Maximum;
                        break;
                    case "GpuPerfMed":
                        GSXROverrideSettings.GpuPerfLevel = GSXROverrideSettings.ePerfLevel.Medium;
                        break;
                    case "GpuPerfMin":
                        GSXROverrideSettings.GpuPerfLevel = GSXROverrideSettings.ePerfLevel.Minimum;
                        break;

                    case "ChromaticOn":
                        GSXROverrideSettings.ChromaticAberrationCorrection = GSXROverrideSettings.eChromaticAberrationCorrection.kEnable;
                        break;
                    case "ChromaticOff":
                        GSXROverrideSettings.ChromaticAberrationCorrection = GSXROverrideSettings.eChromaticAberrationCorrection.kDisable;
                        break;

                    default:
                        Debug.LogFormat("SlamThermalEvent: Action {0} not supported", actions[i]);
                        break;
                }
            }
        }
    }

    public class CpuFsa : BaseFsa { }
    public class GpuFsa : BaseFsa { }
    public class SkinFsa : BaseFsa { }

    private CpuFsa cpu = new CpuFsa();
    private GpuFsa gpu = new GpuFsa();
    private SkinFsa skin = new SkinFsa();

    void Start()
    {
        enabled = GSXRConfigOptions.Instance.ThermalEnabled;
        if (!GSXRConfigOptions.Instance.ThermalEnabled) return;

        //Register for SlamEvents
        GSXRManager.Instance.AddEventListener(this);

        // Configure state machines
        cpu.actions[0] = GSXRConfigOptions.Instance.CpuState0;
        cpu.actions[1] = GSXRConfigOptions.Instance.CpuState1;
        cpu.actions[2] = GSXRConfigOptions.Instance.CpuState2;
        cpu.actions[3] = GSXRConfigOptions.Instance.CpuState3;
        cpu.actions[4] = GSXRConfigOptions.Instance.CpuState4;

        gpu.actions[0] = GSXRConfigOptions.Instance.GpuState0;
        gpu.actions[1] = GSXRConfigOptions.Instance.GpuState1;
        gpu.actions[2] = GSXRConfigOptions.Instance.GpuState2;
        gpu.actions[3] = GSXRConfigOptions.Instance.GpuState3;
        gpu.actions[4] = GSXRConfigOptions.Instance.GpuState4;

        skin.actions[0] = GSXRConfigOptions.Instance.SkinState0;
        skin.actions[1] = GSXRConfigOptions.Instance.SkinState1;
        skin.actions[2] = GSXRConfigOptions.Instance.SkinState2;
        skin.actions[3] = GSXRConfigOptions.Instance.SkinState3;
        skin.actions[4] = GSXRConfigOptions.Instance.SkinState4;
    }

    public void OnSlamEvent(GSXRManager.SlamEvent ev)
    {
        switch (ev.eventType)
        {
            case GSXRManager.slamEventType.kEventThermal:
                //Debug.LogFormat("OnSlamEvent: data {0} {1}", ev.eventData.thermal.zone, ev.eventData.thermal.level);
                UpdateThermalEvent((int)ev.eventData.thermal.zone, (int)ev.eventData.thermal.level);
                break;
            case GSXRManager.slamEventType.kEventVrModeStarted:
                SetThermalEvent(GSXRConfigOptions.Instance.ThermalStatesDefault);
                break;
        }
    }

    public void Update()
    {
        if (GSXRConfigOptions.Instance.ThermalTouchTest && Input.GetMouseButtonDown(0))
        {
            UpdateThermalEvent((int)Random.Range(0, 3), (int)Random.Range(0, 5));
        }
    }

    private void UpdateThermalEvent(int zone, int level)
    {
        Debug.LogFormat("SlamThermalEvent: zone {0} level {1}", zone, level);
        switch(zone)
        {
            case 0: // cpu
                cpu.state = level;
                break;
            case 1: // gpu
                gpu.state = level;
                break;
            case 2: // skin
                skin.state = level;
                break;
        }
    }

    private void SetThermalEvent(List<string> states)
    {
        for (int i = 0; i < states.Count; i++)
        {
            switch(states[i])
            {
                case "CpuState0":
                    UpdateThermalEvent(0, 0);
                    break;
                case "CpuState1":
                    UpdateThermalEvent(0, 1);
                    break;
                case "CpuState2":
                    UpdateThermalEvent(0, 2);
                    break;
                case "CpuState3":
                    UpdateThermalEvent(0, 3);
                    break;
                case "CpuState4":
                    UpdateThermalEvent(0, 4);
                    break;

                case "GpuState0":
                    UpdateThermalEvent(1, 0);
                    break;
                case "GpuState1":
                    UpdateThermalEvent(1, 1);
                    break;
                case "GpuState2":
                    UpdateThermalEvent(1, 2);
                    break;
                case "GpuState3":
                    UpdateThermalEvent(1, 3);
                    break;
                case "GpuState4":
                    UpdateThermalEvent(1, 4);
                    break;

                case "SkinState0":
                    UpdateThermalEvent(2, 0);
                    break;
                case "SkinState1":
                    UpdateThermalEvent(2, 1);
                    break;
                case "SkinState2":
                    UpdateThermalEvent(2, 2);
                    break;
                case "SkinState3":
                    UpdateThermalEvent(2, 3);
                    break;
                case "SkinState4":
                    UpdateThermalEvent(2, 4);
                    break;
            }
        }
    }
}
