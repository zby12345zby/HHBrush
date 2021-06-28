using UnityEngine;

namespace TiltBrush
{
    public class NewSketchButton : OptionButton
    {
        [SerializeField] private float m_AdjustDistanceAmount;
        [SerializeField] private Renderer m_NewSketchButtonBG;

        protected override void AdjustButtonPositionAndScale(
            float posAmount, float scaleAmount, float boxColliderGrowAmount)
        {
            SetMaterialFloat("_Distance", posAmount != 0 ? m_AdjustDistanceAmount : 0.0f);
            SetMaterialFloat("_Grayscale", posAmount != 0 ? 0.0f : 1.0f);
            m_NewSketchButtonBG.material.SetFloat("_Grayscale", posAmount != 0 ? 0.0f : 1.0f);
            base.AdjustButtonPositionAndScale(posAmount, scaleAmount, boxColliderGrowAmount);
        }

        protected override void OnButtonPressed()
        {
            if (!SketchControlsScript.m_Instance.SketchHasChanges())//执行了
            {
                Vector3 vPos = PointerManager.m_Instance.MainPointer.transform.position;
                if (App.VrSdk.GetControllerDof() == VrSdk.DoF.Six)//执行了
                {
                    vPos = InputManager.m_Instance.GetControllerPosition(
                        InputManager.ControllerName.Wand);
                }
                AudioManager.m_Instance.PlayIntroTransitionSound(vPos);
            }

            PanelManager.m_Instance.ToggleSketchbookPanels();//这个函数的意义：；切换显示画笔的的UI面板，不影响默认的画笔笔刷（注销此方法依然可以画出）
            App.Instance.ExitIntroSketch();//退出当前的功能，进入画笔功能
            PromoManager.m_Instance.RequestAdvancedPanelsPromo();

            // Change the shown sketchset by simulating a press on the corresponding gallery button.
            //通过模拟按相应的“多媒体资料”按钮来更改显示的草图集。
            SketchbookPanel panel = m_Manager.GetComponent<SketchbookPanel>();
            if (SketchCatalog.m_Instance.GetSet(SketchSetType.User).NumSketches == 0)
            {
                panel.ButtonPressed(GalleryButton.Type.Showcase);
            }
            else
            {
                panel.ButtonPressed(GalleryButton.Type.Local);
            }
        }
    }
} // namespace TiltBrush
