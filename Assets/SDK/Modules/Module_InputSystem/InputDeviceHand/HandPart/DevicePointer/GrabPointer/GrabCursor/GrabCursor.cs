using UnityEngine;
using DG.Tweening;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceHand {
    public class GrabCursor : DefaultCursor {

        public GrabPointer grabPointer {
            get {
                return Transition<GrabPointer>(pointerBase);
            }
        }

        public override void OnSCStart() {
            base.OnSCStart();
            foreach (var item in CursorPartDir.Values) {
                if (item.CursorType == CursorPartType.MoveArrowsEastWest || item.CursorType == CursorPartType.MoveArrowsNorthSouth) {
                    item.ModuleStart();
                }
            }
        }


        public override void UpdateOtherCursorVisual() {
            if (pointerBase.detectorBase.inputDevicePartBase.inputDataBase.inputKeys.GetKeyDown(InputKeyCode.Enter)) {
                foreach (var item in CursorPartDir.Values) {
                    if (item.CursorType == CursorPartType.MoveArrowsEastWest || item.CursorType == CursorPartType.MoveArrowsNorthSouth) {
                        item.ModuleStop();
                    }
                }
            } else if (pointerBase.detectorBase.inputDevicePartBase.inputDataBase.inputKeys.GetKeyUp(InputKeyCode.Enter)) {
                foreach (var item in CursorPartDir.Values) {
                    if (item.CursorType == CursorPartType.MoveArrowsEastWest || item.CursorType == CursorPartType.MoveArrowsNorthSouth) {
                        item.ModuleStart();
                    }
                }
            }
        }
    }
}
