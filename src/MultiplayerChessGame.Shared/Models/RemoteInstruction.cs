namespace MultiplayerChessGame.Shared.Models
{
    public enum RemoteInstructionType
    {
        ChessChange,
        ChessMove,
        PullSharedGameState,
        PushSharedGameState,
        UndoChessBoard,
        RedoChessBoard,
    }

    public class RemoteInstruction
    {
        public RemoteInstructionType Type { get; set; }
    }
}
