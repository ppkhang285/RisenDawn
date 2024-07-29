using Mono.Cecil.Cil;
using NaughtyAttributes;

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;




public class MapSpawnUtils
{

    private Vector2 roomWidthRange;
    private Vector2 roomHeightRange;

    private int roomNumber;

    private float spawnCircleRadius = 1f;
    private float egdeOffset = 3f;
    private int count = 0;
    private int separateLoopCount = 1000;

    private bool separateComplete  = false;


    private List<Vector2> roomPosList;
    private List<RoomInfo> roomInfos;

    private float tile_size = 4;
    private float heightThreshold = 0;
    private float widthThreshold = 0;

    private List<MVertex> vertices;
    private List<MEdge> edges;
    private List<MEdge> mininumEdges;

    private Dictionary<Vector2, RoomInfo> posToRoomDic;
    private Dictionary<MVertex, RoomInfo> vertexToRoomDic;

    public float maxHeight = 0;
    public float maxWidth = 0;

    public void SpawnMap(MapConfig mapSpawnConfig)
    {
        // Setup 
        roomHeightRange = mapSpawnConfig.roomHeightRange;
        roomWidthRange = mapSpawnConfig.roomWidthRange;
        roomNumber = mapSpawnConfig.roomNumber;

        Setup();

        // Call methods to spawn map
        Spawn();
    }

    
    

    private void Setup()
    {
        roomPosList = new List<Vector2>();
        roomInfos = new List<RoomInfo>();
        vertices = new List<MVertex>();
        edges = new List<MEdge>();
        mininumEdges = new List<MEdge>();

        posToRoomDic = new Dictionary<Vector2, RoomInfo>();
        vertexToRoomDic = new Dictionary<MVertex, RoomInfo>();
    }
    private void ClearData()
    {
        count = 0;
        widthThreshold = 0;
        heightThreshold = 0;
        separateComplete = false;

        roomPosList.Clear();
        roomInfos.Clear();
        vertices.Clear();
        edges.Clear();
        mininumEdges.Clear();
    }


    private void Spawn()
    {
        ClearData();
        SpawnRooms();
        SeparateRooms();
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

            maxHeight = Mathf.Max(maxHeight, height);
            maxWidth = Mathf.Max(maxWidth, width);

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


    private void CreateGraph()
    {
        List<Vector2> tempList = new List<Vector2>();

        foreach(var room in roomInfos)
        {
            tempList.Add(room.center);
            posToRoomDic.TryAdd(room.center, room);
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



            vertexToRoomDic.TryAdd(v0, posToRoomDic[v0.position]);
            vertexToRoomDic.TryAdd(v1, posToRoomDic[v1.position]);
            vertexToRoomDic.TryAdd(v2, posToRoomDic[v2.position]);

            edges.Add(new MEdge(v0, v1));
            edges.Add(new MEdge(v1, v2));
            edges.Add(new MEdge(v0, v2));
        }

    }

    /** <Summary>
     * Find if a vertex appear in List
     * </Summary>
     * <Returns name="vertex">
     * Return Vertex if true <br/>
     * Else: <br/>
     * - Add new Vertex to List <br/>
     * - Return new Vertex<br/>
     * </Returns>
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
    
    /// <summary>
    /// Find Min Spanning tree using Kruskal Algorithm
    /// </summary>
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
                edge.v0.addAdjacent(edge.v1);
                edge.v1.addAdjacent(edge.v0);

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
    

    public List<MEdge> GetEdgeList()
    {
        return mininumEdges;
    }

    public List<MVertex> GetVertexList()
    {
        return vertices;
    }

    public List<RoomInfo> GetRoomList()
    {
        return roomInfos;
    }
    
    public Dictionary<MVertex, RoomInfo> GetVertexToRoomDic()
    {
        return vertexToRoomDic;
    }
}
