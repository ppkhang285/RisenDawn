using Mono.Cecil.Cil;
using NaughtyAttributes;

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;


public class RoomInfo
{
    public Vector2 center;
    public int width;
    public int height;

    public RoomInfo(Vector2 center, int width, int height)
    {
        this.center = center;
        this.width = width;
        this.height = height;
    }

    public float Left()
    {
        return (float)center.x - width/2;
    }

    public float Right()
    {
        return (float)center.x + width / 2;
    }

    public float Top()
    {
        return (float)center.y + height / 2;
    }

    public float Bottom()
    {
        return (float)center.y - height / 2;
    }
}

/*
 * Map vertex = Room
 */
public class MVertex
{
    public Vector2 position;
    public List<MVertex> adjacents;

    public MVertex(Vector2 position)
    {
        this.position = position;
        adjacents = new List<MVertex>();
    }
    public MVertex(Vector2 position, List<MVertex> adjacents)
    {
        this.position = position;
        this.adjacents = adjacents;
    }

    public void addAdjacent(MVertex vertex)
    {
        if (!adjacents.Contains(vertex))
        {
            adjacents.Add(vertex);
        }
    }

    public bool Equal(MVertex vertex)
    {
        return position == vertex.position;
    }


}

/*
 * Map Edge = Hallway
 */
public class MEdge
{
    public MVertex v0;
    public MVertex v1;

    public MEdge(MVertex v0, MVertex v1)
    {
        this.v0 = v0;
        this.v1 = v1;
    }
    public bool Equal(MEdge other)
    {
        return (v0.Equal(other.v0) && v1.Equal(other.v1))
            || (v1.Equal(other.v0) && v0.Equal(other.v1));
    }

}

public class MapSpawnUtils : MonoBehaviour
{
    public float spawnCircleRadius;
    public Vector2 roomWidthRange;
    public Vector2 roomHeightRange;

    public int roomNumber;

    public float egdeOffset = 1f;
    //
    private int count = 0;
    private int separateLoopCount = 1000;

    private bool separateComplete  = false;
    private bool drawLock = true;

    private List<Vector2> roomPosList;
    private List<RoomInfo> roomInfos;
    private List<RoomInfo> mainRooms;

    private float tile_size = 4;
    private float heightThreshold = 0;
    private float widthThreshold = 0;

    private List<MVertex> vertices;
    private List<MEdge> edges;
    private List<MEdge> mininumEdges;

    public void Start()
    {
        Setup();
        
    }
    private void OnDrawGizmos()
    {
        // Only draw when unlocking
        // Prevent error when not running Unity
        if (!drawLock)
        {
            DrawRooms();
         
            DrawEdge();
        }
    }
    /*
     * DRAWING METHODS
     */
    private void DrawRooms()
    {
        Gizmos.color = Color.green;

        foreach (var roomInfo in roomInfos)
        {

            Vector3 size = new Vector3(roomInfo.width, roomInfo.height, 0);

            Gizmos.DrawWireCube(roomInfo.center, size);
        }

    }


    private void DrawMainRooms()
    {
        Gizmos.color = Color.red;

        foreach (var roomInfo in mainRooms)
        {
            Vector3 size = new Vector3(roomInfo.width, roomInfo.height, 0);

            Gizmos.DrawWireCube(roomInfo.center, size);
        }
    }

    private void DrawEdge()
    {

        // Draw Edge for Spanning Tree
        Gizmos.color = Color.yellow;

        foreach(var edge in mininumEdges)
        {
            Vector3 v1 = edge.v0.position;
            Vector3 v2 = edge.v1.position;

            Gizmos.DrawLine(v1, v2);
        }
    }

    

    private void Setup()
    {
        roomPosList = new List<Vector2>();
        roomInfos = new List<RoomInfo>();
        mainRooms = new List<RoomInfo>();
        vertices = new List<MVertex>();
        edges = new List<MEdge>();
        mininumEdges = new List<MEdge>();
    }
    private void ClearData()
    {
        count = 0;
        widthThreshold = 0;
        heightThreshold = 0;
        separateComplete = false;

        roomPosList.Clear();
        roomInfos.Clear();
        mainRooms.Clear();
        vertices.Clear();
        edges.Clear();
        mininumEdges.Clear();
    }

    [Button]
    private void Spawn()
    {
        ClearData();
        SpawnRooms();
        SeparateRooms();
        ChooseMainRooms();
    }

    [Button]
    private void Calculate()
    {

        CreateGraph();
        
    }
    
    
    public float roundM(float n, float m)
    {
        if (n >= 0)
        {
            return Mathf.FloorToInt((n + m - 1) / (float)m) * m;
        }
        else
        {
            return Mathf.FloorToInt(n / (float)m) * m;
        }
    }

    public Vector2 roundM(Vector2 vector)
    {
     
        return new Vector2(Mathf.Round(vector.x), Mathf.Round(vector.y));
    }
    public Vector2 getRandomPointInCircle(float radius)
    {
        float t = 2 * Mathf.PI * Random.Range(0f, 1f);
        float u = Random.Range(0, 1) + Random.Range(0f, 1f);
        float r = 0;

        if (u > 1)
        {
            r = 2 - u;
        }
        else
        {
            r = u;
        }

        float x = roundM(radius * r * Mathf.Cos(t), tile_size);
        float y = roundM(radius * r * Mathf.Sin(t), tile_size);

        return new Vector2(x, y);
    }
    private void SpawnRooms()
    {
        drawLock = false;

        for (int i = 0; i < roomNumber; i++)
        {
            roomPosList.Add(getRandomPointInCircle(spawnCircleRadius));
        }

        foreach (var roomPos in roomPosList)
        {
            int width =  Mathf.FloorToInt(Random.Range(roomWidthRange.x, roomWidthRange.y));
            int height = Mathf.FloorToInt(Random.Range(roomHeightRange.x, roomHeightRange.y));

            width = (int)roundM(width, tile_size);
            height = (int)roundM(height, tile_size);

            widthThreshold += width;
            heightThreshold += height;

            roomInfos.Add(new RoomInfo(roomPos, width, height));

        }

        widthThreshold = widthThreshold / roomNumber;
        heightThreshold = heightThreshold / roomNumber;

    }
    
    
            
    private bool CheckOverLap(RoomInfo room1, RoomInfo room2)
    {
        // Left, right, Up, Down
        if (room1.Left()  > room2.Right() + egdeOffset) return false;
        if (room1.Right() + egdeOffset < room2.Left() ) return false;
        if (room1.Top() + egdeOffset < room2.Bottom() ) return false;
        if (room1.Bottom() > room2.Top() + egdeOffset) return false;

        return true;
    }

    private void SeparateRooms()
    {

        while (!separateComplete && count <= separateLoopCount)
        {
            count++;
            separateComplete = true;

            foreach (RoomInfo room1 in roomInfos)
            {
                foreach (RoomInfo room2 in roomInfos)
                {
                    if (room1.center == room2.center || !CheckOverLap(room1, room2))
                    {
                        continue;
                    }
                    separateComplete = false;

                    Vector2 disVector = room2.center - room1.center;
                    disVector = roundM(disVector.normalized);
                    room1.center -= disVector;
                    room2.center += disVector;

                }
            }
            
        }


    }
    private IEnumerator Co_SperateRoom()
    {

        while (!separateComplete && count <= separateLoopCount)
        {
            count++;
            separateComplete = true;

            foreach(RoomInfo room1 in roomInfos)
            {
                foreach (RoomInfo room2 in roomInfos)
                {
                    if (room1.center == room2.center || !CheckOverLap(room1, room2))
                    {
                        continue;
                    }
                    separateComplete=false;

                    Vector2 disVector = room2.center - room1.center;
                    disVector = roundM(disVector.normalized);
                    room1.center -= disVector;
                    room2.center += disVector;
                    
                }
            }
            yield return null;//new WaitForSeconds(0.05f);
        }

        
    }


    private void ChooseMainRooms()
    {
        foreach(var room in roomInfos)
        {
            if (room.width > widthThreshold && room.height > heightThreshold)
            {
                mainRooms.Add(room);
            }
        }
    }

    private void CreateGraph()
    {
        List<Vector2> tempList = new List<Vector2>();

        foreach(var room in roomInfos)
        {
            tempList.Add(room.center);
        }

        // Algorithm to create Edge list of Graph
        // Remove it to new Algorithm
        FindTriangulation(tempList);

        // Finding mininum spanning tree of Graph to make path
        FindSpanningTree();

    }
    
    private void FindTriangulation(List<Vector2> vertexList)
    {
        List<Triangle> triangles = DelaunayTriangulation.Triangulate(vertexList);
        

        foreach (var tri in triangles)
        {
            MVertex v0 = FindVertex(tri.v0, vertices);
            MVertex v1 = FindVertex(tri.v1, vertices);
            MVertex v2 = FindVertex(tri.v2, vertices);

            edges.Add(new MEdge(v0, v1));
            edges.Add(new MEdge(v1, v2));
            edges.Add(new MEdge(v0, v2));
        }

    }

    /*
     * Find if a vertex appear in List
     * Return Vertex if true
     * Else:
     * - Add new Vertex to List
     * - Return new Vertex
     */
    private MVertex FindVertex(Vector2 v, List<MVertex> vertices)
    {
        foreach(var vertex in vertices)
        {
            if (vertex.position == v) return vertex;
        }

        MVertex newVertex = new MVertex(v);
        vertices.Add(newVertex);

        return newVertex;
    }
    private bool Contains(MEdge inEdge, List<MEdge> edges)
    {
     
        foreach(var edge in edges)
        {
            if (edge.Equal(inEdge))
            {
                return true;
            }
        }

        return false;
    }

    private bool Contains(MVertex inVertex, List<MVertex> vertices)
    {

        foreach (var vertex in vertices)
        {
            if (vertex.Equal(inVertex))
            {
                return true;
            }
        }

        return false;
    }
    /*
     * Find Min Spanning tree
     * Using Kruskal Algorithm
     */
    private void FindSpanningTree()
    {
        Dictionary<int, int> parent = new Dictionary<int, int>();
        Dictionary<MVertex, int> id = new Dictionary<MVertex, int>();

        for(int i = 0; i < vertices.Count; i++)
        {
            MVertex v = vertices[i];

            id.Add(v, i);
            parent.Add(i, i);

        }

        

        foreach (var edge in edges)
        {
            int idV0 = id[edge.v0];
            int idV1 = id[edge.v1];

            while (parent[idV0] != idV0) idV0 = parent[idV0];
            while (parent[idV1] != idV1) idV1 = parent[idV1];
            
            if (idV0 != idV1)
            {
                mininumEdges.Add(edge);
                
                if (idV0 < idV1)
                {
                    parent[idV1] = id[edge.v0];
                }
                else
                {
                    parent[idV0] = id[edge.v1];
                }
            }
        }
    }
    



}
