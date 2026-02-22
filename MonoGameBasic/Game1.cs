using System;
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

    // 1x1 white texture used for drawing filled rectangles (health bar, etc.)
    private Texture2D _pixelTexture;

    private Player _player;
    private Level _level;
    private int _currentLevelIndex = 1;

    private KeyboardState _previousKeyboardState;

    // Settings state
    private int _settingsSelection = 0;      // 0 = Player Speed, 1 = Enemy Speed
    private int _playerSpeedOption = 1;      // index into SpeedMultipliers
    private int _enemySpeedOption  = 1;

    private static readonly float[]  SpeedMultipliers = { 0.5f, 1.0f, 1.5f };
    private static readonly string[] SpeedLabels      = { "Slow", "Normal", "Fast" };

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

        // Reusable 1x1 white pixel for drawing solid rectangles
        _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        _pixelTexture.SetData(new[] { Color.White });

        var startPos = new Vector2(50, 50);
        _player = new Player(GraphicsDevice, startPos);
        _level  = new Level(GraphicsDevice, _currentLevelIndex, SpeedMultipliers[_enemySpeedOption]);
    }

    protected override void Update(GameTime gameTime)
    {
        var kstate      = Keyboard.GetState();
        var gamePadState = GamePad.GetState(PlayerIndex.One);

        // Edge-detection helpers (true only on the frame the key is first pressed)
        bool upPressed    = kstate.IsKeyDown(Keys.Up)    && !_previousKeyboardState.IsKeyDown(Keys.Up);
        bool downPressed  = kstate.IsKeyDown(Keys.Down)  && !_previousKeyboardState.IsKeyDown(Keys.Down);
        bool leftPressed  = kstate.IsKeyDown(Keys.Left)  && !_previousKeyboardState.IsKeyDown(Keys.Left);
        bool rightPressed = kstate.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right);
        bool enterPressed = kstate.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter);
        bool escPressed   = (kstate.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
                            || gamePadState.Buttons.Back == ButtonState.Pressed;

        switch (_currentState)
        {
            case GameState.Menu:
                if (escPressed)
                    Exit();

                if (enterPressed)
                {
                    _currentLevelIndex = 1;
                    _player = new Player(GraphicsDevice, new Vector2(50, 50));
                    _player.Speed = 200f * SpeedMultipliers[_playerSpeedOption];
                    _level = new Level(GraphicsDevice, _currentLevelIndex, SpeedMultipliers[_enemySpeedOption]);
                    _currentState = GameState.Playing;
                }

                if (kstate.IsKeyDown(Keys.O) && !_previousKeyboardState.IsKeyDown(Keys.O))
                    _currentState = GameState.Settings;
                break;

            case GameState.Settings:
                if (escPressed)
                    _currentState = GameState.Menu;

                // Navigate rows
                if (upPressed)   _settingsSelection = Math.Max(0, _settingsSelection - 1);
                if (downPressed) _settingsSelection = Math.Min(1, _settingsSelection + 1);

                // Change selected value
                if (_settingsSelection == 0)
                {
                    if (leftPressed)  _playerSpeedOption = Math.Max(0, _playerSpeedOption - 1);
                    if (rightPressed) _playerSpeedOption = Math.Min(2, _playerSpeedOption + 1);
                }
                else
                {
                    if (leftPressed)  _enemySpeedOption = Math.Max(0, _enemySpeedOption - 1);
                    if (rightPressed) _enemySpeedOption = Math.Min(2, _enemySpeedOption + 1);
                }
                break;

            case GameState.Playing:
                if (escPressed)
                    _currentState = GameState.Menu;

                _player.Update(gameTime, GraphicsDevice.Viewport);
                Vector2 playerCenter = _player.Position + new Vector2(_player.Width / 2f, _player.Height / 2f);
                _level.Update(gameTime, GraphicsDevice.Viewport, playerCenter);

                bool levelComplete = _level.CheckCollisions(_player);

                if (_player.Health <= 0)
                    _currentState = GameState.GameOver;

                if (levelComplete)
                {
                    _currentLevelIndex++;
                    if (_currentLevelIndex > 10)
                    {
                        _currentState = GameState.Victory;
                    }
                    else
                    {
                        _level = new Level(GraphicsDevice, _currentLevelIndex, SpeedMultipliers[_enemySpeedOption]);
                        _player.Position = new Vector2(50, 50);
                    }
                }
                break;

            case GameState.GameOver:
            case GameState.Victory:
                if (enterPressed)
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

        switch (_currentState)
        {
            case GameState.Menu:
                _spriteBatch.DrawString(_font, "Blob Game",              new Vector2(100, 100), Color.White);
                _spriteBatch.DrawString(_font, "Press Enter to Start",   new Vector2(100, 150), Color.White);
                _spriteBatch.DrawString(_font, "Press O for Settings",   new Vector2(100, 200), Color.White);
                _spriteBatch.DrawString(_font, "Press Escape to Quit",   new Vector2(100, 250), Color.LightGray);
                break;

            case GameState.Settings:
                _spriteBatch.DrawString(_font, "Settings", new Vector2(100, 60), Color.White);
                _spriteBatch.DrawString(_font, "Up/Down to select    Left/Right to change", new Vector2(100, 90), Color.LightGray);

                // Row 0: Player Speed
                Color playerRowColor = _settingsSelection == 0 ? Color.Yellow : Color.White;
                string playerArrow   = _settingsSelection == 0 ? "> " : "  ";
                _spriteBatch.DrawString(_font,
                    $"{playerArrow}Player Speed:  < {SpeedLabels[_playerSpeedOption]} >",
                    new Vector2(100, 160), playerRowColor);

                // Row 1: Enemy Speed
                Color enemyRowColor = _settingsSelection == 1 ? Color.Yellow : Color.White;
                string enemyArrow   = _settingsSelection == 1 ? "> " : "  ";
                _spriteBatch.DrawString(_font,
                    $"{enemyArrow}Enemy Speed:   < {SpeedLabels[_enemySpeedOption]} >",
                    new Vector2(100, 210), enemyRowColor);

                _spriteBatch.DrawString(_font, "Press Escape to go back", new Vector2(100, 310), Color.LightGray);
                break;

            case GameState.Playing:
                _level.Draw(_spriteBatch);
                _player.Draw(_spriteBatch);
                DrawHUD();
                break;

            case GameState.GameOver:
                _spriteBatch.DrawString(_font, "Game Over",                    new Vector2(100, 100), Color.Red);
                _spriteBatch.DrawString(_font, "Press Enter to return to Menu", new Vector2(100, 150), Color.White);
                break;

            case GameState.Victory:
                _spriteBatch.DrawString(_font, "Victory! You beat all 10 levels!", new Vector2(100, 100), Color.Gold);
                _spriteBatch.DrawString(_font, "Press Enter to return to Menu",    new Vector2(100, 150), Color.White);
                break;
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawHUD()
    {
        const int barX      = 10;
        const int barY      = 10;
        const int barWidth  = 200;
        const int barHeight = 16;

        float healthRatio = Math.Max(0f, _player.Health / 100f);

        // Choose bar color based on remaining health
        Color barColor = healthRatio > 0.5f ? Color.LimeGreen
                       : healthRatio > 0.25f ? Color.Yellow
                       : Color.Red;

        // Background (depleted portion)
        _spriteBatch.Draw(_pixelTexture, new Rectangle(barX, barY, barWidth, barHeight), Color.DarkRed);
        // Foreground (remaining health)
        _spriteBatch.Draw(_pixelTexture, new Rectangle(barX, barY, (int)(barWidth * healthRatio), barHeight), barColor);

        _spriteBatch.DrawString(_font, $"HP: {_player.Health}",         new Vector2(barX + barWidth + 8, barY - 2), Color.White);
        _spriteBatch.DrawString(_font, $"Level: {_currentLevelIndex}/10", new Vector2(barX, barY + barHeight + 4), Color.White);
    }
}
