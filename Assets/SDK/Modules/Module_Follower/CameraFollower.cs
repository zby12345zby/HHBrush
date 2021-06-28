using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SC.XR.Unity {
    public class CameraFollower : FollowerBase {

        bool isInit = false;

        protected override void OnEnable() {
            base.OnEnable(); 
            isInit = false;
        }

        protected override void Follow() {
            if (API_GSXR_Slam.SlamManager?.head == null) {
                return;
            }

            if (isInit == false) {
                transform.position = CalculateWindowPosition(API_GSXR_Slam.SlamManager.head);
                transform.rotation = CalculateWindowRotation(API_GSXR_Slam.SlamManager.head);
                isInit = true;
            }

            transform.position = Vector3.Lerp(transform.position, CalculateWindowPosition(API_GSXR_Slam.SlamManager.head), WindowFollowSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, CalculateWindowRotation(API_GSXR_Slam.SlamManager.head), WindowFollowSpeed * Time.deltaTime);
        }
    }
}
