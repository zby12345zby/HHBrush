using SC.XR.Unity.Module_InputSystem.InputDeviceHand;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandMenuExample : MonoBehaviour
{
    public GameObject objectMenu;
    public Vector3 offsetCenter = new Vector3(0,0,0.05f);
    private Vector3 initlocalScale;

    TextMeshPro textMesh;
    int PalmFlatFaceHeadStartCount = 0;
    int PalmFlatFaceHeadFlatCount = 0;
    int PalmFlatFaceHeadMiddelFlatAndCatchCount = 0;
    int PalmFlatFaceHeadCatchCount = 0;
    int PalmFlatFaceHeadEndCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        InputDeviceHandPartEventBase.eventDelegate += HandEvent;
        initlocalScale = objectMenu.transform.localScale; 
        textMesh = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy() {

        InputDeviceHandPartEventBase.eventDelegate -= HandEvent;
    }

    void HandEvent(InputDeviceHandPart inputDeviceHandPart, HandEventType eventType, float eventPercent) {
        if (inputDeviceHandPart.PartType == SC.XR.Unity.Module_InputSystem.InputDevicePartType.HandLeft) {

            textMesh.text = "Event: ";
            if (eventType == HandEventType.PalmFlatFaceHeadStart) {
                objectMenu.transform.localScale = initlocalScale;
                objectMenu.SetActive(true);

                textMesh.text += eventType + "";
                PalmFlatFaceHeadStartCount++;

            } else if (eventType == HandEventType.PalmFlatFaceHeadFlat || eventType == HandEventType.PalmFlatFaceHeadMiddelFlatAndCatch) {

                objectMenu.transform.localScale = Vector3.Lerp(objectMenu.transform.localScale, initlocalScale, 0.1f);
                objectMenu.transform.rotation = inputDeviceHandPart.inputDataHand.handInfo.centerRotation;
                objectMenu.transform.position = inputDeviceHandPart.inputDataHand.handInfo.centerPosition + inputDeviceHandPart.inputDataHand.handInfo.centerRotation * offsetCenter;
               
                textMesh.text += eventType+"";
                if (eventType == HandEventType.PalmFlatFaceHeadFlat) {
                    PalmFlatFaceHeadFlatCount++;
                } else {
                    PalmFlatFaceHeadMiddelFlatAndCatchCount++;
                }

            } else if (eventType == HandEventType.PalmFlatFaceHeadCatch) {

                objectMenu.transform.rotation = inputDeviceHandPart.inputDataHand.handInfo.centerRotation;
                objectMenu.transform.position = inputDeviceHandPart.inputDataHand.handInfo.centerPosition + inputDeviceHandPart.inputDataHand.handInfo.centerRotation * offsetCenter;
                objectMenu.transform.localScale = Vector3.Lerp(objectMenu.transform.localScale, initlocalScale*0.2f, 0.1f);
                
                textMesh.text += eventType + "";
                PalmFlatFaceHeadCatchCount++;

            } else if (eventType == HandEventType.PalmFlatFaceHeadEnd) {

                objectMenu.SetActive(false);
                objectMenu.transform.localScale = initlocalScale; 
               
                textMesh.text += eventType + "";
                PalmFlatFaceHeadEndCount++;

            }
            textMesh.text += "\nPalmFlatFaceHeadStartCount       " + PalmFlatFaceHeadStartCount +
    "\nPalmFlatFaceHeadFlatCount       " + PalmFlatFaceHeadFlatCount +
     "\nPalmFlatFaceHeadMiddelFlatAndCatchCount  " + PalmFlatFaceHeadMiddelFlatAndCatchCount +
      "\nPalmFlatFaceHeadCatchCount       " + PalmFlatFaceHeadCatchCount +
       "\nPalmFlatFaceHeadEndCount       " + PalmFlatFaceHeadEndCount;
        }

    }
}
