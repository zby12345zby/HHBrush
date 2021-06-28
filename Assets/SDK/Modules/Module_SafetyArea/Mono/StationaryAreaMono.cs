using SC.XR.Unity.Module_InputSystem;
using SC.XR.Unity.Module_InputSystem.InputDeviceHand;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StationaryAreaMono : SafetyAreaBase
{
    private bool isFreeze = true;
    private StationaryAreaStep stationaryAreaStep;
    private GroundHeightStep groundHeightStep;
    private float radius;

    public override void Update()
    {
        base.Update();

        if (!isFreeze)
        {
            Vector3 headPosition = API_GSXR_Slam.SlamManager.head.position;
            stationaryAreaStep.SetCircleCenter(headPosition);
            Vector2 circleCenter = stationaryAreaStep.GetCircleCenter();
            this.gameObject.transform.position = new Vector3(circleCenter.x, groundHeightStep.GetPlaneHeight(), circleCenter.y);
        }

        Vector4[] positionList = GetInteractionObjectPosition().ToArray();
        if (positionList.Length != 0)
        {
            meshRenderer.sharedMaterial.SetInt("positionCount", positionList.Length);
            meshRenderer.sharedMaterial.SetVector("interactionPosition", positionList[0]);
        }

        float distance = Vector2.Distance(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.z), new Vector2(API_GSXR_Slam.SlamManager.head.position.x, API_GSXR_Slam.SlamManager.head.position.z));
        float alpha = Mathf.Max(0, (distance - radius * 0.5f) * SafetyAreaManager.Instance.AlphaParam);
        meshRenderer.sharedMaterial.SetFloat("alpha", alpha);

        //var lerp = Mathf.PingPong(Time.time, duration) / duration;
        Color colorStart = colorSections[SafetyAreaManager.Instance.OriginSafetyAreaColorIndex].startColor;
        Color colorEnd = colorSections[SafetyAreaManager.Instance.OriginSafetyAreaColorIndex].endColor;
        meshRenderer.sharedMaterial.SetColor("_Color1", colorStart);
        meshRenderer.sharedMaterial.SetColor("_Color2", colorEnd);

        var lerp = Mathf.PingPong(Time.time, duration) / duration;
        meshRenderer.sharedMaterial.SetColor("settingsColor", Color.Lerp(nearColorStart, nearColorEnd, lerp));

        meshRenderer.sharedMaterial.SetInt("bowHead", 0);
        meshRenderer.sharedMaterial.SetVector("_MainTex_ST", new Vector4((float)perimeter / PlayAreaConstant.SAFETY_AREA_HEIGHT * 7f, 7f, 0f, -0.05f));

        if (outOfSafetyArea == null)
        {
            GameObject outOfSafetyAreaResource = Resources.Load<GameObject>("OutofSafetyArea");
            outOfSafetyArea = GameObject.Instantiate(outOfSafetyAreaResource, this.transform);
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

    public void FreezeStationaryAreaPosition()
    {
        isFreeze = true;
    }

    public void UnFreezeStationaryAreaPosition()
    {
        isFreeze = false;
    }

    public void SetRadius(float radius)
    {
        this.radius = radius;
    }

    public float GetRadius()
    {
        return this.radius;
    }

    public void SetMesh(Mesh mesh, float radius)
    {
        SetRadius(radius);
        if (stationaryAreaStep == null)
        {
            stationaryAreaStep = SafetyAreaManager.Instance.GetStep<StationaryAreaStep>(SafetyAreaStepEnum.StationaryArea);
        }

        if (groundHeightStep == null)
        {
            groundHeightStep = SafetyAreaManager.Instance.GetStep<GroundHeightStep>(SafetyAreaStepEnum.GroundHeight);
        }

        meshFilter = this.gameObject.AddComponent<MeshFilter>();
        meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        //meshRenderer.material = Resources.Load<Material>("Material/SafetyEdgeMat");
        meshFilter.mesh = mesh;
        SetOriginHeight(groundHeightStep.GetPlaneHeight());
        perimeter = 2f * Mathf.PI * radius;
    }
}
