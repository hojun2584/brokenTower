using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomClient;
using CustomPacket;
using ServerCore;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine;
using System.Collections;
using Hojun;

namespace Assets.Scripts.Network.Packet
{
    public abstract class PacketHandler
    {
        protected ServerSession session;
        public PacketHandler(ServerSession session)
        {
            this.session = session;
        }
        public abstract void PacketHandle(ArraySegment<byte> buffer);
    }
    public class ConnetPacketHandle : PacketHandler
    {

        public ConnetPacketHandle(ServerSession session) : base(session)
        {   
        }

        public override void PacketHandle(ArraySegment<byte> buffer)
        {
            
            ConnectPacket packet = new ConnectPacket();
            packet.Read(buffer);
            NetworkManager.instance.session.SessionId = packet.sessionId;
        }
    }

    public class SignCompleHandle : PacketHandler
    {
        public SignCompleHandle(ServerSession session) : base(session)
        {

        }

        public override void PacketHandle(ArraySegment<byte> buffer)
        {
            SignCompletePacket packet = new SignCompletePacket();

            packet.Read(buffer);

            string floatingString = packet.isSuceess ? "Success" : "Fail";
            JobQueue.Instance.Add(() => { 
                FloatingBar.Instance.SetTmpText = floatingString;
                UnityEngine.Debug.Log(packet.isSuceess);
            });
            

        }
    }
    public class LoginResultHandle : PacketHandler
    {
        public LoginResultHandle(ServerSession session) : base(session)
        {
        }

        public override void PacketHandle(ArraySegment<byte> buffer)
        {
            LoginResultPacket packet = new LoginResultPacket();
            packet.Read(buffer);
            if (packet.ErrorCode == 0)
            {
                JobQueue.Instance.Add(() =>
                {
                    FloatingBar.Instance.SetTmpText = "Login Success";
                    SceneManager.LoadScene("Lobby");
                    GameManager.Instance.CurrentPlayer = packet.PlayerInfo;
                });
            }
            else
            {
                JobQueue.Instance.Add(() =>
                {
                    FloatingBar.Instance.SetTmpText = "Login Fail";
                });
            }

        }
    }
    public class RoomListHandle : PacketHandler
    {
        public RoomListHandle(ServerSession session) : base(session)
        {
        }

        public override void PacketHandle(ArraySegment<byte> buffer)
        {
            RoomListPacket packet = new RoomListPacket();
            packet.Read(buffer);

            JobQueue.Instance.Add(() => 
            { 
                LobbyManager.Instance.RoomList = packet.roomList;
            });


        }
    }
    public class EnterRoomHandle : PacketHandler
    {
        public EnterRoomHandle(ServerSession session) : base(session)
        {
        }

        public override void PacketHandle(ArraySegment<byte> buffer)
        {

            EnterRoomPacket packet = new EnterRoomPacket();
            packet.Read(buffer);

            JobQueue.Instance.Add(() =>
            {
                UnityEngine.Debug.Log("방에 들어간 다음에 해야 할 일들임");
                UnityEngine.Debug.Log(packet.roomNum);

                LobbyManager.Instance.CurrentGameRoom = packet.room;
                SceneManager.LoadScene("GameRoom");
            });
        }
    }

    public class StartGameResultHandle : PacketHandler
    {
        public StartGameResultHandle(ServerSession session) : base(session)
        {
        }
        public override void PacketHandle(ArraySegment<byte> buffer)
        {
            StartGameResultPacket packet = new StartGameResultPacket();
            packet.Read(buffer);
            JobQueue.Instance.Add(() =>
            {
                if (packet.isStart)
                {
                    SceneManager.LoadScene("GameScene");
                }
                else
                {
                    FloatingBar.Instance.SetTmpText = "Start Fail";
                }
            });
        }
    }

    public class ChangeRoomHandle : PacketHandler
    {
        public ChangeRoomHandle(ServerSession session) : base(session)
        {
        }

        public override void PacketHandle(ArraySegment<byte> buffer)
        {
            RoomPacket packet = new RoomPacket();
            packet.Read(buffer);

            JobQueue.Instance.Add(() => {
                LobbyManager.Instance.CurrentGameRoom = packet.roomInfo;
            });

        }
    }


    public class SummonResultHandle : PacketHandler
    {
        public SummonResultHandle(ServerSession session) : base(session)
        {
        }
        public override void PacketHandle(ArraySegment<byte> buffer)
        {
            SummonResultPacket packet = new SummonResultPacket();
            packet.Read(buffer);
            JobQueue.Instance.Add(() =>
            {
                UnityEngine.Debug.Log("Summon Packet Received");

                UnityEngine.Debug.Log(packet.nodeId);

                Node spawnNode = GamePlayManager.Instance.nodes.Find((x)=> x.NodeId == packet.nodeId);

                GamePlayManager.Instance.SpawnCharacter(spawnNode);
            });

            //SummondPacket packet = new SummondPacket();
            //packet.Read(buffer);
            //JobQueue.Instance.Add(() => {
            //    GamePlayManager gamePlayManager = GameObject.FindObjectOfType<GamePlayManager>();
            //    Node node = gamePlayManager.nodes.Where(x => x.NodeId == packet.nodeId).FirstOrDefault();
            //    if (node != null)
            //    {
            //        Vector3 spawnPosition = node.GetPositionSetY(5f);
            //        GameObject spawnCharater = GameObject.Instantiate(gamePlayManager.warrior, spawnPosition, Quaternion.identity);
            //        gamePlayManager.SetSpawn(spawnCharater);
            //    }
            //});
        }
    }
}
