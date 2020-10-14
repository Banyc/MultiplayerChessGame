using System.Threading;
using System.Runtime.CompilerServices;
using System;
using System.Drawing;
using System.Collections.Generic;
namespace MultiplayerChessGame.Shared.Models
{
    public class InvalidChessOperation : Exception
    {
        public InvalidChessOperation() : base()
        {
        }

        public InvalidChessOperation(string message) : base(message)
        {
        }

        public InvalidChessOperation(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public partial class GameBoard
    {
        public bool Availability { get; set; } = true;
        public Dictionary<Point, Chess> LocationChess { get; set; }
        // controlled by ChessMove instruction
        public Point? PreviousLocation { get; set; }
        public Point? CurrentLocation { get; set; }
        // controlled by ChessChange instruction
        public Point? NewChessLocation { get; set; }

        public void AddChessMove(ChessMove chessMove)
        {
            this.Availability = false;
            try
            {
                PerformChessMove(chessMove);
                if (!this.LocationChess.ContainsKey(chessMove.From))
                {
                    throw new InvalidChessOperation();
                    // return;
                }
                Chess movingChess = this.LocationChess[chessMove.From];
                this.LocationChess.Remove(chessMove.From);
                this.LocationChess[chessMove.To] = movingChess;
                this.PreviousLocation = chessMove.From;
                this.CurrentLocation = chessMove.To;
                this.NewChessLocation = null;
            }
            finally
            {
                this.Availability = true;
            }
        }

        public void AddChessChange(ChessChange chessChange)
        {
            this.Availability = false;
            try
            {
                PerformChessChange(chessChange);
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
                this.NewChessLocation = chessChange.Position;
            }
            finally
            {
                this.Availability = true;
            }
        }
    }
}
