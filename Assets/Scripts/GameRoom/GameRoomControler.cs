using Assets.Scripts;
using CustomPacket;
using Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoomControler : MonoBehaviour
{
    
    public List<PlayerInfoViewer> infoList;


    public GameRoom Room { 
        get => room; 
        set
        {
            room = value;
            List<PlayerStruct> playerList = room.GetPlayerList();


            for (int i = 0; i < infoList.Count && i < playerList.Count; i++)
            {
                infoList[i].PlayerInfo = playerList[i];
            }
        }
    }
    GameRoom room;

    public void Awake()
    {
        LobbyManager.Instance.ChangeCurrentRoom += UpdatePlayer;
    }

    public void Start()
    {
        Room = LobbyManager.Instance.CurrentGameRoom;
    }

    public void UpdatePlayer()
    {
        Room = LobbyManager.Instance.CurrentGameRoom;
    }


    public void StartButton()
    {
        if ( room.IsStartAble() )
        {
            
            StartGamePacket packet = new StartGamePacket(this.room.roomNum ,Room.roomMaster);
            packet.roomNum = room.roomNum;
            
            NetworkManager.instance.session.Send(packet.Write());


        }
    }



}
