using SC.XR.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class AndroidPermission : AndroidPluginBase
{
    private static AndroidPermission Instant;
    public static AndroidPermission getInstant {
        get {
            if(Instant == null) {
                Instant = new AndroidPermission();
            }
            return Instant;
        }
    }
    private AndroidJavaClass AndroidPermissionClass { get; set; }
    private AndroidJavaObject AndroidPermissionObject { get; set; }
    private AndroidPermission() {
        AndroidPermissionClass = GetAndroidJavaClass("com.example.libpermission.PermissionRequest");
        AndroidPermissionObject = ClassFunctionCallStatic<AndroidJavaObject>(AndroidPermissionClass, "getInstant", CurrentActivity);

    }

    string[] permissionArray;
    public void GetPermission(List<string> permissionList) {
        if (permissionList == null || permissionList.Count == 0) {
            return;
        }
        permissionArray = permissionList.ToArray();
        ObjectFunctionCall2(AndroidPermissionObject, "RequestPermission", permissionArray);
    }

    [Obsolete("Please Use PermissionRequest.getInstanct.GetPerssion(Permission.XXX)")]
    public void GetPermission(bool Camera = true, bool ExternalStorageRead = true, bool ExternalStorageWrite = true, bool Microphone = false, bool FineLocation = false, bool CoarseLocation = false) {

        List<string> permissionList = new List<string>();

        if (Camera) {
            PermissionRequest.getInstance.GetPerssion(Permission.Camera);
        }
        if (ExternalStorageRead) {
            PermissionRequest.getInstance.GetPerssion(Permission.ExternalStorageRead);
        }
        if (ExternalStorageWrite) {
            PermissionRequest.getInstance.GetPerssion(Permission.ExternalStorageWrite);
        }
        if (Microphone) {
            PermissionRequest.getInstance.GetPerssion(Permission.Microphone);
        }
        if (FineLocation) {
            PermissionRequest.getInstance.GetPerssion(Permission.FineLocation);
        }
        if (CoarseLocation) {
            PermissionRequest.getInstance.GetPerssion(Permission.CoarseLocation);
        }
    }
}
