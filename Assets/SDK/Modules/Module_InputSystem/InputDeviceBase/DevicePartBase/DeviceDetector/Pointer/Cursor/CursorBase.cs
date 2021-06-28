using UnityEngine;
using System;
using System.Collections.Generic;

namespace SC.XR.Unity.Module_InputSystem {
    public abstract class CursorBase : SCModuleMono {

        PointerBase mPointerBase;
        public PointerBase pointerBase {
            get {
                if(mPointerBase == null) {
                    mPointerBase = GetComponentInParent<PointerBase>();
                }
                return mPointerBase;
            }
            private set {
                mPointerBase = value;
            }
        }

        Gazeloading mGazeloading;
        public Gazeloading Gazeloading
        {
            get
            {
                if (mGazeloading == null)
                {
                    mGazeloading = GetComponentInChildren<Gazeloading>(true);
                }
                return mGazeloading;
            }
            private set
            {
                mGazeloading = value;
            }
        }


        public float DefaultDistance = 3f;

        private Vector3 _initLocalScale;
        private Vector3 _tempLocalScale;
        [SerializeField]
        private float initLocalScaleFactor = 1;
        private float _initLocalScaleConvertToMeter = 0.8f;


        protected CursorInPercent cursorInPercent;
        public float visualPercent { get; private set; }

        private GameObject hitObject;
        public override void OnSCAwake()
        {
            base.OnSCAwake();
            AddModule(Gazeloading);
            transform.localScale = transform.localScale * initLocalScaleFactor;
           _initLocalScale = transform.localScale;
        }

        public override void OnSCStart() {
            base.OnSCStart();
            _tempLocalScale = _initLocalScale;

            if (pointerBase.detectorBase.inputDevicePartBase.inputDataBase.SCPointEventData.pointerCurrentRaycast.gameObject != null) {
                hitObject = pointerBase.detectorBase.inputDevicePartBase.inputDataBase.SCPointEventData.pointerCurrentRaycast.gameObject;
                cursorInPercent = pointerBase.detectorBase.inputDevicePartBase.inputDataBase.SCPointEventData.pointerCurrentRaycast.gameObject.GetComponent<CursorInPercent>();
                if (cursorInPercent != null && cursorInPercent.enabled) {
                    visualPercent = cursorInPercent.CurrentInPercentStart0;
                } else {
                    visualPercent = 1;
                }
            }
        }

        public override void OnSCLateUpdate() {
            base.OnSCLateUpdate();
            UpdateTransform();
            UpdateCursorInPercent();
            UpdateCursorVisual();
        }

        public override void OnSCDestroy() {
            base.OnSCDestroy();
            pointerBase = null;
        }

        public virtual void UpdateTransform() { }
        
        public abstract void UpdateCursorVisual();

        public void StartGazeAnimation(float timer) {
            Gazeloading.timer = timer;
            Gazeloading.ModuleStart();
        }
        public void StopGazeAnimation()
        {
            Gazeloading.ModuleStop();

        }
        float distance;
        protected virtual void UpdatelocalScale() {
            if(pointerBase.detectorBase.inputDevicePartBase.inputDataBase.SCPointEventData.DownPressGameObject && pointerBase.detectorBase.inputDevicePartBase.inputDevicePartUIBase?.modelBase.HitStartPoint) {
                distance = Vector3.Distance(pointerBase.detectorBase.inputDevicePartBase.inputDataBase.SCPointEventData.dragObjPosition, pointerBase.detectorBase.inputDevicePartBase.inputDevicePartUIBase.modelBase.HitStartPoint.position);
            }else if(pointerBase.detectorBase.inputDevicePartBase.inputDataBase.SCPointEventData.pointerCurrentRaycast.gameObject && pointerBase.detectorBase.inputDevicePartBase.inputDevicePartUIBase?.modelBase.HitStartPoint) {
                distance = Vector3.Distance(pointerBase.detectorBase.inputDevicePartBase.inputDataBase.SCPointEventData.pointerCurrentRaycast.worldPosition, pointerBase.detectorBase.inputDevicePartBase.inputDevicePartUIBase.modelBase.HitStartPoint.position);
            }  else {
                distance = DefaultDistance;
            }
            distance = Mathf.Clamp(distance, _initLocalScaleConvertToMeter, 1.5f);
            _tempLocalScale = distance / _initLocalScaleConvertToMeter * _initLocalScale;
            transform.localScale = _tempLocalScale;
        }

        protected virtual void UpdateCursorInPercent() {

            if (pointerBase.detectorBase.inputDevicePartBase.inputDataBase.SCPointEventData.pointerCurrentRaycast.gameObject != null) {

                if (hitObject != pointerBase.detectorBase.inputDevicePartBase.inputDataBase.SCPointEventData.pointerCurrentRaycast.gameObject) {
                    hitObject = pointerBase.detectorBase.inputDevicePartBase.inputDataBase.SCPointEventData.pointerCurrentRaycast.gameObject;
                    cursorInPercent = pointerBase.detectorBase.inputDevicePartBase.inputDataBase.SCPointEventData.pointerCurrentRaycast.gameObject.GetComponent<CursorInPercent>();
                }

                if (cursorInPercent != null && cursorInPercent.enabled) {
                    visualPercent = cursorInPercent.CurrentInPercentStart0;
                } else {
                    visualPercent = 1;
                }
            }

            if (cursorInPercent != null && cursorInPercent.enabled) {
                visualPercent = cursorInPercent.CurrentInPercentStart0;
            } else {
                visualPercent = 1;
            }
            //Debug.Log("visualPercent:" + visualPercent);
        }
    }
}
