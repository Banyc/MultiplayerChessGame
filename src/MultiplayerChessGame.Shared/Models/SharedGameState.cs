using System.Collections.Generic;
using System.Linq;
using MultiplayerChessGame.Shared.Helpers;

namespace MultiplayerChessGame.Shared.Models
{
    public class SharedGameState
    {
        public int Step { get; set; } = 0;
        public GameBoard Board { get; set; }
        public Stack<string> BoardHistory { get; set; } = new Stack<string>();

        public void AddChessMove(ChessMove chessMove)
        {
            lock (this)
            {
                SaveHistory();
                this.Board.AddChessMove(chessMove);
            }
        }

        public void AddChessChange(ChessChange chessChange)
        {
            lock (this)
            {
                SaveHistory();
                this.Board.AddChessChange(chessChange);
            }
        }

        public void UndoHistory()
        {
            lock (this)
            {
                if (this.BoardHistory.Count == 0)
                {
                    return;
                }
                string previousBoard = this.BoardHistory.Pop();
                this.Board = Newtonsoft.Json.JsonConvert.DeserializeObject<GameBoard>(previousBoard);
                this.Step--;
            }
        }

        public void OverwriteState(SharedGameState state)
        {
            this.Step = state.Step;
            this.Board = state.Board;
            this.BoardHistory = state.BoardHistory;
        }

        private void SaveHistory()
        {
            this.BoardHistory.Push(Newtonsoft.Json.JsonConvert.SerializeObject(this.Board));
            this.Step++;
        }
    }
}
