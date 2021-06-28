using UnityEngine;
using UnityEngine.UI;

namespace SC.XR.Unity.Module_InputSystem {

    public enum EffectWay { 
        Scale,
        Transparency,
        Both,
        Null
    }

    public abstract class CursorPartBase : SCModuleMono {
        /// <summary>
        /// Cursor Type
        /// </summary>
        public CursorPartType CursorType;

        public CursorBase Cursor { get; protected set; }

        public EffectWay effectWay = EffectWay.Transparency;

        protected Vector3 initLocalscale;

        public MeshRenderer meshRender;
        public Color meshRenderInitColor;

        public Image image;
        public Color imageInitColor;

        public Color tempEffectColor;

        public override void OnSCAwake() {
            base.OnSCAwake();
            if(CursorType == CursorPartType.UnDefined) {
                DebugMy.Log("CursorType UnDefined !", this);
            }
            initLocalscale = transform.localScale;
            if (image && image.material) {
                imageInitColor = image.material.color;
            }
            if (meshRender && meshRender.material) {
                meshRenderInitColor = meshRender.material.color;
            }
        }
        public override void OnSCStart() {
            base.OnSCStart();
            Cursor = GetComponentInParent<CursorBase>();
        }

        public override void OnSCLateUpdate() {
            base.OnSCLateUpdate();

            if (Cursor == null)
                return;

            if (effectWay == EffectWay.Transparency) {
                EffectTransparency();
            } else if (effectWay == EffectWay.Scale) {
                EffectScale();
            } else if (effectWay == EffectWay.Both) {
                EffectScale();
                EffectTransparency();
            }
        }

        public override void OnSCDisable() {
            base.OnSCDisable();
            if (image && image.material) {
                image.material.color = imageInitColor;
            }
            if (meshRender && meshRender.material) {
                meshRender.material.color = meshRenderInitColor;
            }
        }
        public override void OnSCDestroy() {
            base.OnSCDestroy();
            Cursor = null;
        }

        protected virtual void EffectScale() {
            transform.localScale = initLocalscale * Cursor.visualPercent;
        }

        protected virtual void EffectTransparency() {
            if (image && image.material) {
                tempEffectColor = imageInitColor;
                tempEffectColor.a = imageInitColor.a * Cursor.visualPercent;
                image.material.color = tempEffectColor;
            }
        }

    }
}
