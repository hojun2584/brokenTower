using CustomPacket;
using Server;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    public event Action CreateRoomClick;
    public event Action roomListSet;
    List<GameRoom> roomList;
    public Action ChangeCurrentRoom;

    public GameRoom CurrentGameRoom 
    { get => currentGameRoom; 
        set
        {
            currentGameRoom = value;
            ChangeCurrentRoom?.Invoke();
        }
    }
    GameRoom currentGameRoom;

    public List<GameRoom> RoomList 
    {
        get => roomList;
        set
        {
            roomList = value;
            roomListSet?.Invoke();
        }
    }


    public void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(this);
        
        roomList = new List<GameRoom>();
        DontDestroyOnLoad(this);
    }

    public void Start()
    {
        ChangeCurrentRoom += () => { Debug.Log("Current Room Changed"); };
        GameManager.Instance.enterLobby += EnterLobby;
    }

    public void Update()
    {
        
    }

    public void EnterLobby()
    {
        NetworkManager.instance.session.Send(new LobbyEnterPacket().Write());
    }

    public void CreateGameRoom()
    {
        CreateRoomClick?.Invoke();
    }


}
