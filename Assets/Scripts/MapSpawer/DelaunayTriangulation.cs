using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;




/*
 * Vertex class can use Vector2 instead
 */

public class Circle
{
    public Vector2 center;
    public float radius;

    public Circle(Vector2 center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }
}


public class Edge
{
    public Vector2 v0;
    public Vector2 v1;

    public Edge(Vector2 v0, Vector2 v1)
    {
        this.v0 = v0;
        this.v1 = v1;
    }
    public bool Equal(Edge other)
    {
        return (v0 == other.v0 && v1 == other.v1) 
            || (v1 == other.v0 && v0 == other.v1);
    }

}

public class Triangle
{
    public Vector2 v0;
    public Vector2 v1;
    public Vector2 v2;

    public Circle circumcircle;

    public Triangle(Vector2 v0, Vector2 v1, Vector2 v2)
    {
        this.v0 = v0; this.v1 = v1; this.v2 = v2;
        this.circumcircle = GetCircumcircle(v0, v1, v2);

    }

    public bool InCircumcircle(Vector2 v)
    {
        float dx = this.circumcircle.center.x - v.x;
        float dy = this.circumcircle.center.y - v.y;
        return Mathf.Sqrt(dx * dx + dy * dy) < this.circumcircle.radius;
    }

    public Circle GetCircumcircle(Vector2 pointA, Vector2 pointB, Vector2 pointC)
    {
        LinearEquation lineAB = new LinearEquation(pointA, pointB);
        LinearEquation lineBC = new LinearEquation(pointB, pointC);

        Vector2 midPointAB = Vector2.Lerp(pointA, pointB, 0.5f);
        Vector2 midPointBC = Vector2.Lerp(pointB, pointC, 0.5f);

        LinearEquation perpendicularAB = lineAB.PerpendicularLineAt(midPointAB);
        LinearEquation perpendicularBC = lineBC.PerpendicularLineAt(midPointBC);

        Vector2 circumcenter = GetCrossingPoint(perpendicularAB, perpendicularBC);
        float radius = Vector2.Distance(circumcenter, pointA);

        return new Circle(circumcenter, radius);
        
    }

    public UnityEngine.Vector2 GetCrossingPoint(LinearEquation line1, LinearEquation line2)
    {
        float A1 = line1._A;
        float A2 = line2._A;
        float B1 = line1._B;
        float B2 = line2._B;
        float C1 = line1._C;
        float C2 = line2._C;

        //
        float Determinant = A1 * B2 - A2 * B1;
        float DeterminantX = C1 * B2 - C2 * B1;
        float DeterminantY = A1 * C2 - A2 * C1;

        float x = DeterminantX / Determinant;
        float y = DeterminantY / Determinant;

        return new Vector2(x, y);

    }
}

public class  LinearEquation
{
    public float _A;
    public float _B;
    public float _C;

    public LinearEquation()
    {
        _A = 0f;
        _B = 0f;
        _C = 0f;
    }
    public LinearEquation(Vector2 pointA, Vector2 pointB)
    {
        float deltaX = pointB.x - pointA.x;
        float deltaY = pointB.y - pointA.y;
        _A = deltaY;
        _B = -deltaX;
        _C = _A * pointA.x + _B * pointA.y;
    }
    public LinearEquation PerpendicularLineAt(Vector3 point)
    {
        LinearEquation newLine = new LinearEquation();

        newLine._A = -_B;
        newLine._B = _A;
        newLine._C = newLine._A * point.x + newLine._B * point.y;

        return newLine;
    }
}

public static class DelaunayTriangulation
{
    
    public static Triangle SuperTriangle(List<Vector2> vertices)
    {
        float minx = float.MaxValue;
        float miny = float.MaxValue;
        float maxx = float.MinValue;
        float maxy = float.MinValue;
        foreach(Vector2 vertex in vertices) 
        { 
            minx = Mathf.Min(minx, vertex.x);
            miny = Mathf.Min(miny, vertex.y);
            maxx = Mathf.Max(maxx, vertex.x);
            maxy = Mathf.Max(maxy, vertex.y);
        }

        float dx = (maxx - minx) * 10;
        float dy = (maxy - miny) * 10;

        Vector2 v0 = new Vector2(minx - dx, miny - dy * 3);
        Vector2 v1 = new Vector2(minx - dx, maxy + dy);
        Vector2 v2 = new Vector2(maxx + dx * 3, maxy + dy);

        return new Triangle(v0, v1, v2);
    }

    public static List<Triangle> Triangulate(List<Vector2> vertices)
    {
        Triangle superTriangle = SuperTriangle(vertices);

        List<Triangle> triangles = new List<Triangle>
        {
            superTriangle
        };

        //Triangulate each vertex
        foreach(Vector2 vertex in vertices)
        {
            triangles = AddVertex(vertex, triangles);
        }

        // Remove triangles share edge with super
        triangles = triangles.FindAll(triangle =>
        {
            return !(triangle.v0 == superTriangle.v0 || triangle.v0 == superTriangle.v1 || 
            triangle.v0 == superTriangle.v2 || triangle.v1 == superTriangle.v0 || 
            triangle.v1 == superTriangle.v1 || triangle.v1 == superTriangle.v2 ||
            triangle.v2 == superTriangle.v0 || triangle.v2 == superTriangle.v1 || 
            triangle.v2 == superTriangle.v2);
        });

        return triangles;
           
    }

    public static List<Triangle> AddVertex(Vector2 vertex, List<Triangle> triangles)
    {
        List<Edge> edges = new List<Edge>();
   

        //Remove triangles
        triangles = triangles.FindAll(triangle =>
        {
            if (triangle.InCircumcircle(vertex))
            {
                edges.Add(new Edge(triangle.v0, triangle.v1));
                edges.Add(new Edge(triangle.v1, triangle.v2));
                edges.Add(new Edge(triangle.v2, triangle.v0));
                //addingTriangles.Add(triangle);
                return false;
            }
            return true;
        });



        // Get unique edge
        edges = UniqueEdges(edges);

        //Create new triangle
        foreach (Edge edge in edges)
        {
            triangles.Add(new Triangle(edge.v0, edge.v1, vertex));
        }

        return triangles;
    }
    private static List<Edge> UniqueEdges(List<Edge> edges)
    {
        List<Edge> uniqueEdges = new List<Edge>();
        for(int i = 0; i < edges.Count; i++)
        {
            bool isUnique = true;

            for(int j = 0 ; j < edges.Count; j++)
            {
                if (i !=j && edges[i].Equal(edges[j]))
                {
                    isUnique = false;
                    break;
                }
            }

            if (isUnique)
            {
                uniqueEdges.Add(edges[i]);
            }
        }

        return uniqueEdges;
        
        
    }

}
