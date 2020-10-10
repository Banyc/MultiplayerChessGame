using System.Drawing;
using System.Collections.Generic;
namespace MultiplayerChessGame.Shared.Models
{
    public class GameBoard
    {
        public Dictionary<Point, Chess> LocationChess { get; set; }

        public void AddChessMove(ChessMove chessMove)
        {
            if (!this.LocationChess.ContainsKey(chessMove.From))
            {
                return;
            }
            Chess movingChess = this.LocationChess[chessMove.From];
            this.LocationChess.Remove(chessMove.From);
            this.LocationChess[chessMove.To] = movingChess;
        }

        public void AddChessChange(ChessChange chessChange)
        {
            if (chessChange.NewChess == null)
            {
                // remove key-value
                if (this.LocationChess.ContainsKey(chessChange.Position))
                {
                    this.LocationChess.Remove(chessChange.Position);
                }
            }
            else
            {
                // change value
                this.LocationChess[chessChange.Position] = chessChange.NewChess;
            }
        }
    }
}
