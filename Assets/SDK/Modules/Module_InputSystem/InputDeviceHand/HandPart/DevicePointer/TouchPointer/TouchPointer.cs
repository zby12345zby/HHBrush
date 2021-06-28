using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceHand {

    public class TouchPointer : PointerBase, INearPointer 
    {
        public HandDetector handDetector {
            get {
                return detectorBase as HandDetector;
            }
        }

        public float TouchDetectRadius { get => 0.1f; }
        public bool IsNearObject { get => newClosestTouchable!=null;  }
        public override PointerType PointerType => PointerType.Touch;
        private Collider[] queryBuffer;
        private Collider[] smallerqueryBuffer = new Collider[60];
        private Collider[] bigqueryBuffer = new Collider[180];
        private bool isTouchableDetect = false;

        public Action<bool> TargetDetectModelChange;


        SCPointEventData scPointEventData {
            get {
                return handDetector.inputDevicePartBase.inputDataBase.SCPointEventData;
            }
        }

        private Vector3 mPreviousTouchPosition;
        public Vector3 PreviousTouchPosition {
            get {
                if(mPreviousTouchPosition == Vector3.zero) {
                    mPreviousTouchPosition = ToucherPosition;
                }
                return mPreviousTouchPosition;
            }
            private set {
                mPreviousTouchPosition = value;
            }
        }

        private Vector3 mToucherPosition;
        public Vector3 ToucherPosition {
            get {
                return mToucherPosition = Vector3.Lerp(mToucherPosition, handDetector.inputDeviceHandPart.inputDeviceHandPartUI.modelHand.ActiveHandModel.GetJointTransform(FINGER.forefinger, JOINT.One).transform.position,0.5f);
                //return gT26DofDetector.inputDeviceGT26DofPart.inputDeviceGT26DofPartUI.modelGT26Dof.TouchCursor.transform.position;
            }
        }

        public Vector3 mTouchDetectCenterPosition;
        public Vector3 TouchDetectCenterPosition {
            get {
                return mTouchDetectCenterPosition = Vector3.Lerp(mTouchDetectCenterPosition, handDetector.inputDeviceHandPart.inputDataHand.handInfo.MainTouchDetectCenterPosition, 0.5f);
            }
        }

        public float DistToTouchable {
            get;
            private set;
        }

        float closestDistance;
        Vector3 closestNormal;
        BaseNearInteractionTouchable newClosestTouchable;

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
        public override void OnSCDisable() {
            base.OnSCDisable();
            if(currentPokeDownObject) {
                PokeUp(currentTouchableObject);
            }
            currentPokeDownObject = null;
            currentTouchableObject = null;
            IsFocusLocked = false;
            PreviousTouchPosition = Vector3.zero;
        }

        protected override void UpdateTransform() {
            if (newClosestTouchable) {
                start = ToucherPosition + TouchDetectRadius * closestNormal;
                end = ToucherPosition - TouchDetectRadius * closestNormal;
                transform.rotation = Quaternion.LookRotation(end - start);
                transform.position = start;
            } else {
                transform.rotation = Quaternion.identity;
                transform.position = Vector3.zero;
            }
        }
        
        private GameObject currentPokeDownObject = null;

        private BaseNearInteractionTouchable currentTouchableObject = null;

        protected override void DoTargetDetect() {
            
            if (newClosestTouchable == null)
                return;

            SCInputModule.Instance.ProcessCS(scPointEventData, transform, LayerMask, TouchDetectRadius * 2);

            IsFocusLocked = currentPokeDownObject != null;

            //Debug.Log(scPointEventData.pointerCurrentRaycast.gameObject);

            if(scPointEventData.pointerCurrentRaycast.gameObject) {

                DistToTouchable = Vector3.Distance(transform.position, scPointEventData.pointerCurrentRaycast.worldPosition) - TouchDetectRadius;
                bool newIsDown = (DistToTouchable < 0.0f);
                bool newIsUp = (DistToTouchable > newClosestTouchable.DebounceThreshold);

                if(newIsDown && currentPokeDownObject == null) {

                    if(IsObjectPartOfTouchable(scPointEventData.pointerCurrentRaycast.gameObject, newClosestTouchable)) {
                        currentPokeDownObject = scPointEventData.pointerCurrentRaycast.gameObject;
                        currentTouchableObject = newClosestTouchable;
                        PokeDown(currentTouchableObject);
                    }
                } else if(currentPokeDownObject) {
                    if(newIsUp) {
                        PokeUp(currentTouchableObject);
                        currentPokeDownObject = null ;
                        currentTouchableObject = null;
                    } else {
                        PokeUpdated(currentTouchableObject);
                    }
                }
            } else {
                if(currentPokeDownObject) {
                    PokeUp(currentTouchableObject);
                    currentPokeDownObject = null;
                    currentTouchableObject = null;
                }
            }

            PreviousTouchPosition = ToucherPosition;

        }

        private void PokeDown(BaseNearInteractionTouchable touchableObj) {
            if (touchableObj.EventsToReceive == TouchableEventType.Auto) {
                handDetector.inputDevicePartBase.inputDataBase.inputKeys.InputDataAddKey(InputKeyCode.Enter, InputKeyState.DOWN);
                HandDispatcher.OnPokeDown(touchableObj.gameObject, this, scPointEventData);
            } else if(touchableObj.EventsToReceive == TouchableEventType.Pointer) {
                handDetector.inputDevicePartBase.inputDataBase.inputKeys.InputDataAddKey(InputKeyCode.Enter, InputKeyState.DOWN);
            } else if(touchableObj.EventsToReceive == TouchableEventType.Touch) {
                HandDispatcher.OnPokeDown(touchableObj.gameObject, this, scPointEventData);
            }
        }
        private void PokeUp(BaseNearInteractionTouchable touchableObj) {
            if (touchableObj.EventsToReceive == TouchableEventType.Auto) {
                handDetector.inputDevicePartBase.inputDataBase.inputKeys.InputDataAddKey(InputKeyCode.Enter, InputKeyState.UP);
                HandDispatcher.OnPokeUp(touchableObj.gameObject, this, scPointEventData);
            } else if (touchableObj.EventsToReceive == TouchableEventType.Pointer) {
                handDetector.inputDevicePartBase.inputDataBase.inputKeys.InputDataAddKey(InputKeyCode.Enter, InputKeyState.UP);
            } else if(touchableObj.EventsToReceive == TouchableEventType.Touch) {
                HandDispatcher.OnPokeUp(touchableObj.gameObject, this, scPointEventData);
            }
        }

        private void PokeUpdated(BaseNearInteractionTouchable touchableObj) {
            if (touchableObj.EventsToReceive == TouchableEventType.Auto) {
                HandDispatcher.OnPokeUpdated(touchableObj.gameObject, this, scPointEventData);
            } else if (touchableObj.EventsToReceive == TouchableEventType.Pointer) {

            } else if(touchableObj.EventsToReceive == TouchableEventType.Touch) {
                HandDispatcher.OnPokeUpdated(touchableObj.gameObject, this, scPointEventData);
            }
        }

        private static bool IsObjectPartOfTouchable(GameObject targetObject, BaseNearInteractionTouchable touchable) {
            return targetObject != null && touchable != null &&
                               (targetObject == touchable.gameObject ||
                               // Descendant game objects are touchable as well. In particular, this is needed to be able to send
                               // touch events to Unity UI control elements.
                               (targetObject.transform != null && touchable.gameObject.transform != null &&
                               targetObject.transform.IsChildOf(touchable.gameObject.transform)));
        }

        public bool FindClosestTouchableForLayerMask() {
            isTouchableDetect = false;

            newClosestTouchable = null;
            closestDistance = float.PositiveInfinity;
            closestNormal = Vector3.zero;
            Array.Clear(queryBuffer,0, queryBuffer.Length);

            int numColliders = Physics.OverlapSphereNonAlloc(TouchDetectCenterPosition, TouchDetectRadius, queryBuffer, LayerMask);
            if(numColliders == queryBuffer.Length) {
                queryBuffer = bigqueryBuffer;
                Debug.LogWarning($"Maximum number of {numColliders} colliders found in TouchPointer overlap query. Consider increasing the query buffer size in the input system settings.");
                numColliders = Physics.OverlapSphereNonAlloc(TouchDetectCenterPosition, TouchDetectRadius, queryBuffer, LayerMask);
            }

            for(int i = 0; i < numColliders; i++) {

                var collider = queryBuffer[i];
                var touchable = collider.GetComponent<BaseNearInteractionTouchable>();
                if(touchable) {
                    isTouchableDetect = true;
                    float distance = touchable.DistanceToTouchable(ToucherPosition, out Vector3 normal);
                    if(distance < closestDistance) {
                        newClosestTouchable = touchable;
                        closestDistance = distance;
                        closestNormal = normal;
                    }
                }
            }

            // Unity UI does not provide an equivalent broad-phase test to Physics.OverlapSphere,
            // so we have to use a static instances list to test all NearInteractionTouchableUnityUI
            for (int i = 0; i < NearInteractionTouchableUnityUI.Instances.Count; i++) {
                NearInteractionTouchableUnityUI touchable = NearInteractionTouchableUnityUI.Instances[i];
                float distance = touchable.DistanceToTouchable(ToucherPosition, out Vector3 normal);
                if (distance <= TouchDetectRadius && distance < closestDistance) {

                    isTouchableDetect = true;

                    newClosestTouchable = touchable;
                    closestDistance = distance;
                    closestNormal = normal;
                }
            }

            //return newClosestTouchable != null;
            return isTouchableDetect;

        }


        public bool TryGetNearGraspPoint(out Vector3 position) {
            position = Vector3.zero;
            return false;
        }

        public bool TryGetNearGraspAxis(out Vector3 axis) {
            axis = transform.forward;
            return true;
        }

        public bool TryGetDistanceToNearestSurface(out float distance) {
            distance = closestDistance;
            return true;
        }

        public bool TryGetNormalToNearestSurface(out Vector3 normal) {
            normal = closestNormal;
            return true;
        }


        void OnDrawGizmos() {
            if(Application.isPlaying) {

                Gizmos.color = Color.black*0.2f;
                Gizmos.DrawSphere(transform.position, 0.01f);
                Gizmos.color = Color.black * 0.1f;
                Gizmos.DrawSphere(TouchDetectCenterPosition, TouchDetectRadius);

                Gizmos.color = Color.blue * 0.3f;
                Gizmos.DrawSphere(start, 0.02f);

                Gizmos.color = Color.blue * 0.2f;
                Gizmos.DrawSphere(end, 0.01f);
            }
        }

    }
}
