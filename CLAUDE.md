# Blob Game — MonoGame Project

## Build & Run
```bash
cd MonoGameBasic
dotnet build        # Build
dotnet run          # Run
```
- .NET 8.0 / MonoGame 3.8 (DesktopGL)
- No external assets — all textures are procedurally generated at runtime
- Single project solution: `AITest.sln`

## Architecture Overview

### Game Loop (`Game1.cs`)
State machine: `Menu → Playing → GameOver / Victory`, plus `Settings`.
- Creates `Player` and `Level`, runs update/draw loop
- Passes `playerCenter` to `Level.Update()` so enemies can aim
- HUD: color-coded health bar + HP text + level counter
- Settings: Player Speed and Enemy Speed multipliers (Slow 0.5x / Normal 1.0x / Fast 1.5x)

### Level System (`Level.cs` + `LevelData.cs`)
- **10 hand-crafted levels** defined as static data in `LevelData.cs`
- `LevelData.cs` contains `EnemyType` enum, `EnemyDefinition`/`ObstacleDefinition`/`LevelDefinition` structs
- `Level.BuildFromData()` reads from `LevelData.Levels[index]` — no procedural generation
- Level owns and updates all enemies, obstacles, projectiles, and the goal
- Collision handling is centralized in `Level.Update()` (enemy-obstacle) and `Level.CheckCollisions()` (player-obstacle, player-enemy, player-projectile, player-goal)

### Enemy Hierarchy (`Entities/EnemyBase.cs` + subclasses)
Abstract base `EnemyBase` with shared fields: `Position`, `PreviousPosition`, `Velocity`, `Texture`, `Bounds`.

| Class | Color | Behavior | Base Speed |
|-------|-------|----------|------------|
| `BouncerEnemy` | Red | Bounces off walls/obstacles, random direction | 150f |
| `TrackerEnemy` | Purple | Steers toward player center each frame | 80f |
| `ShooterEnemy` | Orange | Stationary, fires aimed `Projectile`s on a timer | 0 (projectiles: 200f) |

- `Update(GameTime, Viewport, Vector2 playerPosition)` — playerPosition used by Tracker and Shooter
- `GetNewProjectiles()` — virtual, only ShooterEnemy returns projectiles
- `CreateBorderedTexture()` — shared static helper for bordered rectangle textures

### Other Entities (`Entities/`)
- **Player.cs**: 32x32 white/blue, WASD+arrows, `Health` (100), `InvincibilityTime` (1s after hit, flashes at ~10Hz), `Speed` (200f base)
- **Obstacle.cs**: Variable-size gray bordered rectangle, static collision volume
- **Goal.cs**: 48x48 gold-bordered green square, collision = level complete
- **Projectile.cs**: 8x8 yellow/white, straight-line travel, `IsActive` flag, destroyed by obstacles/viewport

### Damage Model
- Contact with enemy or projectile: **10 damage** + **1 second invincibility** + **30 unit knockback**
- Player starts with 100 HP, preserved across levels
- Health ≤ 0 → GameOver

### Level Progression
- Levels 1-3: Bouncers only (varying sizes/speeds)
- Levels 4-6: Trackers introduced alongside bouncers
- Levels 7-9: Shooters added, cover becomes essential
- Level 10: Open arena, goal in the center, all enemy types
- Completing level 10 → Victory

### Collision System
- AABB (Rectangle intersection) throughout
- Enemy-obstacle: uses `PreviousPosition` to determine approach axis, snaps to obstacle edge, reverses velocity component
- Player-obstacle: pushes player out along shallowest intersection axis
- Projectile-obstacle: destroys projectile

### Key Conventions
- All entities follow: `Position`, `Bounds` (Rectangle), `Update()`, `Draw(SpriteBatch)`
- Viewport is ~800x480
- Edge-detection for menu input (compare current vs previous keyboard state)
- Speed multiplier from Settings applies to all enemy types via constructor parameters

## File Map
```
MonoGameBasic/
├── Program.cs              Entry point
├── Game1.cs                Game loop, state machine, HUD, settings
├── Level.cs                Level construction, update, collision handling
├── LevelData.cs            Static level definitions (10 levels) + enums/structs
└── Entities/
    ├── EnemyBase.cs        Abstract enemy base class
    ├── BouncerEnemy.cs     Bouncing enemy
    ├── TrackerEnemy.cs     Player-chasing enemy
    ├── ShooterEnemy.cs     Projectile-firing enemy
    ├── Projectile.cs       Shooter projectile
    ├── Player.cs           Player character
    ├── Obstacle.cs         Static wall/barrier
    └── Goal.cs             Level exit trigger
```
