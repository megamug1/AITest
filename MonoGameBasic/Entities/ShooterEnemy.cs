using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameBasic.Entities
{
    public class ShooterEnemy : EnemyBase
    {
        private float _shootInterval;
        private float _shootTimer;
        private float _projectileSpeed;
        private List<Projectile> _pendingProjectiles;

        public ShooterEnemy(GraphicsDevice graphicsDevice, Vector2 startPosition,
                            float speedMultiplier = 1.0f, float shootInterval = 2.0f, int size = 36)
            : base(graphicsDevice, startPosition)
        {
            _shootInterval = shootInterval;
            _shootTimer = shootInterval;
            _projectileSpeed = 200f * speedMultiplier;
            _pendingProjectiles = new List<Projectile>();

            Velocity = Vector2.Zero; // stationary by default

            Texture = CreateBorderedTexture(graphicsDevice, size, size,
                new Color(255, 165, 0), new Color(180, 100, 0));
        }

        public override void Update(GameTime gameTime, Viewport viewport, Vector2 playerPosition)
        {
            PreviousPosition = Position;
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += Velocity * deltaTime;
            BounceOffViewport(viewport);

            _shootTimer -= deltaTime;
            if (_shootTimer <= 0f)
            {
                _shootTimer = _shootInterval;
                FireProjectile(playerPosition);
            }
        }

        private void FireProjectile(Vector2 playerPosition)
        {
            Vector2 center = Position + new Vector2(Width / 2f, Height / 2f);
            Vector2 direction = playerPosition - center;
            if (direction.LengthSquared() > 0.01f)
            {
                direction.Normalize();
                _pendingProjectiles.Add(
                    new Projectile(_graphicsDevice, center, direction * _projectileSpeed));
            }
        }

        public override List<Projectile> GetNewProjectiles()
        {
            var result = new List<Projectile>(_pendingProjectiles);
            _pendingProjectiles.Clear();
            return result;
        }
    }
}
