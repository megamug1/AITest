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

            // Bordered texture: dark outline, medium gray interior
            Texture = new Texture2D(graphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isBorder = x == 0 || y == 0 || x == width - 1 || y == height - 1;
                    data[y * width + x] = isBorder ? new Color(55, 55, 55) : new Color(110, 110, 110);
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
