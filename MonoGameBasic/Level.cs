using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameBasic.Entities;

namespace MonoGameBasic
{
    public class Level
    {
        public List<Obstacle> Obstacles { get; private set; }
        public List<EnemyBase> Enemies { get; private set; }
        public List<Projectile> Projectiles { get; private set; }
        public Goal Goal { get; private set; }

        private GraphicsDevice _graphicsDevice;
        private int _levelIndex;
        private float _enemySpeedMultiplier;

        public Level(GraphicsDevice graphicsDevice, int levelIndex, float enemySpeedMultiplier = 1.0f)
        {
            _graphicsDevice = graphicsDevice;
            _levelIndex = levelIndex;
            _enemySpeedMultiplier = enemySpeedMultiplier;

            Obstacles = new List<Obstacle>();
            Enemies = new List<EnemyBase>();
            Projectiles = new List<Projectile>();

            BuildFromData();
        }

        private void BuildFromData()
        {
            var def = LevelData.Levels[_levelIndex - 1];

            foreach (var o in def.Obstacles)
                Obstacles.Add(new Obstacle(_graphicsDevice, new Vector2(o.X, o.Y), o.Width, o.Height));

            foreach (var e in def.Enemies)
            {
                var pos = new Vector2(e.X, e.Y);

                switch (e.Type)
                {
                    case EnemyType.Bouncer:
                        Enemies.Add(new BouncerEnemy(_graphicsDevice, pos,
                            e.SpeedOrInterval * _enemySpeedMultiplier, e.Size));
                        break;

                    case EnemyType.Tracker:
                        Enemies.Add(new TrackerEnemy(_graphicsDevice, pos,
                            e.SpeedOrInterval * _enemySpeedMultiplier, e.Size));
                        break;

                    case EnemyType.Shooter:
                        float interval = e.SpeedOrInterval / _enemySpeedMultiplier;
                        Enemies.Add(new ShooterEnemy(_graphicsDevice, pos,
                            _enemySpeedMultiplier, interval, e.Size));
                        break;
                }
            }

            Goal = new Goal(_graphicsDevice, new Vector2(def.GoalX, def.GoalY));
        }

        public void Update(GameTime gameTime, Viewport viewport, Vector2 playerPosition)
        {
            foreach (var enemy in Enemies)
            {
                enemy.Update(gameTime, viewport, playerPosition);

                // Collect projectiles spawned by shooters
                var newProjectiles = enemy.GetNewProjectiles();
                Projectiles.AddRange(newProjectiles);

                // Enemy vs obstacle collision
                foreach (var obstacle in Obstacles)
                {
                    if (!enemy.Bounds.Intersects(obstacle.Bounds))
                        continue;

                    Rectangle intersection = Rectangle.Intersect(enemy.Bounds, obstacle.Bounds);

                    bool wasLeftOf  = enemy.PreviousPosition.X + enemy.Width <= obstacle.Bounds.Left;
                    bool wasRightOf = enemy.PreviousPosition.X >= obstacle.Bounds.Right;
                    bool wasAbove   = enemy.PreviousPosition.Y + enemy.Height <= obstacle.Bounds.Top;
                    bool wasBelow   = enemy.PreviousPosition.Y >= obstacle.Bounds.Bottom;

                    bool horizontalApproach = wasLeftOf || wasRightOf;
                    bool verticalApproach   = wasAbove  || wasBelow;

                    if (horizontalApproach || (!verticalApproach && intersection.Width < intersection.Height))
                    {
                        if (enemy.Bounds.Center.X < obstacle.Bounds.Center.X)
                            enemy.Position = new Vector2(obstacle.Bounds.Left - enemy.Width, enemy.Position.Y);
                        else
                            enemy.Position = new Vector2(obstacle.Bounds.Right, enemy.Position.Y);

                        enemy.Velocity = new Vector2(-enemy.Velocity.X, enemy.Velocity.Y);
                    }
                    else
                    {
                        if (enemy.Bounds.Center.Y < obstacle.Bounds.Center.Y)
                            enemy.Position = new Vector2(enemy.Position.X, obstacle.Bounds.Top - enemy.Height);
                        else
                            enemy.Position = new Vector2(enemy.Position.X, obstacle.Bounds.Bottom);

                        enemy.Velocity = new Vector2(enemy.Velocity.X, -enemy.Velocity.Y);
                    }
                }
            }

            // Update projectiles and destroy those that hit obstacles
            foreach (var proj in Projectiles)
            {
                proj.Update(gameTime, viewport);

                if (!proj.IsActive) continue;
                foreach (var obstacle in Obstacles)
                {
                    if (proj.Bounds.Intersects(obstacle.Bounds))
                    {
                        proj.IsActive = false;
                        break;
                    }
                }
            }

            Projectiles.RemoveAll(p => !p.IsActive);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var obstacle in Obstacles)
                obstacle.Draw(spriteBatch);

            foreach (var enemy in Enemies)
                enemy.Draw(spriteBatch);

            foreach (var proj in Projectiles)
                proj.Draw(spriteBatch);

            Goal?.Draw(spriteBatch);
        }

        public bool CheckCollisions(Player player)
        {
            // Obstacles — push player out along shallowest axis
            foreach (var obstacle in Obstacles)
            {
                if (player.Bounds.Intersects(obstacle.Bounds))
                {
                    Rectangle intersection = Rectangle.Intersect(player.Bounds, obstacle.Bounds);

                    if (intersection.Width < intersection.Height)
                    {
                        if (player.Bounds.Center.X < obstacle.Bounds.Center.X)
                            player.Position = new Vector2(player.Position.X - intersection.Width, player.Position.Y);
                        else
                            player.Position = new Vector2(player.Position.X + intersection.Width, player.Position.Y);
                    }
                    else
                    {
                        if (player.Bounds.Center.Y < obstacle.Bounds.Center.Y)
                            player.Position = new Vector2(player.Position.X, player.Position.Y - intersection.Height);
                        else
                            player.Position = new Vector2(player.Position.X, player.Position.Y + intersection.Height);
                    }
                }
            }

            // Enemies — 10 damage per hit with invincibility window
            foreach (var enemy in Enemies)
            {
                if (player.Bounds.Intersects(enemy.Bounds) && !player.IsInvincible)
                {
                    player.Health -= 10;
                    player.InvincibilityTime = 1.0f;

                    var direction = player.Position - enemy.Position;
                    if (direction != Vector2.Zero)
                    {
                        direction.Normalize();
                        player.Position += direction * 30f;
                    }
                }
            }

            // Projectiles — same damage/knockback, destroys projectile on hit
            foreach (var proj in Projectiles)
            {
                if (!proj.IsActive) continue;
                if (player.Bounds.Intersects(proj.Bounds) && !player.IsInvincible)
                {
                    player.Health -= 10;
                    player.InvincibilityTime = 1.0f;
                    proj.IsActive = false;

                    var direction = player.Position - proj.Position;
                    if (direction != Vector2.Zero)
                    {
                        direction.Normalize();
                        player.Position += direction * 30f;
                    }
                }
            }

            // Goal
            if (Goal != null && player.Bounds.Intersects(Goal.Bounds))
                return true;

            return false;
        }
    }
}
