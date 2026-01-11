using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameBasic.Entities
{
    public class Goal
    {
        public Vector2 Position { get; private set; }
        public int Width { get; private set; } = 32;
        public int Height { get; private set; } = 32;
        public Texture2D Texture { get; private set; }

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

        public Goal(GraphicsDevice graphicsDevice, Vector2 position)
        {
            Position = position;

            // Create texture programmatically (Green Square)
            Texture = new Texture2D(graphicsDevice, Width, Height);
            Color[] data = new Color[Width * Height];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Green;
            Texture.SetData(data);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
