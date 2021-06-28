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
using System.Collections.Generic;
using System.Linq;

namespace TiltBrush
{
    using SketchCommands = InputManager.SketchCommands;

    /// <summary>
    /// Handling controller input calls 处理控制器输入调用
    /// </summary>
    public abstract class ControllerInfo
    {
        // The various inputs available on VR controlllers. //VR控制器上提供的各种输入。

        // The invalid position (0,0) is excluded from all the pad buttons,
        // except for "Any".
        //无效位置（0,0）从所有键盘按钮中排除，              //除了“任何”。
        protected static bool IsInPosition(Vector2 padOrStickPos, VrInput input)
        {
            switch (input)
            {
                case VrInput.Button01 /*Half_Left*/:
                    return padOrStickPos.x < 0.0f;
                case VrInput.Button02 /*Half_Right*/:
                    return padOrStickPos.x > 0.0f;
                case VrInput.Button05 /*Quad_Up*/:
                    return padOrStickPos.y > 0.0f && (Mathf.Abs(padOrStickPos.y) > Mathf.Abs(padOrStickPos.x));
                case VrInput.Button06 /*Quad_Down*/:
                    return padOrStickPos.y < 0.0f && (Mathf.Abs(padOrStickPos.y) > Mathf.Abs(padOrStickPos.x));

                // Quad Left/Right are unused, could enable if we need to support.
                // However, even up/down is only used in an experimental feature.

                case VrInput.Any:
                    return true;

                default:
                    // We rely on falling through to this in cases where we're not trying to limit input.
                    return true;
            }
        }

        // Instance

        // The Behavior is always on the root of the object, and its direct child is a ControllerGeometry
        // This value never changes.
        public BaseControllerBehavior Behavior { get { return m_Behavior; } }

        // The same as Behavior.Transform; it also never changes.
        public Transform Transform { get { return m_Transform; } }

        // The same as Behavior.Geometry. This may change if controllers get swapped.
        public ControllerGeometry Geometry { get { return Behavior.ControllerGeometry; } }

        // This value never changes.
        public ControllerTutorialScript Tutorial { get { return m_Tutorial; } }

        // This indicates whether the underlying tracked object is in a valid state to provide tracking
        // data.
        // ///这表示基础跟踪对象是否处于有效状态以提供跟踪数据。
        public abstract bool IsTrackedObjectValid { get; set; }

        public VrInput? LastHeldInput
        {
            get { return m_LastHeldInput; }
            set { m_LastHeldInput = value; }
        }

        // These are updated when new poses come in
        public Vector3 m_Position;
        public Vector3 m_Velocity;
        public Vector3 m_Acceleration;
        public bool m_WasTracked;

        public float m_TimeSinceHapticTrigger;
        public int m_HapticPulseCount;
        public float m_HapticTimer;
        public float m_HapticInterval;
        public float m_HapticPulseLength;


        private float ButtonTimerThreshold
        {
            get { return SketchSurfacePanel.m_Instance.ActiveTool.ButtonHoldDuration; }
        }

        private Dictionary<VrInput, Vector2> m_InputHoldTimers =
            new Dictionary<VrInput, Vector2>();
        private VrInput? m_LastHeldInput;
        private readonly Transform m_Transform;
        private readonly BaseControllerBehavior m_Behavior;
        private readonly ControllerTutorialScript m_Tutorial;

        public ControllerInfo(BaseControllerBehavior behavior)
        {
            m_Behavior = behavior;
            m_Transform = behavior.transform;
            m_Tutorial = behavior.GetComponent<ControllerTutorialScript>();

            m_Position = m_Transform.position;
            m_Velocity = Vector3.zero;
            m_Acceleration = Vector3.zero;
            m_WasTracked = false;
            m_HapticPulseCount = 0;
        }

        /// Show (or hide) all renderable objects under the controller.
        public void ShowController(bool bShow)
        {
            Renderer[] aRenderers = Transform.GetComponentsInChildren<Renderer>(includeInactive: true);
            for (int i = 0; i < aRenderers.Length; ++i)
            {
                aRenderers[i].enabled = bShow;
            }
        }

        public virtual void Update()
        {
            // Update haptic pulses and clicks
            if (m_HapticPulseCount > 0)
            {
                m_HapticTimer -= Time.deltaTime;
                if (m_HapticTimer <= 0.0f)
                {
                    m_HapticTimer = m_HapticInterval;
                    --m_HapticPulseCount;
                    TriggerControllerHaptics(m_HapticPulseLength);
                }
            }
        }

        public virtual void LateUpdate()
        {
            // TODO: Why is this done in LateUpdate rather than Update?
            m_TimeSinceHapticTrigger += Time.deltaTime;
        }

        // -------------------------------------------------------------------------------------------- //
        // Command/CommandDown support
        // -------------------------------------------------------------------------------------------- //
        // Maps commands to VR Inputs and returns true if the input is actively pressed

        /// Returns true if the current command is active (e.g. the button is being pressed).
        /// 将命令映射到VR输入，如果主动按下输入，则返回true
        /// 如果当前命令处于活动状态（例如按下按钮），则返回true。
        public bool GetCommand(SketchCommands rCommand)
        {
            switch (rCommand)
            {
                case SketchCommands.Activate:
                    return IsTrigger();
                case SketchCommands.AltActivate:
                    return IsTrigger();
                case SketchCommands.WandRotation:
                    return GetPadTouch() || GetThumbStickTouch();
                case SketchCommands.LockToController:
                    return GetControllerGrip();
                case SketchCommands.Scale:
                    return GetPadTouch() || GetThumbStickTouch();
                case SketchCommands.Panic:
                    return IsTrigger();
                case SketchCommands.MultiCamSelection:
                    return GetVrInput(VrInput.Button04 /*full-pad-button*/);
                case SketchCommands.MenuContextClick:
                    return GetVrInput(VrInput.Button04 /*full-pad-button*/);
                case SketchCommands.ShowPinCushion:
                    return GetVrInput(VrInput.Button03);
                case SketchCommands.DuplicateSelection:
                    return GetVrInput(VrInput.Button04);
                case SketchCommands.Undo:
                    return GetVrInput(VrInput.Button01 /*half_left*/);
                case SketchCommands.Redo:
                    return GetVrInput(VrInput.Button02 /*half_right*/);
            }

            return false;
        }

        /// Returns true if the current command *just* became active (rising-edge trigger). ///如果当前命令*刚刚*激活（上升沿触发器），则返回true。
        public bool GetCommandDown(SketchCommands rCommand)
        {
            switch (rCommand)
            {
                case SketchCommands.Activate:
                    return IsTriggerDown();
                case SketchCommands.RewindTimeline:
                    return GetVrInputDown(VrInput.Button06 /*quad_down*/);
                case SketchCommands.AdvanceTimeline:
                    return GetVrInputDown(VrInput.Button05 /*quad_up*/);
                case SketchCommands.TimelineHome:
                    return IsTrigger() && GetVrInputDown(VrInput.Button06 /*quad_down*/);
                case SketchCommands.TimelineEnd:
                    return IsTrigger() && GetVrInputDown(VrInput.Button05 /*quad_up*/);
                case SketchCommands.Reset:
                    return GetVrInputDown(VrInput.Button05 /*quad_up*/);
                case SketchCommands.Undo:
                    return GetVrInputDown(VrInput.Button01 /*half_left*/);
                case SketchCommands.Redo:
                    return GetVrInputDown(VrInput.Button02 /*half_right*/);
                case SketchCommands.Teleport:
                    return IsTriggerDown();
                case SketchCommands.ToggleDefaultTool:
                    return GetVrInputDown(VrInput.Button03 /*app button*/);
                case SketchCommands.MenuContextClick:
                    return GetVrInputDown(VrInput.Button04 /*full-pad-button*/);
                case SketchCommands.WorldTransformReset:
                    return GetVrInputDown(VrInput.Button04 /*full-pad-button*/);
                case SketchCommands.PinWidget:
                    return IsTriggerDown();
                case SketchCommands.DuplicateSelection:
                    return GetVrInputDown(VrInput.Button04);
                case SketchCommands.ToggleSelection:
                    return GetVrInputDown(VrInput.Button04);
            }
            return false;
        }

        // -------------------------------------------------------------------------------------------- //
        // Command/Input Held support
        // -------------------------------------------------------------------------------------------- //
        // Useful in scenarios where a button should be held for a timeout period, e.g. when sharing
        // videos or trashing gifs.
        //在按钮应保持一段超时时间的情况下非常有用，例如在共享视频或销毁gif时。
        /// Helper function to translate commands to VR Inputs. 帮助函数将命令转换为VR输入。
        public VrInput? GetCommandHoldInput(SketchCommands rCommand)
        {
            switch (rCommand)
            {
                case SketchCommands.Confirm:
                    return VrInput.Button01 /*half_left*/;
                case SketchCommands.Cancel:
                    return VrInput.Button02 /*half_right*/;
                case SketchCommands.Share:
                    return VrInput.Button04 /*full pad*/;
                case SketchCommands.Trash:
                    return VrInput.Button04 /*full pad*/;
                case SketchCommands.DuplicateSelection:
                    return VrInput.Button04 /*full pad*/;
            }

            return null;
        }

        /// Returns true if the given command was held for longer than the threshold.
        /// To get the current progress, see GetCommandHoldProgress.
        /// ///如果给定命令的保留时间超过阈值，则返回true。              ///要获取当前进度，请参阅GetCommandHoldProgress。
        public bool GetCommandHeld(SketchCommands rCommand)
        {
            VrInput? vrInput = GetCommandHoldInput(rCommand);
            if (!vrInput.HasValue)
            {
                return false;
            }
            return UpdateCommandHold(vrInput.Value);
        }

        /// Returns the last controller input that was (or is) held, else null.
        /// Note that this value is reset after a short timeout and is likely to return null unless the
        /// user is actively holding the input.
        public VrInput? GetLastHeldInput()
        {
            Vector2 value;
            if (!m_LastHeldInput.HasValue) { return null; }
            if (!m_InputHoldTimers.TryGetValue(m_LastHeldInput.Value, out value)) { return null; }
            return m_LastHeldInput.Value;
        }

        /// Returns the current progress in the range [0..1] for the currently held command.
        public float GetCommandHoldProgress()
        {
            Vector2 value;
            if (!m_LastHeldInput.HasValue) { return 0f; }
            if (!m_InputHoldTimers.TryGetValue(m_LastHeldInput.Value, out value)) { return 0; }
            return Mathf.Clamp01(value.y / ButtonTimerThreshold);
        }

        /// Returns true if the given button was held longer than the long-press threshold. ///如果给定按钮的按住时间超过长按阈值，则返回true。
        private bool UpdateCommandHold(VrInput input)
        {
            bool active = GetVrInput(input);

            if (!m_InputHoldTimers.ContainsKey(input))
            {
                m_InputHoldTimers.Add(input, Vector2.zero);
            }

            if (!active)
            {
                m_InputHoldTimers[input] = Vector2.zero;
                return false;
            }

            if (m_InputHoldTimers[input].y > ButtonTimerThreshold)
            {
                m_InputHoldTimers[input] = Vector2.zero;
                m_LastHeldInput = null;
                return true;
            }

            m_LastHeldInput = input;
            m_InputHoldTimers[input] += new Vector2(0.0f, Time.deltaTime);
            return false;
        }

        /// Returns the same value as GetVrInput(VrControllerInput.Trigger) //返回与GetVrInput相同的值(VrControllerInput.Trigger)
        public bool IsTrigger()
        {
            return GetVrInput(VrInput.Trigger);
        }

        /// Returns the same value as GetVrInputDown(VrControllerInput.Trigger)
        public bool IsTriggerDown()
        {
            return GetVrInputDown(VrInput.Trigger);
        }

        /// Returns the same value as GetVrInputTouch(VrControllerInput.Touchpad)
        public bool GetPadTouch()
        {
            return GetVrInputTouch(VrInput.Touchpad);
        }

        /// Returns the same value as GetVrInputTouch(VrControllerInput.Thumbstick)
        public bool GetThumbStickTouch()
        {
            return GetVrInputTouch(VrInput.Thumbstick);
        }

        /// Returns the same value as GetVrInput(VrControllerInput.Grip)///返回与GetVrInput相同的值(控制手柄)
        public bool GetControllerGrip()
        {
            return GetVrInput(VrInput.Grip);
        }


        // API for subclass


        // Returns the ratio of how much the trigger is being pulled. ///返回触发的比率。
        /// <summary>
        /// 返回触发的比率。
        /// </summary>
        /// <returns></returns>
        public abstract float GetTriggerRatio();

        // Not filtered
        public abstract Vector2 GetPadValue();

        // Not filtered
        public abstract Vector2 GetThumbStickValue();

        // Filtered
        public abstract Vector2 GetPadValueDelta();

        // Returns a minimally-processed value in [0, 1] representing the grip.
        // You may want to map this through the controller's usable grip range
        // in VrControls.GripActivationRange.
        /// <summary>
        /// 返回[0，1]中表示夹点的最小处理值。您可能希望将其映射到中控制器的可用夹点范围VrControls.GripActivationRange文件.
        /// </summary>
        /// <returns></returns>
        public abstract float GetGripValue();

        public abstract float GetTriggerValue();

        // TODO(XXX): This function is a misnomer.  For controllers with pad input, it returns a delta,
        // for controllers with stick input, it returns the current stick value.
        public abstract float GetScrollXDelta();

        // TODO(XXX): This function is a misnomer.  For controllers with pad input, it returns a delta,
        // for controllers with stick input, it returns the current stick value.
        public abstract float GetScrollYDelta();

        /// Returns true if the specified input is currently being activated.
        ///
        /// - For buttons, this means pressed far enough that the button is engaged, and may
        ///   involve some hysteresis to prevent bouncing.
        /// - For thumbsticks, this means pushed far enough that the value is outside the deadzone.
        /// - For touchpads, this means being touched at all.
        /// 如果当前正在激活指定的输入，则返回true。
        /// -对于按钮，这意味着按下足够远，使按钮接合，并可能涉及一些滞后，以防止反弹。
        /// -对于指杆，这意味着推得足够远，使值超出死区。
        /// -对于触摸板，这意味着完全可以触摸。
        public abstract bool GetVrInput(VrInput input);

        /// Returns true if the specified input has just been activated (rising-edge trigger).
        /// 如果指定的输入刚刚激活（上升边触发器），则返回true。
        public abstract bool GetVrInputDown(VrInput input);

        /// Returns true if the specified input is currently being touched, and if the controller
        /// supports it (currently: Oculus Touch, Knuckles).
        ///
        /// Because of hardware limitations, on supported controllers it is possible for this
        /// to return false when GetVrInput() returns true.
        ///
        /// Some implementations may not support touch but will synthesize a value based on
        /// whether GetVrInput() returns true. For these implementations you _are_ guaranteed
        /// that GetVrInput implies GetVrInputTouch.
        ///
        /// TODO: this function is hard to describe right now. We should clarify constraints. eg:
        /// - Implementations must return false if touch is not supported
        /// - or, implementations must return true if GetVrInput() returns true (despite capacitative
        ///   lag etc)
        public abstract bool GetVrInputTouch(VrInput input);

        /// Trigger a haptic pulse for the given duration.///在给定的时间内触发触觉脉冲。
        public abstract void TriggerControllerHaptics(float seconds);
    }
}  // namespace TiltBrush
