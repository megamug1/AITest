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

        public Level(GraphicsDevice graphicsDevice)
        {
            Obstacles = new List<Obstacle>();
            Enemies = new List<Enemy>();

            // Create a simple level layout

            // Walls (Obstacles)
            // A vertical wall in the middle
            Obstacles.Add(new Obstacle(graphicsDevice, new Vector2(300, 100), 50, 300));
            // Some horizontal blocks
            Obstacles.Add(new Obstacle(graphicsDevice, new Vector2(100, 300), 150, 50));
            Obstacles.Add(new Obstacle(graphicsDevice, new Vector2(500, 200), 100, 50));

            // Enemies
            Enemies.Add(new Enemy(graphicsDevice, new Vector2(100, 100)));
            Enemies.Add(new Enemy(graphicsDevice, new Vector2(500, 100)));

            // Goal
            Goal = new Goal(graphicsDevice, new Vector2(700, 400));
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
