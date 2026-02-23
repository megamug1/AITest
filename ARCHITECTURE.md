# Architecture Diagram

## Component Relationships

```mermaid
classDiagram
    direction TB

    class Game1 {
        -GameState _currentState
        -Player _player
        -Level _level
        -int _currentLevelIndex
        -float[] SpeedMultipliers
        +Update(GameTime)
        +Draw(GameTime)
        -DrawHUD()
    }

    class Level {
        +List~EnemyBase~ Enemies
        +List~Obstacle~ Obstacles
        +List~Projectile~ Projectiles
        +Goal Goal
        -BuildFromData()
        +Update(GameTime, Viewport, Vector2 playerPos)
        +CheckCollisions(Player) bool
        +Draw(SpriteBatch)
    }

    class LevelData {
        +LevelDefinition[] Levels$
    }
    class LevelDefinition {
        +ObstacleDefinition[] Obstacles
        +EnemyDefinition[] Enemies
        +float GoalX, GoalY
    }
    class EnemyDefinition {
        +EnemyType Type
        +float X, Y
        +float SpeedOrInterval
        +int Size
    }

    class EnemyBase {
        <<abstract>>
        +Vector2 Position
        +Vector2 PreviousPosition
        +Vector2 Velocity
        +Rectangle Bounds
        +Update(GameTime, Viewport, Vector2 playerPos)*
        +GetNewProjectiles() List~Projectile~
        +Draw(SpriteBatch)
        #BounceOffViewport(Viewport)
        #CreateBorderedTexture(...)$
    }

    class BouncerEnemy {
        Bounces off walls and obstacles
        Random direction, configurable size
        Speed: 150f × multiplier
    }
    class TrackerEnemy {
        Steers toward player each frame
        Slower but persistent
        Speed: 80f × multiplier
    }
    class ShooterEnemy {
        Stationary, fires aimed projectiles
        Configurable shoot interval
        -float _shootTimer
        -FireProjectile(Vector2 playerPos)
        +GetNewProjectiles() List~Projectile~
    }

    class Player {
        +Vector2 Position
        +float Speed
        +int Health
        +float InvincibilityTime
        +bool IsInvincible
        +Update(GameTime, Viewport)
        +Draw(SpriteBatch)
    }

    class Projectile {
        +Vector2 Position
        +Vector2 Velocity
        +bool IsActive
        +Update(GameTime, Viewport)
        +Draw(SpriteBatch)
    }

    class Obstacle {
        +Vector2 Position
        +int Width, Height
        +Rectangle Bounds
        +Draw(SpriteBatch)
    }

    class Goal {
        +Vector2 Position
        +Rectangle Bounds
        +Draw(SpriteBatch)
    }

    Game1 *-- "1" Player : owns
    Game1 *-- "1" Level : owns
    Level *-- "many" EnemyBase : owns
    Level *-- "many" Obstacle : owns
    Level *-- "many" Projectile : owns
    Level *-- "1" Goal : owns
    Level ..> LevelData : reads definitions
    LevelData *-- "10" LevelDefinition
    LevelDefinition *-- "many" EnemyDefinition
    LevelDefinition *-- "many" ObstacleDefinition

    EnemyBase <|-- BouncerEnemy
    EnemyBase <|-- TrackerEnemy
    EnemyBase <|-- ShooterEnemy
    ShooterEnemy ..> Projectile : creates
```

## Collision Map

```mermaid
flowchart LR
    subgraph "Level.Update()"
        E[EnemyBase] -- "bounce off" --> O[Obstacle]
        P2[Projectile] -- "destroyed by" --> O
    end

    subgraph "Level.CheckCollisions()"
        PL[Player] -- "pushed out" --> O
        PL -- "10 dmg + knockback<br/>1s invincibility" --> E
        PL -- "10 dmg + knockback<br/>destroy projectile" --> P2
        PL -- "level complete" --> G[Goal]
    end
```

## Game State Flow

```mermaid
stateDiagram-v2
    [*] --> Menu
    Menu --> Playing : Enter
    Menu --> Settings : O
    Menu --> [*] : Escape

    Settings --> Menu : Escape

    Playing --> Menu : Escape
    Playing --> Playing : Goal reached<br/>(next level)
    Playing --> Victory : Beat level 10
    Playing --> GameOver : Health ≤ 0

    GameOver --> Menu : Enter
    Victory --> Menu : Enter
```

## Level Progression

```mermaid
flowchart LR
    subgraph "Levels 1-3"
        L1["1: Tutorial<br/>1 bouncer"]
        L2["2: Corridor<br/>2 bouncers"]
        L3["3: Maze<br/>3 bouncers<br/>mixed sizes"]
    end

    subgraph "Levels 4-6"
        L4["4: Tracker Intro<br/>2 bouncers<br/>1 tracker"]
        L5["5: Double Trouble<br/>2 bouncers<br/>2 trackers"]
        L6["6: Gauntlet<br/>3 fast bouncers<br/>1 tracker"]
    end

    subgraph "Levels 7-10"
        L7["7: Shooter Intro<br/>2 bouncers, 1 tracker<br/>1 shooter"]
        L8["8: Crossfire<br/>1 bouncer, 2 trackers<br/>2 shooters"]
        L9["9: Fortress<br/>2 bouncers, 2 trackers<br/>2 shooters"]
        L10["10: Final<br/>All types<br/>Goal in center"]
    end

    L1 --> L2 --> L3 --> L4 --> L5 --> L6 --> L7 --> L8 --> L9 --> L10
```

## Conceptual Game Model

How the game works from a player's perspective, and how the systems interact each frame.

```mermaid
flowchart TB
    subgraph INPUT["Player Input"]
        KB["Keyboard<br/>WASD / Arrows"]
    end

    subgraph GAMELOOP["Game Loop — each frame"]
        direction TB

        subgraph UPDATE["Update Phase"]
            direction LR
            PM["Player Movement<br/>Speed × deltaTime<br/>Clamped to viewport"]
            EM["Enemy Behavior"]
            PU["Projectile Movement<br/>Straight line, destroy<br/>if off-screen"]
        end

        subgraph ENEMIES["Enemy AI"]
            direction LR
            B["🔴 Bouncer<br/>Random direction<br/>Reflects off surfaces"]
            T["🟣 Tracker<br/>Steers toward player<br/>every frame"]
            S["🟠 Shooter<br/>Aims at player<br/>Fires on timer"]
        end

        subgraph COLLISION["Collision Resolution"]
            direction TB
            C1["Player ↔ Obstacle<br/>Push player out"]
            C2["Player ↔ Enemy<br/>10 damage + knockback<br/>1s invincibility"]
            C3["Player ↔ Projectile<br/>10 damage + knockback<br/>Destroy projectile"]
            C4["Player ↔ Goal<br/>Level complete!"]
            C5["Enemy ↔ Obstacle<br/>Snap + reflect velocity"]
            C6["Projectile ↔ Obstacle<br/>Destroy projectile"]
        end
    end

    subgraph OUTCOME["Outcomes"]
        direction LR
        NEXT["Next Level<br/>Reset position<br/>Keep health"]
        OVER["Game Over<br/>Health ≤ 0"]
        WIN["Victory<br/>Beat level 10"]
    end

    KB --> PM
    PM --> COLLISION
    EM --> COLLISION
    PU --> COLLISION
    ENEMIES --> EM
    S -.-> PU

    C4 --> NEXT
    C4 --> WIN
    C2 --> OVER
    C3 --> OVER
```

```mermaid
flowchart LR
    subgraph FRAME["Single Frame Lifecycle"]
        direction LR
        I["Read<br/>Input"] --> U["Update<br/>Positions"] --> C["Resolve<br/>Collisions"] --> R["Check<br/>Results"] --> D["Draw<br/>Everything"]
    end

    subgraph ENTITIES["What Gets Drawn"]
        direction TB
        E1["🔴 Enemies — bordered rectangles"]
        E2["⬜ Player — white/blue 32×32<br/>Flashes when invincible"]
        E3["⬛ Obstacles — gray walls"]
        E4["🟡 Projectiles — 8×8 yellow dots"]
        E5["🟢 Goal — 48×48 gold-bordered green"]
        E6["❤️ HUD — health bar + level counter"]
    end

    D --> ENTITIES
```

```mermaid
flowchart TB
    subgraph DAMAGE["Damage & Survival Model"]
        direction TB
        HP["Player HP: 100"]
        HIT["Hit by enemy<br/>or projectile"]
        DMG["-10 HP"]
        INV["1 second invincibility<br/>Player flashes"]
        KB2["Knocked back 30 units<br/>away from threat"]
        SAFE["Invincible — no damage<br/>until timer expires"]

        HP --> HIT --> DMG --> INV --> KB2 --> SAFE
        SAFE -.->|"timer expires"| HIT
    end
```
