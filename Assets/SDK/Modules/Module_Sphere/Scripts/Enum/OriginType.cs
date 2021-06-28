using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OriginType
{
    /// <summary>
    /// Using the Pointer's center point of the SDK as the origin.
    /// </summary>
    Pointer,

    /// <summary>
    /// Using the Cursor's center point of the SDK as the origin.
    /// </summary>
    Cursor,

    /// <summary>
    /// Using the Transform's center point of the SDK as the origin.
    /// </summary>
    Transform
}
