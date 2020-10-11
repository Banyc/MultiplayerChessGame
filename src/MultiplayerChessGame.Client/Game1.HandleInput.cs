using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MultiplayerChessGame.Client.Helpers;
using MultiplayerChessGame.Shared.Models;

namespace MultiplayerChessGame.Client
{
    public partial class Game1
    {
        private void HandleSelectionMoveKey(Keys key)
        {
            if (!KeyboardHelper.HasBeenPressed(key))
            {
                return;
            }
            switch (key)
            {
                case Keys.Up:
                    switch (_playerSide)
                    {
                        case PlayerSide.Black:
                            SelectionGoDown();
                            break;
                        case PlayerSide.White:
                            SelectionGoUp();
                            break;
                    }
                    break;
                case Keys.Down:
                    switch (_playerSide)
                    {
                        case PlayerSide.Black:
                            SelectionGoUp();
                            break;
                        case PlayerSide.White:
                            SelectionGoDown();
                            break;
                    }
                    break;
                case Keys.Left:
                    switch (_playerSide)
                    {
                        case PlayerSide.Black:
                            SelectionGoRight();
                            break;
                        case PlayerSide.White:
                            SelectionGoLeft();
                            break;
                    }
                    break;
                case Keys.Right:
                    switch (_playerSide)
                    {
                        case PlayerSide.Black:
                            SelectionGoLeft();
                            break;
                        case PlayerSide.White:
                            SelectionGoRight();
                            break;
                    }
                    break;
            }
        }
        private void SelectionGoUp()
        {
            if (_selectionLogicPosition.Y <= 0)
            {
                return;
            }
            _selectionLogicPosition.Y--;
        }
        private void SelectionGoDown()
        {
            if (_selectionLogicPosition.Y >= 7)
            {
                return;
            }
            _selectionLogicPosition.Y++;
        }
        private void SelectionGoLeft()
        {
            if (_selectionLogicPosition.X <= 0)
            {
                return;
            }
            _selectionLogicPosition.X--;
        }
        private void SelectionGoRight()
        {
            if (_selectionLogicPosition.X >= 7)
            {
                return;
            }
            _selectionLogicPosition.X++;
        }

        private void HandleChessMoveKey(Keys key)
        {
            if (!KeyboardHelper.HasBeenPressed(key))
            {
                return;
            }
            switch (key)
            {
                case Keys.Z:
                    _selectedLogicalPosition = new Point(
                        _selectionLogicPosition.X,
                        _selectionLogicPosition.Y
                    );
                    break;
                case Keys.X:
                    if (!_selectedLogicalPosition.HasValue)
                    {
                        break;
                    }
                    ChessMove moveInstruction = new ChessMove
                    {
                        From = new System.Drawing.Point(
                            _selectedLogicalPosition.Value.X,
                            _selectedLogicalPosition.Value.Y
                        ),
                        To = new System.Drawing.Point(
                            _selectionLogicPosition.X,
                            _selectionLogicPosition.Y
                        )
                    };
                    _client.Client.Send(moveInstruction);
                    // _state.AddChessMove(moveInstruction);
                    _selectedLogicalPosition = null;
                    break;
            }
        }

        private void HandleChessChangeKey(Keys key)
        {
            if (!KeyboardHelper.HasBeenPressed(key))
            {
                return;
            }
            bool isProceeded = false;
            Chess newChess = null;
            switch (key)
            {
                case Keys.Q:
                    newChess = new Chess
                    {
                        ID = _random.Next().ToString(),
                        Side = PlayerSide.White,
                        Type = ChessType.Pawn
                    };
                    isProceeded = true;
                    break;
                case Keys.W:
                    newChess = new Chess
                    {
                        ID = _random.Next().ToString(),
                        Side = PlayerSide.White,
                        Type = ChessType.Knight
                    };
                    isProceeded = true;
                    break;
                case Keys.E:
                    newChess = new Chess
                    {
                        ID = _random.Next().ToString(),
                        Side = PlayerSide.White,
                        Type = ChessType.Bishop
                    };
                    isProceeded = true;
                    break;
                case Keys.R:
                    newChess = new Chess
                    {
                        ID = _random.Next().ToString(),
                        Side = PlayerSide.White,
                        Type = ChessType.Rook
                    };
                    isProceeded = true;
                    break;
                case Keys.T:
                    newChess = new Chess
                    {
                        ID = _random.Next().ToString(),
                        Side = PlayerSide.White,
                        Type = ChessType.King
                    };
                    isProceeded = true;
                    break;
                case Keys.Y:
                    newChess = new Chess
                    {
                        ID = _random.Next().ToString(),
                        Side = PlayerSide.White,
                        Type = ChessType.Queen
                    };
                    isProceeded = true;
                    break;
                case Keys.A:
                    newChess = new Chess
                    {
                        ID = _random.Next().ToString(),
                        Side = PlayerSide.Black,
                        Type = ChessType.Pawn
                    };
                    isProceeded = true;
                    break;
                case Keys.S:
                    newChess = new Chess
                    {
                        ID = _random.Next().ToString(),
                        Side = PlayerSide.Black,
                        Type = ChessType.Knight
                    };
                    isProceeded = true;
                    break;
                case Keys.D:
                    newChess = new Chess
                    {
                        ID = _random.Next().ToString(),
                        Side = PlayerSide.Black,
                        Type = ChessType.Bishop
                    };
                    isProceeded = true;
                    break;
                case Keys.F:
                    newChess = new Chess
                    {
                        ID = _random.Next().ToString(),
                        Side = PlayerSide.Black,
                        Type = ChessType.Rook
                    };
                    isProceeded = true;
                    break;
                case Keys.G:
                    newChess = new Chess
                    {
                        ID = _random.Next().ToString(),
                        Side = PlayerSide.Black,
                        Type = ChessType.King
                    };
                    isProceeded = true;
                    break;
                case Keys.H:
                    newChess = new Chess
                    {
                        ID = _random.Next().ToString(),
                        Side = PlayerSide.Black,
                        Type = ChessType.Queen
                    };
                    isProceeded = true;
                    break;
                case Keys.Delete:
                    newChess = null;
                    isProceeded = true;
                    break;
            }
            if (isProceeded)
            {
                ChessChange changeInstruction = new ChessChange
                {
                    NewChess = newChess,
                    Position = new System.Drawing.Point(
                        _selectionLogicPosition.X,
                        _selectionLogicPosition.Y
                    )
                };
                _client.Client.Send(changeInstruction);
                // _state.AddChessChange(changeInstruction);
            }
        }

        private void HandleUndoChessBoard(Keys key)
        {
            if (!KeyboardHelper.HasBeenPressed(key))
            {
                return;
            }
            switch (key)
            {
                case Keys.Back:
                    RemoteInstruction instruction = new RemoteInstruction()
                    {
                        Type = RemoteInstructionType.UndoChessBoard
                    };
                    _client.Client.Send(instruction);
                    // _state.UndoHistory();
                    break;
            }
        }

        private void HandleRedoChessBoard(Keys key)
        {
            if (!KeyboardHelper.HasBeenPressed(key))
            {
                return;
            }
            switch (key)
            {
                case Keys.OemPlus:
                    RemoteInstruction instruction = new RemoteInstruction()
                    {
                        Type = RemoteInstructionType.RedoChessBoard
                    };
                    _client.Client.Send(instruction);
                    // _state.RedoHistory();
                    break;
            }
        }

        private void HandleSwitchSide(Keys key)
        {
            if (!KeyboardHelper.HasBeenPressed(key))
            {
                return;
            }
            switch (key)
            {
                case Keys.D0:
                    _playerSide = (PlayerSide)(((int)_playerSide + 1) % Enum.GetValues(typeof(PlayerSide)).Length);
                    break;
            }
        }
    }
}
