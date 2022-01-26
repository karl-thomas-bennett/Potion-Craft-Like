using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class Line : MonoBehaviour
{
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> points = new List<Vector2>();
    public List<Vector2> oldPoints = new List<Vector2>();

    private readonly int lineEndRoundness = 10;

    void Update()
    {
        if (points.Count > 1 && !ListsAreEqual(points, oldPoints))
        {
            vertices.Clear();
            triangles.Clear();
            AddStart();
            AddMiddle();
            AddEnd();
            SetMesh();

            oldPoints = new List<Vector2>(points);
        }
    }

    public bool ListsAreEqual(List<Vector2> a, List<Vector2> b)
    {
        if(a.Count != b.Count)
        {
            return false;
        }
        for(int i = 0; i < a.Count; i++)
        {
            if(a[i] != b[i])
            {
                return false;
            }
        }
        return true;
    }

    public void AddStart()
    {
        Vector2 lineDirection = (points[1] - points[0]).normalized;
        Vector2 perpendicular = Vector2.Perpendicular(lineDirection);
        for(int i = 0; i < lineEndRoundness; i++)
        {
            vertices.Add(points[0] + Rotate(-perpendicular, -i * Mathf.PI / (lineEndRoundness - 1)));
        }
        for(int i = 0; i < lineEndRoundness - 2; i++)
        {
            triangles.Add(0); triangles.Add(i + 1); triangles.Add(i + 2);
        }
    }
    public void AddMiddle()
    {

        for(int i = 0; i < points.Count - 1; i++)
        {
            Vector2 lineIn = (points[i + 1] - points[i]).normalized;
            if (i != points.Count - 2)
            {
                Vector2 lineOut = (points[i + 2] - points[i + 1]).normalized;

                Vector2 toPoint = (-lineIn + lineOut).normalized;
                if(toPoint.x == 0 && toPoint.y == 0)
                {
                    toPoint = Vector2.Perpendicular(lineIn);
                }
                Vector2 point = points[i + 1] + toPoint;

                if (IsLeft(point - points[i], lineIn))
                {
                    
                    vertices.Add(points[i + 1] + toPoint);
                    vertices.Add(points[i + 1] - toPoint);
                }
                else
                {
                    vertices.Add(points[i + 1] - toPoint);
                    vertices.Add(points[i + 1] + toPoint);
                }
            }

            if(points.Count == 2)
            {
                triangles.Add(lineEndRoundness - 1); triangles.Add(vertices.Count); triangles.Add(0);
                triangles.Add(lineEndRoundness - 1); triangles.Add(vertices.Count + lineEndRoundness - 1); triangles.Add(vertices.Count); 
            }
            
            if(i == 0)
            {
                //connect start
                triangles.Add(lineEndRoundness - 1); triangles.Add(vertices.Count - 2); triangles.Add(0);
                triangles.Add(lineEndRoundness - 1); triangles.Add(vertices.Count - 1); triangles.Add(vertices.Count - 2); 
            }
            else if(i == points.Count - 2)
            {
                //connect end
                triangles.Add(vertices.Count); triangles.Add(vertices.Count - 2); triangles.Add(vertices.Count + lineEndRoundness - 1);
                triangles.Add(vertices.Count - 2); triangles.Add(vertices.Count - 1); triangles.Add(vertices.Count + lineEndRoundness - 1);
            }
            else
            {
                triangles.Add(vertices.Count - 3); triangles.Add(vertices.Count - 2); triangles.Add(vertices.Count - 4);  
                triangles.Add(vertices.Count - 3); triangles.Add(vertices.Count - 1); triangles.Add(vertices.Count - 2); 
            }
            
        }
    }

    private bool IsLeft(Vector2 A, Vector2 B)
    {
        return -A.x * B.y + A.y * B.x < 0;
    }
    public void AddEnd()
    {
        Vector2 lineDirection = (points[points.Count - 1] - points[points.Count - 2]).normalized;
        Vector2 perpendicular = Vector2.Perpendicular(lineDirection);
        for (int i = 0; i < lineEndRoundness; i++)
        {
            Vector2 rotated = Rotate(-perpendicular, i * Mathf.PI / (lineEndRoundness - 1));
            vertices.Add(points[points.Count - 1] + rotated);
        }
        for (int i = 0; i < lineEndRoundness - 2; i++)
        {
            triangles.Add(vertices.Count - lineEndRoundness + i + 2); triangles.Add(vertices.Count - lineEndRoundness + i + 1); triangles.Add(vertices.Count - lineEndRoundness); 
        }
    }

    public Vector2 Rotate(Vector2 v, float angle)
    {
        return new Vector2
        (
            v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle),
            v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle)
        );
    }

    public void SetMesh()
    {
        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray()
        };
        if (TryGetComponent(out MeshFilter filter))
        {
            filter.mesh = mesh;
        }
        else
        {
            gameObject.AddComponent<MeshFilter>();
            MeshFilter f = GetComponent<MeshFilter>();
            f.mesh = mesh;
        }
        if (!TryGetComponent(out MeshRenderer r))
        {
            gameObject.AddComponent<MeshRenderer>();
        }
    }
}
