using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameBasic.Entities
{
    public abstract class EnemyBase
    {
        public Vector2 Position;
        public Vector2 PreviousPosition { get; protected set; }
        public Vector2 Velocity;
        public Texture2D Texture { get; protected set; }
        public int Width => Texture.Width;
        public int Height => Texture.Height;

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

        protected GraphicsDevice _graphicsDevice;

        protected EnemyBase(GraphicsDevice graphicsDevice, Vector2 startPosition)
        {
            _graphicsDevice = graphicsDevice;
            Position = startPosition;
            PreviousPosition = startPosition;
        }

        public abstract void Update(GameTime gameTime, Viewport viewport, Vector2 playerPosition);

        public virtual List<Projectile> GetNewProjectiles() => new List<Projectile>();

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }

        protected void BounceOffViewport(Viewport viewport)
        {
            if (Position.X < 0) { Position.X = 0; Velocity.X *= -1; }
            else if (Position.X > viewport.Width - Width) { Position.X = viewport.Width - Width; Velocity.X *= -1; }

            if (Position.Y < 0) { Position.Y = 0; Velocity.Y *= -1; }
            else if (Position.Y > viewport.Height - Height) { Position.Y = viewport.Height - Height; Velocity.Y *= -1; }
        }

        protected static Texture2D CreateBorderedTexture(GraphicsDevice gd, int w, int h, Color border, Color fill)
        {
            var tex = new Texture2D(gd, w, h);
            Color[] data = new Color[w * h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    bool isBorder = x == 0 || y == 0 || x == w - 1 || y == h - 1;
                    data[y * w + x] = isBorder ? border : fill;
                }
            tex.SetData(data);
            return tex;
        }
    }
}
