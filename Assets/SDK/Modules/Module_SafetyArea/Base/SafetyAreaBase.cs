using SC.XR.Unity.Module_InputSystem;
using SC.XR.Unity.Module_InputSystem.InputDeviceHand;
using System.Collections.Generic;
using UnityEngine;

public class SafetyAreaBase : MonoBehaviour
{
    protected float originHeight;
    protected MeshRenderer meshRenderer;
    protected MeshFilter meshFilter;
    protected List<Vector4> positionList;
    protected GameObject outOfSafetyArea;
    protected float perimeter;

    protected float duration = 1.0f;

    protected List<ColorSection> colorSections = new List<ColorSection>()
    {
        new ColorSection(){ startColor = new Color(0.129f, 0.235f, 0.886f), endColor = new Color(0.118f, 0.365f, 0.051f) },
        new ColorSection(){ startColor = new Color(0.890f, 0.102f, 0.090f), endColor = new Color(0f, 0.482f, 0.012f) },
        new ColorSection(){ startColor = new Color(0.263f, 0.145f, 0.176f), endColor = new Color(0.639f, 0.098f, 0.3012f) }
    };

    protected Color nearColorStart = new Color(0.8902f, 0.102f, 0.09f, 1f);
    protected Color nearColorEnd = new Color(0.8902f, 0.5843f, 0.102f, 1f);

    /// <summary>
    /// 设置原始高度用于计算高度delta
    /// </summary>
    /// <param name="height"></param>
    public void SetOriginHeight(float height)
    {
        this.originHeight = height;
    }

    /// <summary>
    ///  获取原始高度
    /// </summary>
    /// <returns></returns>
    public float GetOriginHeight()
    {
        return this.originHeight;
    }

    /// <summary>
    /// 设置周长
    /// </summary>
    /// <param name="perimeter"></param>
    public void SetPerimeter(float perimeter)
    {
        this.perimeter = perimeter;
    }

    /// <summary>
    /// 获取周长
    /// </summary>
    /// <returns></returns>
    public float GetPerimter()
    {
        return perimeter;
    }

    /// <summary>
    /// 重新设置高度
    /// </summary>
    /// <param name="height"></param>
    public void ResetPlaneHeight(float height)
    {
        float heightdelta = height - originHeight;
        this.transform.position = Vector3.up * heightdelta;
    }

    public void SetMaterial(Material mat)
    {
        meshRenderer.sharedMaterial = mat;
    }

    public virtual void Update()
    {
        if (outOfSafetyArea == null)
        {
            GameObject outOfSafetyAreaResource = Resources.Load<GameObject>("OutofSafetyArea");
            outOfSafetyArea = GameObject.Instantiate(outOfSafetyAreaResource, this.transform);
        }

        if (meshRenderer == null)
        {
            meshRenderer = this.GetComponent<MeshRenderer>();
        }

        if (meshFilter == null)
        {
            meshFilter = this.GetComponent<MeshFilter>();
        }

        //if (!SafetyAreaManager.Instance.isSettingSafetyArea)
        //{
        //    meshRenderer.enabled = API_GSXR_Slam.GSXR_Get_OfflineMapRelocState() == 1;
        //}
        //else
        //{
        //    meshRenderer.enabled = true;
        //}
    }

    protected List<Vector4> GetInteractionObjectPosition()
    {
        if (positionList == null)
        {
            positionList = new List<Vector4>();
        }
        positionList.Clear();

        if (Module_InputSystem.instance == null) return positionList;

        Vector3 headTransformPosition = GSXRManager.Instance.head.position + GSXRManager.Instance.head.forward * 0.5f;
        Vector3 viewportPosition = GSXRManager.Instance.monoCamera.WorldToViewportPoint(headTransformPosition);
        positionList.Add(new Vector4(viewportPosition.x, viewportPosition.y, viewportPosition.z, 1f));

        if (Module_InputSystem.instance.IsSomeDeviceActiveWithoutHead)
        {
            if (Module_InputSystem.instance.GetInputDeviceStatus(InputDeviceType.GGT26Dof))
            {
                InputDeviceBase inputDevice = Module_InputSystem.instance.GetInputDevice<InputDeviceBase>(InputDeviceType.GGT26Dof);
                foreach (var part in inputDevice.inputDevicePartList)
                {
                    if (part.inputDataBase.isVaild == true)
                    {
                        GameObject handModlePart = (part as InputDeviceHandPart).inputDeviceHandPartUI.modelHand.ActiveHandModel.GetJointTransform(FINGER.forefinger, JOINT.One).gameObject;
                        Vector3 partPosition = handModlePart.transform.position + handModlePart.transform.forward * 0.5f;
                        Vector3 viewportPartPosition = GSXRManager.Instance.monoCamera.WorldToViewportPoint(partPosition);
                        positionList.Add(new Vector4(viewportPartPosition.x, viewportPartPosition.y, viewportPartPosition.z, 1f));
                    }
                }
            }
            else if (Module_InputSystem.instance.GetInputDeviceStatus(InputDeviceType.KS))
            {
                InputDeviceBase inputDevice = Module_InputSystem.instance.GetInputDevice<InputDeviceBase>(InputDeviceType.KS);
                foreach (var part in inputDevice.inputDevicePartList)
                {
                    if (part.inputDataBase.isVaild == true)
                    {
                        GameObject KSPart = part.inputDevicePartUIBase.gameObject;
                        Vector3 partPosition = KSPart.transform.position + KSPart.transform.forward * 0.5f;
                        Vector3 viewportPartPosition = GSXRManager.Instance.monoCamera.WorldToViewportPoint(partPosition);
                        positionList.Add(new Vector4(viewportPartPosition.x, viewportPartPosition.y, viewportPartPosition.z, 1f));
                    }
                }
            }
            else if (Module_InputSystem.instance.GetInputDeviceStatus(InputDeviceType.BT3Dof))
            {
                InputDeviceBase inputDevice = Module_InputSystem.instance.GetInputDevice<InputDeviceBase>(InputDeviceType.BT3Dof);
                foreach (var part in inputDevice.inputDevicePartList)
                {
                    if (part.inputDataBase.isVaild == true)
                    {
                        GameObject BT3DofPart = part.inputDevicePartUIBase.gameObject;
                        Vector3 partPosition = BT3DofPart.transform.position + BT3DofPart.transform.forward * 0.5f;
                        Vector3 viewportPartPosition = GSXRManager.Instance.monoCamera.WorldToViewportPoint(partPosition);
                        positionList.Add(new Vector4(viewportPartPosition.x, viewportPartPosition.y, viewportPartPosition.z, 1f));
                    }
                }
            }
        }
        return positionList;
    }
}
