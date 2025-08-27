using System.Collections;
using System.Collections.Generic;
using CustomPacket;
using Server;
using ServerCore;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyController : MonoBehaviour
{
    private bool isDataLoaded = false;

    public void CreateRoomButton()
    {

        CreateRoomPacket packet = new CreateRoomPacket.PacketBuilder().
            SetOwnerInfo(GameManager.Instance.CurrentPlayer).
            Build();
        NetworkManager.instance.session.Send(packet.Write());
    }

    public void RefreshRoomButton()
    {
        RequestRoomListPacket packet = new RequestRoomListPacket();
        NetworkManager.instance.session.Send(packet.Write());
    }


    IEnumerator RefreshRoomList()
    {
        while (true)
        {
            RequestRoomListPacket packet = new RequestRoomListPacket();
            NetworkManager.instance.session.Send(packet.Write());
            Debug.Log("refresh Room ");
            yield return new WaitForSeconds(5f);
        }
    }

    public void Start()
    {
        StartCoroutine(RefreshRoomList());
    }

    public void OnDestroy()
    {
        StopCoroutine(RefreshRoomList());
    }


}
