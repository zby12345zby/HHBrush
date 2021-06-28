using SC.XR.Unity.Module_InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TiltBrush;

public class FollowRayPos : MonoBehaviour
{

    private void Start()
    {

    }
    void LateUpdate()
    {
        FollowRay();
    }
    void FollowRay()
    {
        if (API_GSXR_Module_InputSystem_KS.GSXR_KSRight != null
            && API_GSXR_Module_InputSystem_KS.GSXR_KSRight.inputDataKS.isVaild)
        {
            // transform.forward = (API_GSXR_Module_InputSystem_KS.GSXR_KSRight.detectorBase.pointerBase.cursorBase.transform.position -
            //API_GSXR_Module_InputSystem_KS.GSXR_KSRight.detectorBase.pointerBase.transform.position).normalized;
            transform.forward = API_GSXR_Module_InputSystem_KS.GSXR_KSRight.detectorBase.pointerBase.transform.forward;
            transform.position = API_GSXR_Module_InputSystem_KS.GSXR_KSRight.detectorBase.pointerBase.transform.position +
                                                                                                                         transform.forward.normalized * 0.05f;
        }
        else if (API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTRight != null
            && API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTRight.inputDataGGT26Dof.isVaild)
        {
            //transform.forward = (API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTRight.detectorBase.pointerBase.cursorBase.transform.position -
            //API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTRight.detectorBase.pointerBase.transform.position).normalized;
            transform.forward = API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTRight.detectorBase.pointerBase.transform.forward;
            transform.position = API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTRight.detectorBase.pointerBase.transform.position;// +
                                                                                                                                //transform.forward.normalized * 0.01f;
        }
        //else
        //{
        //    transform.forward = ViewpointScript.m_Instance._head.forward;
        //    transform.position = ViewpointScript.m_Instance._head.position + transform.forward.normalized * 0.6f;
        //}
    }
}
