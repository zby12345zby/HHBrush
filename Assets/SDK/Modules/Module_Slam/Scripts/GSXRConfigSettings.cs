using UnityEngine;
using System.Collections;

public class GSXRConfigSettings : MonoBehaviour
{
    public Transform reticle;

    void Awake()
    {
    }

    IEnumerator Start()
    {
        if (GSXRManager.Instance == null || GSXRConfigOptions.Instance == null)
        {
            yield break;
        }

        if (GSXRConfigOptions.Instance.TrackEyesEnabled.HasValue)
        {
            GSXRManager.Instance.settings.trackEyes = GSXRConfigOptions.Instance.TrackEyesEnabled.Value;
        }

        if (GSXRConfigOptions.Instance.TrackPositionEnabled.HasValue)
        {
            GSXRManager.Instance.settings.trackPosition = GSXRConfigOptions.Instance.TrackPositionEnabled.Value;
        }

        if (GSXRManager.Instance.gaze != null && GSXRConfigOptions.Instance.GazeReticleEnabled.HasValue)
        {
            GSXRManager.Instance.gaze.gameObject.SetActive(GSXRConfigOptions.Instance.GazeReticleEnabled.Value);
        }

        if (reticle == null && GSXRManager.Instance.reticleOverlay != null)
        {
            reticle = GSXRManager.Instance.reticleOverlay.transform;
        }

        if (reticle != null)
        {
            if (GSXRConfigOptions.Instance.GazeReticleEnabled.HasValue)
                reticle.gameObject.SetActive(GSXRConfigOptions.Instance.GazeReticleEnabled.Value);
            else if (GSXRConfigOptions.Instance.FocusEnabled)
                reticle.gameObject.SetActive(GSXRConfigOptions.Instance.FocusEnabled);
        }

        yield return new WaitUntil(() => GSXRManager.Instance.Initialized);

        if (GSXRConfigOptions.Instance.UseFixedViewport)
        {
            DisableSlamInput();
            SetSlamCameraView(GSXRConfigOptions.Instance.FixedViewportPosition, GSXRConfigOptions.Instance.FixedViewportEulerAnglesRotation);
        }

        if (GSXRConfigOptions.Instance.OverrideRenderTextureMSAA != 0)
        {
            SetSlamRenderTextureAntialiasing(GSXRConfigOptions.Instance.OverrideRenderTextureMSAA);
        }

        if (GSXRConfigOptions.Instance.FreezeAnimations)
        {
            FreezeAllAnimationsAtTime(Mathf.Max(0, GSXRConfigOptions.Instance.FreezeAnimationsAtTimeInSecs));
        }

        if (GSXRConfigOptions.Instance.DisableAudio)
        {
            DisableAudio();
        }

        if (GSXRConfigOptions.Instance.FoveationEnabled)
        {
            SetFoveatedRendering(GSXRConfigOptions.Instance.FoveationGain, GSXRConfigOptions.Instance.FoveationArea, GSXRConfigOptions.Instance.FoveationMinimum);
        }
    }

    void Update()
    {
        if (!GSXRManager.Instance)
        {
            return;
        }

        if (GSXRConfigOptions.Instance.FocusEnabled)
        {
            UpdateFocus();
        }
    }

    private void FreezeAllAnimationsAtTime(float timeInSec)
    {
        Animator[] animators = GameObject.FindObjectsOfType<Animator>();
        foreach (Animator animator in animators)
        {
            animator.updateMode = AnimatorUpdateMode.Normal;
            animator.Update(timeInSec);
        }

        Time.timeScale = 0;
    }

    private void DisableSlamInput()
    {
        if (!GSXRManager.Instance)
        {
            return;
        }

        GSXRManager.Instance.DisableInput = true;
    }

    private void SetSlamCameraView(Vector3 position, Vector3 eulerAnglesRotation)
    {
        if(!GSXRManager.Instance)
        {
            return;
        }

        GSXRManager.Instance.transform.position = position;
        GSXRManager.Instance.transform.eulerAngles = eulerAnglesRotation;
    }

    private void SetSlamRenderTextureAntialiasing(int mode)
    {
        if (!GSXRManager.Instance)
        {
            return;
        }

        switch (mode)
        {
            case 1:
                GSXROverrideSettings.EyeAntiAliasing = GSXROverrideSettings.eAntiAliasing.k1;
                break;
            case 2:
                GSXROverrideSettings.EyeAntiAliasing = GSXROverrideSettings.eAntiAliasing.k2;
                break;
            case 4:
                GSXROverrideSettings.EyeAntiAliasing = GSXROverrideSettings.eAntiAliasing.k4;
                break;

            default:
                Debug.LogError("Antialiasing: " + mode + " not supported!");
                break;
        }
    }

    private void DisableAudio()
    {

        AudioSource [] audioSources = GameObject.FindObjectsOfType<AudioSource>();
        foreach(AudioSource audio in audioSources)
        {
            audio.enabled = false;
        }
    }

    private void SetFoveatedRendering(Vector2 gain, float area, float minimum)
    {
        GSXRManager.Instance.settings.foveationGain = gain;
        GSXRManager.Instance.settings.foveationArea = area;
        GSXRManager.Instance.settings.foveationMinimum = minimum;
    }

    private float focusTime = 0;
    private Vector2 focusPosition = Vector2.zero;
    private float focusLength = 3;
    private void UpdateFocus()
    {
        var amplitude = GSXRConfigOptions.Instance.FocusAmplitude;
        var frequency = GSXRConfigOptions.Instance.FocusFrequency;
        var speed = GSXRConfigOptions.Instance.FocusSpeed;
        if (speed == 0f) speed = 1f;

        focusPosition.x = Mathf.Cos(focusTime * frequency.x) * amplitude.x;
        focusPosition.y = Mathf.Cos(focusTime * frequency.y) * amplitude.y;

        focusTime += Time.deltaTime * speed;

        GSXRManager.Instance.FocalPoint = focusPosition;

        if (reticle != null)
        {
            var position = reticle.localPosition;
            position.x = focusPosition.x;
            position.y = focusPosition.y;
            position.z = 1;
            position *= focusLength;
            reticle.localPosition = position;
        }
    }

}
