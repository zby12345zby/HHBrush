using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GSXRDebugHud : MonoBehaviour, GSXRManager.SlamEventListener
{
    public GSXRManager slamManager;

    public GameObject EventDisplay;
    public GameObject PositionWarning;
    public GameObject FramesPerSecond;
    public GameObject Orientation;
    public GameObject Position;
    public GameObject Eyes;

    private Text _eventText;
    private Text _warningText;
    private Text _fpsText;
    private Text _orientationText;
    private Text _positionText;
    private Text _eyesText;

    private float _framesPerSecond = 0;

    private void Awake()
    {
        _eventText = EventDisplay.GetComponent<Text>();
        _warningText = PositionWarning.GetComponent<Text>();
        _fpsText = FramesPerSecond.GetComponent<Text>();
        _orientationText = Orientation.GetComponent<Text>();
        _positionText = Position.GetComponent<Text>();
        _eyesText = Eyes.GetComponent<Text>();
    }

    private void Start()
    {
        slamManager = GSXRManager.Instance;
        Debug.Assert(slamManager != null, "GSXRManager object not found");
        if (slamManager != null)
        {
            slamManager.AddEventListener(this); // Register for SlamEvents
            StartCoroutine(CalculateFramesPerSecond());
            StartCoroutine(DisplayEvents());
            if (_eyesText != null) _eyesText.enabled = slamManager.settings.trackEyes /*&& (GSXRPlugin.Instance.GetTrackingMode() & (int)GSXRPlugin.TrackingMode.kTrackingEye) != 0*/;
            if (_positionText != null) _positionText.enabled = slamManager.settings.trackPosition /*&& (GSXRPlugin.Instance.GetTrackingMode() & (int)GSXRPlugin.TrackingMode.kTrackingPosition) != 0*/;
        }
        if (_eventText != null) _eventText.gameObject.SetActive(false);
        if (_warningText != null) _warningText.gameObject.SetActive(false);
    }

    private void LateUpdate () 
    {
        if (slamManager == null)
            return;

        var headTransform = slamManager.head;

        transform.position = headTransform.position;
        transform.rotation = headTransform.rotation;
		
        Quaternion orientation = headTransform.localRotation;
        if (_orientationText != null && _orientationText.isActiveAndEnabled)
        {
            //_orientationText.text = string.Format("{0:F3}, {1:F3}, {2:F3}, {3:F3}", orientation.x, orientation.y, orientation.z, orientation.w);
            var rotation = orientation.eulerAngles;
            _orientationText.text = string.Format("Pitch {0:F2}, Yaw {1:F2}, Roll {2:F2}",
                rotation.x > 180 ? rotation.x - 360 : rotation.x, 
                rotation.y > 180 ? rotation.y - 360 : rotation.y, 
                rotation.z > 180 ? rotation.z - 360 : rotation.z);
            _orientationText.color = (slamManager.status.pose & (int)GSXRPlugin.TrackingMode.kTrackingOrientation) == 0 ? Color.red : Color.green;
        }

        Vector3 position = headTransform.localPosition;
        if (_positionText != null && _positionText.isActiveAndEnabled)
        {
            _positionText.text = string.Format("X {0:F2}, Y {1:F2}, Z {2:F2}", position.x, position.y, position.z);
            _positionText.color = (slamManager.status.pose & (int)GSXRPlugin.TrackingMode.kTrackingPosition) == 0 ? Color.red : Color.green;
        }

        if (_eyesText != null && _eyesText.isActiveAndEnabled)
        {
            float eyeLeftOpenness = GSXRManager.Instance.eyePose.leftOpenness;
            float eyeRightOpenness = GSXRManager.Instance.eyePose.rightOpenness;
            _eyesText.text = string.Format("{0:F2} Eye Openness {1:F2}", eyeLeftOpenness, eyeRightOpenness);
            _eyesText.color = (slamManager.status.pose & (int)GSXRPlugin.TrackingMode.kTrackingEye) == 0 ? Color.red : Color.green;
        }

        if (_fpsText != null && _fpsText.isActiveAndEnabled)
        {
            int fps = Mathf.RoundToInt(_framesPerSecond);
            int refreshRate = Mathf.RoundToInt(GSXRPlugin.Instance.deviceInfo.displayRefreshRateHz);
            _fpsText.text = string.Format("{0} / {1} FPS", fps, refreshRate);
            _fpsText.color = fps < refreshRate ? Color.yellow : Color.green;
        }

        if (_warningText != null && slamManager.settings.trackPosition && slamManager.settings.trackPosition && (GSXRPlugin.Instance.GetTrackingMode() & (int)GSXRPlugin.TrackingMode.kTrackingPosition) != 0)
        {
            var isValid = (slamManager.status.pose & (int)GSXRPlugin.TrackingMode.kTrackingPosition) != 0;
            _warningText.gameObject.SetActive(!isValid);
        }
    }

    private IEnumerator CalculateFramesPerSecond()
    {
        int lastFrameCount = 0;

        while (true)
        {
            yield return new WaitForSecondsRealtime(1.0f);

            var currentFrameCount = slamManager.FrameCount;
            var elapsedFrames = currentFrameCount - lastFrameCount;
            _framesPerSecond = elapsedFrames / 1.0f;
            lastFrameCount = currentFrameCount;
        }
    }

    private List<GSXRManager.slamEventType> events = new List<GSXRManager.slamEventType>();
    private IEnumerator DisplayEvents()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(2.0f);

            if (!slamManager.IsOverlayFading())
            {
                if (events.Count > 0)
                {
                    var eventType = events[0];
                    _eventText.gameObject.SetActive(true);
                    _eventText.text = eventType.ToString();
                    events.RemoveAt(0);
                }
                else if (_eventText.gameObject.activeSelf)
                {
                    _eventText.gameObject.SetActive(false);
                }
            }
        }
    }

    public void OnSlamEvent(GSXRManager.SlamEvent ev)
    {
        switch (ev.eventType)
        {
            case GSXRManager.slamEventType.kEventNone:
                break;
            default:
                if (!events.Contains(ev.eventType)) events.Add(ev.eventType);
                break;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

}
