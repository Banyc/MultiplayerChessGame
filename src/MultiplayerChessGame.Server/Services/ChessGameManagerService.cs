using MultiplayerChessGame.Shared.Helpers;
using MultiplayerChessGame.Shared.Models;

namespace MultiplayerChessGame.Server.Services
{
    public class ChessGameManagerService
    {
        public SharedGameState SharedGameState { get; }

        public ChessGameManagerService()
        {
            this.SharedGameState = SharedGameStateHelper.GetNewSharedGameState();
        }
    }
}
