using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePainter : MonoBehaviour
{
    

    [SerializeField] private TileBase tile;
    [SerializeField] private Tilemap tilemap;

    public static TilePainter Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }


    /// <Summary>Paint wall and ground from room</Summary>
    public void PaintRoomFoundation(RoomInfo room)
    {
        Vector3Int start = new Vector3Int(room.position.x, room.position.y);
        Vector3Int end = start + new Vector3Int(room.width, room.height);


        BoxFill(tilemap, tile, start, end);
    }


    public void BoxFill(Tilemap map, TileBase tile, Vector3Int start, Vector3Int end)
    {
        //Determine directions on X and Y axis
        var xDir = start.x < end.x ? 1 : -1;
        var yDir = start.y < end.y ? 1 : -1;

        //How many tiles on each axis?
        int xCols = 1 + Mathf.Abs(start.x - end.x);
        int yCols = 1 + Mathf.Abs(start.y - end.y);

        //Start painting
        for (var x = 0; x < xCols; x++)
        {
            for (var y = 0; y < yCols; y++)
            {
                var tilePos = start + new Vector3Int(x * xDir, y * yDir, 0);
                map.SetTile(tilePos, tile);
            }
        }
    }

}
