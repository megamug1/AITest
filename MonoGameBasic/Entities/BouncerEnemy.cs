using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameBasic.Entities
{
    public class BouncerEnemy : EnemyBase
    {
        public BouncerEnemy(GraphicsDevice graphicsDevice, Vector2 startPosition,
                            float speedMultiplier = 1.0f, int size = 32)
            : base(graphicsDevice, startPosition)
        {
            float speed = 150f * speedMultiplier;
            var random = new Random(startPosition.GetHashCode());
            double angle = random.NextDouble() * Math.PI * 2;
            Velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;

            Texture = CreateBorderedTexture(graphicsDevice, size, size,
                new Color(220, 50, 50), new Color(140, 20, 20));
        }

        public override void Update(GameTime gameTime, Viewport viewport, Vector2 playerPosition)
        {
            PreviousPosition = Position;
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity * deltaTime;
            BounceOffViewport(viewport);
        }
    }
}
