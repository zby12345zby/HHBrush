using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudPoints : MonoBehaviour
{
    [SerializeField]
    private Transform pointPrefab;
    private GameObject cloudFather;
    private List<GameObject> cloudPointList = new List<GameObject>();

    bool isStop = false;

    int dataNum = 0;
    ulong timeStamp = 0;
    float[] pointArray = new float[5000 * 3];

    // Start is called before the first frame update
    void Start()
    {
        cloudFather = new GameObject("CloudPoint");
        cloudFather.transform.position = Vector3.zero;
        GameObject obj;
        foreach (var item in pointArray) {
            obj = Instantiate<GameObject>(pointPrefab.gameObject);
            obj.transform.SetParent(cloudFather.transform,false);
            cloudPointList.Add(obj);
            obj.SetActive(false);
        }

        StartCoroutine(UpdateCloudPoint());
    }


    IEnumerator UpdateCloudPoint() {
        yield return new WaitUntil(()=>(API_GSXR_Slam.SlamManager!=null && API_GSXR_Slam.SlamManager.IsRunning));

        if (API_GSXR_Slam.GSXR_Is_EnablePointCloudData() == false) {
            API_GSXR_Slam.GSXR_Set_PointCloudData(true);
        }

        while ( !isStop) {

            yield return new WaitForSeconds(1);
            API_GSXR_Slam.GSXR_Get_PointCloudData(ref dataNum,ref timeStamp,pointArray);
            Debug.Log("CloudPoints Enable:"+ API_GSXR_Slam.GSXR_Is_EnablePointCloudData() + dataNum + " "+ timeStamp +" "+pointArray.Length);
            if (dataNum > 0) {
                foreach (var item in cloudPointList) {
                    item.SetActive(false);
                }

                for (int i = 0; i <= dataNum; i ++) {
                    cloudPointList[i].transform.position = new Vector3(pointArray[i * 3], pointArray[i * 3 + 1], -pointArray[i * 3 + 2]);
                    cloudPointList[i].SetActive(true);
                }
            }
        }
    }
}