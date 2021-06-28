using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace SC.XR.Unity {
    [RequireComponent(typeof(SCKeyboardMono))]
    public class SCKeyboardFollower : FollowerBase {

        Vector3 viewPoint = Vector3.zero;

        bool isFollowing = false;
        Vector2 initViewPoint;

        protected override void OnEnable() {
            base.OnEnable();
            initViewPoint = API_GSXR_Slam.SlamManager.leftCamera.WorldToViewportPoint(CalculateWindowPosition(API_GSXR_Slam.SlamManager.head));
            isFollowing = true;
            StopFollower = false;
        }

        protected override void Follow() {

            if (API_GSXR_Slam.SlamManager.leftCamera == null || API_GSXR_Slam.SlamManager.head == null)
                return;

            viewPoint = API_GSXR_Slam.SlamManager.leftCamera.WorldToViewportPoint(transform.position);

            if (viewPoint.x < (initViewPoint.x - 1f) || viewPoint.y < (initViewPoint.y - 0.5f) || viewPoint.x > (initViewPoint.x + 1f) || viewPoint.y > (initViewPoint.y + 1f) || Vector3.Magnitude(API_GSXR_Slam.SlamManager.head.position - transform.position) > (WindowDistance+0.1f)) {
                isFollowing = true;
            } else if (Mathf.Abs(viewPoint.x - initViewPoint.x) < 0.03f && Mathf.Abs(viewPoint.y - initViewPoint.y) < 0.03f) {
                isFollowing = false;
                StopFollower = true;
            }

            if (isFollowing) {
                transform.position = Vector3.Lerp(transform.position, CalculateWindowPosition(API_GSXR_Slam.SlamManager.head), WindowFollowSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, CalculateWindowRotation(API_GSXR_Slam.SlamManager.head), WindowFollowSpeed * Time.deltaTime);
            }

        }
    }
}
