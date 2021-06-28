using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SC.XR.Unity.Module_InputSystem.InputDeviceHand {

    public class InputDataHand: InputDataBase {

        public InputDeviceHandPart inputDeviceHandPart;
        public InputDataHand(InputDeviceHandPart inputDeviceHandPart) : base(inputDeviceHandPart) {
            this.inputDeviceHandPart = inputDeviceHandPart;
        }


        public class HandsInfo {
            /// <summary>
            /// 手原始数据
            /// </summary>
            public float[] originDataMode = new float[256];
            public float[] originDataPose = new float[128];

            public int handAmount = 0;

            /// <summary>
            /// 数据里是否存在双手
            /// </summary>
            public bool handLeftFind = false;
            public bool handRightFind = false;

            /// <summary>
            /// 数据里左右手的index
            /// </summary>
            public int handLeftIndex = 0;
            public int handRighIndex = 0;

            /// <summary>
            /// 左右手数据存储结构
            /// </summary>
            public handInfo handLeft;
            public handInfo handRight;
        }




        public bool isFound = false;//是否有数据
        public handInfo handInfo;


    }
}
