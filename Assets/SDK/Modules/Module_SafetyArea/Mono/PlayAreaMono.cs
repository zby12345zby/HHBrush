using SC.XR.Unity.Module_InputSystem;
using SC.XR.Unity.Module_InputSystem.InputDeviceHand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAreaMono : SafetyAreaBase
{
    private Color[] colors;

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        Vector4[] positionList = GetInteractionObjectPosition().ToArray();
        if (positionList.Length != 0)
        {
            meshRenderer.sharedMaterial.SetInt("positionCount", positionList.Length);
            meshRenderer.sharedMaterial.SetVector("interactionPosition", positionList[0]);
        }

        float alpha = GetPlaneFillPercentage(GSXRManager.Instance.head.position);
        meshRenderer.sharedMaterial.SetFloat("alpha", alpha);

        Color colorStart = colorSections[SafetyAreaManager.Instance.OriginSafetyAreaColorIndex].startColor;
        Color colorEnd = colorSections[SafetyAreaManager.Instance.OriginSafetyAreaColorIndex].endColor;
        meshRenderer.sharedMaterial.SetColor("_Color1", colorStart);
        meshRenderer.sharedMaterial.SetColor("_Color2", colorEnd);

        var lerp = Mathf.PingPong(Time.time, duration) / duration;
        meshRenderer.sharedMaterial.SetColor("settingsColor", Color.Lerp(nearColorStart, nearColorEnd, lerp)) ;

        meshRenderer.sharedMaterial.SetVector("_MainTex_ST", new Vector4(Mathf.RoundToInt(perimeter / PlayAreaConstant.SAFETY_AREA_HEIGHT * 7f), 7f, 0f, -0.05f));

        if (SafetyAreaManager.Instance.ShowAreaWhenBowHead)
        {
            if (Vector3.Angle(GSXRManager.Instance.head.forward, Vector3.down) < PlayAreaConstant.HEAD_BOW_ANGLE)
            {
                meshRenderer.sharedMaterial.SetInt("bowHead", 1);
            }
            else
            {
                meshRenderer.sharedMaterial.SetInt("bowHead", 0);
            }
        }
        else
        {
            meshRenderer.sharedMaterial.SetInt("bowHead", 0);
        }

        if (!SafetyAreaManager.Instance.isSettingSafetyArea)
        {
            if (alpha >= 10f)
            {
                outOfSafetyArea.SetActive(true);
            }
            else
            {
                outOfSafetyArea.SetActive(false);
            }
        }
        else
        {
            outOfSafetyArea.SetActive(false);
        }
    }

    public void SetColor(Color[] colors)
    {
        this.colors = colors;
    }

    public Color[] GetColor()
    {
        return this.colors;
    }

    public void SetMesh(Mesh mesh, Color[] colors, float perimeter)
    {
        SetColor(colors);

        meshFilter = this.gameObject.AddComponent<MeshFilter>();
        meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        //meshRenderer.material = Resources.Load<Material>("Material/SafetyEdgeMat");
        meshFilter.mesh = mesh;
        GroundHeightStep groundHeightStep = SafetyAreaManager.Instance.GetStep<GroundHeightStep>(SafetyAreaStepEnum.GroundHeight);
        //originHeight = groundHeightStep.GetPlaneHeight();
        SetOriginHeight(groundHeightStep.GetPlaneHeight());
        this.perimeter = perimeter;
    }

    public float GetPlaneFillPercentage(Vector3 position)
    {
        Vector3 localPosition = transform.InverseTransformPoint(position);
        List<int> effectIndices = SafetyAreaVertexHelper.CaculateEffectVerticeIndices(localPosition, 1f);
        if (effectIndices.Count == 0) return 999f;
        int fillCount = 0;
        for (int i = 0; i < effectIndices.Count; i++)
        {
            if (colors[effectIndices[i]] == Color.white)
            {
                fillCount++;
            }
        }
        return (effectIndices.Count - fillCount) / (effectIndices.Count * 1f / SafetyAreaManager.Instance.AlphaParam);
    }
}
