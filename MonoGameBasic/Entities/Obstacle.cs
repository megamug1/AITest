using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameBasic.Entities
{
    public class Obstacle
    {
        public Vector2 Position { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Texture2D Texture { get; private set; }

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

        public Obstacle(GraphicsDevice graphicsDevice, Vector2 position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;

            // Stone wall with moss texture
            Texture = new Texture2D(graphicsDevice, width, height);
            Color[] data = new Color[width * height];
            var stoneDark = new Color(60, 56, 50);
            var stoneLight = new Color(110, 105, 95);
            var mortar = new Color(45, 42, 38);
            var moss = new Color(50, 82, 32);
            var outlineColor = new Color(30, 28, 24);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    {
                        data[y * width + x] = outlineColor;
                        continue;
                    }

                    int brickH = 8, brickW = 12;
                    int row = y / brickH;
                    int offset = (row % 2 == 0) ? 0 : brickW / 2;
                    int bx = (x + offset) % brickW;
                    int by = y % brickH;

                    Color pixel;
                    if (bx == 0 || by == 0)
                        pixel = mortar;
                    else
                    {
                        int brickId = row * 17 + (x + offset) / brickW * 7;
                        float v = (brickId % 20) / 20f;
                        pixel = Color.Lerp(stoneDark, stoneLight, v);
                    }

                    float mossChance = (float)Math.Sin(x * 0.7 + y * 0.5) * (float)Math.Cos(x * 0.3 - y * 0.8);
                    if (mossChance > 0.4f && (y < 4 || y > height - 5 || x < 4 || x > width - 5))
                        pixel = moss;

                    data[y * width + x] = pixel;
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
