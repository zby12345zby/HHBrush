using SC.XR.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class PermissionRequest : MonoBehaviour
{

    public bool Camera = false;
    public bool ExternalStorageRead = false;
    public bool ExternalStorageWrite = false;
    public bool Microphone = false;
    public bool FineLocation = false;
    public bool CoarseLocation = false;

    static Coroutine requestC;
    static Dictionary<string,bool> permissionDic = new Dictionary<string, bool>();
    List<string> permission = new List<string>();

    private static PermissionRequest instance;
    public static PermissionRequest getInstance {
        get {
            if (instance == null) {
                instance = new GameObject("PermissionRequest").AddComponent<PermissionRequest>();
            }
            return instance;
        }
    }

    void Awake() {
        instance = this;
    }

    /// <summary>
    /// Like UnityEngine.Android.Permission.Camera
    /// </summary>
    /// <param name="permission">Like Permission.Camera</param>
    public void GetPerssion(string permission="") {
        if (permission != "" && ! permissionDic.ContainsKey(permission) && !Permission.HasUserAuthorizedPermission(permission)) {
            permissionDic.Add(permission,true);
        }

        if (requestC == null) {
            requestC = StartCoroutine(RequestPerssion());
        }
    }


    void OnEnable() {

        if (Camera) {
            GetPerssion(Permission.Camera);
        }
        if (ExternalStorageRead) {
            GetPerssion(Permission.ExternalStorageRead);
        }
        if (ExternalStorageWrite) {
            GetPerssion(Permission.ExternalStorageWrite);
        }
        if (Microphone) {
            GetPerssion(Permission.Microphone);
        }
        if (FineLocation) {
            GetPerssion(Permission.FineLocation);
        }
        if (CoarseLocation) {
            GetPerssion(Permission.CoarseLocation);
        }
    }

    IEnumerator RequestPerssion() {
        yield return new WaitForEndOfFrame();
        if (permissionDic.Count > 0) {
            permission.Clear();
            foreach (var item in permissionDic.Keys) {
                permission.Add(item);
                DebugMy.Log("RequestPerssion:" + item, this, true);
            }
            AndroidPermission.getInstant.GetPermission(permission);
            permissionDic.Clear();
        }
        requestC = null;
    }


}
