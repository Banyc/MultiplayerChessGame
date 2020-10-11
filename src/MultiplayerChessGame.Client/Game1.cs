using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MultiplayerChessGame.Client.Helpers;
using MultiplayerChessGame.Client.Services;
using MultiplayerChessGame.Shared.Helpers;
using MultiplayerChessGame.Shared.Models;

namespace MultiplayerChessGame.Client
{
    public partial class Game1 : Game
    {
        private readonly Random _random = new Random();
        private readonly SharedGameState _state;
        private GameClientService _client;
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private readonly List<Texture2D> _chessTextures = new List<Texture2D>();
        private Vector2 _chessGrillePosition = Vector2.Zero;
        private Texture2D _chessGrille;
        private Texture2D _selectionSquare;
        private Texture2D _chessMoveSourceSquare;
        private Texture2D _chessMoveDestinationSquare;
        private Texture2D _newChessSquare;
        private Point _selectionLogicPosition = Point.Zero;
        private Point? _selectedLogicalPosition = null;
        private PlayerSide _playerSide = PlayerSide.White;

        private readonly Dictionary<string, (int textureIndex, Vector2 position)> _chessPiecesPositions = new Dictionary<string, (int textureIndex, Vector2 position)>();
        private readonly float _grilleTextureScale = 0.9f;
        private float ChessTextureScale
        {
            get
            {
                int chessTextureHeight = this.GrilleTextureActualHeight / 8;
                return (float)chessTextureHeight / _chessTextures[0].Height;
            }
        }
        private float SelectionTextureScale
        {
            get
            {
                int selectionTextureHeight = this.GrilleTextureActualHeight / 8;
                return (float)selectionTextureHeight / _selectionSquare.Height;
            }
        }
        private float NewChessSquareTextureScale
        {
            get
            {
                int squareTextureHeight = this.GrilleTextureActualHeight / 8;
                return (float)squareTextureHeight / _newChessSquare.Height;
            }
        }
        private int GrilleTextureActualWidth
        {
            get
            {
                return (int)(_chessGrille.Width * _grilleTextureScale);
            }
        }
        private int GrilleTextureActualHeight
        {
            get
            {
                return (int)(_chessGrille.Height * _grilleTextureScale);
            }
        }

        public Game1()
        {
            _state = SharedGameStateHelper.GetNewSharedGameState();

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _client = new GameClientService(_state);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _chessGrille = this.Content.Load<Texture2D>("chessgrille");
            Texture2D chessPieces = this.Content.Load<Texture2D>("chesspieces");
            _selectionSquare = this.Content.Load<Texture2D>("selected");
            _chessMoveSourceSquare = this.Content.Load<Texture2D>("purpleSquare");
            _chessMoveDestinationSquare = this.Content.Load<Texture2D>("purpleSquare");
            _newChessSquare = this.Content.Load<Texture2D>("blueSquare");

            // cut chess pieces
            const int numPiecesX = 6;
            const int numPiecesY = 2;
            int chessPieceHeight = chessPieces.Height / numPiecesY;
            int chessPieceWidth = chessPieces.Width / numPiecesX;
            for (int y = 0; y + chessPieceHeight <= chessPieces.Height; y += chessPieceHeight)
            {
                for (int x = 0; x + chessPieceWidth <= chessPieces.Width; x += chessPieceWidth)
                {
                    Rectangle sourceRectangle = new Rectangle(x, y, chessPieceWidth, chessPieceHeight);
                    Texture2D newTexture = new Texture2D(this.GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
                    Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
                    chessPieces.GetData(0, sourceRectangle, data, 0, data.Length);
                    newTexture.SetData(data);
                    // put new texture into an array
                    _chessTextures.Add(newTexture);
                }
            }

            // adjust grille position to center
            _chessGrillePosition.X = (this.GraphicsDevice.Viewport.Width - this.GrilleTextureActualWidth) / 2;
            _chessGrillePosition.Y = (this.GraphicsDevice.Viewport.Height - this.GrilleTextureActualHeight) / 2;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            UpdateChessPositions();
            // keyboard selection
            KeyboardState keyboardState = KeyboardHelper.GetState();
            Keys[] pressedKeys = keyboardState.GetPressedKeys();
            foreach (Keys key in pressedKeys)
            {
                HandleSelectionMoveKey(key);
                // move chess
                HandleChessMoveKey(key);
                // change chess
                HandleChessChangeKey(key);
                // undo
                HandleUndoChessBoard(key);
                // switch side
                HandleSwitchSide(key);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            // grille
            _spriteBatch.Draw(_chessGrille, _chessGrillePosition, null, Color.White, 0f,
                            Vector2.Zero, _grilleTextureScale, SpriteEffects.None, 0f);
            // chess pieces
            foreach ((string chessId, (int textureIndex, Vector2 position)) in _chessPiecesPositions)
            {
                _spriteBatch.Draw(_chessTextures[textureIndex], position, null, Color.White, 0f,
                                Vector2.Zero, this.ChessTextureScale, SpriteEffects.None, 0f);
            }
            // selected square
            if (_selectedLogicalPosition.HasValue)
            {
                _spriteBatch.Draw(_selectionSquare, GetActualPositionInGrille(_selectedLogicalPosition.Value), null, Color.White, 0f,
                                Vector2.Zero, this.SelectionTextureScale, SpriteEffects.None, 0f);
            }
            // selection square
            _spriteBatch.Draw(_selectionSquare, GetActualPositionInGrille(_selectionLogicPosition), null, Color.White, 0f,
                            Vector2.Zero, this.SelectionTextureScale, SpriteEffects.None, 0f);
            // new chess location
            if (_state.Board.NewChessLocation != null)
            {
                _spriteBatch.Draw(_newChessSquare, GetActualPositionInGrille(_state.Board.NewChessLocation.Value), null, Color.White, 0f,
                                Vector2.Zero, this.NewChessSquareTextureScale, SpriteEffects.None, 0f);
            }
            // chess move previous location
            if (_state.Board.PreviousLocation != null)
            {
                _spriteBatch.Draw(_chessMoveSourceSquare, GetActualPositionInGrille(_state.Board.PreviousLocation.Value), null, Color.White, 0f,
                                Vector2.Zero, this.NewChessSquareTextureScale, SpriteEffects.None, 0f);
            }
            // chess move current location
            if (_state.Board.CurrentLocation != null)
            {
                _spriteBatch.Draw(_chessMoveDestinationSquare, GetActualPositionInGrille(_state.Board.CurrentLocation.Value), null, Color.White, 0f,
                                Vector2.Zero, this.NewChessSquareTextureScale, SpriteEffects.None, 0f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void UpdateChessPositions()
        {
            _chessPiecesPositions.Clear();
            try
            {
                foreach (var item in _state.Board.LocationChess)
                {
                    // int positionX = (8 * (int)_playerSide - item.Key.X) * this.GrilleTextureActualWidth / 8;
                    // int positionY = (8 * (int)_playerSide - item.Key.Y) * this.GrilleTextureActualHeight / 8;
                    int positionX = item.Key.X * this.GrilleTextureActualWidth / 8;
                    int positionY = item.Key.Y * this.GrilleTextureActualHeight / 8;
                    if (_playerSide == PlayerSide.Black)
                    {
                        positionX = (7 - item.Key.X) * this.GrilleTextureActualWidth / 8;
                        positionY = (7 - item.Key.Y) * this.GrilleTextureActualHeight / 8;
                    }
                    Vector2 chessPosition = new Vector2(positionX, positionY) + _chessGrillePosition;
                    chessPosition.X += (this.GrilleTextureActualWidth / 8 - _chessTextures[0].Width * this.ChessTextureScale) / 2;
                    int textureIndex = (int)item.Value.Side * 6 + (int)item.Value.Type;
                    // Texture2D chessTexture = _chessTextures[textureIndex];
                    _chessPiecesPositions.Add(item.Value.ID, (textureIndex, chessPosition));
                }
            }
            catch (InvalidOperationException ex)
            {
                // An unhandled exception of type 'System.InvalidOperationException' occurred in System.Private.CoreLib.dll: 'Collection was modified; enumeration operation may not execute.'
                // ignore
            }
        }

        private Vector2 GetActualPositionInGrille(System.Drawing.Point logicalPosition)
        {
            Point point = new Point(logicalPosition.X, logicalPosition.Y);
            return GetActualPositionInGrille(point);
        }
        private Vector2 GetActualPositionInGrille(Point logicalPosition)
        {
            Vector2 offsetPositionToGrille = new Vector2(
                logicalPosition.X * this.GrilleTextureActualWidth / 8,
                logicalPosition.Y * this.GrilleTextureActualHeight / 8
            );
            if (_playerSide == PlayerSide.Black)
            {
                offsetPositionToGrille = new Vector2(
                    (7 - logicalPosition.X) * this.GrilleTextureActualWidth / 8,
                    (7 - logicalPosition.Y) * this.GrilleTextureActualHeight / 8
                );
            }
            return offsetPositionToGrille + _chessGrillePosition;
        }
    }
}
