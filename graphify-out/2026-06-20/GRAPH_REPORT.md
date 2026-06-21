# Graph Report - dungeon-knight-3D  (2026-06-20)

## Corpus Check
- 113 files · ~5,370,459 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 1537 nodes · 2717 edges · 110 communities (109 shown, 1 thin omitted)
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- [[_COMMUNITY_World 1-1 Bootstrap|World 1-1 Bootstrap]]
- [[_COMMUNITY_3D Scene Bootstrap|3D Scene Bootstrap]]
- [[_COMMUNITY_Character Sprite Factory|Character Sprite Factory]]
- [[_COMMUNITY_3D Player Animation|3D Player Animation]]
- [[_COMMUNITY_Player Combat & Inventory|Player Combat & Inventory]]
- [[_COMMUNITY_Animation Mixer Playables|Animation Mixer Playables]]
- [[_COMMUNITY_2D Game Session|2D Game Session]]
- [[_COMMUNITY_3D Player Controller|3D Player Controller]]
- [[_COMMUNITY_Pixel Sprite Factory|Pixel Sprite Factory]]
- [[_COMMUNITY_3D Dungeon Enemy|3D Dungeon Enemy]]
- [[_COMMUNITY_2D Game HUD|2D Game HUD]]
- [[_COMMUNITY_3D Player Visuals|3D Player Visuals]]
- [[_COMMUNITY_Character Frame Animation|Character Frame Animation]]
- [[_COMMUNITY_3D Interactables|3D Interactables]]
- [[_COMMUNITY_Ceiling Blade Hazard|Ceiling Blade Hazard]]
- [[_COMMUNITY_Skeleton Minion AI|Skeleton Minion AI]]
- [[_COMMUNITY_Treasure Chest|Treasure Chest]]
- [[_COMMUNITY_Timed Fire Trap|Timed Fire Trap]]
- [[_COMMUNITY_Health System|Health System]]
- [[_COMMUNITY_3D Wall Torch|3D Wall Torch]]
- [[_COMMUNITY_Gate Door Visual|Gate Door Visual]]
- [[_COMMUNITY_Chasing Bat AI|Chasing Bat AI]]
- [[_COMMUNITY_Skeleton Archer AI|Skeleton Archer AI]]
- [[_COMMUNITY_3D Skeleton Visuals|3D Skeleton Visuals]]
- [[_COMMUNITY_Tower Ambush Lever|Tower Ambush Lever]]
- [[_COMMUNITY_Player Inventory|Player Inventory]]
- [[_COMMUNITY_Key Guardian Boss|Key Guardian Boss]]
- [[_COMMUNITY_Breakable Platform|Breakable Platform]]
- [[_COMMUNITY_2D Camera Follow|2D Camera Follow]]
- [[_COMMUNITY_Drop-Through Surfaces|Drop-Through Surfaces]]
- [[_COMMUNITY_Coin Pickup|Coin Pickup]]
- [[_COMMUNITY_Player Interaction|Player Interaction]]
- [[_COMMUNITY_Skeleton Death Effect|Skeleton Death Effect]]
- [[_COMMUNITY_Breakable Crate|Breakable Crate]]
- [[_COMMUNITY_3D Soul Remnant|3D Soul Remnant]]
- [[_COMMUNITY_Key Pickup|Key Pickup]]
- [[_COMMUNITY_Key Visual|Key Visual]]
- [[_COMMUNITY_Retro Audio|Retro Audio]]
- [[_COMMUNITY_Key Guardian Visual|Key Guardian Visual]]
- [[_COMMUNITY_Bonfire|Bonfire]]
- [[_COMMUNITY_Arena Encounter|Arena Encounter]]
- [[_COMMUNITY_3D Combat Feedback|3D Combat Feedback]]
- [[_COMMUNITY_3D Chest Visual|3D Chest Visual]]
- [[_COMMUNITY_3D Dungeon Hazard|3D Dungeon Hazard]]
- [[_COMMUNITY_3D Billboard & Key Drop|3D Billboard & Key Drop]]
- [[_COMMUNITY_Interaction Feedback UI|Interaction Feedback UI]]
- [[_COMMUNITY_Exit Door|Exit Door]]
- [[_COMMUNITY_Simple Gate|Simple Gate]]
- [[_COMMUNITY_3D Camera Follow|3D Camera Follow]]
- [[_COMMUNITY_3D Mini-Boss Arena|3D Mini-Boss Arena]]
- [[_COMMUNITY_3D Pickups|3D Pickups]]
- [[_COMMUNITY_3D Shortcut Elevator|3D Shortcut Elevator]]
- [[_COMMUNITY_3D Game HUD|3D Game HUD]]
- [[_COMMUNITY_Hit Burst Effect|Hit Burst Effect]]
- [[_COMMUNITY_Combat Feedback (Hitstop)|Combat Feedback (Hitstop)]]
- [[_COMMUNITY_Arrow Projectile|Arrow Projectile]]
- [[_COMMUNITY_Gate Lever|Gate Lever]]
- [[_COMMUNITY_Slash Effect|Slash Effect]]
- [[_COMMUNITY_Drift Sprite|Drift Sprite]]
- [[_COMMUNITY_Flicker Sprite|Flicker Sprite]]
- [[_COMMUNITY_Loot Spawner|Loot Spawner]]
- [[_COMMUNITY_Damage Popup|Damage Popup]]
- [[_COMMUNITY_Squash & Stretch Anim|Squash & Stretch Anim]]
- [[_COMMUNITY_Death Shard|Death Shard]]
- [[_COMMUNITY_Damage Hazard|Damage Hazard]]
- [[_COMMUNITY_3D Moving Platform|3D Moving Platform]]
- [[_COMMUNITY_Lightning Flash|Lightning Flash]]
- [[_COMMUNITY_Coin Dropper|Coin Dropper]]
- [[_COMMUNITY_Shield Dropper|Shield Dropper]]
- [[_COMMUNITY_Lore Tablet|Lore Tablet]]
- [[_COMMUNITY_Enemy Health Bar|Enemy Health Bar]]
- [[_COMMUNITY_Game Constants|Game Constants]]
- [[_COMMUNITY_Shield Pickup|Shield Pickup]]
- [[_COMMUNITY_Interactable Interface|Interactable Interface]]
- [[_COMMUNITY_Community 74|Community 74]]
- [[_COMMUNITY_Community 75|Community 75]]
- [[_COMMUNITY_Community 76|Community 76]]
- [[_COMMUNITY_Community 77|Community 77]]
- [[_COMMUNITY_Community 78|Community 78]]
- [[_COMMUNITY_Community 79|Community 79]]
- [[_COMMUNITY_Community 80|Community 80]]
- [[_COMMUNITY_Community 81|Community 81]]
- [[_COMMUNITY_Community 82|Community 82]]
- [[_COMMUNITY_Community 83|Community 83]]
- [[_COMMUNITY_Community 84|Community 84]]
- [[_COMMUNITY_Community 85|Community 85]]
- [[_COMMUNITY_Community 86|Community 86]]
- [[_COMMUNITY_Community 88|Community 88]]
- [[_COMMUNITY_Community 89|Community 89]]
- [[_COMMUNITY_Community 90|Community 90]]

## God Nodes (most connected - your core abstractions)
1. `WorldOneOneBootstrap` - 82 edges
2. `DungeonKnight3DBootstrap` - 68 edges
3. `Vector2` - 59 edges
4. `CharacterSpriteFactory` - 45 edges
5. `PlayerModelVisual3D` - 44 edges
6. `PlayerController2D` - 43 edges
7. `RiggedSkeletonEnemyVisual3D` - 39 edges
8. `Sprite` - 35 edges
9. `PlayerController3D` - 35 edges
10. `PixelSpriteFactory` - 30 edges

## Surprising Connections (you probably didn't know these)
- `Health` --inherits--> `MonoBehaviour`  [EXTRACTED]
  Assets/Scripts/Combat/Health.cs →   _Bridges community 18 → community 44_
- `CombatFeedback` --inherits--> `MonoBehaviour`  [EXTRACTED]
  Assets/Scripts/Core/CombatFeedback.cs →   _Bridges community 44 → community 54_
- `RetroAudio` --inherits--> `MonoBehaviour`  [EXTRACTED]
  Assets/Scripts/Core/RetroAudio.cs →   _Bridges community 44 → community 37_
- `ArrowProjectile` --inherits--> `MonoBehaviour`  [EXTRACTED]
  Assets/Scripts/Enemies/ArrowProjectile.cs →   _Bridges community 44 → community 55_
- `ChasingBatAI` --inherits--> `MonoBehaviour`  [EXTRACTED]
  Assets/Scripts/Enemies/ChasingBatAI.cs →   _Bridges community 44 → community 21_

## Import Cycles
- None detected.

## Communities (110 total, 1 thin omitted)

### Community 0 - "World 1-1 Bootstrap"
Cohesion: 0.10
Nodes (14): bool, Color, float, GameObject, SimpleGate, Sprite, SpriteRenderer, Transform (+6 more)

### Community 1 - "3D Scene Bootstrap"
Cohesion: 0.09
Nodes (16): bool, Bounds, Color, GameObject, Material, PlayerController3D, Quaternion, Renderer (+8 more)

### Community 2 - "Character Sprite Factory"
Cohesion: 0.13
Nodes (10): Action, Color, Dictionary, int, Sprite, string, Texture2D, Vector2 (+2 more)

### Community 3 - "3D Player Animation"
Cohesion: 0.09
Nodes (19): AnimationPlayableOutput, AnimationClip, AnimationClipPlayable, Animator, Color, float, int, Material (+11 more)

### Community 4 - "Player Combat & Inventory"
Cohesion: 0.07
Nodes (16): bool, Collider2D, float, Health, int, LayerMask, List, PlayerInventory (+8 more)

### Community 5 - "Animation Mixer Playables"
Cohesion: 0.10
Nodes (17): AnimationMixerPlayable, AnimationClip, AnimationClipPlayable, Animator, bool, Bounds, Color, float (+9 more)

### Community 6 - "2D Game Session"
Cohesion: 0.10
Nodes (14): bool, Color, float, GameObject, GUIStyle, Health, IEnumerator, int (+6 more)

### Community 7 - "3D Player Controller"
Cohesion: 0.11
Nodes (11): bool, CharacterController, Collider, DungeonEnemy3D, float, int, string, Transform (+3 more)

### Community 8 - "Pixel Sprite Factory"
Cohesion: 0.19
Nodes (6): Color, Dictionary, int, Sprite, DungeonKnight, PixelSpriteFactory

### Community 9 - "3D Dungeon Enemy"
Cohesion: 0.12
Nodes (13): bool, CharacterController, Color, float, int, PlayerController3D, Quaternion, Renderer (+5 more)

### Community 10 - "2D Game HUD"
Cohesion: 0.23
Nodes (10): bool, Color, GameObject, GUIStyle, Health, PlayerController2D, PlayerInventory, Rect (+2 more)

### Community 11 - "3D Player Visuals"
Cohesion: 0.20
Nodes (10): float, int, PlayerController3D, Sprite, SpriteRenderer, Transform, Vector3, DirectionBucket (+2 more)

### Community 12 - "Character Frame Animation"
Cohesion: 0.15
Nodes (12): float, int, PlayerController2D, Rigidbody2D, Sprite, SpriteRenderer, Transform, SkeletonArcherAI (+4 more)

### Community 13 - "3D Interactables"
Cohesion: 0.11
Nodes (11): bool, int, PlayerController3D, string, Transform, Vector3, DungeonChestVisual3D, DungeonShortcutElevator3D (+3 more)

### Community 14 - "Ceiling Blade Hazard"
Cohesion: 0.16
Nodes (11): Collider2D, Color, float, int, Sprite, SpriteRenderer, Transform, Vector2 (+3 more)

### Community 15 - "Skeleton Minion AI"
Cohesion: 0.12
Nodes (12): bool, Color, float, int, Quaternion, Rigidbody2D, SpriteRenderer, Transform (+4 more)

### Community 16 - "Treasure Chest"
Cohesion: 0.12
Nodes (11): bool, GameObject, IEnumerator, int, SimpleGate, Sprite, SpriteRenderer, Transform (+3 more)

### Community 17 - "Timed Fire Trap"
Cohesion: 0.17
Nodes (10): Collider2D, Color, float, int, Sprite, SpriteRenderer, Transform, Vector2 (+2 more)

### Community 18 - "Health System"
Cohesion: 0.13
Nodes (8): bool, Color, Coroutine, IEnumerator, int, SpriteRenderer, DungeonKnight.Combat, Health

### Community 19 - "3D Wall Torch"
Cohesion: 0.18
Nodes (9): Color, float, int, Light, Sprite, SpriteRenderer, Vector3, AnimatedWallTorch3D (+1 more)

### Community 20 - "Gate Door Visual"
Cohesion: 0.15
Nodes (9): bool, Color, float, Sprite, SpriteRenderer, Vector2, Vector3, DungeonKnight.Interactables (+1 more)

### Community 21 - "Chasing Bat AI"
Cohesion: 0.15
Nodes (9): float, int, Rigidbody2D, SpriteRenderer, Transform, Vector2, Vector3, ChasingBatAI (+1 more)

### Community 22 - "Skeleton Archer AI"
Cohesion: 0.16
Nodes (9): float, int, Rigidbody2D, Sprite, SpriteRenderer, Transform, Vector2, DungeonKnight.Enemies (+1 more)

### Community 23 - "3D Skeleton Visuals"
Cohesion: 0.21
Nodes (8): bool, float, Sprite, SpriteRenderer, Transform, Vector3, DungeonKnight.Level, SkeletonEnemyVisual3D

### Community 24 - "Tower Ambush Lever"
Cohesion: 0.20
Nodes (9): bool, GameObject, Health, List, SimpleGate, SpriteRenderer, Transform, DungeonKnight.Interactables (+1 more)

### Community 25 - "Player Inventory"
Cohesion: 0.16
Nodes (4): bool, DungeonKnight.Player, PlayerInventory, ShieldKind

### Community 26 - "Key Guardian Boss"
Cohesion: 0.19
Nodes (7): float, int, Sprite, SpriteRenderer, Transform, DungeonKnight.Enemies, KeyGuardianBoss

### Community 27 - "Breakable Platform"
Cohesion: 0.16
Nodes (9): bool, Collider2D, Collision2D, float, IEnumerator, SpriteRenderer, Vector3, BreakablePlatform (+1 more)

### Community 28 - "2D Camera Follow"
Cohesion: 0.18
Nodes (8): bool, float, Rigidbody2D, Transform, Vector2, Vector3, CameraFollow2D, DungeonKnight.Level

### Community 29 - "Drop-Through Surfaces"
Cohesion: 0.19
Nodes (6): bool, Collider2D, float, Rigidbody2D, DropThroughSurface, DungeonKnight.Level

### Community 30 - "Coin Pickup"
Cohesion: 0.16
Nodes (8): Collider2D, Collision2D, float, GameObject, int, Vector2, CoinPickup, DungeonKnight.Loot

### Community 31 - "Player Interaction"
Cohesion: 0.19
Nodes (8): Color, GUIStyle, IInteractable, LayerMask, Rect, Transform, DungeonKnight.Player, PlayerInteraction

### Community 32 - "Skeleton Death Effect"
Cohesion: 0.27
Nodes (6): Health, Sprite, Vector2, Vector3, DungeonKnight.Enemies, SkeletonDeathEffect

### Community 33 - "Breakable Crate"
Cohesion: 0.18
Nodes (7): bool, Health, int, Sprite, Vector3, BreakableCrate, DungeonKnight.Interactables

### Community 34 - "3D Soul Remnant"
Cohesion: 0.19
Nodes (7): Collider, float, int, Material, Vector3, DungeonKnight.Level, DungeonSoulRemnant3D

### Community 35 - "Key Pickup"
Cohesion: 0.18
Nodes (7): Collider2D, Collision2D, float, GameObject, Vector2, DungeonKnight.Loot, KeyPickup

### Community 36 - "Key Visual"
Cohesion: 0.21
Nodes (8): Color, float, Sprite, SpriteRenderer, Vector2, Vector3, DungeonKnight.Loot, KeyVisual

### Community 37 - "Retro Audio"
Cohesion: 0.27
Nodes (5): Dictionary, AudioClip, AudioSource, DungeonKnight, RetroAudio

### Community 38 - "Key Guardian Visual"
Cohesion: 0.21
Nodes (7): Color, float, Sprite, SpriteRenderer, Vector2, DungeonKnight.Enemies, KeyGuardianVisual

### Community 39 - "Bonfire"
Cohesion: 0.18
Nodes (7): bool, float, GameObject, Sprite, SpriteRenderer, Bonfire, DungeonKnight.Interactables

### Community 40 - "Arena Encounter"
Cohesion: 0.18
Nodes (7): bool, Collider2D, Health, List, SimpleGate, ArenaEncounter, DungeonKnight.Level

### Community 41 - "3D Combat Feedback"
Cohesion: 0.21
Nodes (7): Color, float, Material, Renderer, Vector3, CombatFeedback3D, DungeonKnight.Level

### Community 42 - "3D Chest Visual"
Cohesion: 0.21
Nodes (8): bool, float, GameObject, Light, Quaternion, Transform, DungeonChestVisual3D, DungeonKnight.Level

### Community 43 - "3D Dungeon Hazard"
Cohesion: 0.18
Nodes (7): Collider, float, int, string, Vector3, DungeonHazard3D, DungeonKnight.Level

### Community 44 - "3D Billboard & Key Drop"
Cohesion: 0.17
Nodes (7): Transform, Health, DungeonKnight.Level, DungeonSpriteBillboard3D, DungeonKnight.Loot, KeyDropper, MonoBehaviour

### Community 45 - "Interaction Feedback UI"
Cohesion: 0.23
Nodes (7): Color, float, GUIStyle, Rect, string, DungeonKnight.UI, InteractionFeedback

### Community 46 - "Exit Door"
Cohesion: 0.22
Nodes (5): bool, Collider2D, GameObject, DungeonKnight.Interactables, ExitDoor

### Community 47 - "Simple Gate"
Cohesion: 0.18
Nodes (7): bool, Collider2D, float, SpriteRenderer, Vector3, DungeonKnight.Interactables, SimpleGate

### Community 48 - "3D Camera Follow"
Cohesion: 0.22
Nodes (6): float, PlayerController3D, Transform, Vector3, CameraFollow3D, DungeonKnight.Level

### Community 49 - "3D Mini-Boss Arena"
Cohesion: 0.22
Nodes (7): bool, Collider, DungeonEnemy3D, GameObject, string, DungeonKnight.Level, DungeonMiniBossArena3D

### Community 50 - "3D Pickups"
Cohesion: 0.18
Nodes (6): Collider, float, int, DungeonKnight.Level, DungeonPickup3D, PickupKind

### Community 51 - "3D Shortcut Elevator"
Cohesion: 0.24
Nodes (7): bool, float, PlayerController3D, Transform, Vector3, DungeonKnight.Level, DungeonShortcutElevator3D

### Community 52 - "3D Game HUD"
Cohesion: 0.29
Nodes (6): Color, GUIStyle, PlayerController3D, Rect, DungeonKnight.UI, GameHud3D

### Community 53 - "Hit Burst Effect"
Cohesion: 0.22
Nodes (7): Color, float, Sprite, SpriteRenderer, Vector3, DungeonKnight.UI, HitBurst

### Community 54 - "Combat Feedback (Hitstop)"
Cohesion: 0.29
Nodes (4): Coroutine, IEnumerator, CombatFeedback, DungeonKnight

### Community 55 - "Arrow Projectile"
Cohesion: 0.22
Nodes (6): Collider2D, float, int, Vector2, ArrowProjectile, DungeonKnight.Enemies

### Community 56 - "Gate Lever"
Cohesion: 0.27
Nodes (7): bool, GameObject, SimpleGate, SpriteRenderer, Transform, DungeonKnight.Interactables, GateLever

### Community 57 - "Slash Effect"
Cohesion: 0.20
Nodes (7): bool, float, int, SpriteRenderer, Vector2, DungeonKnight.UI, SlashEffect

### Community 58 - "Drift Sprite"
Cohesion: 0.25
Nodes (5): float, Vector2, Vector3, DriftSprite, DungeonKnight.Level

### Community 59 - "Flicker Sprite"
Cohesion: 0.22
Nodes (6): Color, float, SpriteRenderer, Vector3, DungeonKnight.Level, FlickerSprite

### Community 60 - "Loot Spawner"
Cohesion: 0.33
Nodes (4): Sprite, Vector3, DungeonKnight.Loot, LootSpawner

### Community 61 - "Damage Popup"
Cohesion: 0.25
Nodes (6): Color, float, Vector3, TextMesh, DamagePopup, DungeonKnight.UI

### Community 62 - "Squash & Stretch Anim"
Cohesion: 0.22
Nodes (6): float, PlayerController2D, Rigidbody2D, Vector3, DungeonKnight.Visuals, SquashStretchAnimator

### Community 63 - "Death Shard"
Cohesion: 0.25
Nodes (4): float, SpriteRenderer, DeathShard, DungeonKnight.Enemies

### Community 64 - "Damage Hazard"
Cohesion: 0.25
Nodes (5): Collider2D, float, int, DamageHazard, DungeonKnight.Level

### Community 65 - "3D Moving Platform"
Cohesion: 0.29
Nodes (4): float, Vector3, DungeonKnight.Level, DungeonMovingPlatform3D

### Community 66 - "Lightning Flash"
Cohesion: 0.32
Nodes (4): float, SpriteRenderer, DungeonKnight.Level, LightningFlash

### Community 67 - "Coin Dropper"
Cohesion: 0.25
Nodes (4): Health, int, CoinDropper, DungeonKnight.Loot

### Community 68 - "Shield Dropper"
Cohesion: 0.29
Nodes (4): Health, Vector3, DungeonKnight.Loot, ShieldDropper

### Community 69 - "Lore Tablet"
Cohesion: 0.29
Nodes (4): GameObject, string, DungeonKnight.Interactables, LoreTablet

### Community 70 - "Enemy Health Bar"
Cohesion: 0.29
Nodes (4): GUIStyle, Health, DungeonKnight.UI, EnemyHealthBar

### Community 71 - "Game Constants"
Cohesion: 0.33
Nodes (5): float, int, LayerMask, DungeonKnight, GameConstants

### Community 72 - "Shield Pickup"
Cohesion: 0.33
Nodes (4): GameObject, IInteractable, DungeonKnight.Loot, ShieldPickup

### Community 73 - "Interactable Interface"
Cohesion: 0.40
Nodes (3): GameObject, DungeonKnight.Interactables, IInteractable

### Community 74 - "Community 74"
Cohesion: 0.11
Nodes (20): dependencies, depth, source, version, dependencies, depth, source, version (+12 more)

### Community 75 - "Community 75"
Cohesion: 0.10
Nodes (21): dependencies, depth, source, version, dependencies, depth, source, version (+13 more)

### Community 76 - "Community 76"
Cohesion: 0.10
Nodes (21): dependencies, depth, source, version, dependencies, depth, source, version (+13 more)

### Community 77 - "Community 77"
Cohesion: 0.11
Nodes (18): dependencies, depth, source, url, version, dependencies, depth, source (+10 more)

### Community 78 - "Community 78"
Cohesion: 0.09
Nodes (22): dependencies, depth, source, version, dependencies, depth, source, url (+14 more)

### Community 79 - "Community 79"
Cohesion: 0.18
Nodes (11): dependencies, dependencies, depth, source, version, dependencies, depth, source (+3 more)

### Community 80 - "Community 80"
Cohesion: 0.14
Nodes (15): dependencies, depth, source, version, depth, dependencies, depth, source (+7 more)

### Community 81 - "Community 81"
Cohesion: 0.17
Nodes (11): dependencies, com.besty.unity-skills, com.unity.2d.sprite, com.unity.2d.tilemap, com.unity.ai.assistant, com.unity.ai.inference, com.unity.modules.audio, com.unity.sdk.linux-x86_64 (+3 more)

### Community 82 - "Community 82"
Cohesion: 0.18
Nodes (11): dependencies, depth, source, url, version, depth, source, url (+3 more)

### Community 83 - "Community 83"
Cohesion: 0.17
Nodes (12): dependencies, depth, source, url, version, dependencies, depth, source (+4 more)

### Community 84 - "Community 84"
Cohesion: 0.12
Nodes (17): dependencies, depth, hash, source, version, dependencies, depth, source (+9 more)

### Community 85 - "Community 85"
Cohesion: 0.18
Nodes (11): dependencies, depth, source, version, dependencies, depth, source, version (+3 more)

### Community 86 - "Community 86"
Cohesion: 0.20
Nodes (10): dependencies, depth, source, version, dependencies, depth, source, version (+2 more)

### Community 88 - "Community 88"
Cohesion: 0.40
Nodes (3): CodexUnitySkillsBootstrap, DungeonKnight.Editor, MenuItem

### Community 89 - "Community 89"
Cohesion: 0.40
Nodes (4): Como abrirlo, Controles, Dungeon Knight 3D, Que incluye esta base 3D

## Knowledge Gaps
- **522 isolated node(s):** `DungeonKnight.Editor`, `MenuItem`, `DungeonKnight.Combat`, `int`, `bool` (+517 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **1 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `WorldOneOneBootstrap` connect `World 1-1 Bootstrap` to `3D Billboard & Key Drop`?**
  _High betweenness centrality (0.077) - this node is a cross-community bridge._
- **Why does `DungeonKnight3DBootstrap` connect `3D Scene Bootstrap` to `3D Billboard & Key Drop`?**
  _High betweenness centrality (0.051) - this node is a cross-community bridge._
- **Why does `GameSession` connect `2D Game Session` to `3D Billboard & Key Drop`?**
  _High betweenness centrality (0.047) - this node is a cross-community bridge._
- **What connects `DungeonKnight.Editor`, `MenuItem`, `DungeonKnight.Combat` to the rest of the system?**
  _522 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `World 1-1 Bootstrap` be split into smaller, more focused modules?**
  _Cohesion score 0.09548229548229548 - nodes in this community are weakly interconnected._
- **Should `3D Scene Bootstrap` be split into smaller, more focused modules?**
  _Cohesion score 0.08698474521259332 - nodes in this community are weakly interconnected._
- **Should `Character Sprite Factory` be split into smaller, more focused modules?**
  _Cohesion score 0.12895927601809956 - nodes in this community are weakly interconnected._