using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;


namespace SC.XR.Unity.Module_InputSystem.InputDeviceHand {

    /// <summary>
    /// Event类型，注意Event触发后，下一帧LateUpdate将复位
    /// </summary>
    public enum HandEventType {

        JointTouchEnter,
        JointTouchExit,
        
        TurnFaceDown,
        TurnFaceDrag,
        TurnFaceUp,

        Push,
        Pull,
        
        Left,
        Rigth,
        
        TurnLeft,
        TurnRight,
        
        FingerClick,

        CatchReady,
        CatchDown,
        CatchDrag,
        CatchUp,
        CatchEnd,

        PinchDown,
        PinchDrag,
        PinchUp,

        Throw,
        NoThrow,


        Losting,//丢失中都是进度0-100
        Lost,//彻底丢失100
        Found,//识别到

        PalmFaceHeadStart,
        PalmFaceHead,
        PalmFaceHeadEnd,

        PalmFlatFaceHeadStart,
        PalmFlatFaceHeadFlat,
        PalmFlatFaceHeadMiddelFlatAndCatch,
        PalmFlatFaceHeadCatch,
        PalmFlatFaceHeadEnd,

        Null,

    }

}







