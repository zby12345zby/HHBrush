using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SC.XR.Unity.Module_PlatformAccount
{
    public class BaseCoroutineForms :BaseUIForms
    {

        public Coroutine sendCodeCooling = null;
        public bool isSendCodeCooling = false;
        
        public IEnumerator SendCodeCooling(int rest, int total, string coolingStr, string activateStr, Button btn, Text btnText)
        {
            isSendCodeCooling = true;

            int coolingTime = 0;

            if ((total - rest) <= 3)
            {
                coolingTime = 60;
            }
            else if ((total - rest) >= 4 && (total - rest) <= 10)
            {
                coolingTime = 120;
            }
            else
            {
                coolingTime = 240;
            }

            int time = 0;

            while (time < coolingTime)
            {
                time++;
                btnText.text = coolingStr + " (" + (coolingTime - time) + "s" + ")";
                yield return new WaitForSeconds(1);
            }
            ClickableBtn(btn);
            btnText.text = activateStr;
            isSendCodeCooling = false;
            sendCodeCooling = null;
        }

    }

}

