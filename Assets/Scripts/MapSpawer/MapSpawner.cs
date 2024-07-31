using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner
{ 
    private MapConfig mapConfig;

    // Vertices and Edges for minimap / terminal
    public List<MVertex> vertices;
    public List<MEdge> edges;
    public RoomInfo startRoom;

    // Room list for spawn in world space
    public List<RoomInfo> roomList;
    public Dictionary<MVertex, RoomInfo> vertexToRoomDic;
    private int roomNumber;
    private float maxRoomWidth;
    private float maxRoomHeight;
    private float gridWidth;
    private float gridHeight;
    private float roomOffset = 12f;

    private MapSpawnUtils mapSpawnUtils;
    private TilePainter tilePainter;
    

    public MapSpawner(MapConfig mapConfig, TilePainter tilePainter)
    {
        this.mapConfig = mapConfig;
        this.tilePainter = tilePainter;
        mapSpawnUtils = new MapSpawnUtils();
    }


    public void SpawnMap()
    {
        GetSpawnData();
        DrawWorldMap();
    }

    /// <summary>
    /// Spawn map in World position
    /// </summary>
    private void DrawWorldMap()
    {
        int gridX = 0;
        int gridY = 0;

        foreach(MVertex v in vertices)
        {

            if (gridX >= gridWidth)
            {
                gridX = 0;
                gridY++;
            }

            Vector2 worldPos = GetWorldPosFromGrid(gridX, gridY);
            RoomInfo room = vertexToRoomDic[v];

            room.position = Vector2Int.RoundToInt(worldPos);



            tilePainter.PaintRoomFoundation(room);

            gridX++;

            
        }
    }

    private void GetSpawnData()
    {
        mapSpawnUtils.SpawnMap(mapConfig);
        vertices = mapSpawnUtils.GetVertexList();
        edges = mapSpawnUtils.GetEdgeList();
        roomList = mapSpawnUtils.GetRoomList();
        vertexToRoomDic = mapSpawnUtils.GetVertexToRoomDic();

        roomNumber = roomList.Count;
        maxRoomHeight = mapSpawnUtils.maxHeight + roomOffset;
        maxRoomWidth = mapSpawnUtils.maxWidth + roomOffset;

        // Find grid size from roomNumber
        float sqrt = Mathf.Sqrt(roomNumber);
        gridHeight = Mathf.FloorToInt(sqrt);
        gridWidth = Mathf.CeilToInt((float)roomNumber / gridHeight);

        startRoom = roomList[0];
    }

    /// <summary>
    /// Return Top-Left of room's World position from Grid position
    /// </summary>
    private Vector2 GetWorldPosFromGrid(int x, int y)
    {
        int newX = x * (int)maxRoomWidth;
        int newY = (y+1) * (int)maxRoomHeight;
        return Vector2.right * newX + Vector2.up * newY;
    }

}
