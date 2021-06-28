using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceHand {

    public class GrabPointer : PointerBase,INearPointer {
        public HandDetector handDetector {
            get {
                return detectorBase as HandDetector;
            }
        }

        public float GrabActiveRadius { get => 0.07f; }

        private float GrabRadius { get => 0.035f; }

        public bool IsNearObject { get; set; }

        public override PointerType PointerType =>PointerType.Grab;


        private Collider[] queryBuffer;
        private Collider[] smallerqueryBuffer = new Collider[60];
        private Collider[] bigqueryBuffer = new Collider[180];


        RaycastResult grabSimlateResult;

        Vector3 OverlapSphereCenter {
            get {
                return handDetector.inputDeviceHandPart.inputDataHand.handInfo.MainGrabPosition;
            }
        }

        float closestDistance;
        Vector3 closestNormal;
        NearInterationGrabbable newClosestGrabbable;
        Vector3 closestPoint;

        Vector3 start, end;

        public override void OnSCAwake() {
            base.OnSCAwake();
            queryBuffer = smallerqueryBuffer;
        }

        public override void OnSCStart() {
            base.OnSCStart();
            queryBuffer = smallerqueryBuffer;
            lineBase?.ModuleStop();
        }

        public override void OnSCLateUpdate() {
            base.OnSCLateUpdate();
            if (newClosestGrabbable) {
                if (cursorBase && !cursorBase.IsModuleStarted) {
                    cursorBase.ModuleStart();
                }
            } else {
                if (cursorBase && cursorBase.IsModuleStarted) {
                    cursorBase.ModuleStop();
                }
            }
        }

        public override void OnSCDisable() {
            base.OnSCDisable();
            IsFocusLocked = false;
        }


        protected override void UpdateTransform() {
            start = OverlapSphereCenter;
            transform.position = start;

            transform.rotation = Quaternion.LookRotation(start - API_GSXR_Slam.SlamManager.head.position, API_GSXR_Slam.SlamManager.head.up);

            //if (newClosestGrabbable) {
            //    end = closestPoint;
            //    if (end - start != Vector3.zero) {
            //        transform.rotation = Quaternion.LookRotation(end - start);
            //    } else {
            //        transform.rotation = Quaternion.identity;
            //    }
            //    transform.rotation = newClosestGrabbable.transform.rotation;
            //} else {
              //  transform.rotation = handDetector.inputDeviceHandPart.inputDataHand.handInfo.MainGrabRotation;
            //}
        }

        protected override void DoTargetDetect() {
            if(IsFocusLocked || newClosestGrabbable) {
                SCInputModule.Instance.ProcessCS(handDetector.inputDevicePartBase.inputDataBase.SCPointEventData, transform, LayerMask, MaxDetectDistance, true, grabSimlateResult);
                IsFocusLocked = handDetector.inputDevicePartBase.inputDataBase.SCPointEventData.DownPressGameObject != null;
            }
        }

        public bool FindClosestGrabbableForLayerMask() {

            newClosestGrabbable = null;
            closestDistance = float.PositiveInfinity;
            IsNearObject = false;
            Array.Clear(queryBuffer, 0, queryBuffer.Length);

            int numColliders = Physics.OverlapSphereNonAlloc(OverlapSphereCenter, GrabActiveRadius, queryBuffer, LayerMask);

            if (numColliders == queryBuffer.Length) {
                queryBuffer = bigqueryBuffer;
                Debug.LogWarning($"Maximum number of {numColliders} colliders found in GrabPointer overlap query. Consider increasing the query buffer size in the input system settings.");
                numColliders = Physics.OverlapSphereNonAlloc(OverlapSphereCenter, GrabActiveRadius, queryBuffer, LayerMask);
            }

            for (int i = 0; i < numColliders; i++) {
                var collider = queryBuffer[i];
                var nearInteration = collider.GetComponent<NearInterationGrabbable>();

                IsNearObject = nearInteration != null;
                if(IsNearObject) {
                    break;
                }
            }

            if(IsNearObject == false) {
                return false;
            }

            numColliders = Physics.OverlapSphereNonAlloc(OverlapSphereCenter, GrabRadius, queryBuffer, LayerMask);
            
            Vector3 objectHitPoint = Vector3.zero;

            for(int i = 0; i < numColliders; i++) {
                var collider = queryBuffer[i];
                var touchable = collider.GetComponent<NearInterationGrabbable>();
                if(touchable) {
                    
                    objectHitPoint = collider.ClosestPoint(OverlapSphereCenter);
                    float distance = (objectHitPoint - OverlapSphereCenter).magnitude;

                    if(distance < closestDistance) {
                        newClosestGrabbable = touchable;
                        closestDistance = distance;
                        closestPoint = objectHitPoint;
                    }
                }
            }

            if(newClosestGrabbable != null) {
                //Debug.Log("newClosestGarbbable:" + closest.name);
                grabSimlateResult.gameObject = newClosestGrabbable.gameObject;
                grabSimlateResult.worldPosition = closestPoint;
            }

            return IsNearObject;

        }

        public bool TryGetNearGraspPoint(out Vector3 position) {
            throw new NotImplementedException();
        }

        public bool TryGetNearGraspAxis(out Vector3 axis) {
            throw new NotImplementedException();
        }

        public bool TryGetDistanceToNearestSurface(out float distance) {
            throw new NotImplementedException();
        }

        public bool TryGetNormalToNearestSurface(out Vector3 normal) {
            throw new NotImplementedException();
        }

        void OnDrawGizmos() {
            if(Application.isPlaying) {

                Gizmos.color = Color.black * 0.3f;
                Gizmos.DrawSphere(transform.position, 0.01f);
                Gizmos.color = Color.black * 0.2f;
                Gizmos.DrawSphere(OverlapSphereCenter, GrabRadius);
                Gizmos.color = Color.black * 0.1f;
                Gizmos.DrawSphere(OverlapSphereCenter, GrabActiveRadius);

                if(newClosestGrabbable) {
                    Gizmos.color = Color.blue * 0.3f;
                    Gizmos.DrawSphere(start, 0.02f);
                    Gizmos.color = Color.blue * 0.2f;
                    Gizmos.DrawSphere(end, 0.01f);
                }
            }
        }

    }
}
