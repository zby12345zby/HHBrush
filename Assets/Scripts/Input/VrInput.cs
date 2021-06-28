// Copyright 2020 The Tilt Brush Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace TiltBrush
{

    /// Tilt Brush-specific abstraction over inputs that are commonly
    /// provided by VR controllers. Mostly used with ControllerInfo.GetVrInput()
    /// ///在VR控制器通常提供的输入上倾斜笔刷特定的抽象。主要用于ControllerInfo.GetVrInput()
    public enum VrInput
    {
        Button01,           // Pad_Left on Vive, X or A buttons on Rift.  //在Vive上用左键，在Rift上用X或A键。
        Button02,           // Pad_Right on Vive, Y or B buttons on Rift. //在Vive上按右键，在Rift上按Y或B键。
        Button03,           // Menu Button on Vive, Y or B on Rift        //Vive上的菜单按钮，Rift上的Y或B
        Button04,           // Full-pad click on Vive, X or A on the Rift.//全键盘点击Vive，Rift上的X或A

        // --------------------------------------------------------- //
        // Vive up/down quads, only used in experimental
        // --------------------------------------------------------- //
        Button05,           // Quad_Up on vive, Y and B on Rift            //Quad_Up在vive上，Y和B在Rift上
        Button06,           // Quad_Down on vive, X and A on Rift          //Quad_Down在vive，X和A在Rift
                            // --------------------------------------------------------- //

        Directional,        // Thumbstick if one exists; otherwise touchpad.
        Trigger,            // Trigger Button on Vive, Primary Trigger Button on Rift. // Vive上的触发按钮，Rift上的主触发按钮。
        Grip,               // Grip Button on Vive, Secondary Trigger on Rift.         //Vive上的抓地力按钮，Rift上的二级触发器。
        Any,
        Thumbstick,         // TODO: standardize spelling: ThumbStick or Thumbstick?   //上面的摇杆
        Touchpad
    }
} // namespace TiltBrush
