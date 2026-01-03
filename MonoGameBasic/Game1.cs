using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameBasic;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _blobTexture;
    private Vector2 _blobPosition;
    private float _blobSpeed;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Initial position
        _blobPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        _blobSpeed = 200f; // Pixels per second

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Create a simple texture programmatically (a white square)
        _blobTexture = new Texture2D(GraphicsDevice, 32, 32);
        Color[] data = new Color[32 * 32];
        for(int i = 0; i < data.Length; ++i) data[i] = Color.White;
        _blobTexture.SetData(data);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var kstate = Keyboard.GetState();
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.W))
        {
            _blobPosition.Y -= _blobSpeed * deltaTime;
        }

        if (kstate.IsKeyDown(Keys.Down) || kstate.IsKeyDown(Keys.S))
        {
            _blobPosition.Y += _blobSpeed * deltaTime;
        }

        if (kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.A))
        {
            _blobPosition.X -= _blobSpeed * deltaTime;
        }

        if (kstate.IsKeyDown(Keys.Right) || kstate.IsKeyDown(Keys.D))
        {
            _blobPosition.X += _blobSpeed * deltaTime;
        }

        // Keep within bounds
        var viewport = GraphicsDevice.Viewport;
        if (_blobPosition.X < 0) _blobPosition.X = 0;
        if (_blobPosition.Y < 0) _blobPosition.Y = 0;
        if (_blobPosition.X > viewport.Width - _blobTexture.Width) _blobPosition.X = viewport.Width - _blobTexture.Width;
        if (_blobPosition.Y > viewport.Height - _blobTexture.Height) _blobPosition.Y = viewport.Height - _blobTexture.Height;


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        _spriteBatch.Draw(_blobTexture, _blobPosition, Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
