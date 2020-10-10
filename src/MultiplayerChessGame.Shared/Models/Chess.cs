namespace MultiplayerChessGame.Shared.Models
{
    // the order is critical
    public enum ChessType
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        King,
        Queen,
    }

    // the order is critical
    public enum PlayerSide
    {
        White,
        Black,
    }

    public class Chess
    {
        public string ID { get; set; }
        public PlayerSide Side { get; set; }
        public ChessType Type { get; set; }
    }
}
