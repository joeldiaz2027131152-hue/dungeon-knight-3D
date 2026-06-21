# Graph Report - dungeon-knight-3D  (2026-06-20)

## Corpus Check
- 66 files · ~664,234 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 768 nodes · 1165 edges · 69 communities (68 shown, 1 thin omitted)
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS · INFERRED: 3 edges (avg confidence: 0.8)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `6c90319d`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- [[_COMMUNITY_Community 0|Community 0]]
- [[_COMMUNITY_Community 1|Community 1]]
- [[_COMMUNITY_Community 2|Community 2]]
- [[_COMMUNITY_Community 3|Community 3]]
- [[_COMMUNITY_Community 4|Community 4]]
- [[_COMMUNITY_Community 6|Community 6]]
- [[_COMMUNITY_Community 7|Community 7]]
- [[_COMMUNITY_Community 9|Community 9]]
- [[_COMMUNITY_Community 11|Community 11]]
- [[_COMMUNITY_Community 13|Community 13]]
- [[_COMMUNITY_Community 14|Community 14]]
- [[_COMMUNITY_Community 15|Community 15]]
- [[_COMMUNITY_Community 19|Community 19]]
- [[_COMMUNITY_Community 20|Community 20]]
- [[_COMMUNITY_Community 25|Community 25]]
- [[_COMMUNITY_Community 26|Community 26]]
- [[_COMMUNITY_Community 28|Community 28]]
- [[_COMMUNITY_Community 30|Community 30]]
- [[_COMMUNITY_Community 41|Community 41]]
- [[_COMMUNITY_Community 46|Community 46]]
- [[_COMMUNITY_Community 47|Community 47]]
- [[_COMMUNITY_Community 48|Community 48]]
- [[_COMMUNITY_Community 49|Community 49]]
- [[_COMMUNITY_Community 53|Community 53]]
- [[_COMMUNITY_Community 54|Community 54]]
- [[_COMMUNITY_Community 55|Community 55]]
- [[_COMMUNITY_Community 57|Community 57]]
- [[_COMMUNITY_Community 58|Community 58]]
- [[_COMMUNITY_Community 59|Community 59]]
- [[_COMMUNITY_Community 71|Community 71]]
- [[_COMMUNITY_Community 74|Community 74]]
- [[_COMMUNITY_Community 75|Community 75]]
- [[_COMMUNITY_Community 78|Community 78]]
- [[_COMMUNITY_Community 79|Community 79]]
- [[_COMMUNITY_Community 100|Community 100]]
- [[_COMMUNITY_Community 101|Community 101]]
- [[_COMMUNITY_Community 102|Community 102]]
- [[_COMMUNITY_Community 103|Community 103]]
- [[_COMMUNITY_Community 104|Community 104]]
- [[_COMMUNITY_Community 105|Community 105]]
- [[_COMMUNITY_Community 106|Community 106]]
- [[_COMMUNITY_Community 108|Community 108]]
- [[_COMMUNITY_Community 111|Community 111]]
- [[_COMMUNITY_Community 112|Community 112]]
- [[_COMMUNITY_Community 114|Community 114]]
- [[_COMMUNITY_Community 115|Community 115]]
- [[_COMMUNITY_Community 116|Community 116]]
- [[_COMMUNITY_Community 119|Community 119]]
- [[_COMMUNITY_Community 121|Community 121]]

## God Nodes (most connected - your core abstractions)
1. `CharacterSpriteFactory` - 45 edges
2. `Sprite` - 35 edges
3. `PlayerController3D` - 33 edges
4. `PixelSpriteFactory` - 30 edges
5. `Sprite` - 26 edges
6. `RiggedSkeletonEnemyVisual3D` - 21 edges
7. `DungeonEnemy3D` - 20 edges
8. `Health` - 16 edges
9. `SkeletonEnemyVisual3D` - 16 edges
10. `DungeonInteractable3D` - 15 edges

## Surprising Connections (you probably didn't know these)
- `RiggedSkeletonEnemyVisual3D` --references--> `AnimationMixerPlayable`  [EXTRACTED]
  Assets/Scripts/Level/RiggedSkeletonEnemyVisual3D.cs → Assets/Scripts/Level/RiggedSkeletonAnimationGraph3D.cs

## Import Cycles
- None detected.

## Communities (69 total, 1 thin omitted)

### Community 0 - "Community 0"
Cohesion: 0.15
Nodes (9): AnimationClip, AnimationClipPlayable, AnimationMixerPlayable, bool, float, int, DungeonKnight.Level, RiggedSkeletonAnimationGraph3D (+1 more)

### Community 1 - "Community 1"
Cohesion: 0.22
Nodes (7): bool, DungeonKnight3DAssets, DungeonKnight3DEnemySpawner, DungeonKnight3DInteractableBuilder, Vector3, DungeonKnight3DBootstrap, DungeonKnight.Level

### Community 2 - "Community 2"
Cohesion: 0.13
Nodes (10): Action, Color, Dictionary, int, Sprite, string, Texture2D, CharacterSpriteFactory (+2 more)

### Community 3 - "Community 3"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.nuget.mono-cecil

### Community 4 - "Community 4"
Cohesion: 0.14
Nodes (10): Animator, bool, float, string, Transform, Vector3, DungeonKnight.Level, RiggedSkeletonEnemyVisual3D (+2 more)

### Community 6 - "Community 6"
Cohesion: 0.19
Nodes (6): Color, Dictionary, int, Sprite, DungeonKnight, PixelSpriteFactory

### Community 7 - "Community 7"
Cohesion: 0.18
Nodes (7): Collider, float, int, string, Vector3, DungeonHazard3D, DungeonKnight.Level

### Community 9 - "Community 9"
Cohesion: 0.12
Nodes (12): CharacterController, float, int, Transform, Vector3, DungeonKnight.Player, PlayerController3D, PlayerAttackResolver3D (+4 more)

### Community 11 - "Community 11"
Cohesion: 0.14
Nodes (12): bool, CharacterController, Color, float, int, PlayerController3D, Vector3, DungeonEnemyDamageFlash3D (+4 more)

### Community 13 - "Community 13"
Cohesion: 0.13
Nodes (15): dependencies, depth, source, version, dependencies, depth, source, version (+7 more)

### Community 14 - "Community 14"
Cohesion: 0.10
Nodes (21): dependencies, depth, source, version, dependencies, depth, source, version (+13 more)

### Community 15 - "Community 15"
Cohesion: 0.13
Nodes (16): dependencies, depth, source, version, dependencies, depth, source, version (+8 more)

### Community 19 - "Community 19"
Cohesion: 0.13
Nodes (8): bool, Color, Coroutine, IEnumerator, int, SpriteRenderer, DungeonKnight.Combat, Health

### Community 20 - "Community 20"
Cohesion: 0.11
Nodes (18): dependencies, depth, source, url, version, dependencies, depth, source (+10 more)

### Community 25 - "Community 25"
Cohesion: 0.12
Nodes (16): dependencies, depth, source, version, dependencies, depth, source, url (+8 more)

### Community 26 - "Community 26"
Cohesion: 0.13
Nodes (15): dependencies, depth, source, version, dependencies, depth, source, version (+7 more)

### Community 28 - "Community 28"
Cohesion: 0.13
Nodes (9): bool, int, PlayerController3D, string, Transform, Vector3, InteractionKind, DungeonInteractable3D (+1 more)

### Community 30 - "Community 30"
Cohesion: 0.20
Nodes (3): Vector3, DungeonKnight.Player, PlayerState3D

### Community 41 - "Community 41"
Cohesion: 0.06
Nodes (23): Dictionary, float, Vector3, Animator, PlayerController3D, Color, PlayerController3D, AudioClip (+15 more)

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

### Community 53 - "Community 53"
Cohesion: 0.12
Nodes (17): dependencies, depth, hash, source, version, dependencies, depth, source (+9 more)

### Community 54 - "Community 54"
Cohesion: 0.18
Nodes (11): dependencies, depth, source, version, dependencies, depth, source, version (+3 more)

### Community 55 - "Community 55"
Cohesion: 0.29
Nodes (4): Coroutine, IEnumerator, CombatFeedback, DungeonKnight

### Community 57 - "Community 57"
Cohesion: 0.20
Nodes (11): dependencies, depth, dependencies, depth, source, url, version, source (+3 more)

### Community 58 - "Community 58"
Cohesion: 0.12
Nodes (16): dependencies, dependencies, depth, source, version, dependencies, depth, source (+8 more)

### Community 59 - "Community 59"
Cohesion: 0.29
Nodes (6): float, PlayerController3D, Transform, Vector3, CameraFollow3D, DungeonKnight.Level

### Community 71 - "Community 71"
Cohesion: 0.25
Nodes (5): Color, float, Renderer, DungeonEnemyDamageFlash3D, DungeonKnight.Level

### Community 74 - "Community 74"
Cohesion: 0.33
Nodes (5): float, int, DungeonKnight, GameConstants, LayerMask

### Community 75 - "Community 75"
Cohesion: 0.14
Nodes (12): Collider, float, int, Color, Material, Transform, Vector3, DungeonKnight.Level (+4 more)

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

### Community 108 - "Community 108"
Cohesion: 0.24
Nodes (7): DungeonKnight3DAssets, GameObject, PlayerController3D, Transform, Vector3, DungeonKnight3DEnemySpawner, DungeonKnight.Level

### Community 112 - "Community 112"
Cohesion: 0.29
Nodes (5): Collider, DungeonInteractable3D, Vector3, DungeonKnight.Player, PlayerInteractionScanner3D

### Community 114 - "Community 114"
Cohesion: 0.33
Nodes (4): Collider, Vector3, DungeonKnight.Player, PlayerAttackResolver3D

### Community 115 - "Community 115"
Cohesion: 0.33
Nodes (4): float, string, DungeonKnight.Player, PlayerStatusMessenger3D

### Community 116 - "Community 116"
Cohesion: 0.22
Nodes (6): CharacterController, float, Transform, Vector3, DungeonKnight.Player, PlayerMovementMotor3D

### Community 119 - "Community 119"
Cohesion: 0.24
Nodes (7): Color, GameObject, Renderer, Transform, Bounds, DungeonKnight.Level, RiggedSkeletonModelSetup3D

### Community 121 - "Community 121"
Cohesion: 0.13
Nodes (15): dependencies, depth, source, url, version, depth, source, version (+7 more)

## Knowledge Gaps
- **316 isolated node(s):** `DungeonKnight.Editor`, `MenuItem`, `DungeonKnight.Combat`, `int`, `bool` (+311 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **1 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `dependencies` connect `Community 15` to `Community 121`, `Community 26`, `Community 3`, `Community 13`, `Community 14`, `Community 48`, `Community 20`, `Community 53`, `Community 54`, `Community 25`, `Community 58`, `Community 57`?**
  _High betweenness centrality (0.053) - this node is a cross-community bridge._
- **Why does `RiggedSkeletonEnemyVisual3D` connect `Community 4` to `Community 0`, `Community 41`?**
  _High betweenness centrality (0.032) - this node is a cross-community bridge._
- **Why does `PlayerController3D` connect `Community 9` to `Community 41`?**
  _High betweenness centrality (0.029) - this node is a cross-community bridge._
- **What connects `DungeonKnight.Editor`, `MenuItem`, `DungeonKnight.Combat` to the rest of the system?**
  _316 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Community 0` be split into smaller, more focused modules?**
  _Cohesion score 0.14705882352941177 - nodes in this community are weakly interconnected._
- **Should `Community 2` be split into smaller, more focused modules?**
  _Cohesion score 0.12895927601809956 - nodes in this community are weakly interconnected._
- **Should `Community 4` be split into smaller, more focused modules?**
  _Cohesion score 0.14285714285714285 - nodes in this community are weakly interconnected._