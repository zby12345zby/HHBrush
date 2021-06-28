using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRenderClose : MonoBehaviour {
    [SerializeField]
    bool isShowHandRender = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetInputSystem());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SetInputSystem() {
        yield return new WaitUntil(() => API_GSXR_Module_InputSystem.GSXR_Get_Instance() 
        && API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTLeft.inputDeviceGGT26DofPartUI.modelGGT26Dof.ActiveHandModel!=null);
        API_GSXR_Module_InputSystem_GGT26Dof.GSXR_GGTLeft.inputDeviceGGT26DofPartUI.modelGGT26Dof.ActiveHandModel.ShowHandRender = isShowHandRender;
    }
}
