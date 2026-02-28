using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameBasic.Entities
{
    public class Projectile
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Texture2D Texture { get; private set; }
        public int Width => Texture.Width;
        public int Height => Texture.Height;
        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
        public bool IsActive { get; set; } = true;

        public Projectile(GraphicsDevice graphicsDevice, Vector2 startPosition, Vector2 velocity)
        {
            Position = startPosition - new Vector2(4, 4);
            Velocity = velocity;

            // Glowing thorn / magic bolt
            Texture = new Texture2D(graphicsDevice, 8, 8);
            Color[] data = new Color[64];
            var glow = new Color(180, 255, 80);
            var core = new Color(255, 255, 200);
            var rim = new Color(80, 140, 30);

            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                {
                    float dx = x - 3.5f;
                    float dy = y - 3.5f;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (dist > 3.5f) data[y * 8 + x] = Color.Transparent;
                    else if (dist > 2.5f) data[y * 8 + x] = rim;
                    else if (dist > 1.2f) data[y * 8 + x] = glow;
                    else data[y * 8 + x] = core;
                }
            Texture.SetData(data);
        }

        public void Update(GameTime gameTime, Viewport viewport)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity * deltaTime;

            if (Position.X < -Width || Position.X > viewport.Width ||
                Position.Y < -Height || Position.Y > viewport.Height)
            {
                IsActive = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
                spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
