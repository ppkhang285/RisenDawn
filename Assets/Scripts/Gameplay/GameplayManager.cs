using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public LevelManager levelManager;
    public static TilePainter tilePainter;
    public GameObject playerPrefab;
    


    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        levelManager.Setup();
    }

    [Button]
    private void Test()
    {
        levelManager.GenerateMap();
        Instantiate(playerPrefab, levelManager.startRoom.Center(), Quaternion.identity);
        Debug.Log(levelManager.startRoom.position);
        
    }
}
