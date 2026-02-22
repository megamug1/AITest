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

            // Gold double-border with green interior — clearly distinct from all other entities
            Texture = new Texture2D(graphicsDevice, Width, Height);
            Color[] data = new Color[Width * Height];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    bool isOuterBorder = x == 0 || y == 0 || x == Width - 1 || y == Height - 1;
                    bool isInnerBorder = x == 1 || y == 1 || x == Width - 2 || y == Height - 2;
                    if (isOuterBorder || isInnerBorder)
                        data[y * Width + x] = Color.Gold;
                    else
                        data[y * Width + x] = new Color(40, 160, 40);
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
