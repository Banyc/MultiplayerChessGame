namespace MultiplayerChessGame.Shared.Models
{
    public class PushSharedGameState : RemoteInstruction
    {
        public SharedGameState SharedGameState { get; set; }
        public PushSharedGameState()
        {
            base.Type = RemoteInstructionType.PushSharedGameState;
        }
    }
}
