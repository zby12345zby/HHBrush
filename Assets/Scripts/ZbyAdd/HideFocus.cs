using SC.XR.Unity.Module_InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideFocus : MonoBehaviour
{
    public Transform _line;
    //public Transform _CursorVisual;

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(HideFocusMethod());
    }


    IEnumerator HideFocusMethod()
    {

        yield return new WaitForSeconds(2f);
        DoHide();
    }

    void DoHide()
    {
        if (API_GSXR_Module_InputSystem_KS.GSXR_KSRight != null
       && API_GSXR_Module_InputSystem_KS.GSXR_KSRight.inputDataKS.isVaild)
        {
            API_GSXR_Module_InputSystem_KS.GSXR_Get_KSCursor().gameObject.GetComponent<MeshRenderer>().enabled = false;
            API_GSXR_Module_InputSystem_KS.GSXR_Get_KSLine().gameObject.SetActive(false);
        }
        else if (API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTRight != null
            && API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTRight.inputDataGGT26Dof.isVaild)
        {
            //HidefocusMesh();
           // _line.gameObject.SetActive(false);
        }
    }
}
