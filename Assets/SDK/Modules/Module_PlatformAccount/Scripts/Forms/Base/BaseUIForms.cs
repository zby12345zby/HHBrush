using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public class BaseUIForms : MonoBehaviour
    {
        //页面显示
        public virtual void Display()
        {
            this.gameObject.SetActive(true);
            Active();
            Init();
        }

        //页面隐藏
        public virtual void Hiding()
        {
            this.gameObject.SetActive(false);
        }

        public virtual void Active()
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }

        public virtual void Inactive()
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        public virtual void Init()
        { 
            
        }

        public void OnFrameSelect(Image image)
        {
            image.color = ColorConfig.frame_select;
        }

        public void OnFrameError(Image image)
        {
            image.color = ColorConfig.frame_error;
        }

        public void OnFrameNormal(Image image)
        {
            image.color = ColorConfig.frame_normal;
        }

        public void ClickableBtn(Button btn)
        {            
            btn.interactable = true;
            btn.enabled = true;
            btn.GetComponent<Image>().color = ColorConfig.button_image_clickable;
        }

        public void UnClickableBtn(Button btn)
        {            
            btn.interactable = false;
            btn.GetComponent<Image>().color = ColorConfig.button_image_unclickable;
        }

        public void CoolingBtn(Button btn)
        {
            btn.enabled = false;
            btn.GetComponent<Image>().color = ColorConfig.button_image_cooling;
        }

    }
}
