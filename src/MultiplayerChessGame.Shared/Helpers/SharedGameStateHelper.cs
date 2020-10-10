using System.Drawing;
using System.Collections.Generic;
using MultiplayerChessGame.Shared.Models;

namespace MultiplayerChessGame.Shared.Helpers
{
    public static class SharedGameStateHelper
    {
        // top-left as (0, 0)
        public static SharedGameState GetNewSharedGameState()
        {
            return new SharedGameState
            {
                Board = new GameBoard
                {
                    LocationChess = new Dictionary<System.Drawing.Point, Chess>
                    {
                        { new Point(0, 0), new Chess
                            {
                                ID = "black-rook-0",
                                Side = PlayerSide.Black,
                                Type = ChessType.Rook,
                            }
                        },
                        { new Point(1, 0), new Chess
                            {
                                ID = "black-knight-0",
                                Side = PlayerSide.Black,
                                Type = ChessType.Knight,
                            }
                        },
                        { new Point(2, 0), new Chess
                            {
                                ID = "black-bishop-0",
                                Side = PlayerSide.Black,
                                Type = ChessType.Bishop,
                            }
                        },
                        { new Point(3, 0), new Chess
                            {
                                ID = "black-queen-0",
                                Side = PlayerSide.Black,
                                Type = ChessType.Queen,
                            }
                        },
                        { new Point(4, 0), new Chess
                            {
                                ID = "black-king-0",
                                Side = PlayerSide.Black,
                                Type = ChessType.King,
                            }
                        },
                        { new Point(5, 0), new Chess
                            {
                                ID = "black-bishop-1",
                                Side = PlayerSide.Black,
                                Type = ChessType.Bishop,
                            }
                        },
                        { new Point(6, 0), new Chess
                            {
                                ID = "black-knight-1",
                                Side = PlayerSide.Black,
                                Type = ChessType.Knight,
                            }
                        },
                        { new Point(7, 0), new Chess
                            {
                                ID = "black-rook-1",
                                Side = PlayerSide.Black,
                                Type = ChessType.Rook,
                            }
                        },
                        { new Point(0, 1), new Chess
                            {
                                ID = "black-pawn-0",
                                Side = PlayerSide.Black,
                                Type = ChessType.Pawn,
                            }
                        },
                        { new Point(1, 1), new Chess
                            {
                                ID = "black-pawn-1",
                                Side = PlayerSide.Black,
                                Type = ChessType.Pawn,
                            }
                        },
                        { new Point(2, 1), new Chess
                            {
                                ID = "black-pawn-2",
                                Side = PlayerSide.Black,
                                Type = ChessType.Pawn,
                            }
                        },
                        { new Point(3, 1), new Chess
                            {
                                ID = "black-pawn-3",
                                Side = PlayerSide.Black,
                                Type = ChessType.Pawn,
                            }
                        },
                        { new Point(4, 1), new Chess
                            {
                                ID = "black-pawn-4",
                                Side = PlayerSide.Black,
                                Type = ChessType.Pawn,
                            }
                        },
                        { new Point(5, 1), new Chess
                            {
                                ID = "black-pawn-5",
                                Side = PlayerSide.Black,
                                Type = ChessType.Pawn,
                            }
                        },
                        { new Point(6, 1), new Chess
                            {
                                ID = "black-pawn-6",
                                Side = PlayerSide.Black,
                                Type = ChessType.Pawn,
                            }
                        },
                        { new Point(7, 1), new Chess
                            {
                                ID = "black-pawn-7",
                                Side = PlayerSide.Black,
                                Type = ChessType.Pawn,
                            }
                        },
                        { new Point(0, 7), new Chess
                            {
                                ID = "white-rook-0",
                                Side = PlayerSide.White,
                                Type = ChessType.Rook,
                            }
                        },
                        { new Point(1, 7), new Chess
                            {
                                ID = "white-knight-0",
                                Side = PlayerSide.White,
                                Type = ChessType.Knight,
                            }
                        },
                        { new Point(2, 7), new Chess
                            {
                                ID = "white-bishop-0",
                                Side = PlayerSide.White,
                                Type = ChessType.Bishop,
                            }
                        },
                        { new Point(3, 7), new Chess
                            {
                                ID = "white-queen-0",
                                Side = PlayerSide.White,
                                Type = ChessType.Queen,
                            }
                        },
                        { new Point(4, 7), new Chess
                            {
                                ID = "white-king-0",
                                Side = PlayerSide.White,
                                Type = ChessType.King,
                            }
                        },
                        { new Point(5, 7), new Chess
                            {
                                ID = "white-bishop-1",
                                Side = PlayerSide.White,
                                Type = ChessType.Bishop,
                            }
                        },
                        { new Point(6, 7), new Chess
                            {
                                ID = "white-knight-1",
                                Side = PlayerSide.White,
                                Type = ChessType.Knight,
                            }
                        },
                        { new Point(7, 7), new Chess
                            {
                                ID = "white-rook-1",
                                Side = PlayerSide.White,
                                Type = ChessType.Rook,
                            }
                        },
                        { new Point(0, 6), new Chess
                            {
                                ID = "white-pawn-0",
                                Side = PlayerSide.White,
                                Type = ChessType.Pawn,
                            }
                        },
                        { new Point(1, 6), new Chess
                            {
                                ID = "white-pawn-1",
                                Side = PlayerSide.White,
                                Type = ChessType.Pawn,
                            }
                        },
                        { new Point(2, 6), new Chess
                            {
                                ID = "white-pawn-2",
                                Side = PlayerSide.White,
                                Type = ChessType.Pawn,
                            }
                        },
                        { new Point(3, 6), new Chess
                            {
                                ID = "white-pawn-3",
                                Side = PlayerSide.White,
                                Type = ChessType.Pawn,
                            }
                        },
                        { new Point(4, 6), new Chess
                            {
                                ID = "white-pawn-4",
                                Side = PlayerSide.White,
                                Type = ChessType.Pawn,
                            }
                        },
                        { new Point(5, 6), new Chess
                            {
                                ID = "white-pawn-5",
                                Side = PlayerSide.White,
                                Type = ChessType.Pawn,
                            }
                        },
                        { new Point(6, 6), new Chess
                            {
                                ID = "white-pawn-6",
                                Side = PlayerSide.White,
                                Type = ChessType.Pawn,
                            }
                        },
                        { new Point(7, 6), new Chess
                            {
                                ID = "white-pawn-7",
                                Side = PlayerSide.White,
                                Type = ChessType.Pawn,
                            }
                        },
                    }
                }
            };
        }
    }
}
