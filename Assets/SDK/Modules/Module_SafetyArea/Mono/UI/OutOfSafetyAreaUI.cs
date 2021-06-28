using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutOfSafetyAreaUI : MonoBehaviour
{
    public Button safetyAreaButton;
    public Button stationaryAreaButton;

    // Start is called before the first frame update
    private void Start()
    {
        safetyAreaButton.onClick.AddListener(OnSafetyAreaButtonClick);
        stationaryAreaButton.onClick.AddListener(OnStationaryAreaButtonClick);
    }

    private void OnDestroy()
    {
        safetyAreaButton.onClick.RemoveListener(OnSafetyAreaButtonClick);
        stationaryAreaButton.onClick.RemoveListener(OnStationaryAreaButtonClick);
    }

    private void OnEnable()
    {
        Debug.Log("OnEnterSafetyArea");
        SafetyAreaManager.Instance.OnEnterSafetyArea?.Invoke();
    }

    private void OnDisable()
    {
        Debug.Log("OnExitSafetyArea");
        SafetyAreaManager.Instance.OnExitSafetyArea?.Invoke();
    }

    private void OnSafetyAreaButtonClick()
    {
        SafetyAreaManager.Instance.StartSetSafetyArea();
    }

    private void OnStationaryAreaButtonClick()
    {
        SafetyAreaManager.Instance.StartSetSafetyArea();
        SafetyAreaManager.Instance.ChangeStep(SafetyAreaStepEnum.StationaryArea);
    }
}
