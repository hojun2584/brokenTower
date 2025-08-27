using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ServerCore;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using CustomPacket;
using Assets.Scripts.Network.Packet;
using UnityEngine.SceneManagement;
using Server;

namespace CustomClient
{

	public class ServerSession : Session
	{
		public Dictionary<ushort, PacketHandler> packetHandleDic = new Dictionary<ushort, PacketHandler>();
		public GameRoom currentRoom;


		public ServerSession()
		{
			packetHandleDic[(ushort)PacketID.SQLSIGNCOMPLETE] = new SignCompleHandle(this);
			packetHandleDic[(ushort)PacketID.SQLLOGINRESULT] = new LoginResultHandle(this);
			packetHandleDic[(ushort)PacketID.GAMEROOMINFO] = new RoomListHandle(this);
			packetHandleDic[(ushort)PacketID.INIT_PACKET] = new ConnetPacketHandle(this);
            packetHandleDic[(ushort)PacketID.ENTER_ROOM] = new EnterRoomHandle(this);
			packetHandleDic[(ushort)PacketID.CHANGE_ROOM] = new ChangeRoomHandle(this);
			packetHandleDic[(ushort)PacketID.GAME_START_RESULT] = new StartGameResultHandle(this);
			packetHandleDic[(ushort)PacketID.SUMMON] = new EnterRoomHandle(this);
            packetHandleDic[(ushort)PacketID.SUMMON_RESULT] = new SummonResultHandle(this);
        }

		public override void OnConnected(EndPoint endPoint)
		{
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
		}

		public override int OnRecv(ArraySegment<byte> buffer)
		{
            int pos = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            pos += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + pos);
            pos += 2;


            packetHandleDic[id].PacketHandle(buffer);

            JobQueue.Instance.Add(() => { Debug.Log(id + " receive"); });
            return buffer.Count;
		}

		public override void OnSend(int numOfBytes)
		{
			Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
	}

}
