namespace MonoGameBasic
{
    public enum EnemyType { Bouncer, Tracker, Shooter }

    public struct EnemyDefinition
    {
        public EnemyType Type;
        public float X;
        public float Y;
        public float SpeedOrInterval; // Bouncer/Tracker: speed factor. Shooter: shoot interval (seconds).
        public int Size;

        public EnemyDefinition(EnemyType type, float x, float y,
                               float speedOrInterval = 1.0f, int size = 32)
        {
            Type = type; X = x; Y = y;
            SpeedOrInterval = speedOrInterval; Size = size;
        }
    }

    public struct ObstacleDefinition
    {
        public float X, Y;
        public int Width, Height;

        public ObstacleDefinition(float x, float y, int w, int h)
        { X = x; Y = y; Width = w; Height = h; }
    }

    public struct LevelDefinition
    {
        public ObstacleDefinition[] Obstacles;
        public EnemyDefinition[] Enemies;
        public float GoalX, GoalY;
    }

    public static class LevelData
    {
        public static readonly LevelDefinition[] Levels = new LevelDefinition[]
        {
            // ─── Level 1: Tutorial ───────────────────────────────────────
            // Open field with one central wall. One slow bouncer. Teaches basics.
            new LevelDefinition
            {
                Obstacles = new[]
                {
                    new ObstacleDefinition(350, 150, 100, 180),
                },
                Enemies = new[]
                {
                    new EnemyDefinition(EnemyType.Bouncer, 500, 240, 0.7f, 32),
                },
                GoalX = 720, GoalY = 220
            },

            // ─── Level 2: Corridor ───────────────────────────────────────
            // Horizontal walls form a wide corridor. Two bouncers patrol inside.
            new LevelDefinition
            {
                Obstacles = new[]
                {
                    new ObstacleDefinition(200, 100, 400, 30),
                    new ObstacleDefinition(200, 350, 400, 30),
                },
                Enemies = new[]
                {
                    new EnemyDefinition(EnemyType.Bouncer, 350, 200, 0.8f, 32),
                    new EnemyDefinition(EnemyType.Bouncer, 450, 280, 0.9f, 32),
                },
                GoalX = 700, GoalY = 220
            },

            // ─── Level 3: Maze Start ─────────────────────────────────────
            // L-shaped walls. Three bouncers of varying sizes.
            new LevelDefinition
            {
                Obstacles = new[]
                {
                    new ObstacleDefinition(180, 50, 30, 250),
                    new ObstacleDefinition(180, 270, 250, 30),
                    new ObstacleDefinition(500, 180, 30, 250),
                },
                Enemies = new[]
                {
                    new EnemyDefinition(EnemyType.Bouncer, 300, 150, 1.0f, 24),
                    new EnemyDefinition(EnemyType.Bouncer, 400, 350, 0.8f, 40),
                    new EnemyDefinition(EnemyType.Bouncer, 600, 100, 1.1f, 32),
                },
                GoalX = 700, GoalY = 400
            },

            // ─── Level 4: Tracker Introduction ───────────────────────────
            // Cover obstacles scattered around. First tracker appears.
            new LevelDefinition
            {
                Obstacles = new[]
                {
                    new ObstacleDefinition(250, 150, 80, 80),
                    new ObstacleDefinition(500, 280, 80, 80),
                    new ObstacleDefinition(350, 50, 80, 60),
                },
                Enemies = new[]
                {
                    new EnemyDefinition(EnemyType.Bouncer, 400, 200, 1.0f, 32),
                    new EnemyDefinition(EnemyType.Bouncer, 600, 350, 1.0f, 32),
                    new EnemyDefinition(EnemyType.Tracker, 650, 100, 0.8f, 28),
                },
                GoalX = 720, GoalY = 420
            },

            // ─── Level 5: Double Trouble ─────────────────────────────────
            // Vertical walls create chokepoints. Two trackers flank the path.
            new LevelDefinition
            {
                Obstacles = new[]
                {
                    new ObstacleDefinition(300, 0, 30, 200),
                    new ObstacleDefinition(300, 280, 30, 200),
                    new ObstacleDefinition(550, 100, 30, 280),
                },
                Enemies = new[]
                {
                    new EnemyDefinition(EnemyType.Bouncer, 150, 250, 1.0f, 32),
                    new EnemyDefinition(EnemyType.Bouncer, 420, 150, 1.1f, 24),
                    new EnemyDefinition(EnemyType.Tracker, 650, 200, 0.9f, 28),
                    new EnemyDefinition(EnemyType.Tracker, 650, 350, 0.7f, 28),
                },
                GoalX = 700, GoalY = 240
            },

            // ─── Level 6: Gauntlet ───────────────────────────────────────
            // Narrow vertical passages with fast bouncers and a tracker.
            new LevelDefinition
            {
                Obstacles = new[]
                {
                    new ObstacleDefinition(150, 80, 30, 320),
                    new ObstacleDefinition(350, 80, 30, 320),
                    new ObstacleDefinition(550, 80, 30, 320),
                    new ObstacleDefinition(150, 80, 430, 30),
                    new ObstacleDefinition(150, 370, 430, 30),
                },
                Enemies = new[]
                {
                    new EnemyDefinition(EnemyType.Bouncer, 240, 200, 1.2f, 24),
                    new EnemyDefinition(EnemyType.Bouncer, 440, 300, 1.3f, 24),
                    new EnemyDefinition(EnemyType.Bouncer, 440, 150, 1.0f, 32),
                    new EnemyDefinition(EnemyType.Tracker, 650, 240, 1.0f, 28),
                },
                GoalX = 700, GoalY = 240
            },

            // ─── Level 7: Shooter Introduction ───────────────────────────
            // Cover obstacles become essential. First shooter in top-right corner.
            new LevelDefinition
            {
                Obstacles = new[]
                {
                    new ObstacleDefinition(200, 120, 100, 80),
                    new ObstacleDefinition(450, 280, 100, 80),
                    new ObstacleDefinition(350, 200, 60, 60),
                },
                Enemies = new[]
                {
                    new EnemyDefinition(EnemyType.Bouncer, 300, 400, 1.0f, 32),
                    new EnemyDefinition(EnemyType.Bouncer, 500, 100, 1.1f, 32),
                    new EnemyDefinition(EnemyType.Tracker, 600, 350, 0.9f, 28),
                    new EnemyDefinition(EnemyType.Shooter, 700, 50, 2.5f, 36),
                },
                GoalX = 720, GoalY = 420
            },

            // ─── Level 8: Crossfire ──────────────────────────────────────
            // Two shooters in corners. Heavy obstacle cover is your friend.
            new LevelDefinition
            {
                Obstacles = new[]
                {
                    new ObstacleDefinition(200, 100, 120, 40),
                    new ObstacleDefinition(200, 340, 120, 40),
                    new ObstacleDefinition(450, 200, 100, 80),
                    new ObstacleDefinition(350, 50, 40, 100),
                    new ObstacleDefinition(350, 340, 40, 100),
                },
                Enemies = new[]
                {
                    new EnemyDefinition(EnemyType.Bouncer, 250, 220, 1.2f, 24),
                    new EnemyDefinition(EnemyType.Tracker, 500, 100, 1.0f, 28),
                    new EnemyDefinition(EnemyType.Tracker, 500, 380, 0.8f, 28),
                    new EnemyDefinition(EnemyType.Shooter, 700, 50, 2.0f, 36),
                    new EnemyDefinition(EnemyType.Shooter, 700, 400, 2.0f, 36),
                },
                GoalX = 720, GoalY = 230
            },

            // ─── Level 9: The Fortress ───────────────────────────────────
            // Dense vertical walls. Every enemy type present. Fast shooter.
            new LevelDefinition
            {
                Obstacles = new[]
                {
                    new ObstacleDefinition(150, 60, 30, 200),
                    new ObstacleDefinition(150, 340, 30, 120),
                    new ObstacleDefinition(300, 120, 30, 240),
                    new ObstacleDefinition(450, 60, 30, 200),
                    new ObstacleDefinition(450, 340, 30, 120),
                    new ObstacleDefinition(600, 160, 30, 200),
                },
                Enemies = new[]
                {
                    new EnemyDefinition(EnemyType.Bouncer, 220, 180, 1.2f, 24),
                    new EnemyDefinition(EnemyType.Bouncer, 380, 350, 1.3f, 24),
                    new EnemyDefinition(EnemyType.Tracker, 520, 100, 1.0f, 28),
                    new EnemyDefinition(EnemyType.Tracker, 520, 400, 1.1f, 28),
                    new EnemyDefinition(EnemyType.Shooter, 700, 60, 1.8f, 36),
                    new EnemyDefinition(EnemyType.Shooter, 700, 400, 2.2f, 36),
                },
                GoalX = 740, GoalY = 240
            },

            // ─── Level 10: Final Challenge ───────────────────────────────
            // Open arena. Goal in the CENTER. Enemies surround you from all sides.
            new LevelDefinition
            {
                Obstacles = new[]
                {
                    new ObstacleDefinition(300, 180, 80, 80),
                    new ObstacleDefinition(500, 180, 80, 80),
                    new ObstacleDefinition(400, 60, 60, 60),
                    new ObstacleDefinition(400, 360, 60, 60),
                },
                Enemies = new[]
                {
                    new EnemyDefinition(EnemyType.Bouncer, 200, 100, 1.4f, 24),
                    new EnemyDefinition(EnemyType.Bouncer, 200, 380, 1.4f, 24),
                    new EnemyDefinition(EnemyType.Bouncer, 600, 240, 1.2f, 40),
                    new EnemyDefinition(EnemyType.Tracker, 700, 100, 1.2f, 28),
                    new EnemyDefinition(EnemyType.Tracker, 700, 380, 1.2f, 28),
                    new EnemyDefinition(EnemyType.Shooter, 750, 50, 1.5f, 36),
                    new EnemyDefinition(EnemyType.Shooter, 750, 420, 1.5f, 36),
                },
                GoalX = 400, GoalY = 220
            },
        };
    }
}
