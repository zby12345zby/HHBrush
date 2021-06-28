using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceHand.GGT26Dof {
    public class InputDataGetGGT26DofPosture : InputDataGetHandPosture {
        public InputDataGetGGT26Dof inputDataGetGGT26Dof;
        public InputDataGetGGT26DofPosture(InputDataGetGGT26Dof _inputDataGetGGT26Dof) : base(_inputDataGetGGT26Dof) {
            inputDataGetGGT26Dof = _inputDataGetGGT26Dof;
        }

    }
}
