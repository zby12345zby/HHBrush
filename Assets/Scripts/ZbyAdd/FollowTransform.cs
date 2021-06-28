using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TiltBrush
{
    public class FollowTransform : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            if (ViewpointScript.m_Instance != null)
            {
                //transform.SetParent(ViewpointScript.m_Instance._gestureRight);
                //transform.localPosition = new Vector3(0, -0.05f, 0.25f);
                transform.position = new Vector3(1000, 1000, 1000);
                //transform.localRotation = Quaternion.Euler(-20, 0, 0);
                //transform.localScale = Vector3.zero;
                //transform.SetParent(ViewpointScript.m_Instance._controllerRight);
                //transform.position = ViewpointScript.m_Instance._controllerRight.position;
                //transform.rotation = ViewpointScript.m_Instance._controllerRight.rotation;
            }
        }
    }
}