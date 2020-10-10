using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using MultiplayerChessGame.Shared.Models;
using MultiplayerChessGame.Shared.Protocol;

namespace MultiplayerChessGame.Shared.Helpers
{
    public class RemoteInstructionProtocol
    {
        private readonly FramingProtocol _framingProtocol = new FramingProtocol();
        private readonly SharedGameState _gameState;

        public RemoteInstructionProtocol(SharedGameState gameState)
        {
            _gameState = gameState;
        }

        // not including modifying the shared game state
        public byte[] ToLowerLayer(RemoteInstruction instruction)
        {
            // string rawString = null;
            byte[] rawBytes = null;
            switch (instruction.Type)
            {
                case RemoteInstructionType.ChessChange:
                    // rawString = JsonSerializer.Serialize((ChessChange)instruction);
                    rawBytes = JsonSerializer.SerializeToUtf8Bytes((ChessChange)instruction);
                    break;
                case RemoteInstructionType.ChessMove:
                    // rawString = JsonSerializer.Serialize((ChessMove)instruction);
                    rawBytes = JsonSerializer.SerializeToUtf8Bytes((ChessMove)instruction);
                    break;
                case RemoteInstructionType.PushSharedGameState:
                    rawBytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject((PushSharedGameState)instruction));
                    break;
                case RemoteInstructionType.UndoChessBoard:
                    rawBytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(instruction));
                    break;
            }
            if (rawBytes == null)
            {
                return null;
            }

            rawBytes = FramingProtocol.FromHighLayerToHere(rawBytes);
            return rawBytes;
        }

        // including modifying the shared game state
        public void ToHigherLayer(byte[] buffer, long offset, long size, Action<byte[]> otherwise)
        {
            foreach (byte[] frame in _framingProtocol.FromLowLayerToHere(buffer.Skip((int)offset).Take((int)size).ToArray()))
            {
                // string rawString = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
                string rawString = Encoding.UTF8.GetString(frame);
                Console.WriteLine(rawString);
                try
                {
                    // RemoteInstruction instruction = JsonSerializer.Deserialize<RemoteInstruction>(rawString);
                    RemoteInstruction instruction = JsonSerializer.Deserialize<RemoteInstruction>(frame);
                    switch (instruction.Type)
                    {
                        case RemoteInstructionType.ChessChange:
                            // _gameState.Board.AddChessChange((ChessChange)instruction);
                            _gameState.AddChessChange(JsonSerializer.Deserialize<ChessChange>(frame));
                            break;
                        case RemoteInstructionType.ChessMove:
                            // _gameState.Board.AddChessMove((ChessMove)instruction);
                            _gameState.AddChessMove(JsonSerializer.Deserialize<ChessMove>(frame));
                            break;
                        case RemoteInstructionType.UndoChessBoard:
                            _gameState.UndoHistory();
                            break;
                        default:
                            otherwise?.Invoke(frame);
                            break;
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
