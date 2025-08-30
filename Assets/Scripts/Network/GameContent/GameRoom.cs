using Assets.Scripts;
using CustomClient;
using CustomPacket;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Server
{
    public class GameRoom : IPacketSerializeable
    {
        List<PlayerStruct> playerList = new List<PlayerStruct>();
        public int roomNum = -1;

        int startAblePlayer = 2;

        int serverObject = 0;
        
        public int roomMasterSessionId = -1;

        public string roomName;

        public void Enter(PlayerStruct player)
        {
            if (roomMasterSessionId == -1)
                roomMasterSessionId = player.playerSerial;

            playerList.Add(player);
        }

        public void Leave(PlayerStruct player)
        {
            playerList.Remove(player);
        }

        public List<PlayerStruct> GetPlayerList()
        {
            return playerList;
        }

        public bool IsStartAble()
        {
            return playerList.Count >= startAblePlayer;
        }



        public void SerializePacket(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), playerList.Count);
            count += sizeof(int);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), roomNum);
            count += sizeof(int);
            foreach (var item in playerList)
                item.Write(ref buffer,ref count);
        }

        public void DeSerializePacket(ref ArraySegment<byte> buffer , ref ushort count)
        {
            
            int listCount = BitConverter.ToInt32(buffer.Array, buffer.Offset + count);
            count += sizeof(int);

            PlayerStruct item = new PlayerStruct();
            for (int i = 0; i < listCount; i++)
            {
                item.Read(ref buffer, ref count);
                Enter(item);
            }

            ushort length = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += sizeof(ushort);


            roomName = System.Text.Encoding.UTF8.GetString(buffer.Array, buffer.Offset + count, length) + " Game Room";
            count += length;

            roomNum = BitConverter.ToUInt16(buffer.Array,buffer.Offset + count);
            count += sizeof(int);
        }

    }
}
