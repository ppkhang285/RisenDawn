using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera _camera;

    public void Setup()
    {
        _camera = Camera.main;
    }


    /// <summary>
    /// Change Camera to new room
    /// </summary>
    /// <param name="room"></param>
    public void ChangeRoom(RoomInfo room)
    {
        _camera.transform.position = new Vector3(room.Center().x, room.Center().y, _camera.transform.position.z);
    }
}
