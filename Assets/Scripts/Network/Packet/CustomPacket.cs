using Assets.Scripts;
using JetBrains.Annotations;
using Server;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace CustomPacket
{

    public interface IPacketSerializeable
    {
        public void SerializePacket(ref ArraySegment<byte> buffer, ref ushort count);
        public void DeSerializePacket(ref ArraySegment<byte> buffer , ref ushort count);
    }
    
    public enum PacketID
    {
        INIT_PACKET = 1,


        REQUESTROOMINFO = 10,

        ENTERGAMEROOM = 13,

        SQL_SIGN = 20,
        SQLSIGNCOMPLETE = 21,
        SQLLOGIN = 22,
        SQLLOGINRESULT = 23,
        GAME_LOBBY_ENTER = 24,
        GAMEROOMINFO = 25,



        LOBBYINFO = 30,
        RQ_CREATE_GAMEROOM = 31,
        AC_CREATE_GAMEROOM = 32,
        ENTER_ROOM = 33,
        ENTER_ROOM_RESULT = 34,
        CHANGE_ROOM = 35,


        GAME_START = 50,
        GAME_START_RESULT = 51,
        GAME_END = 52,
        GAME_END_RESULT = 53,

        ROOM_MASTER = 60,

        SUMMON = 70,
        SUMMON_RESULT = 71,
    }


    public abstract class Packet
    {

        protected PacketID packetNum;

        public abstract void Read(ArraySegment<byte> buffer);

        public ArraySegment<byte> Write()
        {
            ushort count = 0;

            ArraySegment<byte> resultBuffer = SendBufferHelper.Open(4096);
            count += sizeof(ushort);

            WriteTemplate(ref resultBuffer , ref count);

            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(resultBuffer.Array, resultBuffer.Offset, resultBuffer.Count), count);

            return SendBufferHelper.Close(count);
        }

        protected abstract void WriteTemplate(ref ArraySegment<byte> buffer , ref ushort count);

        protected void WriteString(ref ArraySegment<byte> buffer, ref ushort count, string value)
        {
            byte[] stringBytes = System.Text.Encoding.UTF8.GetBytes(value);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)stringBytes.Length); // 수정된 부분
            count += sizeof(ushort); // 수정된 부분
            Array.Copy(stringBytes, 0, buffer.Array, buffer.Offset + count, stringBytes.Length); // 수정된 부분
            count += (ushort)stringBytes.Length; // 수정된 부분
        }

        protected virtual string ReadString(ref ArraySegment<byte> buffer, ref ushort count)
        {

            ushort strLength = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += sizeof(ushort);
            string result = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + count, strLength);
            count += strLength;

            return result;
        }
    
    }

    public abstract class PacketBuilder<T> where T : Packet, new()
    {
        protected T instance;

        public PacketBuilder()
        {
            instance = new T();
        }
        public T Build()
        {
            return instance;
        }
    }

    public class ConnectPacket : Packet
    {
        public int sessionId;

        public ConnectPacket()
        {
            packetNum = PacketID.INIT_PACKET;
        }

        public class Builder : PacketBuilder<ConnectPacket>
        {
            public Builder()
            {
                instance = new ConnectPacket();
            }
            public Builder SetSessionId(int sessionId)
            {
                instance.sessionId = sessionId;
                return this;
            }
        }

        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 4;
            sessionId = BitConverter.ToInt32(buffer.Array, count);
        }

        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)packetNum);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), sessionId);
            count += sizeof(int);

        }
    }


    public class StartGamePacket : Packet
    {
        public int roomNum;
        public int roomMasterId;
        public StartGamePacket(int roomNum, int roomMasterId)
        {
            packetNum = PacketID.GAME_START;
            this.roomNum = roomNum;
            //this.roomMasterId = roomMasterId;
        }
        public override void Read(ArraySegment<byte> buffer)
        {

        }
        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)packetNum);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), roomNum);
            count += sizeof(int);
            //BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), roomNum);
            //count += sizeof(int);
        }
    }

    public class StartGameResultPacket : Packet
    {
        public bool isStart;
        public int roomMasterId;
        public StartGameResultPacket()
        {
            packetNum = PacketID.GAME_START_RESULT;
        }
        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 4;
            isStart = BitConverter.ToBoolean(buffer.Array, buffer.Offset + count);
            count += sizeof(bool);
            roomMasterId = BitConverter.ToInt32(buffer.Array, buffer.Offset + count);
            count += sizeof(int);
        }
        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)packetNum);
            count += sizeof(ushort);
            // 생략 이건 서버로 보낼 필요가 없음
        }
    }

    public class RoomMasterPacket : Packet
    {
        public int roomMasterId;
        public RoomMasterPacket()
        {
            packetNum = PacketID.ROOM_MASTER;
        }
        public void Init(int roomMasterId)
        {
            this.roomMasterId = roomMasterId;
        }

        public override void Read(ArraySegment<byte> buffer)
        {
        }
        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)packetNum);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), roomMasterId);
            count += sizeof(int);
        }
    }

    public class LobbyEnterPacket : Packet
    {

        public LobbyEnterPacket() 
        {
            packetNum = PacketID.GAME_LOBBY_ENTER;
        }
        public override void Read(ArraySegment<byte> buffer)
        {
            
        }

        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array,buffer.Offset + count , buffer.Count - count) , (ushort)packetNum);
            count += sizeof(ushort);
        }

    }

    //public class RoomListPacket : Packet
    //{

    //    public PlayerStruct test = new PlayerStruct("abcd",3,3);
    //    public int listCount = 0;

    //    public RoomListPacket()
    //    {
    //        packetNum = PacketID.GAMEROOMINFO;
    //    }

    //    public void Init(PlayerStruct roomList)
    //    {
    //        this.test = roomList;
    //    }
    //    public override void Read(ArraySegment<byte> buffer)
    //    {
    //        ushort count = 4;

    //        test.playerName = ReadString(ref buffer, ref count);

    //        test.winCount = BitConverter.ToInt32(buffer.Array, buffer.Offset + count);
    //        count += sizeof(int);
    //        test.loseCount = BitConverter.ToInt32(buffer.Array, buffer.Offset + count);
    //        count += sizeof(int);

    //    }

    //    protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
    //    {

    //        BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)packetNum);
    //        count += sizeof(ushort);

    //        WriteString(ref buffer, ref count, test.playerName);

    //        BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), test.winCount);
    //        count += sizeof(int);
    //        BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), test.loseCount);
    //        count += sizeof(int);
    //    }
    //}

    public class RoomListPacket : Packet
    {

        public List<GameRoom> roomList = new List<GameRoom>();
        public int listCount = 0;
        public RoomListPacket()
        {
            packetNum = PacketID.GAMEROOMINFO;
        }

        public void Init(List<GameRoom> roomList)
        {
            this.roomList = roomList;
        }
        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 4;

            roomList.Clear();
            listCount = BitConverter.ToInt32(buffer.Array, buffer.Offset + count);
            count += sizeof(int);
            
            
            for (int i = 0; i < listCount; i++)
            {
                GameRoom room = new GameRoom();
                room.DeSerializePacket(ref buffer, ref count);
                roomList.Add(room);
            }
            
        }

        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {

            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)packetNum);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), roomList.Count);
            count += sizeof(int);

            foreach (GameRoom room in roomList)
            {
                room.SerializePacket(ref buffer, ref count);
            }

        }
    }
    public class LoginResultPacket : Packet
    {
        public int errorCode;
        public int sessionId;
        public PlayerStruct PlayerInfo { get => player; }
        PlayerStruct player;

        public LoginResultPacket()
        {
            packetNum = PacketID.SQLLOGINRESULT;
        }

        public int ErrorCode { get => errorCode; }

        public void Init(int errorCode, int sessionId,PlayerStruct player)
        {
            this.errorCode = errorCode;
            this.sessionId = sessionId;
            this.player = player;
        }

        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 4;
            errorCode = BitConverter.ToInt32(buffer.Array, buffer.Offset + count);
            count += sizeof(int);
            player.Read(ref buffer, ref count);
        }

        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array,buffer.Offset + count , buffer.Count - count) , (ushort)packetNum);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), ErrorCode);
            count += sizeof(int);
            player.Write(ref buffer, ref count);
        }

    }
    public class LoginPacket : Packet
    {
        public int sessionId;
        public string userId;
        public string userPw;

        public LoginPacket()
        {
            packetNum = PacketID.SQLLOGIN;
        }

        public void Init(string userId, string userPw)
        {
            this.sessionId = NetworkManager.instance.session.SessionId;
            this.userId = userId;
            this.userPw = userPw;
        }

        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 4;

            userId = ReadString(ref buffer,ref count);
            userPw = ReadString(ref buffer,ref count);
        }

        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)packetNum);
            count += sizeof(ushort);

            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), NetworkManager.instance.session.SessionId);
            count += sizeof(int);
            
            WriteString(ref buffer, ref count, userId);
            WriteString(ref buffer, ref count, userPw);

        }
    }
    public class SignCompletePacket : Packet
    {
        
        public bool isSuceess;
        private int errorCode = -1;
        public int ErrorCode { get => errorCode; }

        public void Init(bool isSuceess , int errorcode)
        {
            this.isSuceess = isSuceess;
            this.errorCode = errorcode;
        }
        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 4;
            isSuceess = BitConverter.ToBoolean(buffer.Array, buffer.Offset + count);
            count += sizeof(bool);
            errorCode = BitConverter.ToInt32(buffer.Array, buffer.Offset + count);
        }

        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)PacketID.SQLSIGNCOMPLETE);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), isSuceess);
            count += sizeof(bool);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), ErrorCode);
            count += sizeof(int);
        }
    }
    public class PlayerSignPacket : Packet
    {

        public const ushort packetId = 20;
        string queryResult;

        string id = "testid";
        string pw = "textpw";
        

        public void Init(string id , string pw)
        {
            this.id = id;
            this.pw = pw;
        }

        public override void Read(ArraySegment<byte> buffer)
        {
            ushort size = 0;
            size += sizeof(ushort);
            size += sizeof(ushort);

            id = ReadString(ref buffer, ref size);
            pw = ReadString(ref buffer, ref size);
        }


        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)PacketID.SQL_SIGN);
            count += sizeof(ushort);
            WriteString(ref buffer, ref count, id);
            WriteString(ref buffer, ref count, pw);

        }
    }
    public class CreateRoomPacket : Packet
    {
        public int session_check = -1;
        public PlayerStruct ownerInfo;

        public CreateRoomPacket()
        {
            packetNum = PacketID.RQ_CREATE_GAMEROOM;
        }

        //buillder
        public class PacketBuilder
        {
            CreateRoomPacket packet;
            public PacketBuilder()
            {
                packet = new CreateRoomPacket();
            }
            public PacketBuilder SetOwnerInfo(PlayerStruct owner)
            {
                packet.ownerInfo = owner;
                return this;
            }

            public CreateRoomPacket Build()
            {
                return packet;
            }
        }

        public void Init(PlayerStruct ownerInfo)
        {
            ownerInfo = ownerInfo;
        }
        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 4;
            ownerInfo.Read(ref buffer, ref count);
            count += sizeof(int);
        }

        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count + count), (ushort)packetNum);
            count += sizeof(ushort);

            session_check = NetworkManager.instance.session.SessionId;
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), NetworkManager.instance.session.SessionId);
            count += sizeof(int);
        }
    }
    public class RequestRoomListPacket : Packet
    {
        public List<GameRoom> roomList = new List<GameRoom>();
        public int listCount = 0;

        public RequestRoomListPacket() 
        {
            packetNum = PacketID.REQUESTROOMINFO;
        }

        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 4;

            roomList.Clear();
            listCount = BitConverter.ToInt32(buffer.Array, buffer.Offset + count);
            count += sizeof(int);


            for (int i = 0; i < listCount; i++)
            {
                GameRoom room = new GameRoom();
                room.DeSerializePacket(ref buffer, ref count);
                roomList.Add(room);
            }
        }

        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)packetNum);
            count += sizeof(ushort);
        }
    }

    public class EnterRoomPacket : Packet
    {
        public bool isEnter;
        public int roomNum;
        public GameRoom room = new GameRoom();

        public EnterRoomPacket()
        {
            packetNum = PacketID.ENTER_ROOM;
        }

        public class Builder : PacketBuilder<EnterRoomPacket>
        {
            public Builder SetRoomNum(int roomNum)
            {
                instance.roomNum = roomNum;
                return this;
            }
            public Builder SetGameRoom(GameRoom room)
            {
                instance.room = room;
                return this;
            }
        }

        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 4;
            isEnter = BitConverter.ToBoolean(buffer.Array, buffer.Offset + count);
            count += sizeof(bool);
            roomNum = BitConverter.ToInt32(buffer.Array, buffer.Offset + count);
            count += sizeof(int);
            room.DeSerializePacket(ref buffer, ref count);
        }

        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)packetNum);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), isEnter);
            count += sizeof(bool);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), roomNum);
            count += sizeof(int);
            room.SerializePacket(ref buffer, ref count);
        }
    }


    public class EnterRoomResultPacket : Packet
    {
        public bool isEnter;

        public EnterRoomResultPacket()
        {
            packetNum = PacketID.ENTER_ROOM_RESULT;
        }
        public void Init(bool isEnter)
        {
            this.isEnter = isEnter;
        }

        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 4;

            isEnter = BitConverter.ToBoolean(buffer.Array, buffer.Offset + count);
            count += sizeof(bool);
        }

        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)packetNum);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), isEnter);
            count += sizeof(bool);
        }

    }

    public class RoomPacket : Packet
    {
        public GameRoom roomInfo;
        public RoomPacket()
        {
            packetNum = PacketID.CHANGE_ROOM;
        }

        public void Init(GameRoom roomInfo)
        {
            this.roomInfo = roomInfo;
        }

        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 4;
            roomInfo = new GameRoom();
            roomInfo.DeSerializePacket(ref buffer, ref count);

        }

        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)packetNum);
            count += sizeof(ushort);
            roomInfo.SerializePacket(ref buffer, ref count);
        }
    }

    public class SummondPacket : Packet
    {
        public int nodeId;
        public int roomNum;
        public int playerSessionId;



        public SummondPacket()
        {
            packetNum = PacketID.SUMMON;
        }

        public void Init(int nodeId , int roomNum, int playerSessionId)
        {
            this.nodeId = nodeId;
            this.roomNum = roomNum;
            this.playerSessionId = playerSessionId;
        }

        public override void Read(ArraySegment<byte> buffer)
        {
            
        }

        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)packetNum);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), roomNum);
            count += sizeof(int);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), nodeId);
            count += sizeof(int);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), playerSessionId);
            count += sizeof(int);
        }
    }

    public class SummonResultPacket : Packet
    {
        public int nodeId;
        public int playerSessionId;

        public SummonResultPacket()
        {
            packetNum = PacketID.SUMMON_RESULT;
        }
        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 4;
            nodeId = BitConverter.ToInt32(buffer.Array, buffer.Offset + count);
            count += sizeof(int);
            playerSessionId = BitConverter.ToInt32(buffer.Array, buffer.Offset + count);
            count += sizeof(int);
        }
        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)packetNum);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), nodeId);
            count += sizeof(int);
        }
    }


    public class GameEndPacket : Packet
    {
        public int roomNum;
        public int packetOwnerSessionId;
        public bool isWin;


        public GameEndPacket()
        {
            packetNum = PacketID.GAME_END;
            Write();
        }

        public void Init(int roomNum, int sessionId, bool isWin)
        {
            this.roomNum = roomNum;
            packetOwnerSessionId = sessionId;
            this.isWin = isWin;
        }
        public override void Read(ArraySegment<byte> buffer)
        {
        }
        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)packetNum);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), roomNum);
            count += sizeof(int);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), packetOwnerSessionId);
            count += sizeof(int);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), isWin);
            count += sizeof(bool);
        }
    }
    
    public class GameEndResultPacket : Packet
    {

        // 지금 생각해 보니까 packet ReadOnly WriteOnly 하게 쓰고 있는 것 같은데 이거 그냥 interface 같은걸로 나눴어야 했나 하네
        public bool isEnd;
        public int roomNum;
        public string winner_DB_Id;

        public GameEndResultPacket()
        {
            packetNum = PacketID.GAME_END_RESULT;
        }
        public GameEndResultPacket(ArraySegment<byte> buffer)
        {
            Read(buffer);
        }

        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 4;
            isEnd = BitConverter.ToBoolean(buffer.Array, buffer.Offset + count);
            count += sizeof(bool);
            roomNum = BitConverter.ToInt32(buffer.Array, buffer.Offset + count);
            count += sizeof(int);
            ReadString(ref buffer, ref count);

        }
        protected override void WriteTemplate(ref ArraySegment<byte> buffer, ref ushort count)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)packetNum);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), isEnd);
            count += sizeof(bool);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), roomNum);
            count += sizeof(int);
        }

    }
}
