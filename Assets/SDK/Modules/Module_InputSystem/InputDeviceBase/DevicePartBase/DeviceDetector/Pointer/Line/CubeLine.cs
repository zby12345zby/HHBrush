using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace SC.XR.Unity.Module_InputSystem {

    /// <summary>
    /// 负责显示
    /// </summary>
    public class CubeLine : LineBase {

        [SerializeField]
        private MeshRenderer meshRender;

        protected float initLocalScaleZ;
        [SerializeField]
        protected float initLocalScaleZConvertToMeter;
        private Vector3 tempLocalScale;

        public override void OnSCAwake() {
            if (meshRender) {
                initLocalScaleZ = meshRender.transform.localScale.z;
                tempLocalScale = meshRender.transform.localScale;
            }
        }

        public override void DrawLineIndicate() {
            if (meshRender) {
                pointerBase.detectorBase.inputDevicePartBase.inputDataBase.inputKeys.inputKeyDic.TryGetValue(InputKeyCode.Enter, out inputKeyState);
                if (inputKeyState == InputKeyState.DOWN || inputKeyState == InputKeyState.LONG) {
                    meshRender.material = dragLineMaterial;
                } else {
                    meshRender.material = normalLineMaterial;
                }

                if (pointerBase.detectorBase.inputDevicePartBase.inputDataBase.SCPointEventData.pointerCurrentRaycast.gameObject) {
                    tempLocalScale.z = Mathf.Clamp(
                        Vector3.Distance(pointerBase.detectorBase.inputDevicePartBase.inputDataBase.SCPointEventData.pointerCurrentRaycast.worldPosition, pointerBase.detectorBase.inputDevicePartBase.inputDevicePartUIBase.modelBase.HitStartPoint.position) / initLocalScaleZConvertToMeter * initLocalScaleZ,
                        0.01f, initLocalScaleZ);
                } else {
                    tempLocalScale.z = initLocalScaleZ;
                }
                meshRender.transform.localScale = tempLocalScale;

            }
        }

    }
}
