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
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ControllerName = TiltBrush.InputManager.ControllerName;

namespace TiltBrush
{

    //TODO: Separate basic pointer management (e.g. enumeration, global operations)
    //from higher-level symmetry code.
    //TODO:单独的基本指针管理（例如枚举、全局操作）              
    //来自更高级的对称码。
    public class PointerManager : MonoBehaviour
    {
        /// <summary>
        /// PointerManager的单例
        /// </summary>
        static public PointerManager m_Instance;

        const float STRAIGHTEDGE_PRESSURE = 1f;
        const int STRAIGHTEDGE_DRAWIN_FRAMES = 16;
        const int DEBUG_MULTIPLE_NUM_POINTERS = 3;
        const string PLAYER_PREFS_POINTER_ANGLE_OLD = "Pointer_Angle";
        const string PLAYER_PREFS_POINTER_ANGLE = "Pointer_Angle2";

        // ---- Public types

        /// <summary>
        /// 对称模式
        /// </summary>
        public enum SymmetryMode
        {
            None,
            SinglePlane,
            FourAroundY,
            DebugMultiple,
        }

        // Modifying this struct has implications for binary compatibility.
        // The layout should match the most commonly-seen layout in the binary file.
        // See SketchMemoryScript.ReadMemory.
        //修改此结构会影响二进制兼容性。
        //布局应该与二进制文件中最常见的布局匹配。
        //看到了吗SketchMemoryScript.ReadMemory.
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ControlPoint
        {
            public Vector3 m_Pos;
            public Quaternion m_Orient;

            public const uint EXTENSIONS = (uint)(
                SketchWriter.ControlPointExtension.Pressure |
                SketchWriter.ControlPointExtension.Timestamp);
            public float m_Pressure;
            public uint m_TimestampMs;  // CurrentSketchTime of creation, in milliseconds//当前创建时间（毫秒）
        }

        // TODO: all this should be stored in the PointerScript instead of kept alongside
        //TODO:所有这些都应该存储在PointerScript中，而不是放在一起
        protected class PointerData
        {
            public PointerScript m_Script;
            // The start of a straightedge stroke.
            //直尺划水的开始。
            public TrTransform m_StraightEdgeXf_CS;
            public bool m_UiEnabled;
        }

        // ---- Private types

        /// <summary>
        ///  线条创建状态 
        /// </summary>
        private enum LineCreationState
        {
            // Not drawing a straightedge line.
            //不画直线。
            WaitingForInput,

            // Have first endpoint but not second endpoint.
            //具有第一个端点，但不具有第二个端点。
            RecordingInput,

            // Have both endpoints; drawing the line over multiple frames.
            // Used for brushes that use straightedge proxies, usually because they
            // need to be drawn over time (like particles)
            //具有两个端点；在多个帧上绘制线。
            //用于使用直尺代理的刷子，通常是因为
            //需要随时间绘制（如粒子）
            ProcessingStraightEdge,
        }

        // ---- Private inspector data

        /// <summary>
        /// 最大指针数
        /// </summary>
        [SerializeField] private int m_MaxPointers = 1;

        /// <summary>
        /// 主指针预制件
        /// </summary>
        [SerializeField] private GameObject m_MainPointerPrefab;

        /// <summary>
        /// 辅助指针预制件
        /// </summary>
        [SerializeField] private GameObject m_AuxPointerPrefab;

        [SerializeField] private float m_DefaultPointerAngle = 25.0f;
        [SerializeField] private bool m_DebugViewControlPoints = false;
        [SerializeField] private StraightEdgeGuideScript m_StraightEdgeGuide;
        [SerializeField] private BrushDescriptor m_StraightEdgeProxyBrush;

        [SerializeField] private Transform m_SymmetryWidget;

        [SerializeField] private Vector3 m_SymmetryDebugMultipleOffset = new Vector3(2, 0, 2);
        [SerializeField] private float m_SymmetryPointerStencilBoost = 0.001f;

        [SerializeField] private float m_GestureMinCircleSize;
        [SerializeField] private float m_GestureBeginDist;
        [SerializeField] private float m_GestureCloseLoopDist;
        [SerializeField] private float m_GestureStepDist;
        [SerializeField] private float m_GestureMaxAngle;

        // ---- Private member data

        /// <summary>
        /// //活动指针数
        /// </summary>
        private int m_NumActivePointers = 1;

        private bool m_PointersRenderingRequested;//请求的指针呈现

        /// <summary>
        /// 指针呈现活动状态
        /// </summary>
        private bool m_PointersRenderingActive;

        /// <summary>
        /// 指针在控制器丢失时隐藏
        /// </summary>
        private bool m_PointersHideOnControllerLoss;

        /// <summary>
        /// 自由绘制指针角度
        /// </summary>
        private float m_FreePaintPointerAngle;

        /// <summary>
        /// 当前线条创建状态
        /// </summary>
        private LineCreationState m_CurrentLineCreationState;

        private bool m_LineEnabled = false; //线条已启用
        private int m_EatLineEnabledInputFrames;//Eat线启用的输入帧

        /// This array is horrible. It is sort-of a preallocated pool of pointers,
        /// but different ranges are used for different purposes, and the ranges overlap.
        /// 这个阵法太可怕了。它有点像一个预先分配的指针池，但是不同的范围用于不同的目的，并且范围重叠。
        ///   0       Brush pointer                      画笔指针
        ///   1       2-way symmetry for Brush pointer   画笔指针的双向对称性
        ///   1-3     4-way symmetry for Brush pointer   画笔指针的4向对称性
        ///   2-N     (where 2 == NumUserPointers) Playback for timeline-edit sketches  其中2==NumUserPointers）播放时间轴编辑草图
        ///
        /// The only reason we don't have a ton of bugs stemming from systems stomping
        /// over each others' pointers is that we prevent those systems from being
        /// active simultaneously. eg, 4-way symmetry is not allowed during timeline edit mode;
        /// floating-panel mode doesn't actually _use_ the Wand's pointer, etc.
        ///唯一的原因是我们防止了那些系统同时处于活动状态。例如，在时间轴编辑模式下不允许四向对称；浮动面板模式实际上不使用魔杖的指针，等等。
        private PointerData[] m_Pointers;

        /// <summary>
        /// 在播放模式下
        /// </summary>
        private bool m_InPlaybackMode;

        /// <summary>
        /// 主指针数据
        /// </summary>
        private PointerData m_MainPointerData;

        /// <summary>
        /// 存储的笔刷信息
        /// </summary>
        struct StoredBrushInfo
        {
            public BrushDescriptor brush;//笔刷
            public float size01;//尺寸
            public Color color;//颜色
        }
        private StoredBrushInfo? m_StoredBrushInfo;

        //直边已启用
        private bool m_StraightEdgeEnabled;  // whether the mode is enabled
                                             // Brushes which return true for NeedsStraightEdgeProxy() use a proxy brush when displaying the
                                             // initial straight edge and redraw the line with the real brush at the end. This specifies
                                             // whether that proxy is currently active:
                                             //模式是否启用
                                             //对于NeedsStraightEdgeProxy（）返回true的笔刷在显示
                                             //初始直边和重画线与真正的刷子在结束。这说明
                                             //该代理当前是否处于活动状态：

        /// <summary>
        /// 代理当前是否处于活动状态
        /// </summary>
        private bool m_StraightEdgeProxyActive;

        private CircleGesture m_StraightEdgeGesture;//直尺手势
        private List<ControlPoint> m_StraightEdgeControlPoints_CS;//直尺控制点
        private int m_StraightEdgeControlPointIndex;//直边控制点索引

        private SymmetryMode m_CurrentSymmetryMode;//当前对称模式
        private SymmetryWidget m_SymmetryWidgetScript;//对称小部件脚本
        private bool m_UseSymmetryWidget = false;//使用对称小部件

        // These variables are legacy for supporting z-fighting control on the sketch surface
        // panel in monoscopic mode.
        //这些变量是用于支持草图曲面上的z-fighting控制的遗留变量
        //面板处于单视模式。
        private float m_SketchSurfaceLineDepthVarianceBase = 0.0001f;//草图表面线深度差异基准
        private float m_SketchSurfaceLineDepthVariance = 0.01f;//草图表面线深度变化
        private float m_SketchSurfaceLineDepthIncrement = 0.0001f;//草图表面线深度增量
        private float m_SketchSurfaceLineDepth;//草图表面线深度
        private bool m_SketchSurfaceLineWasEnabled;//草图面线已启用


        // ---- events
        /// <summary>
        /// 主指针笔刷更改
        /// </summary>
        public event Action<TiltBrush.BrushDescriptor> OnMainPointerBrushChange
        {
            add { m_MainPointerData.m_Script.OnBrushChange += value; }
            remove { m_MainPointerData.m_Script.OnBrushChange -= value; }
        }

        /// <summary>
        ///  指针颜色更改
        /// </summary>
        public event Action OnPointerColorChange = delegate { };

        // ---- public properties


        /// <summary>
        /// 主指针数据的PointerScript脚本
        /// </summary>
        public PointerScript MainPointer
        {
            get { return m_MainPointerData.m_Script; }
        }

        /// <summary>
        ///  指针颜色属性（返回或者设置属性）
        /// </summary>
        public Color PointerColor
        {
            get { return m_MainPointerData.m_Script.GetCurrentColor(); }
            set
            {
                for (int i = 0; i < m_NumActivePointers; ++i)
                {
                    m_Pointers[i].m_Script.SetColor(value);
                }
                OnPointerColorChange();
            }
        }

        /// <summary>
        /// 设置指针压力
        /// </summary>
        public float PointerPressure
        {
            set
            {
                for (int i = 0; i < m_NumActivePointers; ++i)
                {
                    m_Pointers[i].m_Script.SetPressure(value);
                }
            }
        }

        /// <summary>
        /// 指示笔刷尺寸是否显示
        /// </summary>
        public bool IndicateBrushSize
        {
            set
            {
                for (int i = 0; i < m_NumActivePointers; ++i)
                {
                    m_Pointers[i].m_Script.ShowSizeIndicator(value);
                }
            }
        }


        /// The number of pointers available with GetTransientPointer()
        /// GetTransientPointer（）可用的指针数
        public int NumTransientPointers { get { return m_Pointers.Length - NumUserPointers; } }

        /// Number of pointers reserved for user (including symmetry)
        /// TODO: handle more intelligently.  Depends on user's access to e.g. 4-way symmetry.
        /// 为用户保留的指针数（包括对称）
        /// TODO:更智能地处理。取决于用户的访问权限，例如4路对称。
        private int NumUserPointers { get { return 2; } }

        /// <summary>
        /// 返回或设置当前对称模式
        /// </summary>
        public SymmetryMode CurrentSymmetryMode
        {
            set { SetSymmetryMode(value); }
            get { return m_CurrentSymmetryMode; }
        }

        /// Returns null if the mirror is not active//如果镜像未激活，则返回null
        public Plane? SymmetryPlane_RS => (m_CurrentSymmetryMode == SymmetryMode.SinglePlane)
            ? (Plane?)m_SymmetryWidgetScript.ReflectionPlane
            : null;


        /// <summary>
        /// 对称模式是否已启用
        /// </summary>
        public bool SymmetryModeEnabled
        {
            get { return m_CurrentSymmetryMode != SymmetryMode.None; }
        }


        /// <summary>
        /// 来自镜像的对称小部件
        /// </summary>
        /// <param name="data"></param>
        public void SymmetryWidgetFromMirror(Mirror data)
        {
            m_SymmetryWidgetScript.FromMirror(data);
        }

        /// <summary>
        /// 要镜像的对称小部件
        /// </summary>
        /// <returns></returns>
        public Mirror SymmetryWidgetToMirror()
        {
            return m_SymmetryWidgetScript.ToMirror();
        }


        /// <summary>
        /// 返回直边导板
        /// </summary>
        public StraightEdgeGuideScript StraightEdgeGuide
        {
            get { return m_StraightEdgeGuide; }
        }

        /// <summary>
        /// 直边模式是否已启用
        /// </summary>
        public bool StraightEdgeModeEnabled
        {
            get { return m_StraightEdgeEnabled; }
            set { m_StraightEdgeEnabled = value; }
        }


        /// <summary>
        /// 直边导向线是否为直线
        /// </summary>
        public bool StraightEdgeGuideIsLine
        {
            get { return StraightEdgeGuide.CurrentShape == StraightEdgeGuideScript.Shape.Line; }
        }


        /// <summary>
        /// 自由绘制指针角度
        /// </summary>
        public float FreePaintPointerAngle
        {
            get { return m_FreePaintPointerAngle; }
            set
            {
                m_FreePaintPointerAngle = value;
                PlayerPrefs.SetFloat(PLAYER_PREFS_POINTER_ANGLE, m_FreePaintPointerAngle);
            }
        }


        /// <summary>
        /// 清除玩家偏好
        /// </summary>
        static public void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteKey(PLAYER_PREFS_POINTER_ANGLE_OLD);
            PlayerPrefs.DeleteKey(PLAYER_PREFS_POINTER_ANGLE);
        }

        // ---- accessors


        /// <summary>
        /// 获取指针
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PointerScript GetPointer(ControllerName name)
        {
            return GetPointerData(name).m_Script;
        }

        // Return a pointer suitable for transient use (like for playback)
        // Guaranteed to be different from any non-null return value of GetPointer(ControllerName)
        // Raise exception if not enough pointers
        //返回适合临时使用的指针（如用于播放）
        //保证与GetPointer（ControllerName）的任何非空返回值不同
        //如果没有足够的指针，则引发异常
        public PointerScript GetTransientPointer(int i)
        {
            return m_Pointers[NumUserPointers + i].m_Script;
        }

        /// The brush size, using "normalized" values in the range [0,1].
        /// Guaranteed to be in [0,1].
        /// 笔刷大小，使用范围[0,1]中的“标准化”值。           
        /// 保证在[0,1]内
        public float GetPointerBrushSize01(InputManager.ControllerName controller)
        {
            return Mathf.Clamp01(GetPointer(controller).BrushSize01);
        }


        /// <summary>
        /// 直边代理是否处于活动状态
        /// </summary>
        /// <returns></returns>
        public bool IsStraightEdgeProxyActive()
        {
            return m_StraightEdgeProxyActive;
        }

        /// <summary>
        /// 主指针正在创建笔划吗?
        /// </summary>
        /// <returns></returns>
        public bool IsMainPointerCreatingStroke()
        {
            return m_MainPointerData.m_Script.IsCreatingStroke();
        }

        /// <summary>
        /// 主指针正在处理线吗？
        /// </summary>
        /// <returns></returns>
        public bool IsMainPointerProcessingLine()
        {
            return m_CurrentLineCreationState == LineCreationState.ProcessingStraightEdge;
        }

        /// <summary>
        /// 设置为播放模式
        /// </summary>
        /// <param name="bInPlaybackMode"></param>
        public void SetInPlaybackMode(bool bInPlaybackMode)
        {
            m_InPlaybackMode = bInPlaybackMode;
        }


        public void EatLineEnabledInput()
        {
            m_EatLineEnabledInputFrames = 2;
        }

        /// Causes pointer manager to begin or end a stroke; takes effect next frame.
        /// 使指针管理器开始或结束笔划；在下一帧生效。
        public void EnableLine(bool bEnable)
        {
            // If we've been requested to eat input, discard any valid input until we've received
            //  some invalid input.
            //如果我们被要求吃输入，丢弃任何有效的输入，直到我们收到一些无效的输入。
            if (m_EatLineEnabledInputFrames > 0)
            {
                if (!bEnable)
                {
                    --m_EatLineEnabledInputFrames;
                }
                m_LineEnabled = false;
            }
            else
            {
                m_LineEnabled = bEnable;
            }
        }

        /// <summary>
        /// 是否启用线条
        /// </summary>
        /// <returns></returns>
        public bool IsLineEnabled()
        {
            return m_LineEnabled;
        }

        /// <summary>
        /// 使用对称小部件
        /// </summary>
        /// <param name="bUse"></param>
        public void UseSymmetryWidget(bool bUse)
        {
            m_UseSymmetryWidget = bUse;
        }



        // ---- Unity events

        void Awake()
        {

            m_Instance = this;

            Debug.Assert(m_MaxPointers > 0);
            m_Pointers = new PointerData[m_MaxPointers];

            for (int i = 0; i < m_Pointers.Length; ++i)
            {
                //set our main pointer as the zero index//将主指针设置为零索引
                bool bMain = (i == 0);
                var data = new PointerData();
                GameObject obj = (GameObject)Instantiate(bMain ? m_MainPointerPrefab : m_AuxPointerPrefab);

                //zby加的
                obj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                obj.transform.localPosition = new Vector3(1000, 1000, 1000);

                obj.transform.parent = transform;
                data.m_Script = obj.GetComponent<PointerScript>();
                data.m_Script.EnableDebugViewControlPoints(bMain && m_DebugViewControlPoints);
                data.m_Script.ChildIndex = i;
                data.m_UiEnabled = bMain;
                m_Pointers[i] = data;
                if (bMain)
                {
                    m_MainPointerData = data;
                }
            }

            m_CurrentLineCreationState = LineCreationState.WaitingForInput;
            m_StraightEdgeProxyActive = false;
            m_StraightEdgeGesture = new CircleGesture();

            if (m_SymmetryWidget)
            {
                m_SymmetryWidgetScript = m_SymmetryWidget.GetComponent<SymmetryWidget>();
            }

            //initialize rendering requests to default to hiding everything
            //将呈现请求初始化为默认值以隐藏所有内容
            m_PointersRenderingRequested = false;
            m_PointersRenderingActive = true;

            m_FreePaintPointerAngle =
                PlayerPrefs.GetFloat(PLAYER_PREFS_POINTER_ANGLE, m_DefaultPointerAngle);
        }

        void Start()
        {
            SetSymmetryMode(SymmetryMode.None, false);

            m_PointersHideOnControllerLoss = App.VrSdk.GetControllerDof() == VrSdk.DoF.Six;

            // Migrate setting, but only if it's non-zero//迁移设置，但仅当其为非零时
            if (PlayerPrefs.HasKey(PLAYER_PREFS_POINTER_ANGLE_OLD))
            {
                var prev = PlayerPrefs.GetFloat(PLAYER_PREFS_POINTER_ANGLE_OLD);
                PlayerPrefs.DeleteKey(PLAYER_PREFS_POINTER_ANGLE_OLD);
                if (prev != 0)
                {
                    PlayerPrefs.SetFloat(PLAYER_PREFS_POINTER_ANGLE, prev);
                }
            }

            RefreshFreePaintPointerAngle();
        }

        void Update()
        {
            if (m_StraightEdgeEnabled && m_CurrentLineCreationState == LineCreationState.RecordingInput)
            {
                m_StraightEdgeGuide.SnapEnabled =
                    InputManager.Brush.GetCommand(InputManager.SketchCommands.MenuContextClick) &&
                    SketchControlsScript.m_Instance.ShouldRespondToPadInput(InputManager.ControllerName.Num);
                m_StraightEdgeGuide.UpdateTarget(MainPointer.transform.position);
            }

            if (SymmetryModeEnabled)
            {
                //if we're not showing the symmetry widget, keep it locked where needed
                //如果我们没有显示对称小部件，请将其锁定在需要的地方
                if (!m_UseSymmetryWidget)
                {
                    if (m_CurrentSymmetryMode == SymmetryMode.SinglePlane)
                    {
                        m_SymmetryWidget.position = Vector3.zero;
                        m_SymmetryWidget.rotation = Quaternion.identity;
                    }
                    else if (m_CurrentSymmetryMode == SymmetryMode.FourAroundY)
                    {
                        m_SymmetryWidget.position = SketchSurfacePanel.m_Instance.transform.position;
                        m_SymmetryWidget.rotation = SketchSurfacePanel.m_Instance.transform.rotation;
                    }
                }
            }

            //update pointers//更新指针
            if (!m_InPlaybackMode && !PanelManager.m_Instance.IntroSketchbookMode)
            {
                // This is special code to prevent z-fighting in monoscopic mode.//这是一个特殊的代码，以防止在单镜模式z-战斗。
                float fPointerLift = 0.0f;
                if (App.VrSdk.GetHmdDof() == VrSdk.DoF.None)
                {
                    if (m_LineEnabled)
                    {
                        // If we just became enabled, randomize our pointer lift start point.
                        //如果我们刚刚被启用，随机化我们的指针提升起点。
                        if (!m_SketchSurfaceLineWasEnabled)
                        {
                            m_SketchSurfaceLineDepth = m_SketchSurfaceLineDepthVarianceBase +
                                UnityEngine.Random.Range(0.0f, m_SketchSurfaceLineDepthVariance);
                        }

                        // While enabled, add depth as a function of distance moved.
                        //启用时，根据移动的距离添加深度。
                        m_SketchSurfaceLineDepth += m_MainPointerData.m_Script.GetMovementDelta() *
                            m_SketchSurfaceLineDepthIncrement;
                    }
                    else
                    {
                        m_SketchSurfaceLineDepth = m_SketchSurfaceLineDepthVarianceBase;
                    }

                    fPointerLift = m_SketchSurfaceLineDepth;
                    m_SketchSurfaceLineWasEnabled = m_LineEnabled;
                }


                // Update each pointer's line depth with the monoscopic sketch surface pointer lift.
                //使用单透视草图曲面指针提升更新每个指针的线深度。
                for (int i = 0; i < m_NumActivePointers; ++i)
                {
                    m_Pointers[i].m_Script.MonoscopicLineDepth = fPointerLift;

                    m_Pointers[i].m_Script.UpdatePointer();
                }

            }

            //update pointer rendering according to state//根据状态更新指针呈现
            if (!m_PointersHideOnControllerLoss || InputManager.Brush.IsTrackedObjectValid)
            {

                //show pointers according to requested visibility//根据请求的可见性显示指针
                SetPointersRenderingEnabled(m_PointersRenderingRequested);
            }
            else
            {

                //turn off pointers//关闭指针
                SetPointersRenderingEnabled(false);
                DisablePointerPreviewLine();
            }
        }

        /// <summary>
        ///  存储笔刷信息
        /// </summary>
        public void StoreBrushInfo()
        {
            m_StoredBrushInfo = new StoredBrushInfo
            {
                brush = MainPointer.CurrentBrush,
                size01 = MainPointer.BrushSize01,
                color = PointerColor,
            };
        }

        /// <summary>
        /// 恢复笔刷信息
        /// </summary>
        public void RestoreBrushInfo()
        {
            if (m_StoredBrushInfo == null) { return; }
            var info = m_StoredBrushInfo.Value;
            SetBrushForAllPointers(info.brush);
            SetAllPointersBrushSize01(info.size01);
            MarkAllBrushSizeUsed();
            PointerColor = info.color;
        }

        /// <summary>
        /// 刷新自由绘制指针角度
        /// </summary>
        public void RefreshFreePaintPointerAngle()
        {
            InputManager.m_Instance.SetControllersAttachAngle(m_FreePaintPointerAngle);
        }

        /// <summary>
        /// 启用设置指针渲染
        /// </summary>
        /// <param name="bEnable"></param>
        void SetPointersRenderingEnabled(bool bEnable)
        {
            if (m_PointersRenderingActive != bEnable)
            {
                foreach (PointerData rData in m_Pointers)
                {
                    rData.m_Script.EnableRendering(bEnable && rData.m_UiEnabled);
                }
                m_PointersRenderingActive = bEnable;
            }
        }

        /// <summary>
        /// 启用指针笔划生成
        /// </summary>
        /// <param name="bActivate"></param>
        public void EnablePointerStrokeGeneration(bool bActivate)
        {
            foreach (PointerData rData in m_Pointers)
            {
                // Note that pointers with m_UiEnabled=false may still be employed during scene playback.
                //请注意，在场景回放期间，仍然可以使用m_UiEnabled=false的指针。
                rData.m_Script.gameObject.SetActive(bActivate);
            }
        }

        /// <summary>
        /// 启用指针灯
        /// </summary>
        /// <param name="bEnable"></param>
        public void EnablePointerLights(bool bEnable)
        {
            foreach (PointerData rData in m_Pointers)
            {
                rData.m_Script.AllowPreviewLight(bEnable && rData.m_UiEnabled);
            }
        }

        /// <summary>
        /// 请求指针呈现
        /// </summary>
        /// <param name="bEnable"></param>
        public void RequestPointerRendering(bool bEnable)
        {
            m_PointersRenderingRequested = bEnable;
        }


        /// <summary>
        /// 设置音频播放指针
        /// </summary>
        public void SetPointersAudioForPlayback()
        {
            foreach (PointerData rData in m_Pointers)
            {
                rData.m_Script.SetAudioClipForPlayback();
            }
        }


        /// <summary>
        /// 获取指针数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private PointerData GetPointerData(ControllerName name)
        {
            // TODO: replace with something better that handles multiple controllers
            //TODO:替换为处理多个控制器的更好的方法
            switch (name)
            {
                case ControllerName.Brush:
                    return m_Pointers[0];
                default:
                    Debug.AssertFormat(false, "No pointer for controller {0}", name);
                    return null;
            }
        }

        /// <summary>
        /// 允许指针预览线
        /// </summary>
        /// <param name="bAllow"></param>
        public void AllowPointerPreviewLine(bool bAllow)
        {
            for (int i = 0; i < m_NumActivePointers; ++i)
            {
                m_Pointers[i].m_Script.AllowPreviewLine(bAllow);
            }
        }

        /// <summary>
        /// 禁用指针预览线
        /// </summary>
        public void DisablePointerPreviewLine()
        {
            for (int i = 0; i < m_NumActivePointers; ++i)
            {
                m_Pointers[i].m_Script.DisablePreviewLine();
            }
        }

        /// <summary>
        /// 重置指针音频
        /// </summary>
        public void ResetPointerAudio()
        {
            for (int i = 0; i < m_NumActivePointers; ++i)
            {
                m_Pointers[i].m_Script.ResetAudio();
            }
        }

        /// <summary>
        /// 设置指针预览线延迟计时器
        /// </summary>
        public void SetPointerPreviewLineDelayTimer()
        {
            for (int i = 0; i < m_NumActivePointers; ++i)
            {
                m_Pointers[i].m_Script.SetPreviewLineDelayTimer();
            }
        }

        /// <summary>
        /// 显式设置所有指针笔刷大小
        /// </summary>
        /// <param name="fSize"></param>
        public void ExplicitlySetAllPointersBrushSize(float fSize)
        {
            for (int i = 0; i < m_NumActivePointers; ++i)
            {
                m_Pointers[i].m_Script.BrushSizeAbsolute = fSize;
            }
        }

        /// <summary>
        /// 标记所有使用的刷子大小
        /// </summary>
        public void MarkAllBrushSizeUsed()
        {
            for (int i = 0; i < m_NumActivePointers; ++i)
            {
                m_Pointers[i].m_Script.MarkBrushSizeUsed();
            }
        }

        /// <summary>
        /// 设置所有指针笔刷大小01
        /// </summary>
        /// <param name="t"></param>
        public void SetAllPointersBrushSize01(float t)
        {
            for (int i = 0; i < m_NumActivePointers; ++i)
            {
                //m_Pointers[i].m_Script.BrushSize01 = t;//zby注释
                m_Pointers[i].m_Script.BrushSize01 = t / 50f;//zby增加
            }
        }

        /// <summary>
        /// 调整所有指针笔刷大小01
        /// </summary>
        /// <param name="dt"></param>
        public void AdjustAllPointersBrushSize01(float dt)
        {
            for (int i = 0; i < m_NumActivePointers; ++i)
            {
                // m_Pointers[i].m_Script.BrushSize01 += dt;//zby注释
                dt /= 50f;
                m_Pointers[i].m_Script.BrushSize01 += dt;//zby增加
            }
        }

        /// <summary>
        /// 为所有指针设置笔刷
        /// </summary>
        /// <param name="desc"></param>
        public void SetBrushForAllPointers(BrushDescriptor desc)
        {
            for (int i = 0; i < m_NumActivePointers; ++i)
            {
                m_Pointers[i].m_Script.SetBrush(desc);
            }
        }

        /// <summary>
        /// 设置指针转换
        /// </summary>
        /// <param name="name"></param>
        /// <param name="v"></param>
        /// <param name="q"></param>
        public void SetPointerTransform(ControllerName name, Vector3 v, Quaternion q)
        {
            Transform pointer = GetPointer(name).transform;
            pointer.position = v;
            pointer.rotation = q;
            UpdateSymmetryPointerTransforms();
        }

        /// <summary>
        /// 设置主指针位置
        /// </summary>
        /// <param name="vPos"></param>
        public void SetMainPointerPosition(Vector3 vPos)
        {
            m_MainPointerData.m_Script.transform.position = vPos;
            UpdateSymmetryPointerTransforms();
        }

        /// <summary>
        ///  设置主指针旋转
        /// </summary>
        /// <param name="qRot"></param>
        public void SetMainPointerRotation(Quaternion qRot)
        {
            m_MainPointerData.m_Script.transform.rotation = qRot;
            UpdateSymmetryPointerTransforms();
        }

        /// <summary>
        /// 设置主指针朝向
        /// </summary>
        /// <param name="vForward"></param>
        public void SetMainPointerForward(Vector3 vForward)
        {
            m_MainPointerData.m_Script.transform.forward = vForward;
            UpdateSymmetryPointerTransforms();
        }

        /// <summary>
        /// 设置对称模式
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="recordCommand"></param>
        public void SetSymmetryMode(SymmetryMode mode, bool recordCommand = true)
        {
            int active = m_NumActivePointers;
            switch (mode)
            {
                case SymmetryMode.None: active = 1; break;
                case SymmetryMode.SinglePlane: active = 2; break;
                case SymmetryMode.FourAroundY: active = 4; break;
                case SymmetryMode.DebugMultiple: active = DEBUG_MULTIPLE_NUM_POINTERS; break;
            }
            int maxUserPointers = m_Pointers.Length;
            if (active > maxUserPointers)
            {
                throw new System.ArgumentException("Not enough pointers for mode");
            }

            m_CurrentSymmetryMode = mode;
            m_NumActivePointers = active;
            m_SymmetryWidgetScript.SetMode(m_CurrentSymmetryMode);
            m_SymmetryWidgetScript.Show(m_UseSymmetryWidget && SymmetryModeEnabled);
            if (recordCommand)
            {
                SketchMemoryScript.m_Instance.RecordCommand(
                  new SymmetryWidgetVisibleCommand(m_SymmetryWidgetScript));
            }

            for (int i = 1; i < m_Pointers.Length; ++i)
            {
                var pointer = m_Pointers[i];
                bool enabled = i < m_NumActivePointers;
                pointer.m_UiEnabled = enabled;
                pointer.m_Script.gameObject.SetActive(enabled);
                pointer.m_Script.EnableRendering(m_PointersRenderingActive && enabled);
                if (enabled)
                {
                    pointer.m_Script.CopyInternals(m_Pointers[0].m_Script);
                }
            }

            App.Switchboard.TriggerMirrorVisibilityChanged();
        }

        /// <summary>
        /// 将对称重设为原点
        /// </summary>
        public void ResetSymmetryToHome()
        {
            m_SymmetryWidgetScript.ResetToHome();
        }

        /// <summary>
        /// 给用户带来对称性
        /// </summary>
        public void BringSymmetryToUser()
        {
            m_SymmetryWidgetScript.BringToUser();
        }

        /// Given the position of a main pointer, find a corresponding symmetry position.
        /// Results are undefined unless you pass MainPointer or one of its
        /// dedicated symmetry pointers.
        /// ///给定主指针的位置，找到相应的对称位置。
        /// ///除非传递MainPointer或其 
        /// ///专用对称指针。
        public TrTransform GetSymmetryTransformFor(PointerScript pointer, TrTransform xfMain)
        {
            int child = pointer.ChildIndex;
            // "active pointers" is the number of pointers the symmetry widget is using,
            // including the main pointer.
            //“active pointers”是symmetry小部件使用的指针数，包括主指针。
            if (child == 0 || child >= m_NumActivePointers)
            {
                return xfMain;
            }

            // This needs to be kept in sync with UpdateSymmetryPointerTransforms
            //这需要与UpdateSymmetryPointerTransforms保持同步
            switch (m_CurrentSymmetryMode)
            {
                case SymmetryMode.SinglePlane:
                    {
                        return m_SymmetryWidgetScript.ReflectionPlane.ReflectPoseKeepHandedness(xfMain);
                    }

                case SymmetryMode.FourAroundY:
                    {
                        // aboutY is an operator that rotates worldspace objects N degrees around the widget's Y
                        TrTransform aboutY;
                        {
                            var xfWidget = TrTransform.FromTransform(m_SymmetryWidget);
                            float angle = (360f * child) / m_NumActivePointers;
                            aboutY = TrTransform.TR(Vector3.zero, Quaternion.AngleAxis(angle, Vector3.up));
                            // convert from widget-local coords to world coords
                            aboutY = aboutY.TransformBy(xfWidget);
                        }
                        return aboutY * xfMain;
                    }

                case SymmetryMode.DebugMultiple:
                    {
                        var xfLift = TrTransform.T(m_SymmetryDebugMultipleOffset * child);
                        return xfLift * xfMain;
                    }

                default:
                    return xfMain;
            }
        }

        /// <summary>
        /// 更新对称指针变换
        /// </summary>
        void UpdateSymmetryPointerTransforms()
        {
            switch (m_CurrentSymmetryMode)
            {
                case SymmetryMode.SinglePlane:
                    {
                        Plane plane = m_SymmetryWidgetScript.ReflectionPlane;
                        TrTransform xf0 = TrTransform.FromTransform(m_MainPointerData.m_Script.transform);
                        TrTransform xf1 = plane.ReflectPoseKeepHandedness(xf0);
                        xf1.ToTransform(m_Pointers[1].m_Script.transform);

                        // This is a hack.
                        // In the event that the user is painting on a plane stencil and that stencil is
                        // orthogonal to the symmetry plane, the main pointer and mirrored pointer will
                        // have the same depth and their strokes will overlap, causing z-fighting.
                        if (WidgetManager.m_Instance.ActiveStencil != null)
                        {
                            m_Pointers[1].m_Script.transform.position +=
                                m_Pointers[1].m_Script.transform.forward * m_SymmetryPointerStencilBoost;
                        }
                        break;
                    }

                case SymmetryMode.FourAroundY:
                    {
                        TrTransform pointer0 = TrTransform.FromTransform(m_MainPointerData.m_Script.transform);
                        // aboutY is an operator that rotates worldspace objects N degrees around the widget's Y
                        TrTransform aboutY;
                        {
                            var xfWidget = TrTransform.FromTransform(m_SymmetryWidget);
                            float angle = 360f / m_NumActivePointers;
                            aboutY = TrTransform.TR(Vector3.zero, Quaternion.AngleAxis(angle, Vector3.up));
                            // convert from widget-local coords to world coords
                            aboutY = xfWidget * aboutY * xfWidget.inverse;
                        }

                        TrTransform cur = TrTransform.identity;
                        for (int i = 1; i < m_NumActivePointers; ++i)
                        {
                            cur = aboutY * cur;   // stack another rotation on top
                            var tmp = (cur * pointer0); // Work around 2018.3.x Mono parse bug
                            tmp.ToTransform(m_Pointers[i].m_Script.transform);
                        }
                        break;
                    }

                case SymmetryMode.DebugMultiple:
                    {
                        var xf0 = m_Pointers[0].m_Script.transform;
                        for (int i = 1; i < m_NumActivePointers; ++i)
                        {
                            var xf = m_Pointers[i].m_Script.transform;
                            xf.position = xf0.position + m_SymmetryDebugMultipleOffset * i;
                            xf.rotation = xf0.rotation;
                        }
                        break;
                    }
            }
        }

        /// Called every frame while Activate is disallowed
        /// ///不允许在激活时调用每帧
        void OnDrawDisallowed()
        {
            InputManager.m_Instance.TriggerHaptics(InputManager.ControllerName.Brush, 0.1f);
        }

        /// <summary>
        /// 自由播放指针数
        /// </summary>
        /// <returns></returns>
        int NumFreePlaybackPointers()
        {
            // TODO: Plumb this info from ScenePlayback so it can emulate pointer usage e.g. while
            // keeping all strokes visible.
            //TODO:从ScenePlayback获取此信息，以便它可以模拟指针使用情况，例如保持所有笔划可见。
            int count = 0;
            for (int i = NumUserPointers; i < m_Pointers.Length; ++i)
            {
                if (!m_Pointers[i].m_Script.IsCreatingStroke())
                {
                    ++count;
                }
            }
            return count;
        }

        /// State-machine update function; always called once per frame.///状态机更新函数；总是每帧调用一次。
        public void UpdateLine()
        {
            bool playbackPointersAvailable = m_NumActivePointers <= NumFreePlaybackPointers();//m_NumActivePointers == 1


            switch (m_CurrentLineCreationState)
            {
                case LineCreationState.WaitingForInput:
                    if (m_LineEnabled)
                    {
                        if (playbackPointersAvailable)//被执行
                        {

                            Transition_WaitingForInput_RecordingInput();
                        }
                        else
                        {
                            OnDrawDisallowed();
                        }
                    }
                    break;

                // TODO: unique state for capturing straightedge 2nd point rather than overload RecordingInput
                //TODO:捕捉直尺第二点而不是过载记录输入的唯一状态
                case LineCreationState.RecordingInput:
                    if (m_LineEnabled)
                    {


                        if (playbackPointersAvailable) //playbackPointersAvailable == true
                        {

                            // Check straightedge gestures.//检查直截了当的手势。
                            if (m_StraightEdgeEnabled) //m_StraightEdgeEnabled == false
                            {
                                CheckGestures();
                            }

                            // check to see if any pointer's line needs to end
                            // TODO: equivalent check during ProcessingStraightEdge
                            //检查指针的线是否需要结束              
                            //TODO:处理TraightEdge期间的等效检查
                            bool bStartNewLine = false;
                            for (int i = 0; i < m_NumActivePointers; ++i)
                            {
                                bStartNewLine = bStartNewLine || m_Pointers[i].m_Script.ShouldCurrentLineEnd();
                            }


                            if (bStartNewLine && !m_StraightEdgeEnabled) //bStartNewLine == false
                            {
                                //if it has, stop this line and start anew//如果有，停止这条线，重新开始
                                FinalizeLine(isContinue: true);
                                InitiateLine(isContinue: true);
                            }
                        }

                        else if (!m_StraightEdgeEnabled) //m_StraightEdgeEnabled == false
                        {
                            OnDrawDisallowed();
                            Transition_RecordingInput_WaitingForInput();
                        }
                    }

                    else
                    {

                        // Transition to either ProcessingStraightEdge or WaitingForInput//转换为ProcessingStraighEdge或WaitingForInput
                        if (m_StraightEdgeProxyActive)//m_StraightEdgeProxyActive == false
                        {


                            if (playbackPointersAvailable)
                            {
                                List<ControlPoint> cps = MainPointer.GetControlPoints();
                                FinalizeLine(discard: true);
                                Transition_RecordingInput_ProcessingStraightEdge(cps);
                            }
                            else
                            {
                                OnDrawDisallowed();
                                // cancel the straight edge//取消直尺
                                m_StraightEdgeProxyActive = false;
                                m_StraightEdgeGuide.HideGuide();
                                m_CurrentLineCreationState = LineCreationState.WaitingForInput;
                            }
                        }
                        else
                        {
                            m_StraightEdgeGuide.HideGuide();
                            var stencil = WidgetManager.m_Instance.ActiveStencil;

                            if (stencil != null)  //stencil == null
                            {
                                stencil.AdjustLift(1);
                            }

                            Transition_RecordingInput_WaitingForInput();
                        }

                        // Eat up tool scale input for heavy grippers.//吃光重型夹持器的工具刻度输入。
                        SketchControlsScript.m_Instance.EatToolScaleInput();
                    }
                    break;

                case LineCreationState.ProcessingStraightEdge:

                    State_ProcessingStraightEdge(terminate: !playbackPointersAvailable);
                    break;
            }
        }

        /// <summary>
        /// 检查手势
        /// </summary>
        void CheckGestures()
        {
            m_StraightEdgeGesture.UpdateGesture(MainPointer.transform.position);
            if (m_StraightEdgeGesture.IsGestureComplete())
            {
                // If gesture succeeded, change the line creator.//如果手势成功，请更改线创建者。
                if (m_StraightEdgeGesture.DidGestureSucceed())
                {
                    FinalizeLine(discard: true);
                    StraightEdgeGuideScript.Shape nextShape = StraightEdgeGuide.CurrentShape;
                    switch (nextShape)
                    {
                        case StraightEdgeGuideScript.Shape.Line:
                            nextShape = StraightEdgeGuideScript.Shape.Circle; break;
                        case StraightEdgeGuideScript.Shape.Circle:
                            {
                                if (App.Config.IsMobileHardware)
                                {
                                    nextShape = StraightEdgeGuideScript.Shape.Line;
                                }
                                else
                                    nextShape = StraightEdgeGuideScript.Shape.Sphere;
                                {
                                }
                            }
                            break;
                        case StraightEdgeGuideScript.Shape.Sphere:
                            nextShape = StraightEdgeGuideScript.Shape.Line; break;
                    }

                    StraightEdgeGuide.SetTempShape(nextShape);
                    StraightEdgeGuide.ResolveTempShape();
                    InitiateLineAt(m_MainPointerData.m_StraightEdgeXf_CS);
                }

                m_StraightEdgeGesture.ResetGesture();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Transition_WaitingForInput_RecordingInput()
        {
            //Debug.Log("m_StraightEdgeEnabled的值为：" + m_StraightEdgeEnabled);



            //(当创建笔划时，m_StraightEdgeEnabled = false)；
            if (m_StraightEdgeEnabled)
            {
                StraightEdgeGuide.SetTempShape(StraightEdgeGuideScript.Shape.Line);
                StraightEdgeGuide.ResolveTempShape();
                m_StraightEdgeGesture.InitGesture(MainPointer.transform.position,
                    m_GestureMinCircleSize, m_GestureBeginDist, m_GestureCloseLoopDist,
                    m_GestureStepDist, m_GestureMaxAngle);
            }




            InitiateLine();
            m_CurrentLineCreationState = LineCreationState.RecordingInput;
            WidgetManager.m_Instance.WidgetsDormant = true;
        }

        private void Transition_RecordingInput_ProcessingStraightEdge(List<ControlPoint> cps)
        {
            Debug.Assert(m_StraightEdgeProxyActive);

            //create straight line//创建直线
            m_StraightEdgeProxyActive = false;
            m_StraightEdgeGuide.HideGuide();

            m_StraightEdgeControlPoints_CS = cps;
            m_StraightEdgeControlPointIndex = 0;

            // Reset pointer to first control point and init all active pointers.//将指针重置为第一个控制点并初始化所有活动指针。
            SetMainPointerPosition(Coords.CanvasPose * m_StraightEdgeControlPoints_CS[0].m_Pos);

            var canvas = App.Scene.ActiveCanvas;
            for (int i = 0; i < m_NumActivePointers; ++i)
            {
                var p = m_Pointers[i];
                TrTransform xf_CS = canvas.AsCanvas[p.m_Script.transform];

                p.m_Script.CreateNewLine(canvas, xf_CS, null);
                p.m_Script.SetPressure(STRAIGHTEDGE_PRESSURE);
                p.m_Script.SetControlPoint(xf_CS, isKeeper: true);
            }

            // Ensure that snap is disabled when we start the stroke.//确保在开始笔划时禁用捕捉。
            m_StraightEdgeGuide.ForceSnapDisabled();

            //do this operation over a series of frames//在一系列帧上执行此操作
            m_CurrentLineCreationState = LineCreationState.ProcessingStraightEdge;
        }

        private void Transition_RecordingInput_WaitingForInput()
        {
            // standard mode, just finalize our line and get ready for the next one//标准模式，完成我们的线路，准备下一个
            FinalizeLine();
            m_CurrentLineCreationState = LineCreationState.WaitingForInput;
        }

        private void State_ProcessingStraightEdge(bool terminate)
        {
            int cpPerFrame = Mathf.Max(
                m_StraightEdgeControlPoints_CS.Count / STRAIGHTEDGE_DRAWIN_FRAMES, 2);

            TrTransform xfCanvas = Coords.CanvasPose;
            for (int p = 0; p < cpPerFrame &&
                 m_StraightEdgeControlPointIndex < m_StraightEdgeControlPoints_CS.Count;
                 p++, m_StraightEdgeControlPointIndex++)
            {
                ControlPoint cp = m_StraightEdgeControlPoints_CS[m_StraightEdgeControlPointIndex];
                TrTransform xfPointer = xfCanvas * TrTransform.TR(cp.m_Pos, cp.m_Orient);
                SetMainPointerPosition(xfPointer.translation);
                SetMainPointerRotation(xfPointer.rotation);
                for (int i = 0; i < m_NumActivePointers; ++i)
                {
                    m_Pointers[i].m_Script.UpdateLineFromObject();
                }

                var stencil = WidgetManager.m_Instance.ActiveStencil;
                if (stencil != null)
                {
                    stencil.AdjustLift(1);
                }
            }

            // we reached the end!我们到了终点！
            if (terminate || m_StraightEdgeControlPointIndex >= m_StraightEdgeControlPoints_CS.Count)
            {
                FinalizeLine();
                m_CurrentLineCreationState = LineCreationState.WaitingForInput;
            }
        }

        // Only called during interactive creation.
        // isContinue is true if the line is the logical (if not physical) continuation
        // of a previous line -- ie, previous line ran out of verts and we transparently
        // stopped and started a new one.
        //仅在交互式创建期间调用。
        //如果该行是前一行的逻辑（如果不是物理）延续，则isContinue为真——即，前一行用完了顶点，我们透明地停止并开始了一个新的顶点。
        void InitiateLine(bool isContinue = false)
        {
            // Turn off the preview when we start drawing //开始绘图时关闭预览
            for (int i = 0; i < m_NumActivePointers; ++i)//此时m_NumActivePointers的数量为1
            {
                m_Pointers[i].m_Script.DisablePreviewLine();
                m_Pointers[i].m_Script.AllowPreviewLine(false);
            }



            //Debug.Log("m_NumActivePointers的数量为：" + m_NumActivePointers);
            //Debug.Log("m_StraightEdgeEnabled的值为：" + m_StraightEdgeEnabled);
            //(当创建笔划时，m_StraightEdgeEnabled = false)；
            if (m_StraightEdgeEnabled)
            {
                // This causes the line to be drawn with a proxy brush; and also to be
                // discarded and redrawn upon completion.
                //这将导致使用代理笔刷绘制线；并且完成后丢弃并重新绘制。
                m_StraightEdgeProxyActive = MainPointer.CurrentBrush.NeedsStraightEdgeProxy;
                // Turn on the straight edge and hold on to our start position//转直边，保持我们的起始位置
                m_StraightEdgeGuide.ShowGuide(MainPointer.transform.position);
                for (int i = 0; i < m_NumActivePointers; ++i)
                {
                    m_Pointers[i].m_StraightEdgeXf_CS = Coords.AsCanvas[m_Pointers[i].m_Script.transform];
                }
            }


            CanvasScript canvas = App.Scene.ActiveCanvas;//此时canvas对应的游戏对象为：MainCanvas
            //Debug.Log(canvas.gameObject.name);

            for (int i = 0; i < m_NumActivePointers; ++i)
            {
                PointerScript script = m_Pointers[i].m_Script;

                var xfPointer_CS = canvas.AsCanvas[script.transform];

                // Pass in parametric stroke creator.//传入参数化笔划生成器。
                ParametricStrokeCreator currentCreator = null;

                //(当创建笔划时，m_StraightEdgeEnabled = false)；
                if (m_StraightEdgeEnabled)
                {
                    switch (StraightEdgeGuide.CurrentShape)
                    {
                        case StraightEdgeGuideScript.Shape.Line:
                            currentCreator = new LineCreator(xfPointer_CS, flat: true);
                            break;
                        case StraightEdgeGuideScript.Shape.Circle:
                            currentCreator = new CircleCreator(xfPointer_CS);
                            break;
                        case StraightEdgeGuideScript.Shape.Sphere:
                            currentCreator = new SphereCreator(xfPointer_CS, script.BrushSizeAbsolute,
                              canvas.transform.GetUniformScale());
                            break;
                    }
                }




                //Debug.Log("m_StraightEdgeProxyActive的值为" + m_StraightEdgeProxyActive);
                //Debug.Log("m_StraightEdgeProxyBrush的值为" + m_StraightEdgeProxyBrush);
                //此时m_StraightEdgeProxyActive的值为false
                //此时m_StraightEdgeProxyBrush的值不为空
                script.CreateNewLine(
                    canvas, xfPointer_CS, currentCreator,
                    m_StraightEdgeProxyActive ? m_StraightEdgeProxyBrush : null);


                script.SetControlPoint(xfPointer_CS, isKeeper: true);
            }
        }

        /// <summary>
        /// 启动线路
        /// </summary>
        /// <param name="mainPointerXf_CS"></param>
        void InitiateLineAt(TrTransform mainPointerXf_CS)
        {
            // Set Main Pointer to transform.
            CanvasScript canvas = App.Scene.ActiveCanvas;
            canvas.AsCanvas[m_MainPointerData.m_Script.transform] = mainPointerXf_CS;

            // Update other pointers.
            UpdateSymmetryPointerTransforms();

            InitiateLine(false);
        }


        // Detach and record lines for all active pointers.//完成LineDetach并记录所有活动指针的线。
        /// <summary>
        /// 定稿线
        /// </summary>
        /// <param name="isContinue"></param>
        /// <param name="discard"></param>
        void FinalizeLine(bool isContinue = false, bool discard = false)
        {
            PointerScript groupStart = null;
            uint groupStartTime = 0;
            //discard or solidify every pointer's active line//丢弃或固化每个指针的活动线
            for (int i = 0; i < m_NumActivePointers; ++i)
            {
                var pointer = m_Pointers[i].m_Script;
                // XXX: when would an active pointer not be creating a line?//XXX:什么时候一个活动指针不会创建一线？
                if (pointer.IsCreatingStroke())
                {
                    bool bDiscardLine = discard || pointer.ShouldDiscardCurrentLine();


                    if (bDiscardLine)
                    {
                        pointer.DetachLine(bDiscardLine, null, SketchMemoryScript.StrokeFlags.None);
                    }
                    else
                    {
                        SketchMemoryScript.StrokeFlags flags = SketchMemoryScript.StrokeFlags.None;
                        if (groupStart == null)
                        {
                            groupStart = pointer;
                            // Capture this, because stroke becomes invalid after being detached.
                            //捕捉这一点，因为笔划在被分离后变得无效。
                            groupStartTime = groupStart.TimestampMs;
                        }
                        else
                        {
                            flags |= SketchMemoryScript.StrokeFlags.IsGroupContinue;
                            // Verify IsGroupContinue invariant
                            Debug.Assert(pointer.TimestampMs == groupStartTime);
                        }
                        pointer.DetachLine(bDiscardLine, null, flags);
                    }
                }
            }
        }
    }
}  // namespace TiltBrush
