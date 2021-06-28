using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct SlamControllerCaps
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public String deviceManufacturer;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public String deviceIdentifier;
    public UInt32 caps; //0 bit = Provides Rotation; 1 bit = Position;
    public UInt32 activeButtons;
    public UInt32 active2DAnalogs;
    public UInt32 active1DAnalogs;
    public UInt32 activeTouchButtons;
};
/// <summary>
/// Slam controller state.
/// This should change if the GSXRControllerState changes in slamApi.h
/// Currently this assumes the default packing in slamApi.h
/// If that changes. Change this accordingly.
/// </summary>
public struct GSXRControllerState {
	public Quaternion rotation;
	public Vector3 position;
	public Vector3 gyro;
	public Vector3 accelerometer;
	public long timestamp;
	public int buttonState;
	[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
	public Vector2[] analog2D;
	[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
	public float[] analog1D;
	public int isTouching;
	public int connectionState;
};