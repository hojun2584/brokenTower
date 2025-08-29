using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoomCameraController : MonoBehaviour
{

    public void Start()
    {
        CameraViewSetting();
    }

    public void CameraViewSetting()
    {
        gameObject.GetComponent<Transform>().rotation = Quaternion.Euler(90f, 180f, 0f);
    }

}
