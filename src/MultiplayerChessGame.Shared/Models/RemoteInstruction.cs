namespace MultiplayerChessGame.Shared.Models
{
    public enum RemoteInstructionType
    {
        ChessChange,
        ChessMove,
        PullSharedGameState,
        PushSharedGameState,
        UndoChessBoard,
    }

    public class RemoteInstruction
    {
        public RemoteInstructionType Type { get; set; }
    }
}
