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

    public class VrControllers : MonoBehaviour
    {
        [SerializeField] private ControllerBehaviorWand m_Wand;//左手柄
        [SerializeField] private ControllerBehaviorBrush m_Brush;//右手柄（画笔）

        // Monobehaviours that must be disabled to disable pose tracking
        //必须禁用的单一行为才能禁用姿势跟踪
        [SerializeField] private MonoBehaviour[] m_TrackingComponents;

        [Header("SteamVR Haptics")]//SteamVR触觉 
        [FormerlySerializedAs("m_HapticsScaleAdjustment")]
        [SerializeField] private float m_HapticsDurationScale = 1.0f;//翻译：触觉时间缩放值
        [SerializeField] private float m_HapticAmplitudeScale = 1.0f;//翻译：触觉幅度缩放值

        [Header("Input Zones")]//输入区域
        [SerializeField] private Vector2 m_TriggerActivationRange = new Vector2(0.15f, .8f);//触发激活范围
        [SerializeField] private Vector2 m_GripActivationRange = new Vector2(0.15f, .8f);//夹点激活范围
        [SerializeField] private Vector2 m_TouchpadActivationRange = new Vector2(-.8f, .8f);//触摸板激活范围
        [SerializeField] private Vector2 m_LogitechPenActivationRange = new Vector2(0.0f, 1.0f);//罗技笔激活范围
        [SerializeField] private float m_WandRotateJoystickPercent = 0.7f;//操纵杆旋转百分比

        // VR headsets (e.g., Rift, Vive, Wmr) use different hardware for their controllers,//VR耳机（如Rift、Vive、Wmr）的控制器使用不同的硬件
        // they require a scaled duration for the haptics to be felt in users hand.//它们需要一个缩放的持续时间，才能让用户在手上感觉到触觉。
        public float HapticsDurationScale
        {
            get { return m_HapticsDurationScale; }
        }

        public float HapticsAmplitudeScale
        {
            get { return m_HapticAmplitudeScale; }
        }


        /// <summary>
        /// 获取指定名字的控制器（左手柄或者右手柄）
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BaseControllerBehavior GetBehavior(InputManager.ControllerName name)
        {
            switch (name)
            {
                case InputManager.ControllerName.Brush:
                    return m_Brush;
                case InputManager.ControllerName.Wand:
                    return m_Wand;
                default:
                    throw new System.ArgumentException(
                        string.Format("Unknown controller behavior {0}", name));
            }
        }

        /// <summary>
        /// 返回包含两个手柄的数组
        /// </summary>
        /// <returns></returns>
        public BaseControllerBehavior[] GetBehaviors()
        {
            Debug.Assert((int)m_Wand.ControllerName == 0);
            Debug.Assert((int)m_Brush.ControllerName == 1);
            // The array is indexed by ControllerName, so the order here is important!
            return new BaseControllerBehavior[] { m_Wand, m_Brush };
        }

        /// Normally all controllers are assumed to be the same style.
        /// 通常假设所有控制器都是相同的样式。
        /// Logitech switches one of them to be a pen.
        /// 罗技公司把其中一个换成了钢笔。
        /// This returns the style that both controllers used before one of them was switched to a pen.
        /// 这将返回两个控制器在其中一个切换到笔之前使用的样式。
        public ControllerStyle BaseControllerStyle
        {
            get
            {
                if (m_Wand.ControllerGeometry.Style != ControllerStyle.LogitechPen)
                {
                    return m_Wand.ControllerGeometry.Style;
                }
                else
                {
                    return m_Brush.ControllerGeometry.Style;
                }
            }
        }


        public Vector2 TouchpadActivationRange
        {
            get { return m_TouchpadActivationRange; }
        }

        public float WandRotateJoystickPercent
        {
            get { return m_WandRotateJoystickPercent; }
        }

        /// The usable range of the raw grip value.
        /// 原始夹点值的可用范围。
        /// This is currently only used as the threshold for analog -> boolean conversion.
        /// 当前仅用作模拟->布尔转换的阈值。
        public Vector2 GripActivationRange
        {
            get { return m_GripActivationRange; }
        }

        public ControllerBehaviorWand Wand
        {
            get { return m_Wand; }
        }

        public ControllerBehaviorBrush Brush
        {
            get { return m_Brush; }
        }

        /// <summary>
        /// 罗技笔是否存在
        /// </summary>
        /// <returns></returns>
        public bool LogitechPenIsPresent()
        {
            return (m_Brush.ControllerGeometry.Style == ControllerStyle.LogitechPen) ||
                   (m_Wand.ControllerGeometry.Style == ControllerStyle.LogitechPen);
        }

        /// <summary>
        /// 主滚动方向是否为X
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool PrimaryScrollDirectionIsX(InputManager.ControllerName name)
        {
            var behavior = GetBehavior(name);
            if (behavior.ControllerGeometry.Style == ControllerStyle.LogitechPen)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 触发激活范围
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Vector2 TriggerActivationRange(InputManager.ControllerName name)
        {
            var behavior = GetBehavior(name);
            if (behavior.ControllerGeometry.Style == ControllerStyle.LogitechPen)
            {
                return m_LogitechPenActivationRange;
            }
            return m_TriggerActivationRange;
        }

        //Enable or disable tracking
        /// <summary>
        /// 启用或禁用跟踪
        /// </summary>
        /// <param name="enabled"></param>
        public void EnablePoseTracking(bool enabled)
        {
            foreach (var comp in m_TrackingComponents)
            {
                comp.enabled = enabled;
            }
        }
    }

}  // namespace TiltBrush
