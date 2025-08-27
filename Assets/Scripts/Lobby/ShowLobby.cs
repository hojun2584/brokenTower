using System.Collections;
using System.Collections.Generic;
using TMPro;
using Hojun;
using UnityEngine;
using Server;
using UnityEngine.SceneManagement;
using ServerCore;
using CustomClient;
using CustomPacket;
using UnityEngine.Experimental.AI;

public class ShowLobby : MonoBehaviour
{
    public GameObject roomPrefab; 
    public Transform canvasTransform; 
    public List<GameRoom> infoList;

    private List<GameObject> roomInfoList;

    void Start()
    {
        LobbyManager.Instance.roomListSet += DisplayListOnCanvas;
        roomInfoList = new List<GameObject>();
        infoList = new List<GameRoom>();

    }

    void DisplayListOnCanvas()
    {
        
        foreach (var item in roomInfoList)
        {
            Destroy(item);
        }

        foreach (GameRoom info in LobbyManager.Instance.RoomList)
        {
            // TextMeshPro 텍스트 오브젝트 생성
            GameObject textObject = Instantiate(roomPrefab, canvasTransform);
            LobbyBoxData roomData = textObject.GetComponent<LobbyBoxData>();

            roomData.roomNameText.text = info.roomName;

            roomData.buttonClick += () => {

                Debug.Log("button click");
                EnterRoomPacket enterRoom = new EnterRoomPacket.Builder().SetRoomNum(info.roomNum).Build();

                NetworkManager.instance.session.Send(enterRoom.Write());

            };

            roomInfoList.Add(textObject);
        }
    }
}
