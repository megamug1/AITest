using System;
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

            // Shadow wraith - ethereal diamond shape with glowing eyes
            int s = size;
            float cx = s / 2f - 0.5f, cy = s / 2f - 0.5f;
            float radius = s / 2f - 1;
            Texture = new Texture2D(graphicsDevice, s, s);
            Color[] data = new Color[s * s];
            var bodyOuter = new Color(60, 18, 70);
            var bodyGlow = new Color(140, 60, 160);
            var eyeGlow = new Color(200, 255, 100);
            var eyeCore = new Color(255, 255, 180);

            for (int y = 0; y < s; y++)
            {
                for (int x = 0; x < s; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    float diamond = Math.Abs(dx) / radius + Math.Abs(dy) / radius;
                    float circle = dist / radius;
                    float shape = diamond * 0.6f + circle * 0.4f;

                    if (dy > 0) shape -= dy / (radius * 3f);

                    if (shape > 1f) { data[y * s + x] = Color.Transparent; continue; }

                    float t = 1f - shape;
                    Color pixel = t < 0.05f ? bodyOuter : Color.Lerp(bodyOuter, bodyGlow, t * 0.7f);

                    if (dist < radius * 0.35f)
                        pixel = Color.Lerp(pixel, bodyGlow, 0.4f);

                    float eyeY = cy - s * 0.08f;
                    float eyeSpacing = s * 0.14f;
                    float leDist = (float)Math.Sqrt((x - (cx - eyeSpacing)) * (x - (cx - eyeSpacing)) + (y - eyeY) * (y - eyeY));
                    float reDist = (float)Math.Sqrt((x - (cx + eyeSpacing)) * (x - (cx + eyeSpacing)) + (y - eyeY) * (y - eyeY));
                    float eyeRadius = s * 0.09f;

                    if (leDist < eyeRadius || reDist < eyeRadius)
                    {
                        float eDist = Math.Min(leDist, reDist);
                        pixel = eDist < eyeRadius * 0.5f ? eyeCore : eyeGlow;
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
