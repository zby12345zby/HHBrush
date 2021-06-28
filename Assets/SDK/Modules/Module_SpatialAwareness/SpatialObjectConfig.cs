using SC.XR.Unity;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "SCMenu/SpatialObjectConfig")]
public class SpatialObjectConfig : ScriptableObject
{
    public List<string> m_PrefabList;
}
