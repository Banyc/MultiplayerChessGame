using System.Reflection;
using System.Linq;
using System.Runtime.CompilerServices;
using System;
using System.Drawing;
using System.Collections.Generic;
namespace MultiplayerChessGame.Shared.Models
{
    public partial class GameBoard
    {
        public PlayerSide WhosTurn { get; set; } = PlayerSide.White;
        // left: queen-side
        // right: king-side
        public bool CouldWhiteCastlingLeft { get; set; } = true;
        public bool CouldWhiteCastlingRight { get; set; } = true;
        public bool CouldBlackCastlingLeft { get; set; } = true;
        public bool CouldBlackCastlingRight { get; set; } = true;
        public Point? FreshTwoStepAdvanceBlackPawnLocation { get; set; } = null;
        public Point? FreshTwoStepAdvanceWhitePawnLocation { get; set; } = null;
        public bool IsDuringPromotion { get; set; } = false;

        // With rules
        private void Perform(RemoteInstruction instruction)
        {
            switch (instruction.Type)
            {
                case RemoteInstructionType.ChessMove:
                    PerformChessMove((ChessMove)instruction);
                    break;
                case RemoteInstructionType.ChessChange:
                    PerformChessChange((ChessChange)instruction);
                    break;
            }
        }

        private void PerformChessMove(ChessMove instruction)
        {
            PlayerSide thisSide = GetThisPlayerSide(instruction);

            // chess if there has a chess
            if (!HasChess(instruction.From, this.LocationChess))
            {
                throw new InvalidChessOperation();
            }
            // check who's turn
            if (thisSide != this.WhosTurn)
            {
                throw new InvalidChessOperation();
            }
            // check if during promotion
            if (this.IsDuringPromotion)
            {
                // only allow to perform chess change
                throw new InvalidChessOperation();
            }

            // if else:
            // basic movement
            //  if checked after
            //  update `FreshTwoStepAdvancePawnLocation`
            // en passant
            //  if checked after
            // castling
            //  if checked before
            //  if checked middle
            //  if checked after
            Chess chess = this.LocationChess[instruction.From];
            if (IsReachable(chess, instruction.From, instruction.To, this.LocationChess))
            {
                if (IsChecked(thisSide, this.LocationChess, instruction))
                {
                    throw new InvalidChessOperation();
                }
                UpdateFreshTwoStepAdvancePawnLocation(chess, instruction.From, instruction.To);
            }
            else if (IsEnPassant(chess, instruction.From, instruction.To))
            {
                // remove the killed pawn
                switch (chess.Side)
                {
                    case PlayerSide.White:
                        this.LocationChess.Remove(this.FreshTwoStepAdvanceBlackPawnLocation.Value);
                        break;
                    case PlayerSide.Black:
                        this.LocationChess.Remove(this.FreshTwoStepAdvanceWhitePawnLocation.Value);
                        break;
                }
            }
            else if (IsCastling(chess, instruction.From, instruction.To))
            {
                // move rook
                Point rookDestination = new Point((instruction.From.X + instruction.To.X) / 2, instruction.From.Y);
                Point rookSource = new Point(0, 0);
                switch (chess.Side)
                {
                    case PlayerSide.White:
                        if (instruction.To.X > instruction.From.X)
                        {
                            rookSource = new Point(7, 7);
                        }
                        else
                        {
                            rookSource = new Point(0, 7);
                        }
                        break;
                    case PlayerSide.Black:
                        if (instruction.To.X > instruction.From.X)
                        {
                            rookSource = new Point(7, 0);
                        }
                        else
                        {
                            rookSource = new Point(0, 0);
                        }
                        break;
                }
                this.LocationChess[rookDestination] = this.LocationChess[rookSource];
                this.LocationChess.Remove(rookSource);
            }
            else
            {
                throw new InvalidChessOperation();
            }

            // castling change availability
            if (chess.Type == ChessType.King)
            {
                switch (chess.Side)
                {
                    case PlayerSide.White:
                        this.CouldWhiteCastlingLeft = false;
                        this.CouldWhiteCastlingRight = false;
                        break;
                    case PlayerSide.Black:
                        this.CouldBlackCastlingLeft = false;
                        this.CouldBlackCastlingRight = false;
                        break;
                }
            }
            else if (chess.Type == ChessType.Rook)
            {
                switch (instruction.From.Y)
                {
                    case 0:
                        switch (instruction.From.X)
                        {
                            case 0:
                                this.CouldBlackCastlingLeft = false;
                                break;
                            case 7:
                                this.CouldBlackCastlingRight = false;
                                break;
                        }
                        break;
                    case 7:
                        switch (instruction.From.X)
                        {
                            case 0:
                                this.CouldWhiteCastlingLeft = false;
                                break;
                            case 7:
                                this.CouldWhiteCastlingRight = false;
                                break;
                        }
                        break;
                }
            }

            // promotion
            if (this.LocationChess[instruction.From].Type == ChessType.Pawn
                && (instruction.To.Y == 7 || instruction.To.Y == 0))
            {
                this.IsDuringPromotion = true;
            }
            else
            {
                this.WhosTurn = GetOppositeSide(GetThisPlayerSide(instruction));
            }
            // ~~mark check~~
        }

        private void PerformChessChange(ChessChange instruction)
        {
            // chess if there has a chess
            if (instruction.NewChess == null)
            {
                throw new InvalidChessOperation();
            }
            // check who's turn
            if (instruction.NewChess?.Side != this.WhosTurn)
            {
                throw new InvalidChessOperation();
            }
            // check chess type
            ChessType[] validChessType = new ChessType[]
            {
                ChessType.Bishop, ChessType.Knight, ChessType.Queen, ChessType.Rook
            };
            if (!validChessType.Contains(instruction.NewChess.Type))
            {
                throw new InvalidChessOperation();
            }

            // promotion
            if (instruction.Position.Y == 0 && instruction.NewChess?.Side == PlayerSide.White
                || instruction.Position.Y == 7 && instruction.NewChess?.Side == PlayerSide.Black)
            { }
            else
            {
                throw new InvalidChessOperation();
            }

            // switch turn
            this.WhosTurn = GetOppositeSide(instruction.NewChess.Side);

            this.IsDuringPromotion = false;
            // ~~mark check~~
        }

        private bool IsCastling(Chess chess, Point from, Point to)
        {
            if (chess.Type != ChessType.King)
            {
                return false;
            }
            if (Math.Abs(to.X - from.X) == 2 && from.X == 4)
            {
                // source -> destination: direction
                switch (to.X)
                {
                    case 2:
                        // king goes left
                        if (!HasChess(new Point(1, from.Y), this.LocationChess)
                            && !HasChess(new Point(2, from.Y), this.LocationChess)
                            && !HasChess(new Point(3, from.Y), this.LocationChess)
                            && (chess.Side == PlayerSide.Black && this.CouldBlackCastlingLeft
                                || chess.Side == PlayerSide.White && this.CouldWhiteCastlingLeft
                                )
                            )
                        {
                            ChessMove moveOneStep = new ChessMove()
                            {
                                From = from,
                                // To = new Point(from.X - 1, from.Y)
                                To = new Point((from.X + to.X) / 2, from.Y)
                            };
                            ChessMove moveTwoSteps = new ChessMove()
                            {
                                From = from,
                                To = to
                            };
                            return !IsChecked(chess.Side, this.LocationChess)
                                && !IsChecked(chess.Side, this.LocationChess, moveOneStep)
                                && !IsChecked(chess.Side, this.LocationChess, moveTwoSteps);
                        }
                        else
                        {
                            return false;
                        }
                    case 6:
                        // king goes right
                        if (!HasChess(new Point(5, from.Y), this.LocationChess)
                            && !HasChess(new Point(6, from.Y), this.LocationChess)
                            && (chess.Side == PlayerSide.Black && this.CouldBlackCastlingRight
                                || chess.Side == PlayerSide.White && this.CouldWhiteCastlingRight
                                )
                            )
                        {
                            ChessMove moveOneStep = new ChessMove()
                            {
                                From = from,
                                // To = new Point(from.X + 1, from.Y)
                                To = new Point((from.X + to.X) / 2, from.Y)
                            };
                            ChessMove moveTwoSteps = new ChessMove()
                            {
                                From = from,
                                To = to
                            };
                            return !IsChecked(chess.Side, this.LocationChess)
                                && !IsChecked(chess.Side, this.LocationChess, moveOneStep)
                                && !IsChecked(chess.Side, this.LocationChess, moveTwoSteps);
                        }
                        else
                        {
                            return false;
                        }
                    default:
                        return false;
                }
            }
            else
                {
                    return false;
                }
            }

            private bool IsEnPassant(Chess chess, Point from, Point to)
            {
                if (chess.Type != ChessType.Pawn)
                {
                    return false;
                }
                switch (chess.Side)
                {
                    case PlayerSide.White:
                        return this.FreshTwoStepAdvanceBlackPawnLocation != null
                            && this.FreshTwoStepAdvanceBlackPawnLocation.Value.X == to.X
                            && this.FreshTwoStepAdvanceBlackPawnLocation.Value.Y == from.Y
                            && Math.Abs(to.X - from.X) == 1
                            && to.Y + 1 == this.FreshTwoStepAdvanceBlackPawnLocation.Value.Y;
                        break;
                    case PlayerSide.Black:
                        return this.FreshTwoStepAdvanceWhitePawnLocation != null
                            && this.FreshTwoStepAdvanceWhitePawnLocation.Value.X == to.X
                            && this.FreshTwoStepAdvanceWhitePawnLocation.Value.Y == from.Y
                            && Math.Abs(to.X - from.X) == 1
                            && to.Y - 1 == this.FreshTwoStepAdvanceWhitePawnLocation.Value.Y;
                        break;
                    default:
                        return false;
                }
            }

            // !!! assume the chess move is VALID 
            private void UpdateFreshTwoStepAdvancePawnLocation(Chess chess, Point from, Point to)
            {
                if (chess.Type == ChessType.Pawn
                    && Math.Abs(from.Y - to.Y) == 2)
                {
                    switch (chess.Side)
                    {
                        case PlayerSide.White:
                            this.FreshTwoStepAdvanceWhitePawnLocation = to;
                            break;
                        case PlayerSide.Black:
                            this.FreshTwoStepAdvanceBlackPawnLocation = to;
                            break;
                    }
                }
                else
                {
                    switch (chess.Side)
                    {
                        case PlayerSide.White:
                            this.FreshTwoStepAdvanceWhitePawnLocation = null;
                            break;
                        case PlayerSide.Black:
                            this.FreshTwoStepAdvanceBlackPawnLocation = null;
                            break;
                    }
                }
            }

            static object SpinLock_IsChecked = new object();
            private static bool IsChecked(PlayerSide side, Dictionary<Point, Chess> locationChess, ChessMove after = null)
            {
                lock (SpinLock_IsChecked)
                {
                    Chess killedChess = null;
                    bool isChecked = false;
                    if (after != null)
                    {
                        if (locationChess.ContainsKey(after.To))
                        {
                            killedChess = locationChess[after.To];
                        }
                        locationChess[after.To] = locationChess[after.From];
                        locationChess.Remove(after.From);
                    }

                    Point kingLocation = GetKingLocation(side, locationChess);
                    foreach ((Point location, Chess chess) in locationChess)
                    {
                        if (chess.Side == side)
                        {
                            continue;
                        }
                        if (IsReachable(chess, location, kingLocation, locationChess))
                        {
                            isChecked = true;
                            break;
                        }
                    }

                    // restore
                    if (after != null)
                    {
                        locationChess[after.From] = locationChess[after.To];
                        locationChess.Remove(after.To);
                        if (killedChess != null)
                        {
                            locationChess[after.To] = killedChess;
                        }
                    }

                    return isChecked;
                }
            }

            private static Point GetKingLocation(PlayerSide side, Dictionary<Point, Chess> locationChess, ChessMove after = null)
            {
                if (after != null && locationChess[after.From].Type == ChessType.King)
                {
                    return after.To;
                }
                Point kingLocation = locationChess.Single(xxxx =>
                                    xxxx.Value.Type == ChessType.King
                                    && xxxx.Value.Side == side).Key;
                return kingLocation;
            }

            private PlayerSide GetThisPlayerSide(ChessMove instruction)
            {
                return this.LocationChess[instruction.From].Side;
            }

            private static bool IsReachable(Chess chess, Point from, Point to, Dictionary<Point, Chess> locationChess)
            {
                switch (chess.Type)
                {
                    case ChessType.Pawn:
                        // excluding en passant
                        return IsPawnReachable(chess, from, to, locationChess);
                        break;
                    case ChessType.Knight:
                        return IsKnightReachable(chess, from, to, locationChess);
                        break;
                    case ChessType.Bishop:
                        return IsBishopReachable(chess, from, to, locationChess);
                        break;
                    case ChessType.Rook:
                        return IsRookReachable(chess, from, to, locationChess);
                        break;
                    case ChessType.King:
                        return IsKingReachable(chess, from, to, locationChess);
                        break;
                    case ChessType.Queen:
                        return IsRookReachable(chess, from, to, locationChess) || IsBishopReachable(chess, from, to, locationChess);
                        break;
                    default:
                        return false;
                        break;
                }
            }

            private static bool IsPawnReachable(Chess chess, Point from, Point to, Dictionary<Point, Chess> locationChess)
            {
                switch (chess.Side)
                {
                    case PlayerSide.White:
                        if (to.Y == from.Y - 1)
                        {
                            if (to.X == from.X)
                            {
                                // pawn go straight forward
                                return !locationChess.ContainsKey(to);
                            }
                            else if (to.X == from.X + 1 || to.X == from.X - 1)
                            {
                                return IsOppositeChess(to, locationChess, chess.Side);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (to.Y == from.Y - 2)
                        {
                            return !HasChess(new Point(from.X, from.Y - 1), locationChess)
                                && !HasChess(to, locationChess)
                                && from.Y == 6;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    case PlayerSide.Black:
                        if (to.Y == from.Y + 1)
                        {
                            if (to.X == from.X)
                            {
                                // pawn go straight forward
                                return !locationChess.ContainsKey(to);
                            }
                            else if (to.X == from.X + 1 || to.X == from.X - 1)
                            {
                                return IsOppositeChess(to, locationChess, chess.Side);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (to.Y == from.Y + 2)
                        {
                            return !HasChess(new Point(from.X, from.Y + 1), locationChess)
                                && !HasChess(to, locationChess)
                                && from.Y == 1;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    default:
                        return false;
                }
            }

            private static bool IsKnightReachable(Chess chess, Point from, Point to, Dictionary<Point, Chess> locationChess)
            {
                int[] offsets = new int[]
                {
                1, 2
                };
                int[] coefficients = new int[]
                {
                -1, 1
                };
                // x_offset = 1 and y_offset = 2
                // or x_offset = 2 and y_offset = 1
                for (int yOffsetIndex = 0; yOffsetIndex < offsets.Length; yOffsetIndex++)
                {
                    int yOffset = offsets[yOffsetIndex];
                    int xOffset = offsets[(yOffsetIndex + 1) % offsets.Length];
                    // x_offset might be negative
                    foreach (int yCoefficient in coefficients)
                    {
                        // y_offset might be negative
                        foreach (int xCoefficient in coefficients)
                        {
                            // check destination
                            if (from.X + xCoefficient * xOffset == to.X
                                    && from.Y + yCoefficient * yOffset == to.Y)
                            {
                                return IsKillableOrStandable(to, locationChess, chess.Side);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
                return false;
            }

            private static bool IsBishopReachable(Chess chess, Point from, Point to, Dictionary<Point, Chess> locationChess)
            {
                if (from == to)
                {
                    // no marching in pace
                    return false;
                }
                // source -> destination: diagonal
                else if (Math.Abs(to.X - from.X) == Math.Abs(to.Y - from.Y))
                {
                    return IsStraightLineReachable(chess, from, to, locationChess);
                }
                else
                {
                    return false;
                }
            }

            private static bool IsRookReachable(Chess chess, Point from, Point to, Dictionary<Point, Chess> locationChess)
            {
                if (from == to)
                {
                    return false;
                }
                // source -> destination: vertical or horizontal
                else if (from.X == to.X || from.Y == to.Y)
                {
                    return IsStraightLineReachable(chess, from, to, locationChess);
                }
                else
                {
                    return false;
                }
            }

            private static bool IsKingReachable(Chess chess, Point from, Point to, Dictionary<Point, Chess> locationChess)
            {
                if (from == to)
                {
                    return false;
                }
                // source -> destination: one step only
                else if (Math.Abs(from.X - to.X) <= 1 && Math.Abs(from.Y - to.Y) <= 1)
                {
                    return IsStraightLineReachable(chess, from, to, locationChess);
                }
                else
                {
                    return false;
                }
            }

            private static bool IsStraightLineReachable(Chess chess, Point from, Point to, Dictionary<Point, Chess> locationChess)
            {
                // source -> destination: orientation
                int xCoefficient;
                int yCoefficient;
                if (to.X > from.X)
                {
                    xCoefficient = 1;
                }
                else if (to.X < from.X)
                {
                    xCoefficient = -1;
                }
                else
                {
                    xCoefficient = 0;
                }
                if (to.Y > from.Y)
                {
                    yCoefficient = 1;
                }
                else if (to.Y < from.Y)
                {
                    yCoefficient = -1;
                }
                else
                {
                    yCoefficient = 0;
                }
                // source -> destination: no chess is in between
                int xOffset = 1;
                int yOffset = 1;
                while (from.X + xCoefficient * xOffset != to.X
                    || from.Y + yCoefficient * yOffset != to.Y)
                {
                    if (HasChess(new Point(
                            from.X + xCoefficient * xOffset,
                            from.Y + yCoefficient * yOffset
                        ), locationChess))
                    {
                        return false;
                    }
                    xOffset++;
                    yOffset++;
                }
                // destination: killable or standable
                return IsKillableOrStandable(new Point(
                    from.X + xCoefficient * xOffset,
                    from.Y + yCoefficient * yOffset
                ), locationChess, chess.Side);
            }

            private static PlayerSide GetOppositeSide(PlayerSide thisSide)
            {
                return (PlayerSide)(((int)thisSide + 1) % Enum.GetValues(typeof(PlayerSide)).Length);
            }

            private static bool IsOppositeChess(Point location, Dictionary<Point, Chess> locationChess, PlayerSide thisSide)
            {
                if (locationChess.ContainsKey(location)
                    && locationChess[location].Side == GetOppositeSide(thisSide))
                {
                    // pawn kill other chess
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private static bool HasChess(Point location, Dictionary<Point, Chess> locationChess)
            {
                return locationChess.ContainsKey(location);
            }

            private static bool IsKillableOrStandable(Point location, Dictionary<Point, Chess> locationChess, PlayerSide thisSide)
            {
                if (!HasChess(location, locationChess))
                {
                    // standable
                    return true;
                }
                else
                {
                    // killable
                    return IsOppositeChess(location, locationChess, thisSide);
                }
            }
        }
    }
