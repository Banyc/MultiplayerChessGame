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
        public Stack<string> BoardRedo { get; set; } = new Stack<string>();

        public void AddChessMove(ChessMove chessMove)
        {
            lock (this)
            {
                SaveHistory();
                try
                {
                    this.Board.AddChessMove(chessMove);
                    // we are in the new branch of history, the old history should be removed
                    this.BoardRedo.Clear();
                }
                catch (InvalidChessOperation)
                {
                    UndoHistory();
                }
            }
        }

        public void AddChessChange(ChessChange chessChange)
        {
            lock (this)
            {
                SaveHistory();
                this.Board.AddChessChange(chessChange);
                // we are in the new branch of history, the old history should be removed
                this.BoardRedo.Clear();
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
                SaveRedoHistory();
                string previousBoard = this.BoardHistory.Pop();
                this.Board = Newtonsoft.Json.JsonConvert.DeserializeObject<GameBoard>(previousBoard);
                this.Step--;
            }
        }

        public void RedoHistory()
        {
            lock (this)
            {
                if (this.BoardRedo.Count == 0)
                {
                    return;
                }
                SaveHistory();
                string redoBoard = this.BoardRedo.Pop();
                this.Board = Newtonsoft.Json.JsonConvert.DeserializeObject<GameBoard>(redoBoard);
            }
        }

        public void OverwriteState(SharedGameState state)
        {
            this.Step = state.Step;
            this.Board = state.Board;
            this.BoardHistory = state.BoardHistory;
            this.BoardRedo = state.BoardRedo;
        }

        private void SaveHistory()
        {
            this.BoardHistory.Push(Newtonsoft.Json.JsonConvert.SerializeObject(this.Board));
            this.Step++;
        }

        private void SaveRedoHistory()
        {
            this.BoardRedo.Push(Newtonsoft.Json.JsonConvert.SerializeObject(this.Board));
        }
    }
}
