using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayAreaNotEnoughUI : MonoBehaviour
{
    public Action OnSwitchToStationaryAreaClick
    {
        get;
        set;
    }

    public Action OnRedrawAreaClick
    {
        get;
        set;
    }

    public Action OnBackClick
    {
        get;
        set;
    }

    public Button redrawButton;
    public Button switchToStationaryAreaButton;
    public Button backButton;

    public void Init()
    {
        redrawButton.onClick.AddListener(()=>
        {
            OnRedrawAreaClick();
        });

        switchToStationaryAreaButton.onClick.AddListener(()=>
        {
            OnSwitchToStationaryAreaClick?.Invoke();
        });

        backButton.onClick.AddListener(()=>
        {
            OnBackClick?.Invoke();
        });
    }

    public void Release()
    {
        redrawButton.onClick.RemoveAllListeners();
        switchToStationaryAreaButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
    }
}
