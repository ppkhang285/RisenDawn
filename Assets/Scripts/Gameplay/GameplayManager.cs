using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    public LevelManager levelManager;
    public CameraController cameraController;

    public static TilePainter tilePainter;


    public GameObject playerPrefab;
    public GameObject player;

    public int testRoom;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        levelManager.Setup();
    }

    [Button]
    private void GenerateLevel()
    {
        levelManager.GenerateMap();

        player = Instantiate(playerPrefab, levelManager.startRoom.Center(), Quaternion.identity);

        cameraController.ChangeRoom(levelManager.startRoom);
        
    }

    [Button]
    private void Teleport()
    {
        RoomInfo room = levelManager.roomList[testRoom];

        player.transform.position = room.Center();
        
        cameraController.ChangeRoom(room); 
    }
}
