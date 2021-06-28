// Copyright 2020 The Tilt Brush Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using UnityEngine.Serialization;

namespace TiltBrush
{

    // Basic controller behaviors shared between the Wand and the Brush, both of which directly derrive
    //控制棒和刷子之间共享的基本控制器行为，两者都直接驱动
    // directly from BaseControllerBehavior.
    //直接从BaseControllerBehavior。
    public class BaseControllerBehavior : MonoBehaviour
    {

        /// <summary>
        /// 夹点状态
        /// </summary>
        public enum GripState
        {
            Standard,
            ReadyToGrip,
            Gripped
        }

        // -------------------------------------------------------------------------------------------- //
        // Inspector Data
        // -------------------------------------------------------------------------------------------- //
        [SerializeField] private InputManager.ControllerName m_ControllerName;//控制器名字
        [SerializeField] private ControllerGeometry m_ControllerGeometryPrefab;//控制器几何预制
        [FormerlySerializedAs("m_Offset")] [SerializeField] private Vector3 m_GeometryOffset;//几何体偏移量
        [FormerlySerializedAs("m_Rotation")]
        [SerializeField]
        private Quaternion m_GeometryRotation
            = Quaternion.identity;//几何体旋转

        // -------------------------------------------------------------------------------------------- //
        // Private Fields
        // -------------------------------------------------------------------------------------------- //

        /// <summary>
        /// 色调
        /// </summary>
        private Color m_Tint;
        /// <summary>
        /// 基本强度 
        /// </summary>
        private float m_BaseIntensity;
        /// <summary>
        /// 发光强度 
        /// </summary>
        private float m_GlowIntensity;

        private GripState m_CurrentGripState;//当前夹点状态
        private ControllerGeometry m_ControllerGeometry;//控制器几何

        // -------------------------------------------------------------------------------------------- //
        // Public Properties
        // -------------------------------------------------------------------------------------------- //

        /// <summary>
        /// 控制器名字
        /// </summary>
        public InputManager.ControllerName ControllerName
        {
            get { return m_ControllerName; }
        }


        private GameObject TransformVisuals
        {
            get { return ControllerGeometry.TransformVisualsRenderer.gameObject; }
        }

        /// <summary>
        /// 获取一个控制器
        /// </summary>
        public ControllerGeometry ControllerGeometry
        {
            get
            {
                if (m_ControllerGeometry == null)
                {
                    InstantiateControllerGeometryFromPrefab(null);
                }
                return m_ControllerGeometry;
            }
        }

        // Returns null if the ControllerName is invalid, or the requested controller does not exist.
        // 如果ControllerName无效或请求的控制器不存在，则返回null。
        /// <summary>
        /// 获取控制器信息
        /// </summary>
        public ControllerInfo ControllerInfo
        {
            get
            {
                int name = (int)ControllerName;
                var controllers = InputManager.Controllers;
                // This handles the ControllerName.None case, too.
                if (name >= 0 && name < controllers.Length)
                {
                    return controllers[name];
                }
                else
                {
                    return null;
                }
            }
        }

        // Override the controller geometry with an instance of the passed in prefab unless we're passed
        //除非我们被传递，否则用传入预制的实例重写控制器几何体
        // null. In that case, use the default controller geometry prefab.
        //空。在这种情况下，请使用默认的控制器几何体预设。
        public void InstantiateControllerGeometryFromPrefab(ControllerGeometry prefab)
        {
            bool changedController = false;
            if (m_ControllerGeometry != null)
            {
                Destroy(m_ControllerGeometry.gameObject);
                changedController = true;
            }

            if (prefab == null)
            {
                prefab = m_ControllerGeometryPrefab;
            }
            SetGeometry(Instantiate(prefab));

            if (changedController)
            {
                InputManager.ControllersHaveChanged();
            }
        }

        public Transform PointerAttachAnchor { get { return ControllerGeometry.PointerAttachAnchor; } }//指针连接锚
        public Transform PointerAttachPoint { get { return ControllerGeometry.PointerAttachPoint; } }//指针附着点
        public Transform ToolAttachAnchor { get { return ControllerGeometry.ToolAttachAnchor; } }//工具固定锚
        public Transform PinCushionSpawn { get { return ControllerGeometry.PinCushionSpawn; } }//针垫产卵

        // -------------------------------------------------------------------------------------------- //
        // Unity Events
        // -------------------------------------------------------------------------------------------- //

        void Awake()
        {
            m_CurrentGripState = GripState.Standard;
        }

        void Update()
        {
            // This is a proxy for "tutorial mode".
            //这是“教程模式”的代理。
            SketchControlsScript.m_Instance.AssignControllerMaterials(m_ControllerName);

            // Skip the tint and animation update if in intro sketch because
            // - nothing but the default has been assigned
            // - the user is not able to change the tint color
            // - we do not want to animate the buttons/pads
            //跳过色调和动画更新如果在简介草图，因为
            //-只分配了默认值              
            //-用户无法更改色调              
            //-我们不想设置按钮/垫的动画
            if (!PanelManager.m_Instance.IntroSketchbookMode)
            {
                // Send a signal to the controller that the materials have been assigned.
                //向管制员发送一个信号，说明材料已分配。
                ControllerGeometry.OnMaterialsAssigned(GetTintColor());
            }

            if (ControllerGeometry.XRayVisuals)
            {
                float XRayHeight_ss = (App.Scene.Pose.translation.y
                                    + App.Scene.Pose.scale * SceneSettings.m_Instance.ControllerXRayHeight);
                bool bControllerUnderground = transform.position.y < XRayHeight_ss;
                bool bHMDUnderground = ViewpointScript.Head.position.y < XRayHeight_ss;
                ControllerGeometry.XRayVisuals.SetActive(bControllerUnderground != bHMDUnderground);
            }

            if (ControllerGeometry.TriggerAnchor != null)
            {
                // This is hooked up for Wmr, Vive.
                // This is not hooked up for the Quest, Rift, Knuckles controller geometry;
                // they work using Animators and AnimateOculusTouchSteam.cs
                Vector2 range = m_ControllerGeometry.TriggerRotation;
                ControllerGeometry.TriggerAnchor.localRotation = Quaternion.AngleAxis(
                    Mathf.Lerp(range.x, range.y, ControllerInfo.GetTriggerRatio()),
                    Vector3.right);
            }

            //
            // If the transform visuals are active and the user is interacting with a widget, add the
            // transform visuals to the highlight queue. Eventually, we may:
            //
            // (a) only have the post process higlight in which case these transform visuals will not need
            //     a renderer/material or
            // (b) modify the highlight queue to a dynamic list that retains state across frames in which
            //     case this logic can be moved into EnableTransformVisuals().
            //
            if (TransformVisuals.activeSelf
                && SketchControlsScript.m_Instance.IsUserAbleToInteractWithAnyWidget())
            {
                App.Instance.SelectionEffect.RegisterMesh(TransformVisuals.GetComponent<MeshFilter>());

                switch (ControllerGeometry.Style)
                {
                    case ControllerStyle.OculusTouch:
                    case ControllerStyle.Knuckles:
                        App.Instance.SelectionEffect.RegisterMesh(
                            ControllerGeometry.JoystickPad.GetComponent<MeshFilter>());
                        break;
                    case ControllerStyle.Vive:
                        App.Instance.SelectionEffect.RegisterMesh(
                            ControllerGeometry.PadMesh.GetComponent<MeshFilter>());
                        break;
                    case ControllerStyle.Wmr:
                        // TODO What should be here?  Joystick or pad?
                        break;
                }
            }

            OnUpdate();
        }

        // -------------------------------------------------------------------------------------------- //
        // Virtual API
        // -------------------------------------------------------------------------------------------- //

        virtual protected void OnUpdate() { }

        virtual public void ActivateHint(bool bActivate) { }

        // Displays the swap effect on the controller. This may be overridden in sublcasses
        // if specific controllers have different implementations.
        virtual public void DisplayControllerSwapAnimation()
        {
            if (!App.Instance.ShowControllers) { return; }
            var highlightEffectPrefab = ControllerGeometry.HighlightEffectPrefab;
            GameObject rEffect = Instantiate(highlightEffectPrefab,
                                             transform.position,
                                             transform.rotation) as GameObject;
            rEffect.transform.parent = m_ControllerGeometry.transform;
            rEffect.transform.localPosition = highlightEffectPrefab.transform.localPosition;
            rEffect.transform.localRotation = highlightEffectPrefab.transform.localRotation;
        }

        // -------------------------------------------------------------------------------------------- //
        // Public API
        // -------------------------------------------------------------------------------------------- //

        /// <summary>
        /// Used to notify the user to look at their controllers.
        /// </summary>
        /// <param name="buzzLength">Duration of haptics in seconds</param>
        /// <param name="numPulses">Number of haptic pulses</param>
        /// <param name="interval">Duration between haptic pulses in seconds</param>
        public void BuzzAndGlow(float buzzLength, int numPulses, float interval)
        {
            //jiggle controller
            if (buzzLength > 0)
            {
                InputManager.m_Instance.TriggerHapticsPulse(
                    m_ControllerName, numPulses, interval, buzzLength);
            }

            //play glow effect
            var activateEffectPrefab = m_ControllerGeometry.ActivateEffectPrefab;
            if (activateEffectPrefab)
            {
                GameObject rGlow = Instantiate(activateEffectPrefab, transform.position, transform.rotation) as GameObject;
                rGlow.transform.parent = m_ControllerGeometry.transform;
                rGlow.transform.localPosition = activateEffectPrefab.transform.localPosition;
                rGlow.transform.localRotation = activateEffectPrefab.transform.localRotation;
            }
        }

        // Helper for SwapBehaviors. Swaps the poses of the two transforms.
        private static void SwapPoses(Transform a, Transform b)
        {
            // Since the parents are the same, we can take a shortcut and swap the local transform
            Debug.Assert(a.parent == b.parent);
            Vector3 tmpPosition = a.localPosition;
            Quaternion tmpRotation = a.localRotation;
            Vector3 tmpScale = a.localScale;
            a.localPosition = b.localPosition;
            a.localRotation = b.localRotation;
            a.localScale = b.localScale;
            b.localPosition = tmpPosition;
            b.localRotation = tmpRotation;
            b.localScale = tmpScale;
        }

        // Helper for SwapGeometry.
        //交换几何的助手。
        // Sets up pointers from this -> geom, and from geom -> this.
        //从this->geom和从geom->this设置指针。
        private void SetGeometry(ControllerGeometry geom)
        {
            m_ControllerGeometry = geom;

            // The back-pointers is implicit; it's geometry.transform.parent. //反向指针是隐式的；它是几何体.transform.parent.
            // worldPositionStays: false because we're about to overwrite it anyway //worldPositionStays:false，因为我们无论如何都要覆盖它
            m_ControllerGeometry.transform.SetParent(this.transform, worldPositionStays: false);
            Quaternion rot = m_GeometryRotation.IsInitialized() ? m_GeometryRotation : Quaternion.identity;

            //zby注释
            //Coords.AsLocal[m_ControllerGeometry.transform] = TrTransform.TRS(m_GeometryOffset, rot, 1);
            //zby增加
            Coords.AsLocal[m_ControllerGeometry.transform] = TrTransform.TRS(m_GeometryOffset, rot, 0.1f);

            m_ControllerGeometry.OnBehaviorChanged();

        }

        /// Swaps the behaviors associated with the controller geometries.
        /// The geometries themselves do not move.
        /// ///交换与控制器几何体关联的行为。              ///几何体本身不会移动。
        public static void SwapBehaviors(BaseControllerBehavior a, BaseControllerBehavior b)
        {
            // Well, this is a bit roundabout, because the behavior is the parent of the geometry
            // rather than vice versa. The geometries swap positions twice: once when they swap parents,
            // and again when their new parents swap places.
            SwapPoses(a.transform, b.transform);
            // Force instantiation using ControllerGeometry accessor
            var tmp = a.ControllerGeometry;
            a.SetGeometry(b.ControllerGeometry);
            b.SetGeometry(tmp);
        }

        public void SetTouchLocatorActive(bool active)
        {
            if (ControllerGeometry.PadTouchLocator != null)
            {
                ControllerGeometry.PadTouchLocator.gameObject.SetActive(active);
            }
        }

        public void SetTouchLocatorPosition(Vector2 loc)
        {
            if (ControllerGeometry.PadTouchLocator != null)
            {
                // Ensure the locator doesn't go beyond the edges of the pad face.
                // This value assumes loc is normalized to the range [-1,1].
                Vector2 offset = new Vector2(loc.x * m_ControllerGeometry.TouchLocatorTranslateScale,
                    loc.y * m_ControllerGeometry.TouchLocatorTranslateScale);
                if (offset.magnitude > m_ControllerGeometry.TouchLocatorTranslateClamp)
                {
                    offset = offset.normalized * m_ControllerGeometry.TouchLocatorTranslateClamp;
                }

                Vector3 pos =
                    new Vector3(offset.x, ControllerGeometry.PadTouchLocator.localPosition.y, offset.y);
                ControllerGeometry.PadTouchLocator.localPosition = pos;
            }
        }

        public void SetTint(Color rTintColor, float fBaseIntensity, float fGlowIntensity)
        {
            m_Tint = rTintColor;
            m_BaseIntensity = fBaseIntensity;
            m_GlowIntensity = fGlowIntensity;

            Color rTintedColor = GetTintColor();
            ControllerGeometry.MainMesh.material.SetColor("_EmissionColor", rTintedColor);
            ControllerGeometry.TriggerMesh.material.SetColor("_EmissionColor", rTintedColor);
            for (int i = 0; i < ControllerGeometry.OtherMeshes.Length; ++i)
            {
                ControllerGeometry.OtherMeshes[i].material.SetColor("_EmissionColor", rTintedColor);
            }
            ControllerGeometry.TransformVisualsRenderer.material.SetColor("_Color", rTintColor);

            if (ControllerGeometry.GuideLine)
            {
                ControllerGeometry.GuideLine.material.SetColor("_EmissionColor", m_Tint * m_BaseIntensity);
            }
        }

        /// <summary>
        /// 获取色调
        /// </summary>
        /// <returns></returns>
        private Color GetTintColor()
        {
            return m_Tint * (m_BaseIntensity + m_GlowIntensity);
        }


        public void EnableTransformVisuals(bool bEnable, float fIntensity)
        {
            TransformVisuals.SetActive(bEnable && App.Instance.ShowControllers);
            ControllerGeometry.TransformVisualsRenderer.material.SetFloat("_Intensity", fIntensity);
        }

        public void SetGripState(GripState state)
        {
            if (m_CurrentGripState != state)
            {
                ControllerStyle style = ControllerGeometry.Style;
                if (style != ControllerStyle.InitializingSteamVR &&
                    style != ControllerStyle.None &&
                    style != ControllerStyle.Unset)
                {

                    bool manuallyAnimateGrips = (style == ControllerStyle.Vive ||
                                                 style == ControllerStyle.Wmr);

                    switch (state)
                    {
                        case GripState.Standard:
                            if (manuallyAnimateGrips)
                            {
                                ControllerGeometry.LeftGripMesh.transform.localPosition = Vector3.zero;
                                ControllerGeometry.RightGripMesh.transform.localPosition = Vector3.zero;
                            }
                            ControllerGeometry.LeftGripMesh.material = ControllerGeometry.BaseGrippedMaterial;
                            ControllerGeometry.RightGripMesh.material = ControllerGeometry.BaseGrippedMaterial;
                            break;
                        case GripState.ReadyToGrip:
                            if (manuallyAnimateGrips)
                            {
                                ControllerGeometry.LeftGripMesh.transform.localPosition =
                                    m_ControllerGeometry.LeftGripPopOutVector;
                                Vector3 vRightPopOut = m_ControllerGeometry.LeftGripPopOutVector;
                                vRightPopOut.x *= -1.0f;
                                ControllerGeometry.RightGripMesh.transform.localPosition = vRightPopOut;
                            }
                            ControllerGeometry.LeftGripMesh.material = m_ControllerGeometry.GripReadyMaterial;
                            ControllerGeometry.RightGripMesh.material = m_ControllerGeometry.GripReadyMaterial;
                            ControllerGeometry.LeftGripMesh.material.SetColor("_Color", m_Tint);
                            ControllerGeometry.RightGripMesh.material.SetColor("_Color", m_Tint);
                            break;
                        case GripState.Gripped:
                            if (manuallyAnimateGrips)
                            {
                                ControllerGeometry.LeftGripMesh.transform.localPosition =
                                    m_ControllerGeometry.LeftGripPopInVector;
                                Vector3 vRightPopIn = m_ControllerGeometry.LeftGripPopInVector;
                                vRightPopIn.x *= -1.0f;
                                ControllerGeometry.RightGripMesh.transform.localPosition = vRightPopIn;
                            }
                            ControllerGeometry.LeftGripMesh.material = m_ControllerGeometry.GrippedMaterial;
                            ControllerGeometry.RightGripMesh.material = m_ControllerGeometry.GrippedMaterial;
                            ControllerGeometry.LeftGripMesh.material.SetColor("_Color", m_Tint);
                            ControllerGeometry.RightGripMesh.material.SetColor("_Color", m_Tint);
                            break;
                    }
                }
            }
            m_CurrentGripState = state;
        }
    }
}  // namespace TiltBrush
