using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameBasic.Entities
{
    public class TrackerEnemy : EnemyBase
    {
        private float _speed;

        public TrackerEnemy(GraphicsDevice graphicsDevice, Vector2 startPosition,
                            float speedMultiplier = 1.0f, int size = 28)
            : base(graphicsDevice, startPosition)
        {
            _speed = 80f * speedMultiplier;

            Texture = CreateBorderedTexture(graphicsDevice, size, size,
                new Color(200, 50, 200), new Color(100, 20, 100));
        }

        public override void Update(GameTime gameTime, Viewport viewport, Vector2 playerPosition)
        {
            PreviousPosition = Position;
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 enemyCenter = Position + new Vector2(Width / 2f, Height / 2f);
            Vector2 direction = playerPosition - enemyCenter;
            if (direction.LengthSquared() > 0.01f)
            {
                direction.Normalize();
                Velocity = direction * _speed;
            }

            Position += Velocity * deltaTime;

            Position.X = MathHelper.Clamp(Position.X, 0, viewport.Width - Width);
            Position.Y = MathHelper.Clamp(Position.Y, 0, viewport.Height - Height);
        }
    }
}
