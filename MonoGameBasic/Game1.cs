using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameBasic.Entities;

namespace MonoGameBasic;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;

    private Player _player;
    private Level _level;
    private int _currentLevelIndex = 1;

    private KeyboardState _previousKeyboardState;

    private enum GameState
    {
        Menu,
        Playing,
        Settings,
        GameOver,
        Victory
    }

    private GameState _currentState;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _currentState = GameState.Menu;
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _font = Content.Load<SpriteFont>("File");

        // Initialize Player
        var startPos = new Vector2(50, 50); // Start at top left
        _player = new Player(GraphicsDevice, startPos);

        // Initialize Level
        _level = new Level(GraphicsDevice, _currentLevelIndex);
    }

    protected override void Update(GameTime gameTime)
    {
        var kstate = Keyboard.GetState();
        var gamePadState = GamePad.GetState(PlayerIndex.One);

        switch (_currentState)
        {
            case GameState.Menu:
                // Only exit if Escape is newly pressed, to avoid exiting immediately after pausing
                if (gamePadState.Buttons.Back == ButtonState.Pressed || (kstate.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape)))
                    Exit();

                if (kstate.IsKeyDown(Keys.Enter))
                {
                    // Reset game state when starting
                    _currentLevelIndex = 1;
                    _player.Health = 100;
                    _player.Position = new Vector2(50, 50);
                    _level = new Level(GraphicsDevice, _currentLevelIndex);
                    _currentState = GameState.Playing;
                }

                if (kstate.IsKeyDown(Keys.O))
                    _currentState = GameState.Settings;
                break;

            case GameState.Settings:
                 if (gamePadState.Buttons.Back == ButtonState.Pressed || kstate.IsKeyDown(Keys.Escape))
                    _currentState = GameState.Menu;
                 break;

            case GameState.Playing:
                if (gamePadState.Buttons.Back == ButtonState.Pressed || kstate.IsKeyDown(Keys.Escape))
                {
                    _currentState = GameState.Menu;
                }

                _player.Update(gameTime, GraphicsDevice.Viewport);
                _level.Update(gameTime, GraphicsDevice.Viewport);

                // Level Collision & Logic
                bool levelComplete = _level.CheckCollisions(_player);

                if (_player.Health <= 0)
                {
                    _currentState = GameState.GameOver;
                }

                if (levelComplete)
                {
                    _currentLevelIndex++;
                    if (_currentLevelIndex > 10)
                    {
                        _currentState = GameState.Victory;
                    }
                    else
                    {
                        // Load next level, preserve health
                        _level = new Level(GraphicsDevice, _currentLevelIndex);
                        _player.Position = new Vector2(50, 50);
                    }
                }
                break;

            case GameState.GameOver:
            case GameState.Victory:
                if (kstate.IsKeyDown(Keys.Enter))
                    _currentState = GameState.Menu;
                break;
        }

        _previousKeyboardState = kstate;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        if (_currentState == GameState.Menu)
        {
            _spriteBatch.DrawString(_font, "Blob Game", new Vector2(100, 100), Color.White);
            _spriteBatch.DrawString(_font, "Press Enter to Start", new Vector2(100, 150), Color.White);
            _spriteBatch.DrawString(_font, "Press O for Settings", new Vector2(100, 200), Color.White);
        }
        else if (_currentState == GameState.Settings)
        {
            _spriteBatch.DrawString(_font, "Settings", new Vector2(100, 100), Color.White);
            _spriteBatch.DrawString(_font, "Nothing here yet...", new Vector2(100, 150), Color.White);
            _spriteBatch.DrawString(_font, "Press Escape to Back", new Vector2(100, 200), Color.White);
        }
        else if (_currentState == GameState.Playing)
        {
            _level.Draw(_spriteBatch);
            _player.Draw(_spriteBatch);
            _spriteBatch.DrawString(_font, $"Level: {_currentLevelIndex}", new Vector2(10, 30), Color.White);
            _spriteBatch.DrawString(_font, $"Health: {_player.Health}", new Vector2(10, 10), Color.White);
        }
        else if (_currentState == GameState.GameOver)
        {
             _spriteBatch.DrawString(_font, "Game Over", new Vector2(100, 100), Color.White);
             _spriteBatch.DrawString(_font, "Press Enter to Menu", new Vector2(100, 150), Color.White);
        }
        else if (_currentState == GameState.Victory)
        {
             _spriteBatch.DrawString(_font, "Victory! You beat 10 levels!", new Vector2(100, 100), Color.White);
             _spriteBatch.DrawString(_font, "Press Enter to Menu", new Vector2(100, 150), Color.White);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
