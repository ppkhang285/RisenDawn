using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
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
public class TestUtils : MonoBehaviour
{
    public float spawnCircleRadius;
    public Vector2 roomWidthRange;
    public Vector2 roomHeightRange;

    public int roomNumber;


    //
    private int separateLoopCount = 1000;
    private bool separateComplete  = false;
    private bool drawLock = true;
    private List<Vector2> roomPosList;
    private List<RoomInfo> roomInfos;
    public void Start()
    {
        Setup();
        
    }

    private void Setup()
    {
        roomPosList = new List<Vector2>();
        roomInfos = new List<RoomInfo>();
    }

    [Button]
    private void Spawn()
    {
        ClearData();
        SpawnRooms(); 

    }
    private void ClearData()
    {
        roomPosList.Clear();
        roomInfos.Clear();
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
        return new Vector2(radius * r * Mathf.Cos(t), radius * r * Mathf.Sin(t));
    }
    private void SpawnRooms()
    {
        for (int i = 0; i < roomNumber; i++)
        {

            roomPosList.Add(getRandomPointInCircle(spawnCircleRadius));
        }
        foreach (var roomPos in roomPosList)
        {
            int width = Mathf.FloorToInt(Random.Range(roomWidthRange.x, roomWidthRange.y));
            int height = Mathf.FloorToInt(Random.Range(roomHeightRange.x, roomHeightRange.y));

            roomInfos.Add(new RoomInfo(roomPos, width, height));

        }

    }
    private void OnDrawGizmos()
    {
        SeparateRooms();
        DrawRooms();
    }
    private void DrawRooms()
    {
        Gizmos.color = Color.green;
        foreach(var roomInfo in roomInfos)
        {
            Vector3 size = new Vector3(roomInfo.width, roomInfo.height, 0);
            Gizmos.DrawWireCube(roomInfo.center, size);
        }
    }
    private void SeparateRooms()
    {
        if (separateComplete) return;
        int count = 0;
        while (!separateComplete)
        {
            Debug.Log(separateComplete);
            count++;
            if (count > separateLoopCount) break;
            // Do somethign
            separateComplete = true;
   
            for(int i = 0; i < roomInfos.Count-1; i++)
            {

                for(int j = i+1; j < roomInfos.Count; j++)
                {
                    if (!CheckOverLap(roomInfos[i], roomInfos[j]))
                    {
                        Debug.Log("No dell overlap");
                        continue;

                    }
                    separateComplete = false;
                    Vector2 disVector = roomInfos[i].center - roomInfos[j].center;
                    roomInfos[i].center += disVector;
                    roomInfos[j].center -= disVector;
                    Debug.Log(roomInfos[i].center + " " + roomInfos[j].center);
                }
            }
        }

    }

    private bool CheckOverLap(RoomInfo room1, RoomInfo room2)
    {
        // Left, right, Up, Down
        if (room1.Left() > room2.Right()) return false;
        if (room1.Right() < room2.Left()) return false;
        if (room1.Top() < room2.Bottom()) return false;
        if (room1.Bottom() > room2.Top()) return false;
        return true;
    }

    
    
}
