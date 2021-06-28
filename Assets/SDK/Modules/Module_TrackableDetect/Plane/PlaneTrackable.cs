using UnityEngine;

public class PlaneTrackable : Trackable
{
    private Vector3[] vertices;

    public PlaneTrackable(int id, Vector3[] vertices) : base(id)
    {
        this.vertices = vertices;
    }

    public Vector3[] GetPolygonVertices()
    {
        return vertices;
    }

    public void UpdateVertices(Vector3[] newVertices)
    {
        this.vertices = newVertices;
    }
}