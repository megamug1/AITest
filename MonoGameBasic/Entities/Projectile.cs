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
            Position = startPosition - new Vector2(4, 4); // center on spawn point
            Velocity = velocity;

            Texture = new Texture2D(graphicsDevice, 8, 8);
            Color[] data = new Color[64];
            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                {
                    bool isBorder = x == 0 || y == 0 || x == 7 || y == 7;
                    data[y * 8 + x] = isBorder ? Color.Yellow : Color.White;
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
