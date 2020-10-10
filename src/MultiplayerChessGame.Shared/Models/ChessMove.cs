using System.Drawing;

namespace MultiplayerChessGame.Shared.Models
{
    public class ChessMove : RemoteInstruction
    {
        public Point From { get; set; }
        public Point To { get; set; }
        public ChessMove()
        {
            base.Type = RemoteInstructionType.ChessMove;
        }
    }
}
