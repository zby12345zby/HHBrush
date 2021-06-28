using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TrackablePlaneMono : MonoBehaviour, IPointerClickHandler
{
    private PlaneTrackable planeTrackable;
    private List<Vector3> meshVertices = new List<Vector3>();
    private List<Vector3> previousMeshVertices = new List<Vector3>();
    private List<int> meshIndices = new List<int>();
    private List<Color> meshColors = new List<Color>();

    private Mesh mesh;
    private MeshRenderer meshRenderer;

    public TextMeshPro verticesCountText;
    public GameObject clickPrefab;
    private MeshCollider collider;
    private Vector3 planeGravity = Vector3.zero;

    public GameObject vertexTextPrefab;
    private List<GameObject> vertexTextList = new List<GameObject>();

    public void Init(PlaneTrackable trackable)
    {
        this.planeTrackable = trackable;
    }

    /// <summary>
    /// The Unity Awake() method.
    /// </summary>
    public void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        //TODO
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (planeTrackable == null)
        {
            return;
        }

        UpdateMesh();
    }

    public void CreateOrHideVerticesText(List<Vector3> vertices)
    {
        while (vertexTextList.Count < vertices.Count)
        {
            GameObject vertexText =  GameObject.Instantiate(vertexTextPrefab, this.transform);
            vertexTextList.Add(vertexText);
        }

        for (int i = 0; i < vertexTextList.Count; i++)
        {
            vertexTextList[i].SetActive(true);
        }

        if (vertexTextList.Count > vertices.Count)
        {
            for (int i = vertices.Count; i < vertexTextList.Count; i++)
            {
                vertexTextList[i].SetActive(false);
            }
        }

        for (int i = 0; i < vertices.Count; i++)
        {
            vertexTextList[i].transform.position = vertices[i];
            vertexTextList[i].GetComponent<TextMeshPro>().text = string.Format("({0},{1},{2})", vertices[i].x, vertices[i].y, vertices[i].z);
        }
    }

    private void UpdateMesh()
    {
        Vector3[] vertices = planeTrackable.GetPolygonVertices();
        verticesCountText.text = vertices.Length.ToString();
        meshVertices.Clear();
        meshVertices.AddRange(vertices);

        if (SCPolygonUtils.AreVerticesEqual(meshVertices, previousMeshVertices))
        {
            return;
        }

        previousMeshVertices.Clear();
        previousMeshVertices.AddRange(meshVertices);

        CreateOrHideVerticesText(meshVertices);

        planeGravity = SCPolygonUtils.GetPolygonGravityPoint(vertices);

        verticesCountText.transform.localPosition = planeGravity + new Vector3(0f, 0.5f, 0f);

        int planePolygonCount = meshVertices.Count;

        meshColors.Clear();

        for (int i = 0; i < planePolygonCount; ++i)
        {
            meshColors.Add(Color.clear);
        }

        // Feather distance 0.2 meters.
        const float featherLength = 0.2f;

        // Feather scale over the distance between plane center and vertices.
        const float featherScale = 0.2f;

        // Add vertex 4 to 7.
        for (int i = 0; i < planePolygonCount; ++i)
        {
            Vector3 v = meshVertices[i];

            // Vector from plane center to current point
            Vector3 d = v - planeGravity;

            float scale = 1.0f - Mathf.Min(featherLength / d.magnitude, featherScale);
            meshVertices.Add((scale * d) + planeGravity);

            meshColors.Add(Color.white);
        }

        meshIndices.Clear();

        int firstOuterVertex = 0;
        int firstInnerVertex = planePolygonCount;

        // Generate triangle (4, 5, 6) and (4, 6, 7).
        for (int i = 0; i < planePolygonCount - 2; ++i)
        {
            meshIndices.Add(firstInnerVertex);
            meshIndices.Add(firstInnerVertex + i + 1);
            meshIndices.Add(firstInnerVertex + i + 2);
        }

        // Generate triangle (0, 1, 4), (4, 1, 5), (5, 1, 2), (5, 2, 6), (6, 2, 3), (6, 3, 7)
        // (7, 3, 0), (7, 0, 4)
        for (int i = 0; i < planePolygonCount; ++i)
        {
            int outerVertex1 = firstOuterVertex + i;
            int outerVertex2 = firstOuterVertex + ((i + 1) % planePolygonCount);
            int innerVertex1 = firstInnerVertex + i;
            int innerVertex2 = firstInnerVertex + ((i + 1) % planePolygonCount);

            meshIndices.Add(outerVertex1);
            meshIndices.Add(outerVertex2);
            meshIndices.Add(innerVertex1);

            meshIndices.Add(innerVertex1);
            meshIndices.Add(outerVertex2);
            meshIndices.Add(innerVertex2);
        }

        mesh.Clear();
        mesh.SetVertices(meshVertices);
        mesh.SetTriangles(meshIndices, 0);
        mesh.SetColors(meshColors);

        UpdateMeshCollider();
    }

    private void UpdateMeshCollider()
    {
        if (collider == null)
        {
            collider = this.gameObject.AddComponent<MeshCollider>();
        }
        collider.sharedMesh = mesh;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 instancePosition = eventData.pointerCurrentRaycast.worldPosition + Vector3.up * 0.3f;
        GameObject instanceObj = Instantiate(clickPrefab);
        instanceObj.transform.position = instancePosition;
        instanceObj.AddComponent<Rigidbody>();
    }
}
