using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo
{
    public Vector2 center;
    public Vector2Int position;
    public int width;
    public int height;
    public int index;
    

    public RoomInfo(Vector2 center, int width, int height)
    {
        this.center = center;
        this.width = width;
        this.height = height;
    }

    public float Left()
    {
        return (float)center.x - width / 2;
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

    /// <summary>
    /// Get Center from Position
    /// </summary>
    /// <returns></returns>
    public Vector2 Center()
    {
        return position - (Vector2.up * height/2) + (Vector2.right * width/2);
    }
}

