using System;
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

        // Invincibility window after taking a hit (seconds remaining)
        public float InvincibilityTime { get; set; } = 0f;
        public bool IsInvincible => InvincibilityTime > 0f;

        public Texture2D Texture { get; private set; }
        public int Width => Texture.Width;
        public int Height => Texture.Height;

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

        public Player(GraphicsDevice graphicsDevice, Vector2 startPosition)
        {
            Position = startPosition;

            // Bordered texture: white outline, blue-tinted interior
            Texture = new Texture2D(graphicsDevice, 32, 32);
            Color[] data = new Color[32 * 32];
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    bool isBorder = x == 0 || y == 0 || x == 31 || y == 31;
                    data[y * 32 + x] = isBorder ? Color.White : new Color(180, 180, 220);
                }
            }
            Texture.SetData(data);
        }

        public void Update(GameTime gameTime, Viewport viewport)
        {
            var kstate = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (InvincibilityTime > 0f)
                InvincibilityTime = Math.Max(0f, InvincibilityTime - deltaTime);

            if (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.W))
                Position.Y -= Speed * deltaTime;

            if (kstate.IsKeyDown(Keys.Down) || kstate.IsKeyDown(Keys.S))
                Position.Y += Speed * deltaTime;

            if (kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.A))
                Position.X -= Speed * deltaTime;

            if (kstate.IsKeyDown(Keys.Right) || kstate.IsKeyDown(Keys.D))
                Position.X += Speed * deltaTime;

            // Keep within viewport bounds
            if (Position.X < 0) Position.X = 0;
            if (Position.Y < 0) Position.Y = 0;
            if (Position.X > viewport.Width - Width) Position.X = viewport.Width - Width;
            if (Position.Y > viewport.Height - Height) Position.Y = viewport.Height - Height;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Flash at ~10Hz while invincible to signal immunity window
            bool visible = !IsInvincible || (int)(InvincibilityTime * 10) % 2 == 0;
            if (visible)
                spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
