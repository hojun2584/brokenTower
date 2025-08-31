using Hojun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoomCameraController : MonoBehaviour
{

    public void Start()
    {
    }

    public void CameraViewSetting()
    {
        gameObject.GetComponent<Transform>().rotation = Quaternion.Euler(90f, 180f, 0f);
        Tower swaper = GamePlayManager.Instance.towers[0];
        GamePlayManager.Instance.towers[0] = GamePlayManager.Instance.towers[1];
        GamePlayManager.Instance.towers[1] = swaper;
    }   

}
