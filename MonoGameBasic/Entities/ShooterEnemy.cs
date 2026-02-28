using System;
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
            Velocity = Vector2.Zero;

            // Skeleton archer - hooded skull with bow
            int s = size;
            float cx = s / 2f - 0.5f, cy = s / 2f - 0.5f;
            float radius = s / 2f - 1;
            Texture = new Texture2D(graphicsDevice, s, s);
            Color[] data = new Color[s * s];
            var outline = new Color(100, 55, 10);
            var bodyDark = new Color(140, 80, 20);
            var bodyLight = new Color(210, 145, 55);
            var skull = new Color(220, 210, 190);
            var eyeSocket = new Color(30, 10, 10);
            var bow = new Color(90, 60, 25);

            for (int y = 0; y < s; y++)
            {
                for (int x = 0; x < s; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    float rx = radius * 0.85f;
                    float ry = radius;
                    float cornerDist = (dx * dx) / (rx * rx) + (dy * dy) / (ry * ry);

                    if (cornerDist > 1.1f) { data[y * s + x] = Color.Transparent; continue; }

                    float t = 1f - cornerDist;
                    Color pixel = t < 0.05f ? outline : Color.Lerp(bodyDark, bodyLight, t * 0.5f);

                    if (dy < -radius * 0.1f)
                        pixel = Color.Lerp(pixel, outline, 0.3f);

                    // Skull face
                    float faceDy = dy + radius * 0.15f;
                    float faceDist = (dx * dx) / (radius * 0.35f * radius * 0.35f) + (faceDy * faceDy) / (radius * 0.3f * radius * 0.3f);
                    if (faceDist < 1f && dy < radius * 0.2f)
                    {
                        pixel = skull;
                        float eyeY = cy - radius * 0.2f;
                        float eyeSpacing = s * 0.12f;
                        float leDist = (float)Math.Sqrt((x - (cx - eyeSpacing)) * (x - (cx - eyeSpacing)) + (y - eyeY) * (y - eyeY));
                        float reDist = (float)Math.Sqrt((x - (cx + eyeSpacing)) * (x - (cx + eyeSpacing)) + (y - eyeY) * (y - eyeY));
                        if (leDist < s * 0.08f || reDist < s * 0.08f)
                            pixel = eyeSocket;
                        if (dy > radius * 0.05f && dy < radius * 0.15f && Math.Abs(dx) < radius * 0.15f)
                            pixel = eyeSocket;
                    }

                    // Bow on the side
                    float bowX = cx + radius * 0.7f;
                    if (dy > -radius * 0.6f && dy < radius * 0.6f)
                    {
                        float bowCurve = (float)Math.Sin(dy / radius * Math.PI) * radius * 0.15f;
                        if (Math.Abs(x - (bowX + bowCurve)) < 1.5f)
                            pixel = bow;
                    }

                    data[y * s + x] = pixel;
                }
            }
            Texture.SetData(data);
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
