using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialGameObjectLoader
{
    private const string SPATIAL_GAMEOBJECT_KEY = "SPATIAL_GAMEOBJECT_KEY";

    private static List<SpatialGameObjectData> CreateNewGameObjectData(SpatialObjectConfig spatialObjectConfig)
    {
        List<SpatialGameObjectData> dataList = new List<SpatialGameObjectData>();
        float startPlacePositionX = -(spatialObjectConfig.m_PrefabList.Count - 1) * 0.3f / 2f;

        for (int i = 0; i < spatialObjectConfig.m_PrefabList.Count; i++)
        {
            string prefabName = spatialObjectConfig.m_PrefabList[i];
            Vector3 localPosition = new Vector3(startPlacePositionX + i * 0.3f, 0, 0);
            SpatialGameObjectData spatialGameObject = new SpatialGameObjectData();
            spatialGameObject.SetPrefabName(prefabName);
            spatialGameObject.SetTransformData(localPosition, Quaternion.identity, Vector3.one * 0.1f);
            dataList.Add(spatialGameObject);
        }
        return dataList;
    }

    public static List<SpatialGameObjectData> LoadGameObjectData(SpatialObjectConfig spatialObjectConfig)
    {
        string savedData = PlayerPrefs.GetString(SPATIAL_GAMEOBJECT_KEY, string.Empty);
        Debug.Log(savedData);
        if (string.IsNullOrEmpty(savedData))
        {
            return CreateNewGameObjectData(spatialObjectConfig);
        }
        return JsonUtility.FromJson<SpatialGameObjectDataList>(savedData).data;
    }

    public static void SaveGameObjectData(List<SpatialGameObjectData> data)
    {
        SpatialGameObjectDataList dataList = new SpatialGameObjectDataList();
        dataList.data = data;
        PlayerPrefs.SetString(SPATIAL_GAMEOBJECT_KEY, JsonUtility.ToJson(dataList));
        PlayerPrefs.Save();
    }

    public static void ClearGameObjectData()
    {
        PlayerPrefs.DeleteKey(SPATIAL_GAMEOBJECT_KEY);
    }
}
