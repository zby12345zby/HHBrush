using SC.XR.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SafetyAreaManager : SingletonMono<SafetyAreaManager>
{
    //是否已从配置文件中读取安全区域
    private static bool hasReadFromFile = false;

    public Action OnBeginSetSafeArea;
    public Action OnFinishSetSafeArea;

    public Action OnEnterSafetyArea;
    public Action OnExitSafetyArea;

    private Dictionary<SafetyAreaStepEnum, ISafetyAreaStep> areaStepDic;

    private GameObject safetyAreaGameObject;
    private SafetyAreaMono safetyAreaMono;
    private ISafetyAreaStep currentStep;

    [HideInInspector]
    public StationaryAreaMono stationaryAreaMono;
    private GameObject stationaryAreaObject;
    [HideInInspector]
    public PlayAreaMono playAreaMono;
    private GameObject playAreaObject;

    [HideInInspector]
    public bool isSettingSafetyArea = false;
    [HideInInspector]
    public bool onlySettingGroundHeight = false;

    private bool showAreaWhenBowHead = true;
    private float originAlphaParam = 0.4f;
    private int originSafetyAreaColorIndex = 0;

    private SafetyAreaInfo currentSafetyAreaInfo = null;

    public bool ShowAreaWhenBowHead
    {
        get
        {
            if (!hasReadFromFile)
            {
                LoadSafetyArea();
            }
            return showAreaWhenBowHead;
        }
        set
        {
            showAreaWhenBowHead = true;
            SaveSafetyArea();
        }
    }

    public int OriginSafetyAreaColorIndex
    {
        get
        {
            if (!hasReadFromFile)
            {
                LoadSafetyArea();
            }
            return originSafetyAreaColorIndex;
        }
        set
        {
            originSafetyAreaColorIndex = value;
            SaveSafetyArea();
        }
    }

    public float OriginAlphaParam
    {
        get
        {
            if (!hasReadFromFile)
            {
                LoadSafetyArea();
            }
            return originAlphaParam;
        }
        set
        {
            originAlphaParam = value;
            SaveSafetyArea();
        }
    }

    //public Color SafetyAreaColor
    //{
    //    get
    //    {
    //        return safetyAreaColor[originSafetyAreaColorIndex];
    //    }
    //}

    public float AlphaParam
    {
        get
        {
            return Mathf.Lerp(5f, 40f, originAlphaParam);
        }
    }

    //private GameObject savedSafetyArea;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GSXRManager.Instance.Initialized);
        if (!hasReadFromFile)
        {
            LoadSafetyArea();
        }
    }

    private void Init()
    {
        InitSafetyAreaMono();
        InitStep(safetyAreaMono);
        safetyAreaMono.Init();
    }

    private void Release()
    {
        OnFinishSetSafeArea?.Invoke();
        if (safetyAreaMono != null)
        {
            safetyAreaMono.Release();
            GameObject.Destroy(safetyAreaGameObject);
            safetyAreaMono = null;
            areaStepDic = null;
        }
    }

    private void InitStep(SafetyAreaMono safetyAreaMono)
    {
        if (areaStepDic == null)
        {
            areaStepDic = new Dictionary<SafetyAreaStepEnum, ISafetyAreaStep>();
            areaStepDic.Add(SafetyAreaStepEnum.GroundHeight, new GroundHeightStep(safetyAreaMono));
            areaStepDic.Add(SafetyAreaStepEnum.PlayArea, new PlayAreaStep(safetyAreaMono));
            areaStepDic.Add(SafetyAreaStepEnum.StationaryArea, new StationaryAreaStep(safetyAreaMono));
            //areaStepDic.Add(SafetyAreaStepEnum.ConfirmPlayArea, new ConfirmPlayAreaStep());
        }
    }

    private void InitSafetyAreaMono()
    {
        if (safetyAreaMono == null)
        {
            GameObject safetyAreaMonoResource = Resources.Load<GameObject>("SafetyAreaMono");
            safetyAreaGameObject = GameObject.Instantiate(safetyAreaMonoResource);
            safetyAreaMono = safetyAreaGameObject.GetComponent<SafetyAreaMono>();
        }
        //safetyAreaMono.Init();
    }

    //创建原地区域
    public void CreateStationarySafetyArea()
    {
        if (stationaryAreaMono == null)
        {
            stationaryAreaObject = new GameObject(PlayAreaConstant.STATIONARY_NAME);
            stationaryAreaObject.layer = 9;//1 << LayerMask.NameToLayer("SafetyArea");
            float groundHeight = GetStep<GroundHeightStep>(SafetyAreaStepEnum.GroundHeight).GetPlaneHeight();
            Mesh cylinderMesh = SafetyAreaVertexHelper.GenerateCylinderMesh(new Vector3(0, groundHeight, 0), groundHeight + PlayAreaConstant.SAFETY_AREA_HEIGHT, groundHeight, PlayAreaConstant.STATIONARY_AREA_RADIUS);
            stationaryAreaMono = stationaryAreaObject.AddComponent<StationaryAreaMono>();
            stationaryAreaMono.SetMesh(cylinderMesh, PlayAreaConstant.STATIONARY_AREA_RADIUS);
        }
    }

    //销毁原地区域
    public void DestroyStationaryArea()
    {
        if (stationaryAreaObject != null)
        {
            GameObject.Destroy(stationaryAreaObject);
        }
        stationaryAreaObject = null;
        stationaryAreaMono = null;
    }

    //创建安全网格
    public void CreatePlayArea(Mesh mesh, Color[] colors, float perimeter)
    {
        if (playAreaObject != null)
        {
            GameObject.Destroy(playAreaObject);
        }
        playAreaObject = new GameObject(PlayAreaConstant.PLAY_AREA_NAME);
        playAreaObject.layer = LayerMask.NameToLayer("SafetyArea");
        playAreaMono = playAreaObject.AddComponent<PlayAreaMono>();
        playAreaMono.SetMesh(mesh, colors, perimeter);
    }

    public void DestroyPlayArea()
    {
        if (playAreaObject != null)
        {
            Debug.LogError("Destroy playAreaObject");
            GameObject.Destroy(playAreaObject);
        }
        playAreaObject = null;
        playAreaMono = null;
    }

    public void StartSetSafetyArea()
    {
        if (safetyAreaMono != null)
        {
            Debug.LogError("last set safety area process not complete");
            return;
        }

        //if (savedSafetyArea != null)
        //{
        //    savedSafetyArea.SetActive(false);
        //}

        Init();
        OnBeginSetSafeArea?.Invoke();
        DestroyExistSafetyArea();
        API_GSXR_Slam.GSXR_ResaveMap("forTest");
        isSettingSafetyArea = true;
    }

    public void StartSetSafetyAreaHeight()
    {
        if (!CheckSafetyAreaExist())
        {
            StartSetSafetyArea();
            return;
        }

        Init();
        OnBeginSetSafeArea?.Invoke();
        onlySettingGroundHeight = true;
    }

    //判断安全区域是否存在
    public bool CheckSafetyAreaExist()
    {
        if (!hasReadFromFile)
        {
            LoadSafetyArea();
        }

        GameObject playArea = GameObject.Find(PlayAreaConstant.PLAY_AREA_NAME);
        if (playArea != null)
        {
            return true;
        }

        GameObject stationaryArea = GameObject.Find(PlayAreaConstant.STATIONARY_NAME);
        if (stationaryArea != null)
        {
            return true;
        }

        return false;
    }

    //重新设置安全区域高度
    public void ResetSafetyAreaHeight()
    {
        GameObject playArea = GameObject.Find(PlayAreaConstant.PLAY_AREA_NAME);
        if (playArea != null)
        {
            playArea.GetComponent<PlayAreaMono>().ResetPlaneHeight(GetStep<GroundHeightStep>(SafetyAreaStepEnum.GroundHeight).GetPlaneHeight());
        }

        GameObject stationaryArea = GameObject.Find(PlayAreaConstant.STATIONARY_NAME);
        if (stationaryArea != null)
        {
            stationaryArea.GetComponent<StationaryAreaMono>().ResetPlaneHeight(GetStep<GroundHeightStep>(SafetyAreaStepEnum.GroundHeight).GetPlaneHeight());
        }
    }

    public void DestroyExistSafetyArea()
    {
        GameObject playArea = GameObject.Find(PlayAreaConstant.PLAY_AREA_NAME);
        if (playArea != null)
        {
            GameObject.Destroy(playArea);
        }

        GameObject stationaryArea = GameObject.Find(PlayAreaConstant.STATIONARY_NAME);
        if (stationaryArea != null)
        {
            GameObject.Destroy(stationaryArea);
        }
    }

    public void ExitSafeAreaStep()
    {
        if (currentStep != null)
        {
            currentStep.OnExitStep();
        }
        currentStep = null;
        Release();
        API_GSXR_Slam.GSXR_SaveMap();
        isSettingSafetyArea = false;
        onlySettingGroundHeight = false;
        stationaryAreaMono = null;
        stationaryAreaObject = null;
        playAreaMono = null;
        playAreaObject = null;
        SaveSafetyArea(true);
    }

    private void LoadSafetyArea()
    {
        hasReadFromFile = true;
        if (!File.Exists(PlayAreaConstant.SAVE_FILE_NAME))
        {
            currentSafetyAreaInfo = new SafetyAreaInfo();
            return;
        }

        string fileStr = File.ReadAllText(PlayAreaConstant.SAVE_FILE_NAME);

        currentSafetyAreaInfo = JsonUtility.FromJson<SafetyAreaInfo>(fileStr);

        showAreaWhenBowHead = currentSafetyAreaInfo.showAreaWhenBowHead;
        originAlphaParam = currentSafetyAreaInfo.originAlphaParam;
        originSafetyAreaColorIndex = currentSafetyAreaInfo.originSafetyAreaColorIndex;

        if (string.IsNullOrEmpty(currentSafetyAreaInfo.safetyAreaName))
        {
            return;
        }

        Mesh edgeMesh = new Mesh();
        edgeMesh.vertices = currentSafetyAreaInfo.vertices.ToArray();
        edgeMesh.uv = currentSafetyAreaInfo.uv.ToArray();
        edgeMesh.triangles = currentSafetyAreaInfo.triangles.ToArray();

        GameObject safetyArea = new GameObject(currentSafetyAreaInfo.safetyAreaName);
        safetyArea.transform.position = currentSafetyAreaInfo.position;
        MeshRenderer meshRenderer = safetyArea.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = safetyArea.AddComponent<MeshFilter>();
        meshFilter.mesh = edgeMesh;
        meshRenderer.sharedMaterial = Resources.Load<Material>("Material/SafetyEdgeMat_Normal");

        SafetyAreaBase safetyAreaBase = null;
        if (currentSafetyAreaInfo.safetyAreaName == PlayAreaConstant.PLAY_AREA_NAME)
        {
            safetyAreaBase = safetyArea.AddComponent<PlayAreaMono>();
            safetyArea.GetComponent<PlayAreaMono>().SetColor(currentSafetyAreaInfo.colors.ToArray());
        }
        else
        {
            safetyAreaBase = safetyArea.AddComponent<StationaryAreaMono>();
            safetyArea.GetComponent<StationaryAreaMono>().SetRadius(currentSafetyAreaInfo.radius);
        }
        safetyAreaBase.SetOriginHeight(currentSafetyAreaInfo.originHeight);
        safetyAreaBase.SetPerimeter(currentSafetyAreaInfo.perimeter);
    }

    private void SaveSafetyArea(bool overrideVertexData = false)
    {
        if (overrideVertexData)
        {
            if (CheckSafetyAreaExist())
            {
                GameObject playArea = GameObject.Find(PlayAreaConstant.PLAY_AREA_NAME);
                if (playArea != null)
                {
                    currentSafetyAreaInfo.safetyAreaName = PlayAreaConstant.PLAY_AREA_NAME;
                    currentSafetyAreaInfo.position = playArea.transform.position;
                    currentSafetyAreaInfo.vertices = new List<Vector3>(playArea.GetComponent<MeshFilter>().mesh.vertices);
                    currentSafetyAreaInfo.triangles = new List<int>(playArea.GetComponent<MeshFilter>().mesh.triangles);
                    currentSafetyAreaInfo.uv = new List<Vector2>(playArea.GetComponent<MeshFilter>().mesh.uv);
                    currentSafetyAreaInfo.originHeight = playArea.GetComponent<SafetyAreaBase>().GetOriginHeight();
                    currentSafetyAreaInfo.perimeter = playArea.GetComponent<SafetyAreaBase>().GetPerimter();
                    currentSafetyAreaInfo.colors = new List<Color>(playArea.GetComponent<PlayAreaMono>().GetColor());
                }

                GameObject stationaryArea = GameObject.Find(PlayAreaConstant.STATIONARY_NAME);
                if (stationaryArea != null)
                {
                    currentSafetyAreaInfo.safetyAreaName = PlayAreaConstant.STATIONARY_NAME;
                    currentSafetyAreaInfo.position = stationaryArea.transform.position;
                    currentSafetyAreaInfo.vertices = new List<Vector3>(stationaryArea.GetComponent<MeshFilter>().mesh.vertices);
                    currentSafetyAreaInfo.triangles = new List<int>(stationaryArea.GetComponent<MeshFilter>().mesh.triangles);
                    currentSafetyAreaInfo.uv = new List<Vector2>(stationaryArea.GetComponent<MeshFilter>().mesh.uv);
                    currentSafetyAreaInfo.originHeight = stationaryArea.GetComponent<SafetyAreaBase>().GetOriginHeight();
                    currentSafetyAreaInfo.perimeter = stationaryArea.GetComponent<SafetyAreaBase>().GetPerimter();
                    currentSafetyAreaInfo.radius = stationaryArea.GetComponent<StationaryAreaMono>().GetRadius();
                }
            }
            else
            {
                currentSafetyAreaInfo.safetyAreaName = string.Empty;
                currentSafetyAreaInfo.position = Vector3.zero;
                currentSafetyAreaInfo.vertices = null;
                currentSafetyAreaInfo.triangles = null;
                currentSafetyAreaInfo.uv = null;
                currentSafetyAreaInfo.originHeight = 0f;
            }
        }

        currentSafetyAreaInfo.showAreaWhenBowHead = ShowAreaWhenBowHead;
        currentSafetyAreaInfo.originAlphaParam = OriginAlphaParam;
        currentSafetyAreaInfo.originSafetyAreaColorIndex = OriginSafetyAreaColorIndex;

        if (!File.Exists(PlayAreaConstant.SAVE_FILE_NAME))
        {
            if (!Directory.Exists(Path.GetDirectoryName(PlayAreaConstant.SAVE_FILE_NAME)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PlayAreaConstant.SAVE_FILE_NAME));
            }
            File.Create(PlayAreaConstant.SAVE_FILE_NAME).Dispose();
        }

        File.WriteAllText(PlayAreaConstant.SAVE_FILE_NAME, JsonUtility.ToJson(currentSafetyAreaInfo));
    }

    public void DestroySafetyArea()
    {
        if (CheckSafetyAreaExist())
        {
            DestroyExistSafetyArea();
        }
        SaveSafetyArea();
    }

    public T GetStep<T>(SafetyAreaStepEnum safetyAreaStepEnum) where T : class, ISafetyAreaStep
    {
        if (!areaStepDic.ContainsKey(safetyAreaStepEnum))
        {
            return default(T);
        }
        return areaStepDic[safetyAreaStepEnum] as T;
    }

    public void ChangeStep(SafetyAreaStepEnum safetyAreaStep)
    {
        if (currentStep != null)
        {
            currentStep.OnExitStep();
        }
        ISafetyAreaStep nextStep = areaStepDic[safetyAreaStep];
        nextStep.OnEnterStep();
        currentStep = nextStep;
    }

    public void DelayExitSafeAreaStep(float delayTime, Action onExitComplete)
    {
        StartCoroutine(ExitSafeAreaStepCoroutine(delayTime, onExitComplete));
    }

    public void DelayChangeStep(SafetyAreaStepEnum safetyAreaStep, float delayTime)
    {
        StartCoroutine(ChangeStepCoroutine(safetyAreaStep, delayTime));
    }

    private IEnumerator ChangeStepCoroutine(SafetyAreaStepEnum safetyAreaStep, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        ChangeStep(safetyAreaStep);
    }

    private IEnumerator ExitSafeAreaStepCoroutine(float delayTime, Action onExitComplete)
    {
        yield return new WaitForSeconds(delayTime);
        onExitComplete?.Invoke();
        ExitSafeAreaStep();
    }
}
