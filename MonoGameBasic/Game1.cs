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

    private KeyboardState _previousKeyboardState;

    private enum GameState
    {
        Menu,
        Playing,
        Settings
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
        _level = new Level(GraphicsDevice);
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
                    _currentState = GameState.Playing;

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
                    // Reset positions or state if desired, or just pause
                }
                _player.Update(gameTime, GraphicsDevice.Viewport);
                _level.Update(gameTime, GraphicsDevice.Viewport);

                // Level Collision & Logic
                bool levelComplete = _level.CheckCollisions(_player);

                if (_player.Health <= 0)
                {
                    // Game Over logic (reset for now)
                    _player.Health = 100;
                    _player.Position = new Vector2(50, 50);
                    _currentState = GameState.Menu;
                }

                if (levelComplete)
                {
                    // Level Completed! Back to menu for now, or reset
                     _player.Health = 100;
                     _player.Position = new Vector2(50, 50);
                    _currentState = GameState.Menu;
                    // Ideally show a "You Win" screen
                }
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
            _spriteBatch.DrawString(_font, $"Health: {_player.Health}", new Vector2(10, 10), Color.White);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
