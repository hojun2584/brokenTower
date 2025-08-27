using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerCore;
using CustomClient;
using System.Net;
using UnityEngine.SceneManagement;
using CustomPacket;

public class NetworkManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static NetworkManager instance;
    public ServerSession session = new ServerSession();

    private void Awake()
    {

        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);

        ConnServer();
    }

    private void ConnServer()
    {
        string host = Dns.GetHostName();
        IPHostEntry iPHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = iPHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
        Connector connector = new Connector();


        connector.Connect(endPoint, () =>
        {
            return session;
        });
        Debug.Log("connect...");
    }

    public void Update()
    {
        if (JobQueue.Instance.jobActions.Count != 0)
        {
            JobQueue.Instance.jobActions.Dequeue().Invoke();
        }
    }


    public void LobbyEnter()
    {
        LobbyEnterPacket lobbyPacket = new LobbyEnterPacket();

        session.Send(lobbyPacket.Write());
    }

}
