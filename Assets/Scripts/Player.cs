using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{

    public struct PlayerStruct
    {
        public PlayerStruct(int playerSerial, string playerName, int winCount, int loseCount)
        {
            this.playerSerial = playerSerial;
            this.playerName = playerName;
            this.winCount = winCount;
            this.loseCount = loseCount;
        }
        public int playerSerial;
        public string playerName;
        public int winCount;
        public int loseCount;

        public PlayerStruct None()
        {
            return new PlayerStruct(-1, null, -1, -1);
        }

        public void Write(ref ArraySegment<byte> buffer, ref ushort count)
        {
            
            byte[] stringBytes = System.Text.Encoding.UTF8.GetBytes(playerName);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), (ushort)stringBytes.Length); // 수정된 부분
            count += sizeof(ushort); // 수정된 부분
            Array.Copy(stringBytes, 0, buffer.Array, buffer.Offset + count, stringBytes.Length); // 수정된 부분
            count += (ushort)stringBytes.Length; // 수정된 부분

            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), winCount);
            count += sizeof(int);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + count, buffer.Count - count), loseCount);
            count += sizeof(int);

        }

        public void Read(ref ArraySegment<byte> buffer, ref ushort count)
        {
            

            ushort strLength = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += sizeof(ushort);
            playerName = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + count, strLength);
            count += strLength;

            winCount = BitConverter.ToInt32(buffer.Array, count);
            count += sizeof(int);
            loseCount = BitConverter.ToInt32(buffer.Array, count);
            count += sizeof(int);
        }

    }
}
