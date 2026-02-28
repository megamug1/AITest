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

            // Forest blob creature with eyes
            int s = size;
            float cx = s / 2f - 0.5f, cy = s / 2f - 0.5f;
            float radius = s / 2f - 1;
            Texture = new Texture2D(graphicsDevice, s, s);
            Color[] data = new Color[s * s];
            var blobOutline = new Color(120, 20, 15);
            var blobDark = new Color(140, 30, 20);
            var blobLight = new Color(200, 70, 45);
            var eyeWhite = new Color(230, 230, 220);
            var pupil = new Color(20, 15, 15);

            for (int y = 0; y < s; y++)
            {
                for (int x = 0; x < s; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    float wobble = 1f + 0.08f * (float)Math.Sin(Math.Atan2(dy, dx) * 5);
                    float adjustedDist = dist / (radius * wobble);

                    if (adjustedDist > 1f) { data[y * s + x] = Color.Transparent; continue; }

                    float t = 1f - adjustedDist;
                    Color pixel = t < 0.08f ? blobOutline : Color.Lerp(blobDark, blobLight, t * 0.8f);

                    float hlDist = (float)Math.Sqrt((dx + radius * 0.3f) * (dx + radius * 0.3f) + (dy + radius * 0.3f) * (dy + radius * 0.3f));
                    if (hlDist < radius * 0.35f)
                        pixel = Color.Lerp(pixel, blobLight, 0.4f * (1f - hlDist / (radius * 0.35f)));

                    float eyeY = cy - s * 0.1f;
                    float eyeSpacing = s * 0.18f;
                    float leDist = (float)Math.Sqrt((x - (cx - eyeSpacing)) * (x - (cx - eyeSpacing)) + (y - eyeY) * (y - eyeY));
                    float reDist = (float)Math.Sqrt((x - (cx + eyeSpacing)) * (x - (cx + eyeSpacing)) + (y - eyeY) * (y - eyeY));
                    float eyeRadius = s * 0.12f;
                    float pupilRadius = s * 0.06f;

                    if (leDist < eyeRadius || reDist < eyeRadius)
                    {
                        pixel = eyeWhite;
                        if (leDist < pupilRadius || reDist < pupilRadius)
                            pixel = pupil;
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
        }
    }
}
