using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Video3D : MonoBehaviour
{
    [SerializeField]
    private VideoClip videoClip;

    private VideoPlayer vp;
    private RenderTexture rt;
    [SerializeField]
    private RawImage ri;
    [SerializeField]
    bool useVideoRosulution = true;

    [SerializeField]
    RectTransform rectTransform;
    

    // Start is called before the first frame update
    void Start()
    {
        if (videoClip && ri) {
            vp = GetComponent<VideoPlayer>();
            if (vp == null) {
                vp = gameObject.AddComponent<VideoPlayer>();
            }
            vp.clip = videoClip;
            vp.isLooping = true;
            if (useVideoRosulution) {
                rt = new RenderTexture((int)vp.clip.width, (int)vp.clip.height, 0);
            } else {
                rt = new RenderTexture(Screen.width / 2, Screen.height, 0);
            }
            
            vp.renderMode = VideoRenderMode.RenderTexture;
            vp.targetTexture = rt;
            ri.texture = rt;
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform) {
                rectTransform.sizeDelta = new Vector2((int)vp.clip.width, (int)vp.clip.height);
            }
        }
    }
}
