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

            // Hooded forest ranger viewed from top-down
            Texture = new Texture2D(graphicsDevice, 32, 32);
            Color[] data = new Color[32 * 32];
            int s = 32;
            float cx = 15.5f, cy = 15.5f;
            var outline = new Color(20, 42, 15);
            var cloakDark = new Color(35, 68, 25);
            var cloakLight = new Color(72, 128, 50);
            var hoodDark = new Color(28, 52, 18);
            var skin = new Color(185, 145, 105);
            var eyeColor = new Color(15, 15, 25);
            var belt = new Color(105, 72, 32);
            var buckle = new Color(180, 160, 60);
            var boot = new Color(72, 48, 22);

            for (int y = 0; y < s; y++)
            {
                for (int x = 0; x < s; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    float ry = 14f;
                    float rx = 11f + dy * 0.15f;
                    if (rx < 3f) rx = 3f;
                    float ovalDist = (dx * dx) / (rx * rx) + (dy * dy) / (ry * ry);

                    if (ovalDist > 1f) { data[y * s + x] = Color.Transparent; continue; }

                    float edge = 1f - ovalDist;
                    Color pixel;
                    if (edge < 0.06f) pixel = outline;
                    else if (y < 13) pixel = Color.Lerp(cloakDark, hoodDark, 0.3f + edge * 0.3f);
                    else pixel = Color.Lerp(cloakDark, cloakLight, edge * 0.8f);

                    if (y >= 3 && y < 12 && Math.Abs(dx) <= 1.5f)
                        pixel = Color.Lerp(pixel, outline, 0.3f);

                    float faceDy = y - 9f;
                    float faceDist = (dx * dx) / 12f + (faceDy * faceDy) / 6f;
                    if (faceDist < 1f && y >= 7 && y <= 12)
                    {
                        pixel = faceDy < -1.5f ? Color.Lerp(skin, hoodDark, 0.7f) : skin;
                        if (y == 10 && (x == 14 || x == 17)) pixel = eyeColor;
                    }

                    if (y >= 21 && y <= 22 && edge > 0.15f)
                    {
                        pixel = belt;
                        if (Math.Abs(dx) <= 1 && y == 21) pixel = buckle;
                    }

                    if (y > 14 && y < 28 && edge > 0.1f)
                    {
                        if (Math.Abs(dx - 4) < 0.8f || Math.Abs(dx + 4) < 0.8f)
                            pixel = Color.Lerp(pixel, cloakDark, 0.3f);
                    }

                    if (y >= 27 && y <= 29 && edge > 0.1f)
                    {
                        if ((dx >= -5 && dx <= -2) || (dx >= 2 && dx <= 5))
                            pixel = boot;
                    }

                    data[y * s + x] = pixel;
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
