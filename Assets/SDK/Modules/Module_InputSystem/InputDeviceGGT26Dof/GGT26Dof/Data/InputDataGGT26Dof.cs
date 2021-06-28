using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceHand.GGT26Dof {

    public class InputDataGGT26Dof: InputDataHand {

        public InputDeviceGGT26DofPart inputDeviceGGT26DofPart;
        public InputDataGGT26Dof(InputDeviceGGT26DofPart _inputDeviceGGT26DofPart) : base(_inputDeviceGGT26DofPart) {
            inputDeviceGGT26DofPart = _inputDeviceGGT26DofPart;

            if(inputDeviceGGT26DofPart.PartType == InputDevicePartType.HandLeft) {
                handsInfo.handLeft = handInfo = new GGT26DofHandInfo("/sdcard/GreyHandConfig.txt");
            } else if(inputDeviceGGT26DofPart.PartType == InputDevicePartType.HandRight) {
                handsInfo.handRight = handInfo = new GGT26DofHandInfo("/sdcard/GreyHandConfig.txt");
            }
        }

        /// <summary>
        /// static 为类所有
        /// </summary>
        public static HandsInfo handsInfo = new HandsInfo();

    }
}
