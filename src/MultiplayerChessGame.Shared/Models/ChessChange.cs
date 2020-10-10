using System.Drawing;

namespace MultiplayerChessGame.Shared.Models
{
    public class ChessChange : RemoteInstruction
    {
        public Point Position { get; set; }
        // NewChess might be null
        public Chess NewChess { get; set; }
        public ChessChange()
        {
            base.Type = RemoteInstructionType.ChessChange;
        }
    }
}
