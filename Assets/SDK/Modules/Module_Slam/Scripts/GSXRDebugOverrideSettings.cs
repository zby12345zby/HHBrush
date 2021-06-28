using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSXRDebugOverrideSettings : MonoBehaviour {

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { GSXROverrideSettings.EyeAntiAliasing = GSXROverrideSettings.eAntiAliasing.k1; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { GSXROverrideSettings.EyeAntiAliasing = GSXROverrideSettings.eAntiAliasing.k2; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { GSXROverrideSettings.EyeAntiAliasing = GSXROverrideSettings.eAntiAliasing.k4; }

        if (Input.GetKeyDown(KeyCode.Q)) { GSXROverrideSettings.EyeResolutionScaleFactor = 0.25f; }
        if (Input.GetKeyDown(KeyCode.W)) { GSXROverrideSettings.EyeResolutionScaleFactor = 0.5f; }
        if (Input.GetKeyDown(KeyCode.E)) { GSXROverrideSettings.EyeResolutionScaleFactor = 0.75f; }
        if (Input.GetKeyDown(KeyCode.R)) { GSXROverrideSettings.EyeResolutionScaleFactor = 1.0f; }
        if (Input.GetKeyDown(KeyCode.T)) { GSXROverrideSettings.EyeResolutionScaleFactor = 1.25f; }
        if (Input.GetKeyDown(KeyCode.Y)) { GSXROverrideSettings.EyeResolutionScaleFactor = 1.5f; }

        if (Input.GetKeyDown(KeyCode.A)) { GSXROverrideSettings.MasterTextureLimit = GSXROverrideSettings.eMasterTextureLimit.k0; }
        if (Input.GetKeyDown(KeyCode.S)) { GSXROverrideSettings.MasterTextureLimit = GSXROverrideSettings.eMasterTextureLimit.k1; }
        if (Input.GetKeyDown(KeyCode.D)) { GSXROverrideSettings.MasterTextureLimit = GSXROverrideSettings.eMasterTextureLimit.k2; }
        if (Input.GetKeyDown(KeyCode.F)) { GSXROverrideSettings.MasterTextureLimit = GSXROverrideSettings.eMasterTextureLimit.k3; }
        if (Input.GetKeyDown(KeyCode.G)) { GSXROverrideSettings.MasterTextureLimit = GSXROverrideSettings.eMasterTextureLimit.k4; }

        if (Input.GetKeyDown(KeyCode.U)) { GSXROverrideSettings.CpuPerfLevel = GSXROverrideSettings.ePerfLevel.Minimum; }
        if (Input.GetKeyDown(KeyCode.I)) { GSXROverrideSettings.CpuPerfLevel = GSXROverrideSettings.ePerfLevel.Medium; }
        if (Input.GetKeyDown(KeyCode.O)) { GSXROverrideSettings.CpuPerfLevel = GSXROverrideSettings.ePerfLevel.Maximum; }
        if (Input.GetKeyDown(KeyCode.P)) { GSXROverrideSettings.CpuPerfLevel = GSXROverrideSettings.ePerfLevel.System; }

        if (Input.GetKeyDown(KeyCode.H)) { GSXROverrideSettings.GpuPerfLevel = GSXROverrideSettings.ePerfLevel.Minimum; }
        if (Input.GetKeyDown(KeyCode.J)) { GSXROverrideSettings.GpuPerfLevel = GSXROverrideSettings.ePerfLevel.Medium; }
        if (Input.GetKeyDown(KeyCode.K)) { GSXROverrideSettings.GpuPerfLevel = GSXROverrideSettings.ePerfLevel.Maximum; }
        if (Input.GetKeyDown(KeyCode.L)) { GSXROverrideSettings.GpuPerfLevel = GSXROverrideSettings.ePerfLevel.System; }

        if (Input.GetKeyDown(KeyCode.Z)) { GSXROverrideSettings.ChromaticAberrationCorrection = GSXROverrideSettings.eChromaticAberrationCorrection.kDisable; }
        if (Input.GetKeyDown(KeyCode.X)) { GSXROverrideSettings.ChromaticAberrationCorrection = GSXROverrideSettings.eChromaticAberrationCorrection.kEnable; }

        if (Input.GetKeyDown(KeyCode.C)) { GSXROverrideSettings.VSyncCount = GSXROverrideSettings.eVSyncCount.k1; }
        if (Input.GetKeyDown(KeyCode.V)) { GSXROverrideSettings.VSyncCount = GSXROverrideSettings.eVSyncCount.k2; }

        if (Input.GetKeyDown(KeyCode.M)) { GSXROverrideSettings.FoveateArea = Mathf.Clamp(GSXROverrideSettings.FoveateArea + 0.1f, 0f, 1f); Debug.LogFormat("FoveateArea: {0}", GSXROverrideSettings.FoveateArea); }
        if (Input.GetKeyDown(KeyCode.N)) { GSXROverrideSettings.FoveateArea = Mathf.Clamp(GSXROverrideSettings.FoveateArea - 0.1f, 0f, 1f); Debug.LogFormat("FoveateArea: {0}", GSXROverrideSettings.FoveateArea); }
        if (Input.GetKeyDown(KeyCode.Period)) { GSXROverrideSettings.FoveateGain = GSXROverrideSettings.FoveateGain + Vector2.one; Debug.LogFormat("FoveateGain: {0}", GSXROverrideSettings.FoveateGain); }
        if (Input.GetKeyDown(KeyCode.Comma)) { GSXROverrideSettings.FoveateGain = GSXROverrideSettings.FoveateGain - Vector2.one; Debug.LogFormat("FoveateGain: {0}", GSXROverrideSettings.FoveateGain); }
    }
}
