using System;
using System.Collections.Generic;
using MultiplayerChessGame.Shared.Helpers;

namespace MultiplayerChessGame.Shared.Protocol
{
    public class FramingProtocol
    {
        private BufferMgr _bufferMgr = new BufferMgr();
        public FramingProtocol()
        {
        }

        public static byte[] FromHighLayerToHere(byte[] dataBytes)
        {
            byte[] data = (byte[])dataBytes;
            int length = data.Length;
            byte[] lengthByte = BitConverter.GetBytes(length);  // 4 Bytes
            List<byte> prefix_data = new List<byte>();
            prefix_data.AddRange(lengthByte);
            prefix_data.AddRange(data);
            return prefix_data.ToArray();
        }

        public IEnumerable<byte[]> FromLowLayerToHere(byte[] dataBytes)
        {
            _bufferMgr.AddBytes(dataBytes, dataBytes.Length);

            byte[] data = _bufferMgr.GetAdequateBytes();
            while (data.Length > 0)
            {
                yield return data;

                data = _bufferMgr.GetAdequateBytes();
            }
        }

        public void Dispose()
        {
        }
    }
}
