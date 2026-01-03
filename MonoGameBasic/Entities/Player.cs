using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameBasic.Entities
{
    public class Player
    {
        public Vector2 Position;
        public float Speed = 200f;
        public int Health { get; set; } = 100;
        public Texture2D Texture { get; private set; }
        public int Width => Texture.Width;
        public int Height => Texture.Height;

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

        public Player(GraphicsDevice graphicsDevice, Vector2 startPosition)
        {
            Position = startPosition;

            // Create texture programmatically (White Square)
            Texture = new Texture2D(graphicsDevice, 32, 32);
            Color[] data = new Color[32 * 32];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
            Texture.SetData(data);
        }

        public void Update(GameTime gameTime, Viewport viewport)
        {
            var kstate = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.W))
                Position.Y -= Speed * deltaTime;

            if (kstate.IsKeyDown(Keys.Down) || kstate.IsKeyDown(Keys.S))
                Position.Y += Speed * deltaTime;

            if (kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.A))
                Position.X -= Speed * deltaTime;

            if (kstate.IsKeyDown(Keys.Right) || kstate.IsKeyDown(Keys.D))
                Position.X += Speed * deltaTime;

            // Keep within bounds
            if (Position.X < 0) Position.X = 0;
            if (Position.Y < 0) Position.Y = 0;
            if (Position.X > viewport.Width - Width) Position.X = viewport.Width - Width;
            if (Position.Y > viewport.Height - Height) Position.Y = viewport.Height - Height;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
