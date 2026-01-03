using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameBasic.Entities
{
    public class Enemy
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Texture2D Texture { get; private set; }
        public int Width => Texture.Width;
        public int Height => Texture.Height;

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

        private Random _random;

        public Enemy(GraphicsDevice graphicsDevice, Vector2 startPosition)
        {
            Position = startPosition;
            _random = new Random();

            // Random initial velocity
            float speed = 150f;
            double angle = _random.NextDouble() * Math.PI * 2;
            Velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;

            // Create texture programmatically (Red Square)
            Texture = new Texture2D(graphicsDevice, 32, 32);
            Color[] data = new Color[32 * 32];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Red;
            Texture.SetData(data);
        }

        public void Update(GameTime gameTime, Viewport viewport)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity * deltaTime;

            // Bounce off walls
            if (Position.X < 0)
            {
                Position.X = 0;
                Velocity.X *= -1;
            }
            else if (Position.X > viewport.Width - Width)
            {
                Position.X = viewport.Width - Width;
                Velocity.X *= -1;
            }

            if (Position.Y < 0)
            {
                Position.Y = 0;
                Velocity.Y *= -1;
            }
            else if (Position.Y > viewport.Height - Height)
            {
                Position.Y = viewport.Height - Height;
                Velocity.Y *= -1;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
