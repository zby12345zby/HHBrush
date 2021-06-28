using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

//namespace SC.XR.Unity
//{
public class TrackableApi
{
    private const int PER_PLANE_DATA_COUNT = 68;

    private static Dictionary<int, PlaneTrackable> trackableDic = new Dictionary<int, PlaneTrackable>();

#if !UNITY_EDITOR

#else
    public static int ScGetPanelInfo(float[] info)
    {
        info[0] = 1f;
        info[1] = 11f;

        info[2] = 2.893829f;
        info[3] = -0.238066f;
        info[4] = 0.532357f;

        info[5] = 2.645216f;
        info[6] = -0.238066f;
        info[7] = -0.600777f;

        info[8] = 1.980295f;
        info[9] = -0.238066f;          
        info[10] = -1.802898f;

        info[11] = -0.126405f;
        info[12] = -0.238066f;
        info[13] = -1.982685f;

        info[14] = -0.821304f;
        info[15] = -0.238066f;
        info[16] = -1.763392f;

        info[17] = -1.553233f;
        info[18] = -0.238066f;
        info[19] = -0.675771f;

        info[20] = -1.991623f;
        info[21] = -0.238066f;
        info[22] = 4.984020f;

        info[23] = -1.931745f;
        info[24] = -0.238066f;
        info[25] = 5.263874f;

        info[26] = -1.695218f;
        info[27] = -0.238066f;
        info[28] = 5.598719f;

        info[29] = 1.800027f;
        info[30] = -0.238066f;
        info[31] = 5.302855f;

        info[32] = 2.451164f;
        info[33] = -0.238066f;
        info[34] = 2.975676f;

        return 0;
    }

    public static int ScGetPanel()
    {
        return 1;
    }
#endif

    /// <summary>
    /// index:
    /// 0: panelId
    /// 1: verticeCount
    /// 2~67 x y z
    /// </summary>
    /// <param name="trackables"></param>
    public static void GetPlaneInfo<T>(List<T> trackables) where T : Trackable
    {
        if (trackables == null)
        {
            Debug.LogError("please init trackables first!!!");
            return;
        }

        trackables.Clear();
        int planeCount =
#if UNITY_EDITOR
            ScGetPanel();
#else
            API_GSXR_Slam.GSXR_Get_PanelNum();
#endif
        float[] rawData = new float[planeCount * PER_PLANE_DATA_COUNT];
#if UNITY_EDITOR
        ScGetPanelInfo(rawData);
#else
        API_GSXR_Slam.GSXR_Get_PanelInfo(rawData);
#endif

        for (int i = 0; i < planeCount; i++)//plane loop
        {
            int planeId = (int)rawData[i * PER_PLANE_DATA_COUNT];
            int planeVerticesCount = (int)rawData[i * PER_PLANE_DATA_COUNT + 1];
            Vector3[] vertices = new Vector3[planeVerticesCount];
            for (int j = 0; j < vertices.Length; j++) // plane vertices loop
            {
                float x = rawData[(i * PER_PLANE_DATA_COUNT + 2) + (vertices.Length - j - 1) * 3];    
                float y = rawData[(i * PER_PLANE_DATA_COUNT + 2) + (vertices.Length - j - 1) * 3 + 1];
                float z = -rawData[(i * PER_PLANE_DATA_COUNT + 2) + (vertices.Length - j - 1) * 3 + 2];//
                vertices[j] = new Vector3(x, y, z);
            }
            PlaneTrackable trackable = CreateTrackable(planeId, vertices);//new PlaneTrackable(planeId, vertices);
            trackables.SafeAdd(trackable);
        }
    }

    private static PlaneTrackable CreateTrackable(int planeId, Vector3[] vertices)
    {
        if (trackableDic.ContainsKey(planeId))
        {
            PlaneTrackable planeTrackableCache = trackableDic[planeId];
            planeTrackableCache.UpdateVertices(vertices);
            return planeTrackableCache;
        }

        PlaneTrackable newTrackablePlane = new PlaneTrackable(planeId, vertices);
        trackableDic.Add(planeId, newTrackablePlane);
        return newTrackablePlane;

    }
}
//}