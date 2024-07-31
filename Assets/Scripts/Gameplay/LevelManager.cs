using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


/**
 * <summary>
 * Use to manage a Level <br/>
 * - Spawn Level <br/>
 * - Store Data of Vertice, Edges, Rooms <br/>
 * </summary>
 */
public class LevelManager : MonoBehaviour
{
    public TileBase tile;
    public Tilemap tileMap;
    public MapConfig mapConfig;

    [HideInInspector]public static TilePainter tilePainter;

    [HideInInspector]public List<MVertex> vertices;
    [HideInInspector]public List<RoomInfo> roomList;
    [HideInInspector]public RoomInfo startRoom;
    [HideInInspector] public Dictionary<MVertex, RoomInfo> vertexRoomDic;


    private MapSpawner mapSpawner;


    
    public void Setup()
    {
        tilePainter = new TilePainter(tileMap, tile);
        mapSpawner = new MapSpawner(mapConfig, tilePainter);
    }

    public void GenerateMap()
    {
        mapSpawner.SpawnMap();
        vertices = mapSpawner.vertices;
        roomList = mapSpawner.roomList;
        vertexRoomDic = mapSpawner.vertexToRoomDic;
        startRoom = mapSpawner.startRoom;
    }
}
