using SC.XR.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrackablePlaneGenerator : MonoBehaviour
{
    public GameObject trackablePlanePrefab;

    private List<PlaneTrackable> newPlanes = new List<PlaneTrackable>();

    public Text text;

    public void Update()
    {
        if (!GSXRManager.Instance.Initialized)
        {
            return;
        }

        text.text = API_GSXR_Slam.GSXR_Get_PanelNum().ToString();

        TrackableManager.Instance.GetTrackables<PlaneTrackable>(newPlanes, TrackableQueryFilter.New);
        for (int i = 0; i < newPlanes.Count; i++)
        {
            GameObject trackablePlaneGameObject = Instantiate(trackablePlanePrefab);
            trackablePlaneGameObject.GetComponent<TrackablePlaneMono>().Init(newPlanes[i]);
        }
    }
}
