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

        public Level(GraphicsDevice graphicsDevice, int levelIndex)
        {
            _graphicsDevice = graphicsDevice;
            _levelIndex = levelIndex;
            _random = new Random(_levelIndex * 100); // Seed for reproducibility

            Obstacles = new List<Obstacle>();
            Enemies = new List<Enemy>();

            GenerateLevel();
        }

        private void GenerateLevel()
        {
            // Base difficulty parameters
            int obstacleCount = 3 + _levelIndex; // Increases with level
            int enemyCount = 1 + _levelIndex;    // Increases with level

            // Prevent spawning on player start area (0,0 to 100,100)
            Rectangle safeZone = new Rectangle(0, 0, 150, 150);

            // Generate Obstacles
            int obstaclesPlaced = 0;
            while (obstaclesPlaced < obstacleCount)
            {
                // Random position within bounds (approx 800x480)
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
                } while (safeZone.Contains(x, y)); // Simple check

                Enemies.Add(new Enemy(_graphicsDevice, new Vector2(x, y)));
            }

            // Goal - placed far away or fixed bottom right
            // For variety, let's place it randomly in the right half
            int goalX = _random.Next(600, 750);
            int goalY = _random.Next(100, 400);
            Goal = new Goal(_graphicsDevice, new Vector2(goalX, goalY));
        }

        public void Update(GameTime gameTime, Viewport viewport)
        {
            foreach (var enemy in Enemies)
            {
                enemy.Update(gameTime, viewport);

                // Enemy vs Obstacle Collision
                foreach (var obstacle in Obstacles)
                {
                    if (enemy.Bounds.Intersects(obstacle.Bounds))
                    {
                         Rectangle intersection = Rectangle.Intersect(enemy.Bounds, obstacle.Bounds);

                         // Determine collision side and reflect velocity
                         if (intersection.Width < intersection.Height)
                         {
                             // Horizontal Collision
                             if (enemy.Bounds.Center.X < obstacle.Bounds.Center.X)
                                 enemy.Position = new Vector2(enemy.Position.X - intersection.Width, enemy.Position.Y);
                             else
                                 enemy.Position = new Vector2(enemy.Position.X + intersection.Width, enemy.Position.Y);

                             enemy.Velocity = new Vector2(enemy.Velocity.X * -1, enemy.Velocity.Y);
                         }
                         else
                         {
                             // Vertical Collision
                             if (enemy.Bounds.Center.Y < obstacle.Bounds.Center.Y)
                                 enemy.Position = new Vector2(enemy.Position.X, enemy.Position.Y - intersection.Height);
                             else
                                 enemy.Position = new Vector2(enemy.Position.X, enemy.Position.Y + intersection.Height);

                             enemy.Velocity = new Vector2(enemy.Velocity.X, enemy.Velocity.Y * -1);
                         }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var obstacle in Obstacles)
            {
                obstacle.Draw(spriteBatch);
            }

            foreach (var enemy in Enemies)
            {
                enemy.Draw(spriteBatch);
            }

            Goal?.Draw(spriteBatch);
        }

        public bool CheckCollisions(Player player)
        {
            // Check Obstacles (Walls) - Prevent movement
            foreach (var obstacle in Obstacles)
            {
                if (player.Bounds.Intersects(obstacle.Bounds))
                {
                    Rectangle intersection = Rectangle.Intersect(player.Bounds, obstacle.Bounds);

                    // Resolve collision by pushing out the player along the shallowest axis
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

            // Check Enemies - Damage
            foreach (var enemy in Enemies)
            {
                if (player.Bounds.Intersects(enemy.Bounds))
                {
                     player.Health -= 1;

                     // Pushback
                     var direction = player.Position - enemy.Position;
                     if (direction != Vector2.Zero)
                     {
                         direction.Normalize();
                         player.Position += direction * 10f;
                     }
                }
            }

            // Check Goal
            if (Goal != null && player.Bounds.Intersects(Goal.Bounds))
            {
                return true; // Level Complete
            }

            return false;
        }
    }
}
