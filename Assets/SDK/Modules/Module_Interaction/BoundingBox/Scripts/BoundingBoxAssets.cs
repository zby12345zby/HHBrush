using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SC.XR.Unity {

    [CreateAssetMenu(menuName = "SCConfig/BoundingBoxAssets")]
    public class BoundingBoxAssets : ScriptableObject {
        public Material boxFocusDisplayMat;
        public Material boxGrabDisplayMat;
        public Material HandleMaterial;
        public Material HandleGrabMaterial;
        public GameObject CornerPrefab;
        public GameObject CornerSlatePrefab;
        public GameObject SidePrefab;
        public GameObject facePrefab;
    }

}
