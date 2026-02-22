using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameBasic.Entities;

namespace MonoGameBasic
{
    public class Level
    {
        public List<Obstacle> Obstacles { get; private set; }
        public List<Enemy> Enemies { get; private set; }
        public Goal Goal { get; private set; }

        private GraphicsDevice _graphicsDevice;
        private int _levelIndex;
        private Random _random;
        private float _enemySpeedMultiplier;

        public Level(GraphicsDevice graphicsDevice, int levelIndex, float enemySpeedMultiplier = 1.0f)
        {
            _graphicsDevice = graphicsDevice;
            _levelIndex = levelIndex;
            _random = new Random(_levelIndex * 100); // Seed for reproducibility
            _enemySpeedMultiplier = enemySpeedMultiplier;

            Obstacles = new List<Obstacle>();
            Enemies = new List<Enemy>();

            GenerateLevel();
        }

        private void GenerateLevel()
        {
            int obstacleCount = 3 + _levelIndex;
            int enemyCount = 1 + _levelIndex;

            Rectangle safeZone = new Rectangle(0, 0, 150, 150);

            // Generate Obstacles
            int obstaclesPlaced = 0;
            while (obstaclesPlaced < obstacleCount)
            {
                int x = _random.Next(100, 700);
                int y = _random.Next(50, 400);
                int w = _random.Next(50, 150);
                int h = _random.Next(50, 150);

                var obstacleRect = new Rectangle(x, y, w, h);
                if (!obstacleRect.Intersects(safeZone))
                {
                    Obstacles.Add(new Obstacle(_graphicsDevice, new Vector2(x, y), w, h));
                    obstaclesPlaced++;
                }
            }

            // Generate Enemies
            for (int i = 0; i < enemyCount; i++)
            {
                int x, y;
                do
                {
                    x = _random.Next(150, 750);
                    y = _random.Next(50, 430);
                } while (safeZone.Contains(x, y));

                Enemies.Add(new Enemy(_graphicsDevice, new Vector2(x, y), _enemySpeedMultiplier));
            }

            // Goal — placed randomly in the right half
            int goalX = _random.Next(600, 750);
            int goalY = _random.Next(100, 400);
            Goal = new Goal(_graphicsDevice, new Vector2(goalX, goalY));
        }

        public void Update(GameTime gameTime, Viewport viewport)
        {
            foreach (var enemy in Enemies)
            {
                enemy.Update(gameTime, viewport);

                // Resolve enemy vs obstacle collisions using previous position for accurate axis detection
                foreach (var obstacle in Obstacles)
                {
                    if (!enemy.Bounds.Intersects(obstacle.Bounds))
                        continue;

                    Rectangle intersection = Rectangle.Intersect(enemy.Bounds, obstacle.Bounds);

                    // Determine which axis was entered by comparing previous position to the obstacle edges.
                    // This is more reliable than comparing centers, especially at corners.
                    bool wasLeftOf  = enemy.PreviousPosition.X + enemy.Width <= obstacle.Bounds.Left;
                    bool wasRightOf = enemy.PreviousPosition.X >= obstacle.Bounds.Right;
                    bool wasAbove   = enemy.PreviousPosition.Y + enemy.Height <= obstacle.Bounds.Top;
                    bool wasBelow   = enemy.PreviousPosition.Y >= obstacle.Bounds.Bottom;

                    bool horizontalApproach = wasLeftOf || wasRightOf;
                    bool verticalApproach   = wasAbove  || wasBelow;

                    // Prefer whichever axis the previous position clearly indicates; fall back to
                    // smallest-intersection heuristic when the enemy spawned inside the obstacle.
                    if (horizontalApproach || (!verticalApproach && intersection.Width < intersection.Height))
                    {
                        // Snap to the correct horizontal edge
                        if (enemy.Bounds.Center.X < obstacle.Bounds.Center.X)
                            enemy.Position = new Vector2(obstacle.Bounds.Left - enemy.Width, enemy.Position.Y);
                        else
                            enemy.Position = new Vector2(obstacle.Bounds.Right, enemy.Position.Y);

                        enemy.Velocity = new Vector2(-enemy.Velocity.X, enemy.Velocity.Y);
                    }
                    else
                    {
                        // Snap to the correct vertical edge
                        if (enemy.Bounds.Center.Y < obstacle.Bounds.Center.Y)
                            enemy.Position = new Vector2(enemy.Position.X, obstacle.Bounds.Top - enemy.Height);
                        else
                            enemy.Position = new Vector2(enemy.Position.X, obstacle.Bounds.Bottom);

                        enemy.Velocity = new Vector2(enemy.Velocity.X, -enemy.Velocity.Y);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var obstacle in Obstacles)
                obstacle.Draw(spriteBatch);

            foreach (var enemy in Enemies)
                enemy.Draw(spriteBatch);

            Goal?.Draw(spriteBatch);
        }

        public bool CheckCollisions(Player player)
        {
            // Obstacles — push player out along the shallowest axis
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

            // Enemies — deal 10 damage per hit, then grant 1 second of invincibility
            foreach (var enemy in Enemies)
            {
                if (player.Bounds.Intersects(enemy.Bounds) && !player.IsInvincible)
                {
                    player.Health -= 10;
                    player.InvincibilityTime = 1.0f;

                    // Knock the player away from the enemy
                    var direction = player.Position - enemy.Position;
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
