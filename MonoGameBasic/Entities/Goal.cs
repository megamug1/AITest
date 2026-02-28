using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameBasic.Entities
{
    public class Goal
    {
        public Vector2 Position { get; private set; }
        public int Width { get; private set; } = 48;
        public int Height { get; private set; } = 48;
        public Texture2D Texture { get; private set; }

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

        public Goal(GraphicsDevice graphicsDevice, Vector2 position)
        {
            Position = position;

            // Glowing forest portal with rune ring
            Texture = new Texture2D(graphicsDevice, Width, Height);
            Color[] data = new Color[Width * Height];
            float cx = Width / 2f - 0.5f;
            float cy = Height / 2f - 0.5f;
            float outerRadius = Width / 2f - 1;
            float innerRadius = outerRadius * 0.55f;
            var portalGlow = new Color(60, 200, 80);
            var portalBright = new Color(120, 255, 140);
            var portalCore = new Color(200, 255, 220);
            var rimGold = new Color(200, 180, 50);
            var rimDark = new Color(140, 120, 30);
            var runeGlow = new Color(180, 255, 100);
            var stone = new Color(80, 75, 65);
            var stoneLight = new Color(100, 95, 85);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    Color pixel = Color.Transparent;

                    if (dist <= outerRadius)
                    {
                        if (dist > outerRadius - 3)
                        {
                            pixel = Color.Lerp(rimDark, rimGold, (outerRadius - dist) / 3f);
                        }
                        else if (dist > innerRadius)
                        {
                            float ringT = (dist - innerRadius) / (outerRadius - 3 - innerRadius);
                            pixel = Color.Lerp(stone, stoneLight, ringT);
                            double angle = Math.Atan2(dy, dx);
                            double runePattern = Math.Sin(angle * 8) * Math.Cos(angle * 3);
                            if (runePattern > 0.7 && dist > innerRadius + 2 && dist < outerRadius - 5)
                                pixel = runeGlow;
                        }
                        else
                        {
                            double angle = Math.Atan2(dy, dx);
                            float swirl = (float)Math.Sin(angle * 3 + dist * 0.3);
                            float t = dist / innerRadius;
                            pixel = t < 0.3f
                                ? Color.Lerp(portalCore, portalBright, t / 0.3f)
                                : Color.Lerp(portalBright, portalGlow, (t - 0.3f) / 0.7f);
                            pixel = Color.Lerp(pixel, portalGlow, swirl * 0.3f);
                        }
                    }

                    data[y * Width + x] = pixel;
                }
            }
            Texture.SetData(data);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
