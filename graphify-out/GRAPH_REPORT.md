# Graph Report - dungeon-knight-3D  (2026-06-20)

## Corpus Check
- 115 files · ~689,988 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 1521 nodes · 2467 edges · 122 communities (120 shown, 2 thin omitted)
- Extraction: 99% EXTRACTED · 1% INFERRED · 0% AMBIGUOUS · INFERRED: 14 edges (avg confidence: 0.8)
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- [[_COMMUNITY_Community 0|Community 0]]
- [[_COMMUNITY_Community 1|Community 1]]
- [[_COMMUNITY_Community 2|Community 2]]
- [[_COMMUNITY_Community 3|Community 3]]
- [[_COMMUNITY_Community 4|Community 4]]
- [[_COMMUNITY_Community 5|Community 5]]
- [[_COMMUNITY_Community 6|Community 6]]
- [[_COMMUNITY_Community 7|Community 7]]
- [[_COMMUNITY_Community 8|Community 8]]
- [[_COMMUNITY_Community 9|Community 9]]
- [[_COMMUNITY_Community 10|Community 10]]
- [[_COMMUNITY_Community 11|Community 11]]
- [[_COMMUNITY_Community 12|Community 12]]
- [[_COMMUNITY_Community 13|Community 13]]
- [[_COMMUNITY_Community 14|Community 14]]
- [[_COMMUNITY_Community 15|Community 15]]
- [[_COMMUNITY_Community 16|Community 16]]
- [[_COMMUNITY_Community 17|Community 17]]
- [[_COMMUNITY_Community 18|Community 18]]
- [[_COMMUNITY_Community 19|Community 19]]
- [[_COMMUNITY_Community 20|Community 20]]
- [[_COMMUNITY_Community 21|Community 21]]
- [[_COMMUNITY_Community 22|Community 22]]
- [[_COMMUNITY_Community 23|Community 23]]
- [[_COMMUNITY_Community 24|Community 24]]
- [[_COMMUNITY_Community 25|Community 25]]
- [[_COMMUNITY_Community 26|Community 26]]
- [[_COMMUNITY_Community 27|Community 27]]
- [[_COMMUNITY_Community 28|Community 28]]
- [[_COMMUNITY_Community 29|Community 29]]
- [[_COMMUNITY_Community 30|Community 30]]
- [[_COMMUNITY_Community 31|Community 31]]
- [[_COMMUNITY_Community 32|Community 32]]
- [[_COMMUNITY_Community 33|Community 33]]
- [[_COMMUNITY_Community 34|Community 34]]
- [[_COMMUNITY_Community 35|Community 35]]
- [[_COMMUNITY_Community 36|Community 36]]
- [[_COMMUNITY_Community 37|Community 37]]
- [[_COMMUNITY_Community 38|Community 38]]
- [[_COMMUNITY_Community 39|Community 39]]
- [[_COMMUNITY_Community 40|Community 40]]
- [[_COMMUNITY_Community 41|Community 41]]
- [[_COMMUNITY_Community 42|Community 42]]
- [[_COMMUNITY_Community 43|Community 43]]
- [[_COMMUNITY_Community 44|Community 44]]
- [[_COMMUNITY_Community 45|Community 45]]
- [[_COMMUNITY_Community 46|Community 46]]
- [[_COMMUNITY_Community 47|Community 47]]
- [[_COMMUNITY_Community 48|Community 48]]
- [[_COMMUNITY_Community 49|Community 49]]
- [[_COMMUNITY_Community 50|Community 50]]
- [[_COMMUNITY_Community 51|Community 51]]
- [[_COMMUNITY_Community 52|Community 52]]
- [[_COMMUNITY_Community 53|Community 53]]
- [[_COMMUNITY_Community 54|Community 54]]
- [[_COMMUNITY_Community 55|Community 55]]
- [[_COMMUNITY_Community 56|Community 56]]
- [[_COMMUNITY_Community 57|Community 57]]
- [[_COMMUNITY_Community 58|Community 58]]
- [[_COMMUNITY_Community 59|Community 59]]
- [[_COMMUNITY_Community 60|Community 60]]
- [[_COMMUNITY_Community 61|Community 61]]
- [[_COMMUNITY_Community 62|Community 62]]
- [[_COMMUNITY_Community 63|Community 63]]
- [[_COMMUNITY_Community 64|Community 64]]
- [[_COMMUNITY_Community 65|Community 65]]
- [[_COMMUNITY_Community 66|Community 66]]
- [[_COMMUNITY_Community 67|Community 67]]
- [[_COMMUNITY_Community 68|Community 68]]
- [[_COMMUNITY_Community 69|Community 69]]
- [[_COMMUNITY_Community 70|Community 70]]
- [[_COMMUNITY_Community 71|Community 71]]
- [[_COMMUNITY_Community 72|Community 72]]
- [[_COMMUNITY_Community 73|Community 73]]
- [[_COMMUNITY_Community 74|Community 74]]
- [[_COMMUNITY_Community 75|Community 75]]
- [[_COMMUNITY_Community 76|Community 76]]
- [[_COMMUNITY_Community 77|Community 77]]
- [[_COMMUNITY_Community 78|Community 78]]
- [[_COMMUNITY_Community 79|Community 79]]
- [[_COMMUNITY_Community 100|Community 100]]
- [[_COMMUNITY_Community 101|Community 101]]
- [[_COMMUNITY_Community 102|Community 102]]
- [[_COMMUNITY_Community 103|Community 103]]
- [[_COMMUNITY_Community 104|Community 104]]
- [[_COMMUNITY_Community 105|Community 105]]
- [[_COMMUNITY_Community 106|Community 106]]
- [[_COMMUNITY_Community 107|Community 107]]
- [[_COMMUNITY_Community 108|Community 108]]
- [[_COMMUNITY_Community 109|Community 109]]
- [[_COMMUNITY_Community 110|Community 110]]
- [[_COMMUNITY_Community 111|Community 111]]
- [[_COMMUNITY_Community 112|Community 112]]
- [[_COMMUNITY_Community 113|Community 113]]
- [[_COMMUNITY_Community 114|Community 114]]
- [[_COMMUNITY_Community 115|Community 115]]
- [[_COMMUNITY_Community 116|Community 116]]
- [[_COMMUNITY_Community 117|Community 117]]
- [[_COMMUNITY_Community 118|Community 118]]
- [[_COMMUNITY_Community 119|Community 119]]
- [[_COMMUNITY_Community 120|Community 120]]
- [[_COMMUNITY_Community 121|Community 121]]

## God Nodes (most connected - your core abstractions)
1. `WorldOneOneBootstrap` - 82 edges
2. `Vector2` - 59 edges
3. `CharacterSpriteFactory` - 45 edges
4. `PlayerController2D` - 43 edges
5. `Sprite` - 35 edges
6. `PlayerController3D` - 33 edges
7. `PixelSpriteFactory` - 30 edges
8. `GameSession` - 30 edges
9. `Sprite` - 26 edges
10. `RiggedSkeletonEnemyVisual3D` - 21 edges

## Surprising Connections (you probably didn't know these)
- `RiggedSkeletonEnemyVisual3D` --references--> `AnimationMixerPlayable`  [EXTRACTED]
  Assets/Scripts/Level/RiggedSkeletonEnemyVisual3D.cs → Assets/Scripts/Level/RiggedSkeletonAnimationGraph3D.cs

## Import Cycles
- None detected.

## Communities (122 total, 2 thin omitted)

### Community 0 - "Community 0"
Cohesion: 0.10
Nodes (14): bool, Color, float, GameObject, SimpleGate, Sprite, SpriteRenderer, Transform (+6 more)

### Community 1 - "Community 1"
Cohesion: 0.22
Nodes (7): bool, DungeonKnight3DAssets, DungeonKnight3DEnemySpawner, DungeonKnight3DInteractableBuilder, Vector3, DungeonKnight3DBootstrap, DungeonKnight.Level

### Community 2 - "Community 2"
Cohesion: 0.13
Nodes (10): Action, Color, Dictionary, int, Sprite, string, Texture2D, Vector2 (+2 more)

### Community 3 - "Community 3"
Cohesion: 0.07
Nodes (16): bool, Collider2D, float, Health, int, LayerMask, List, PlayerInventory (+8 more)

### Community 4 - "Community 4"
Cohesion: 0.07
Nodes (19): AnimationMixerPlayable, AnimationClip, AnimationClipPlayable, bool, float, int, PlayableGraph, Animator (+11 more)

### Community 5 - "Community 5"
Cohesion: 0.10
Nodes (14): bool, Color, float, GameObject, GUIStyle, Health, IEnumerator, int (+6 more)

### Community 6 - "Community 6"
Cohesion: 0.19
Nodes (6): Color, Dictionary, int, Sprite, DungeonKnight, PixelSpriteFactory

### Community 7 - "Community 7"
Cohesion: 0.18
Nodes (7): Collider, float, int, string, Vector3, DungeonHazard3D, DungeonKnight.Level

### Community 8 - "Community 8"
Cohesion: 0.23
Nodes (10): bool, Color, GameObject, GUIStyle, Health, PlayerController2D, PlayerInventory, Rect (+2 more)

### Community 9 - "Community 9"
Cohesion: 0.12
Nodes (12): CharacterController, float, int, Transform, Vector3, DungeonKnight.Player, PlayerController3D, PlayerAttackResolver3D (+4 more)

### Community 10 - "Community 10"
Cohesion: 0.15
Nodes (12): float, int, PlayerController2D, Rigidbody2D, Sprite, SpriteRenderer, Transform, SkeletonArcherAI (+4 more)

### Community 11 - "Community 11"
Cohesion: 0.14
Nodes (12): bool, CharacterController, Color, float, int, PlayerController3D, Vector3, DungeonEnemyDamageFlash3D (+4 more)

### Community 12 - "Community 12"
Cohesion: 0.16
Nodes (11): Collider2D, Color, float, int, Sprite, SpriteRenderer, Transform, Vector2 (+3 more)

### Community 13 - "Community 13"
Cohesion: 0.11
Nodes (20): dependencies, depth, source, version, dependencies, depth, source, version (+12 more)

### Community 14 - "Community 14"
Cohesion: 0.10
Nodes (21): dependencies, depth, source, version, dependencies, depth, source, version (+13 more)

### Community 15 - "Community 15"
Cohesion: 0.10
Nodes (21): dependencies, depth, source, version, dependencies, depth, source, version (+13 more)

### Community 16 - "Community 16"
Cohesion: 0.12
Nodes (12): bool, Color, float, int, Quaternion, Rigidbody2D, SpriteRenderer, Transform (+4 more)

### Community 17 - "Community 17"
Cohesion: 0.12
Nodes (11): bool, GameObject, IEnumerator, int, SimpleGate, Sprite, SpriteRenderer, Transform (+3 more)

### Community 18 - "Community 18"
Cohesion: 0.17
Nodes (10): Collider2D, Color, float, int, Sprite, SpriteRenderer, Transform, Vector2 (+2 more)

### Community 19 - "Community 19"
Cohesion: 0.13
Nodes (8): bool, Color, Coroutine, IEnumerator, int, SpriteRenderer, DungeonKnight.Combat, Health

### Community 20 - "Community 20"
Cohesion: 0.11
Nodes (18): dependencies, depth, source, url, version, dependencies, depth, source (+10 more)

### Community 21 - "Community 21"
Cohesion: 0.15
Nodes (9): bool, Color, float, Sprite, SpriteRenderer, Vector2, Vector3, DungeonKnight.Interactables (+1 more)

### Community 22 - "Community 22"
Cohesion: 0.15
Nodes (9): float, int, Rigidbody2D, SpriteRenderer, Transform, Vector2, Vector3, ChasingBatAI (+1 more)

### Community 23 - "Community 23"
Cohesion: 0.16
Nodes (9): float, int, Rigidbody2D, Sprite, SpriteRenderer, Transform, Vector2, DungeonKnight.Enemies (+1 more)

### Community 24 - "Community 24"
Cohesion: 0.15
Nodes (9): AnimationClip, AnimationClipPlayable, Animator, float, PlayableGraph, string, Transform, DungeonKnight.Visuals (+1 more)

### Community 25 - "Community 25"
Cohesion: 0.09
Nodes (22): dependencies, depth, source, version, dependencies, depth, source, url (+14 more)

### Community 26 - "Community 26"
Cohesion: 0.18
Nodes (11): dependencies, dependencies, depth, source, version, dependencies, depth, source (+3 more)

### Community 27 - "Community 27"
Cohesion: 0.20
Nodes (9): bool, GameObject, Health, List, SimpleGate, SpriteRenderer, Transform, DungeonKnight.Interactables (+1 more)

### Community 28 - "Community 28"
Cohesion: 0.13
Nodes (9): bool, int, PlayerController3D, string, Transform, Vector3, InteractionKind, DungeonInteractable3D (+1 more)

### Community 29 - "Community 29"
Cohesion: 0.16
Nodes (4): bool, DungeonKnight.Player, PlayerInventory, ShieldKind

### Community 30 - "Community 30"
Cohesion: 0.20
Nodes (3): Vector3, DungeonKnight.Player, PlayerState3D

### Community 31 - "Community 31"
Cohesion: 0.19
Nodes (7): float, int, Sprite, SpriteRenderer, Transform, DungeonKnight.Enemies, KeyGuardianBoss

### Community 32 - "Community 32"
Cohesion: 0.16
Nodes (9): bool, Collider2D, Collision2D, float, IEnumerator, SpriteRenderer, Vector3, BreakablePlatform (+1 more)

### Community 33 - "Community 33"
Cohesion: 0.18
Nodes (8): bool, float, Rigidbody2D, Transform, Vector2, Vector3, CameraFollow2D, DungeonKnight.Level

### Community 34 - "Community 34"
Cohesion: 0.19
Nodes (6): bool, Collider2D, float, Rigidbody2D, DropThroughSurface, DungeonKnight.Level

### Community 35 - "Community 35"
Cohesion: 0.16
Nodes (8): Collider2D, Collision2D, float, GameObject, int, Vector2, CoinPickup, DungeonKnight.Loot

### Community 36 - "Community 36"
Cohesion: 0.19
Nodes (8): Color, GUIStyle, IInteractable, LayerMask, Rect, Transform, DungeonKnight.Player, PlayerInteraction

### Community 37 - "Community 37"
Cohesion: 0.27
Nodes (6): Health, Sprite, Vector2, Vector3, DungeonKnight.Enemies, SkeletonDeathEffect

### Community 38 - "Community 38"
Cohesion: 0.18
Nodes (7): bool, Health, int, Sprite, Vector3, BreakableCrate, DungeonKnight.Interactables

### Community 39 - "Community 39"
Cohesion: 0.18
Nodes (7): Collider2D, Collision2D, float, GameObject, Vector2, DungeonKnight.Loot, KeyPickup

### Community 40 - "Community 40"
Cohesion: 0.21
Nodes (8): Color, float, Sprite, SpriteRenderer, Vector2, Vector3, DungeonKnight.Loot, KeyVisual

### Community 41 - "Community 41"
Cohesion: 0.27
Nodes (5): Dictionary, AudioClip, AudioSource, DungeonKnight, RetroAudio

### Community 42 - "Community 42"
Cohesion: 0.21
Nodes (7): Color, float, Sprite, SpriteRenderer, Vector2, DungeonKnight.Enemies, KeyGuardianVisual

### Community 43 - "Community 43"
Cohesion: 0.18
Nodes (7): bool, float, GameObject, Sprite, SpriteRenderer, Bonfire, DungeonKnight.Interactables

### Community 44 - "Community 44"
Cohesion: 0.18
Nodes (7): bool, Collider2D, Health, List, SimpleGate, ArenaEncounter, DungeonKnight.Level

### Community 45 - "Community 45"
Cohesion: 0.23
Nodes (7): Color, float, GUIStyle, Rect, string, DungeonKnight.UI, InteractionFeedback

### Community 46 - "Community 46"
Cohesion: 0.17
Nodes (11): dependencies, com.besty.unity-skills, com.unity.2d.sprite, com.unity.2d.tilemap, com.unity.ai.assistant, com.unity.ai.inference, com.unity.modules.audio, com.unity.sdk.linux-x86_64 (+3 more)

### Community 47 - "Community 47"
Cohesion: 0.25
Nodes (6): DungeonKnight3DAssets, GameObject, Material, Vector3, DungeonKnight3DGeometryBuilder, DungeonKnight.Level

### Community 48 - "Community 48"
Cohesion: 0.17
Nodes (12): dependencies, depth, source, url, version, dependencies, depth, source (+4 more)

### Community 49 - "Community 49"
Cohesion: 0.27
Nodes (5): DungeonKnight3DAssets, GameObject, Vector3, DungeonKnight3DTraversalBuilder, DungeonKnight.Level

### Community 50 - "Community 50"
Cohesion: 0.18
Nodes (6): Collider, float, int, DungeonKnight.Level, DungeonPickup3D, PickupKind

### Community 51 - "Community 51"
Cohesion: 0.29
Nodes (6): Color, GUIStyle, PlayerController3D, Rect, DungeonKnight.UI, GameHud3D

### Community 52 - "Community 52"
Cohesion: 0.22
Nodes (7): Color, float, Sprite, SpriteRenderer, Vector3, DungeonKnight.UI, HitBurst

### Community 53 - "Community 53"
Cohesion: 0.12
Nodes (17): dependencies, depth, hash, source, version, dependencies, depth, source (+9 more)

### Community 54 - "Community 54"
Cohesion: 0.18
Nodes (11): dependencies, depth, source, version, dependencies, depth, source, version (+3 more)

### Community 55 - "Community 55"
Cohesion: 0.29
Nodes (4): Coroutine, IEnumerator, CombatFeedback, DungeonKnight

### Community 56 - "Community 56"
Cohesion: 0.22
Nodes (6): Collider2D, float, int, Vector2, ArrowProjectile, DungeonKnight.Enemies

### Community 57 - "Community 57"
Cohesion: 0.14
Nodes (15): dependencies, depth, source, version, depth, dependencies, depth, source (+7 more)

### Community 58 - "Community 58"
Cohesion: 0.20
Nodes (10): dependencies, depth, source, version, dependencies, depth, source, version (+2 more)

### Community 59 - "Community 59"
Cohesion: 0.29
Nodes (6): float, PlayerController3D, Transform, Vector3, CameraFollow3D, DungeonKnight.Level

### Community 60 - "Community 60"
Cohesion: 0.25
Nodes (5): float, Vector2, Vector3, DriftSprite, DungeonKnight.Level

### Community 61 - "Community 61"
Cohesion: 0.22
Nodes (6): Color, float, SpriteRenderer, Vector3, DungeonKnight.Level, FlickerSprite

### Community 62 - "Community 62"
Cohesion: 0.33
Nodes (4): Sprite, Vector3, DungeonKnight.Loot, LootSpawner

### Community 63 - "Community 63"
Cohesion: 0.25
Nodes (6): Color, float, Vector3, TextMesh, DamagePopup, DungeonKnight.UI

### Community 64 - "Community 64"
Cohesion: 0.22
Nodes (6): float, PlayerController2D, Rigidbody2D, Vector3, DungeonKnight.Visuals, SquashStretchAnimator

### Community 65 - "Community 65"
Cohesion: 0.25
Nodes (4): float, SpriteRenderer, DeathShard, DungeonKnight.Enemies

### Community 66 - "Community 66"
Cohesion: 0.25
Nodes (5): Collider2D, float, int, DamageHazard, DungeonKnight.Level

### Community 67 - "Community 67"
Cohesion: 0.29
Nodes (4): float, Vector3, DungeonKnight.Level, DungeonMovingPlatform3D

### Community 68 - "Community 68"
Cohesion: 0.32
Nodes (4): float, SpriteRenderer, DungeonKnight.Level, LightningFlash

### Community 69 - "Community 69"
Cohesion: 0.25
Nodes (4): Health, int, CoinDropper, DungeonKnight.Loot

### Community 70 - "Community 70"
Cohesion: 0.29
Nodes (4): Health, Vector3, DungeonKnight.Loot, ShieldDropper

### Community 71 - "Community 71"
Cohesion: 0.25
Nodes (5): Color, float, Renderer, DungeonEnemyDamageFlash3D, DungeonKnight.Level

### Community 72 - "Community 72"
Cohesion: 0.29
Nodes (4): Health, DungeonKnight.Loot, KeyDropper, MonoBehaviour

### Community 73 - "Community 73"
Cohesion: 0.29
Nodes (4): GUIStyle, Health, DungeonKnight.UI, EnemyHealthBar

### Community 74 - "Community 74"
Cohesion: 0.33
Nodes (5): float, int, LayerMask, DungeonKnight, GameConstants

### Community 75 - "Community 75"
Cohesion: 0.38
Nodes (5): Color, Material, Transform, Vector3, RiggedSkeletonSwordVisual3D

### Community 76 - "Community 76"
Cohesion: 0.22
Nodes (5): bool, Collider2D, GameObject, DungeonKnight.Interactables, ExitDoor

### Community 77 - "Community 77"
Cohesion: 0.40
Nodes (3): GameObject, DungeonKnight.Interactables, IInteractable

### Community 78 - "Community 78"
Cohesion: 0.40
Nodes (3): CodexUnitySkillsBootstrap, DungeonKnight.Editor, MenuItem

### Community 79 - "Community 79"
Cohesion: 0.40
Nodes (4): Como abrirlo, Controles, Dungeon Knight 3D, Que incluye esta base 3D

### Community 100 - "Community 100"
Cohesion: 0.16
Nodes (11): DungeonKnight3DAssets, DungeonKnight3DEnemySpawner, DungeonKnight3DGeometryBuilder, DungeonKnight3DInteractableBuilder, GameObject, Material, PlayerController3D, Vector3 (+3 more)

### Community 101 - "Community 101"
Cohesion: 0.21
Nodes (8): DungeonKnight3DAssets, GameObject, Material, PlayerController3D, Transform, Vector3, DungeonKnight3DPlayerFactory, DungeonKnight.Level

### Community 102 - "Community 102"
Cohesion: 0.19
Nodes (8): bool, float, int, Sprite, SpriteRenderer, Vector3, DungeonKnight.Level, SkeletonEnemyVisual3D

### Community 103 - "Community 103"
Cohesion: 0.20
Nodes (8): float, int, PlayerController3D, Sprite, SpriteRenderer, Vector3, DungeonKnight.Level, PlayerVisual3D

### Community 104 - "Community 104"
Cohesion: 0.22
Nodes (8): Color, Material, Renderer, Sprite, Texture2D, Vector3, DungeonKnight3DAssets, DungeonKnight.Level

### Community 105 - "Community 105"
Cohesion: 0.23
Nodes (7): DungeonKnight3DAssets, DungeonKnight3DGeometryBuilder, GameObject, Material, Vector3, DungeonKnight3DWorldBuilder, DungeonKnight.Level

### Community 106 - "Community 106"
Cohesion: 0.29
Nodes (7): DungeonInteractable3D, DungeonKnight3DAssets, GameObject, Material, Vector3, DungeonKnight3DInteractableBuilder, DungeonKnight.Level

### Community 107 - "Community 107"
Cohesion: 0.18
Nodes (7): bool, Collider2D, float, SpriteRenderer, Vector3, DungeonKnight.Interactables, SimpleGate

### Community 108 - "Community 108"
Cohesion: 0.24
Nodes (7): DungeonKnight3DAssets, GameObject, PlayerController3D, Transform, Vector3, DungeonKnight3DEnemySpawner, DungeonKnight.Level

### Community 109 - "Community 109"
Cohesion: 0.20
Nodes (7): bool, float, int, SpriteRenderer, Vector2, DungeonKnight.UI, SlashEffect

### Community 110 - "Community 110"
Cohesion: 0.38
Nodes (4): Animator, PlayerController3D, DungeonKnight.Level, PlayerModelVisual3D

### Community 112 - "Community 112"
Cohesion: 0.29
Nodes (5): Collider, DungeonInteractable3D, Vector3, DungeonKnight.Player, PlayerInteractionScanner3D

### Community 113 - "Community 113"
Cohesion: 0.27
Nodes (7): bool, GameObject, SimpleGate, SpriteRenderer, Transform, DungeonKnight.Interactables, GateLever

### Community 114 - "Community 114"
Cohesion: 0.33
Nodes (4): Collider, Vector3, DungeonKnight.Player, PlayerAttackResolver3D

### Community 115 - "Community 115"
Cohesion: 0.33
Nodes (4): float, string, DungeonKnight.Player, PlayerStatusMessenger3D

### Community 116 - "Community 116"
Cohesion: 0.22
Nodes (6): CharacterController, float, Transform, Vector3, DungeonKnight.Player, PlayerMovementMotor3D

### Community 117 - "Community 117"
Cohesion: 0.29
Nodes (4): GameObject, string, DungeonKnight.Interactables, LoreTablet

### Community 118 - "Community 118"
Cohesion: 0.33
Nodes (4): GameObject, IInteractable, DungeonKnight.Loot, ShieldPickup

### Community 119 - "Community 119"
Cohesion: 0.24
Nodes (7): Color, GameObject, Renderer, Transform, Bounds, DungeonKnight.Level, RiggedSkeletonModelSetup3D

### Community 121 - "Community 121"
Cohesion: 0.18
Nodes (11): dependencies, depth, source, url, version, depth, source, url (+3 more)

## Knowledge Gaps
- **565 isolated node(s):** `DungeonKnight.Editor`, `MenuItem`, `DungeonKnight.Combat`, `int`, `bool` (+560 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **2 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `WorldOneOneBootstrap` connect `Community 0` to `Community 72`?**
  _High betweenness centrality (0.066) - this node is a cross-community bridge._
- **Why does `RiggedSkeletonEnemyVisual3D` connect `Community 4` to `Community 72`?**
  _High betweenness centrality (0.047) - this node is a cross-community bridge._
- **Why does `GameSession` connect `Community 5` to `Community 72`?**
  _High betweenness centrality (0.034) - this node is a cross-community bridge._
- **What connects `DungeonKnight.Editor`, `MenuItem`, `DungeonKnight.Combat` to the rest of the system?**
  _565 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Community 0` be split into smaller, more focused modules?**
  _Cohesion score 0.09548229548229548 - nodes in this community are weakly interconnected._
- **Should `Community 2` be split into smaller, more focused modules?**
  _Cohesion score 0.12895927601809956 - nodes in this community are weakly interconnected._
- **Should `Community 3` be split into smaller, more focused modules?**
  _Cohesion score 0.07188160676532769 - nodes in this community are weakly interconnected._