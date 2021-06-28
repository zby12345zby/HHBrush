using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class SpatialAwarenessDemo : MonoBehaviour
{
    public Text relocStateText;
    public Button resaveButton;
    public Button saveMapButton;

    public SpatialObjectConfig spatialObjectConfig;

    public Transform spatialAwarenessRoot;

    public float getRelocStateTime = 1f;
    private float currentTime = 0f;
    private int previousRelocState = 0;
    /// <summary>
    /// 表示是否正在记忆地图
    /// </summary>
    private bool rememberingMap = false;

    /// <summary>
    /// 当识别/丢失地图时触发
    /// </summary>
    private Action<bool> onAwareness;

    List<SpatialGameObjectData> dataList;

    // Start is called before the first frame update
    private void Start()
    {
        resaveButton.onClick.AddListener(OnResaveButtonClick);
        saveMapButton.onClick.AddListener(OnSaveMapButtonClick);
        onAwareness += OnAwareness;
        LoadGameObject();
        ShowGameObject(false);
    }

    private void Update()
    {
        if (rememberingMap) return;

        currentTime += Time.deltaTime;
        if (currentTime > getRelocStateTime)
        {
            currentTime = 0f;
            int currentRelocState = API_GSXR_Slam.GSXR_Get_OfflineMapRelocState();
            relocStateText.text = currentRelocState.ToString();
            Debug.Log("currentRelocState:" + currentRelocState);
            if (previousRelocState != currentRelocState)
            {
                onAwareness?.Invoke(currentRelocState == 1);
            }
            previousRelocState = currentRelocState;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void OnDestroy()
    {
        resaveButton.onClick.RemoveListener(OnResaveButtonClick);
        saveMapButton.onClick.RemoveListener(OnSaveMapButtonClick);
        onAwareness -= OnAwareness;
    }


    private void OnResaveButtonClick()
    {
        rememberingMap = true;
        ShowGameObject(true);
        ClearGameObject();
        LoadGameObject();
        API_GSXR_Slam.GSXR_ResaveMap("forTest");
    }

    private void OnSaveMapButtonClick()
    {
        rememberingMap = false;
        SaveGameObject();
        ShowGameObject(false);
        previousRelocState = 0;
        API_GSXR_Slam.GSXR_SaveMap();
    }

    private void OnAwareness(bool isAware)
    {
        ShowGameObject(isAware);
    }

    private void ShowGameObject(bool showGameObject)
    {
        Debug.Log("ShowGameObject" + showGameObject);
        spatialAwarenessRoot.gameObject.SetActive(showGameObject);
    }

    private void ClearGameObject()
    {
        SpatialGameObjectLoader.ClearGameObjectData();
        for (int i = 0; i < spatialAwarenessRoot.childCount; i++)
        {
            Debug.Log("Destroy");
            Destroy(spatialAwarenessRoot.GetChild(i).gameObject);
        }
    }

    private void SaveGameObject()
    {
        if (dataList != null)
        {
            SpatialGameObjectLoader.SaveGameObjectData(dataList);
        }
    }

    private void LoadGameObject()
    {
        dataList = SpatialGameObjectLoader.LoadGameObjectData(spatialObjectConfig);
        Debug.Log("dataList:" + dataList.Count);
        for (int i = 0; i < dataList.Count; i++)
        {
            Debug.Log("Load");
            SpatialGameObjectData spatialGameObjectData = dataList[i];
            Vector3 localPosition = Vector3.zero;
            Quaternion localRotation = Quaternion.identity;
            Vector3 localScale = Vector3.one;
            GameObject gameObjectResource = Resources.Load<GameObject>(spatialGameObjectData.GetPrefabName());
            spatialGameObjectData.GetTransformData(ref localPosition, ref localRotation, ref localScale);
            GameObject gameObjectInstance = GameObject.Instantiate(gameObjectResource, spatialAwarenessRoot);
            gameObjectInstance.transform.localPosition = localPosition;
            gameObjectInstance.transform.localRotation = localRotation;
            gameObjectInstance.transform.localScale = localScale;
            gameObjectInstance.GetComponent<SpatialGameObjectMono>().SetDataEntity(spatialGameObjectData);
        }
    }
}
