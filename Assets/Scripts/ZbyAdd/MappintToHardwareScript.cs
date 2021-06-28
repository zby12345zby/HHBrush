using SC.XR.Unity.Module_InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public bool IsKeyDown_A { get; set; }
    public bool IsKeyDown_B { get; set; }
    public bool IsKeyDown_X { get; set; }
    public bool IsKeyDown_Y { get; set; }
    public float IsTrigger { get; set; }

    void Start()
    {
        DispatcherBase.KeyDownDelegateRegister(AnyRightKeyDownEventDelegate);
    }
    

    void AnyRightKeyDownEventDelegate(InputKeyCode keycode, InputDevicePartBase part)
    {

        if (InputKeyCode.A == keycode)
        {
            //API_Module_InputSystem_KS.KSKeyAddKey()
            IsKeyDown_A = true;
        }

        
        


        switch (keycode)
        {
            case InputKeyCode.Enter:
                break;
            case InputKeyCode.Cancel:
                break;
            case InputKeyCode.Trigger:
                break;
            case InputKeyCode.Function:
                break;
            case InputKeyCode.Back:
                break;
            case InputKeyCode.Tp:
                break;
            case InputKeyCode.VolumeDown:
                break;
            case InputKeyCode.VolumeUp:
                break;
            case InputKeyCode.A:
                IsKeyDown_A = true;
                break;
            case InputKeyCode.B:
                IsKeyDown_B = true;
                break;
            case InputKeyCode.LjoystickKey:
                break;
            case InputKeyCode.LFunction:
                break;
            case InputKeyCode.LHallInside:
                break;
            case InputKeyCode.LHallForward:
                break;
            case InputKeyCode.LTrigger:
                break;
            case InputKeyCode.X:
                IsKeyDown_X = true;
                break;
            case InputKeyCode.Y:
                IsKeyDown_Y = true;
                break;
            case InputKeyCode.RjoystickKey:
                break;
            case InputKeyCode.RFunction:
                break;
            case InputKeyCode.RHallInside:
                break;
            case InputKeyCode.RHallForward:
                break;
            case InputKeyCode.RTrigger:
                break;
            case InputKeyCode.UP:
                break;
            case InputKeyCode.DOWN:
                break;
            case InputKeyCode.RIGHT:
                break;
            case InputKeyCode.LEFT:
                break;
            case InputKeyCode.OTHER:
                break;
            default:
                break;
        }
    }

}
