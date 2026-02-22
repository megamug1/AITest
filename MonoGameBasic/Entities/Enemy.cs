using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameBasic.Entities
{
    public class Enemy
    {
        public Vector2 Position;
        public Vector2 PreviousPosition { get; private set; }
        public Vector2 Velocity;
        public Texture2D Texture { get; private set; }
        public int Width => Texture.Width;
        public int Height => Texture.Height;

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

        private Random _random;

        public Enemy(GraphicsDevice graphicsDevice, Vector2 startPosition, float speedMultiplier = 1.0f)
        {
            Position = startPosition;
            PreviousPosition = startPosition;
            _random = new Random();

            float speed = 150f * speedMultiplier;
            double angle = _random.NextDouble() * Math.PI * 2;
            Velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;

            // Bordered texture: bright red outline, darker red interior
            Texture = new Texture2D(graphicsDevice, 32, 32);
            Color[] data = new Color[32 * 32];
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    bool isBorder = x == 0 || y == 0 || x == 31 || y == 31;
                    data[y * 32 + x] = isBorder ? new Color(220, 50, 50) : new Color(140, 20, 20);
                }
            }
            Texture.SetData(data);
        }

        public void Update(GameTime gameTime, Viewport viewport)
        {
            PreviousPosition = Position;
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity * deltaTime;

            // Bounce off viewport walls
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
