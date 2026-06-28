using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace DungeonKnight.Player
{
    public class PlayerModelVisual3D : MonoBehaviour
    {
        private const string AnimationPath = "Characters/Player/DarkKnight3D/Animations/ProSwordAndShieldPack/";
        private const string RollAnimationPath = "Characters/Player/DarkKnight3D/Animations/Roll/";
        private const string JumpAnimationPath = "Characters/Player/DarkKnight3D/Animations/Jump/";
        private const string EquipmentPath = "Characters/Player/DarkKnight3D/Equipment/Options/";
        [SerializeField] private float referenceWalkSpeed = 4.2f;
        [SerializeField] private float walkPlaybackAtReferenceSpeed = 1.08f;
        [SerializeField] private float maxLocomotionPlaybackSpeed = 1.85f;
        [SerializeField] private float chargedAttackVisualDuration = 1.35f;
        private static readonly Vector3 SwordGripOffset = Vector3.zero;
        private static readonly Vector3 SwordGripRotation = new Vector3(4f, 82f, -85f);
        private const float SwordGripScale = 0.01f;
        private static readonly Vector3 ShieldGripOffset = new Vector3(-0.001f, 0.0008f, 0.0012f);
        private static readonly Vector3 ShieldGripRotation = new Vector3(82f, 172f, 12f);
        private const float ShieldGripScale = 0.0125f;

        private enum ModelState
        {
            None,
            Idle,
            Walk,
            Run,
            Attack,
            ChargedAttack,
            Block,
            BlockMove,
            Jump
        }

        private PlayerController3D controller;
        private PlayerInventory inventory;
        private Animator animator;
        private PlayableGraph graph;
        private AnimationPlayableOutput output;
        private Playable currentRootPlayable;
        private AnimationClipPlayable currentPlayable;
        private AnimationClipPlayable currentOverlayPlayable;
        private AnimationClip idleClip;
        private AnimationClip walkClip;
        private AnimationClip runClip;
        private AnimationClip rollClip;
        private AnimationClip attackClip;
        private AnimationClip chargedAttackClip;
        private AnimationClip blockIdleClip;
        private AnimationClip blockMoveClip;
        private AnimationClip jumpClip;
        private ModelState currentState = ModelState.None;
        private Transform animatedRoot;
        private Transform hipsRoot;
        private Vector3 visualBaseLocalPosition;
        private Quaternion visualBaseLocalRotation;
        private Vector3 animatedRootBaseLocalPosition;
        private Quaternion animatedRootBaseLocalRotation;
        private Vector3 hipsBaseLocalPosition;
        private int observedAttackSequence = -1;
        private Material swordMaterial;
        private Material shieldMaterial;
        private Material shieldTrimMaterial;
        private Transform swordEquipmentRoot;
        private Transform shieldEquipmentRoot;

        public void Configure(PlayerController3D playerController, Animator modelAnimator)
        {
            ResetRuntimeGraph();
            controller = playerController;
            inventory = controller ? controller.GetComponent<PlayerInventory>() : null;
            animator = modelAnimator;
            if (!animator) return;

            idleClip = LoadClip("sword and shield idle");
            walkClip = LoadClip("sword and shield walk");
            runClip = LoadClip("sword and shield run");
            rollClip = LoadClip(RollAnimationPath, "falling_to_roll");
            attackClip = LoadClip("sword and shield slash");
            chargedAttackClip = LoadClip("Sword And Shield Slash personaje");
            blockIdleClip = LoadClip("sword and shield block idle");
            blockMoveClip = walkClip;
            jumpClip = LoadClip(JumpAnimationPath, "jumping");
            AttachStarterEquipment();
            UpdateEquipmentVisibility();
            CacheRootStabilizers();

            graph = PlayableGraph.Create("Dark Knight 3D Player Animation");
            graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            output = AnimationPlayableOutput.Create(graph, "Animation", animator);
            graph.Play();

            PlayState(ModelState.Idle, true);
            Debug.Log($"Dark Knight 3D animation ready. Idle:{ClipLoaded(idleClip)} Walk:{ClipLoaded(walkClip)} Run:{ClipLoaded(runClip)} Attack:{ClipLoaded(attackClip)} Charged:{ClipLoaded(chargedAttackClip)} Block:{ClipLoaded(blockIdleClip)}");
        }

        private void OnDisable()
        {
            ResetRuntimeGraph();
        }

        private void OnDestroy()
        {
            ResetRuntimeGraph();
        }

        private void Update()
        {
            if (!controller || !animator || !graph.IsValid()) return;

            ModelState nextState = ChooseState();
            bool replayAttack = ShouldReplayAttack(nextState);
            PlayState(nextState, replayAttack);
            UpdateCurrentPlaybackSpeed();
            KeepCurrentClipInRange();
            UpdateEquipmentVisibility();
        }

        private void LateUpdate()
        {
            StabilizeAnimatedRoot();
        }

        private ModelState ChooseState()
        {
            if (controller.IsChargedAttack) return ModelState.ChargedAttack;
            if (controller.IsAttacking) return ModelState.Attack;
            if (!controller.IsGrounded) return ModelState.Jump;
            if (controller.IsBlocking) return controller.IsMoving ? ModelState.BlockMove : ModelState.Block;
            if (controller.IsDashing) return ModelState.Run;
            if (controller.IsMoving) return ModelState.Walk;
            return ModelState.Idle;
        }

        private bool ShouldReplayAttack(ModelState nextState)
        {
            bool attackState = nextState == ModelState.Attack || nextState == ModelState.ChargedAttack;
            if (!attackState || !controller.IsAttacking) return false;
            if (observedAttackSequence == controller.AttackSequence) return false;

            observedAttackSequence = controller.AttackSequence;
            return true;
        }

        private void PlayState(ModelState state, bool force)
        {
            if (!force && currentState == state) return;

            AnimationClip clip = ClipFor(state);
            if (!clip) clip = idleClip;
            if (!clip) return;

            ClearCurrentPlayable();

            if (state == ModelState.BlockMove && walkClip && blockIdleClip)
            {
                PlayBlockMove();
                currentState = state;
                return;
            }

            currentPlayable = AnimationClipPlayable.Create(graph, clip);
            currentPlayable.SetApplyFootIK(true);
            currentPlayable.SetTime(0f);
            currentPlayable.SetSpeed(SpeedFor(state, clip));
            currentPlayable.SetDuration(clip.length);
            currentRootPlayable = currentPlayable;
            output.SetSourcePlayable(currentPlayable);
            currentState = state;
        }

        private void PlayBlockMove()
        {
            AnimationLayerMixerPlayable mixer = AnimationLayerMixerPlayable.Create(graph, 2);
            currentPlayable = AnimationClipPlayable.Create(graph, walkClip);
            currentPlayable.SetApplyFootIK(true);
            currentPlayable.SetTime(0f);
            currentPlayable.SetSpeed(SpeedFor(ModelState.BlockMove, walkClip));
            currentPlayable.SetDuration(walkClip.length);

            currentOverlayPlayable = AnimationClipPlayable.Create(graph, blockIdleClip);
            currentOverlayPlayable.SetApplyFootIK(false);
            currentOverlayPlayable.SetTime(0f);
            currentOverlayPlayable.SetSpeed(1f);
            currentOverlayPlayable.SetDuration(blockIdleClip.length);

            graph.Connect(currentPlayable, 0, mixer, 0);
            graph.Connect(currentOverlayPlayable, 0, mixer, 1);
            mixer.SetInputWeight(0, 1f);
            mixer.SetInputWeight(1, 1f);
            mixer.SetLayerAdditive(1, false);
            mixer.SetLayerMaskFromAvatarMask(1, BuildUpperBodyMask());

            currentRootPlayable = mixer;
            output.SetSourcePlayable(mixer);
        }

        private AnimationClip ClipFor(ModelState state)
        {
            switch (state)
            {
                case ModelState.Walk:
                    return walkClip;
                case ModelState.Run:
                    return rollClip ? rollClip : runClip;
                case ModelState.Attack:
                    return attackClip;
                case ModelState.ChargedAttack:
                    return chargedAttackClip ? chargedAttackClip : attackClip;
                case ModelState.Block:
                    return blockIdleClip;
                case ModelState.BlockMove:
                    return blockMoveClip ? blockMoveClip : walkClip;
                case ModelState.Jump:
                    return jumpClip;
                default:
                    return idleClip;
            }
        }

        private double SpeedFor(ModelState state, AnimationClip clip)
        {
            if (!clip) return 1.0;

            switch (state)
            {
                case ModelState.Attack:
                    return Mathf.Max(0.85f, clip.length / 0.66f);
                case ModelState.ChargedAttack:
                    float chargedDuration = controller ? controller.CurrentAttackDuration : chargedAttackVisualDuration;
                    return Mathf.Max(0.65f, clip.length / Mathf.Max(0.01f, chargedDuration));
                case ModelState.Run:
                    return clip == rollClip ? Mathf.Max(0.32f, clip.length / 1.15f) : 1.05f;
                case ModelState.Jump:
                    return clip == jumpClip ? Mathf.Max(0.7f, clip.length / 0.95f) : 1.0f;
                case ModelState.BlockMove:
                case ModelState.Walk:
                    return LocomotionPlaybackSpeed();
                default:
                    return 1.0;
            }
        }

        private double LocomotionPlaybackSpeed()
        {
            float planarSpeed = controller ? controller.PlanarSpeed : 0f;
            if (planarSpeed <= 0.08f) return 0.0;

            float scaledSpeed = planarSpeed / Mathf.Max(0.01f, referenceWalkSpeed);
            return Mathf.Clamp(scaledSpeed * walkPlaybackAtReferenceSpeed, 0f, maxLocomotionPlaybackSpeed);
        }

        private void UpdateCurrentPlaybackSpeed()
        {
            if (!currentPlayable.IsValid()) return;

            if (currentState == ModelState.Walk)
            {
                currentPlayable.SetSpeed(SpeedFor(ModelState.Walk, walkClip));
                return;
            }

            if (currentState == ModelState.BlockMove)
            {
                currentPlayable.SetSpeed(SpeedFor(ModelState.BlockMove, walkClip));
                if (currentOverlayPlayable.IsValid())
                {
                    currentOverlayPlayable.SetSpeed(1f);
                }
            }
        }

        private void KeepCurrentClipInRange()
        {
            if (!currentPlayable.IsValid()) return;

            if (currentState == ModelState.BlockMove)
            {
                LoopPlayable(currentPlayable, walkClip);
                LoopPlayable(currentOverlayPlayable, blockIdleClip);
                return;
            }

            AnimationClip clip = ClipFor(currentState);
            if (!clip || clip.length <= 0.001f) return;

            double time = currentPlayable.GetTime();
            bool shouldLoop = currentState != ModelState.Attack && currentState != ModelState.ChargedAttack && currentState != ModelState.Jump;
            if (shouldLoop && time >= clip.length)
            {
                currentPlayable.SetTime(time % clip.length);
            }
            else if (!shouldLoop && time > clip.length)
            {
                currentPlayable.SetTime(clip.length);
            }
        }

        private static void LoopPlayable(AnimationClipPlayable playable, AnimationClip clip)
        {
            if (!playable.IsValid() || !clip || clip.length <= 0.001f) return;

            double time = playable.GetTime();
            if (time >= clip.length)
            {
                playable.SetTime(time % clip.length);
            }
        }

        private void ClearCurrentPlayable()
        {
            if (currentRootPlayable.IsValid())
            {
                currentRootPlayable.Destroy();
            }

            currentRootPlayable = Playable.Null;
            currentPlayable = default;
            currentOverlayPlayable = default;
        }

        private static AnimationClip LoadClip(string fileName)
        {
            return LoadClip(AnimationPath, fileName);
        }

        private static AnimationClip LoadClip(string resourcePath, string fileName)
        {
            AnimationClip[] clips = Resources.LoadAll<AnimationClip>(resourcePath + fileName);
            if (clips.Length == 0)
            {
                Debug.LogWarning($"Dark Knight 3D animation clip not found at Resources/{resourcePath}{fileName}");
            }

            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] && !clips[i].name.StartsWith("__preview__", System.StringComparison.Ordinal))
                {
                    return clips[i];
                }
            }

            return clips.Length > 0 ? clips[0] : null;
        }

        private void CacheRootStabilizers()
        {
            animatedRoot = animator ? animator.transform : transform;
            hipsRoot = FindChildRecursive(transform, "mixamorig:Hips");

            visualBaseLocalPosition = transform.localPosition;
            visualBaseLocalRotation = transform.localRotation;
            animatedRootBaseLocalPosition = animatedRoot ? animatedRoot.localPosition : Vector3.zero;
            animatedRootBaseLocalRotation = animatedRoot ? animatedRoot.localRotation : Quaternion.identity;
            hipsBaseLocalPosition = hipsRoot ? hipsRoot.localPosition : Vector3.zero;
        }

        private void StabilizeAnimatedRoot()
        {
            transform.localPosition = visualBaseLocalPosition;
            transform.localRotation = visualBaseLocalRotation;

            if (animatedRoot && animatedRoot != transform)
            {
                animatedRoot.localPosition = animatedRootBaseLocalPosition;
                animatedRoot.localRotation = animatedRootBaseLocalRotation;
            }

            if (hipsRoot)
            {
                Vector3 hipsPosition = hipsRoot.localPosition;
                hipsPosition.x = hipsBaseLocalPosition.x;
                hipsPosition.z = hipsBaseLocalPosition.z;
                hipsRoot.localPosition = hipsPosition;
            }
        }

        private AvatarMask BuildUpperBodyMask()
        {
            AvatarMask mask = new AvatarMask();
            Transform spine = FindChildRecursive(transform, "mixamorig:Spine");
            if (spine)
            {
                mask.AddTransformPath(spine, true);
                return mask;
            }

            AddMaskPath(mask, "mixamorig:Spine1");
            AddMaskPath(mask, "mixamorig:Spine2");
            AddMaskPath(mask, "mixamorig:Neck");
            AddMaskPath(mask, "mixamorig:Head");
            AddMaskPath(mask, "mixamorig:LeftShoulder");
            AddMaskPath(mask, "mixamorig:RightShoulder");
            return mask;
        }

        private void AddMaskPath(AvatarMask mask, string boneName)
        {
            Transform bone = FindChildRecursive(transform, boneName);
            if (bone)
            {
                mask.AddTransformPath(bone, true);
            }
        }

        private void AttachStarterEquipment()
        {
            Transform rightHand = FindChildRecursive(transform, "mixamorig:RightHand");
            Transform leftForeArm = FindChildRecursive(transform, "mixamorig:LeftForeArm");
            Transform leftHand = FindChildRecursive(transform, "mixamorig:LeftHand");

            swordMaterial = NewMaterial("DK3D Starter Sword Steel", new Color(0.74f, 0.75f, 0.72f), 0.25f);
            shieldMaterial = NewMaterial("DK3D Starter Shield Iron", new Color(0.23f, 0.24f, 0.27f), 0.1f);
            shieldTrimMaterial = NewMaterial("DK3D Starter Shield Gold", new Color(0.86f, 0.63f, 0.24f), 0.2f);

            if (rightHand)
            {
                bool hasSword = FindChildRecursive(rightHand, "Heavy Broadsword") || FindChildRecursive(rightHand, "Starter Sword");
                if (!hasSword && !AttachEquipmentModel(rightHand, "sword_02_heavy_broadsword", "Heavy Broadsword", SwordGripOffset, SwordGripRotation, SwordGripScale))
                {
                    CreateStarterSword(rightHand);
                }

                swordEquipmentRoot = FindChildRecursive(rightHand, "Heavy Broadsword") ?? FindChildRecursive(rightHand, "Starter Sword");
            }
            else
            {
                Debug.LogWarning("Dark Knight 3D could not find mixamorig:RightHand for sword.");
            }

            Transform shieldBone = leftHand ? leftHand : leftForeArm;
            if (shieldBone)
            {
                bool hasShield = FindChildRecursive(shieldBone, "Dark Heater Shield") || FindChildRecursive(shieldBone, "Starter Shield");
                if (!hasShield && !AttachEquipmentModel(shieldBone, "shield_03_heater_dark", "Dark Heater Shield", ShieldGripOffset, ShieldGripRotation, ShieldGripScale))
                {
                    CreateStarterShield(shieldBone);
                }

                shieldEquipmentRoot = FindChildRecursive(shieldBone, "Dark Heater Shield") ?? FindChildRecursive(shieldBone, "Starter Shield");
            }
            else
            {
                Debug.LogWarning("Dark Knight 3D could not find left arm bone for shield.");
            }
        }

        private void UpdateEquipmentVisibility()
        {
            if (!inventory && controller) inventory = controller.GetComponent<PlayerInventory>();

            if (swordEquipmentRoot)
            {
                swordEquipmentRoot.gameObject.SetActive(inventory && !inventory.EquippedWeapon.IsEmpty);
            }

            if (shieldEquipmentRoot)
            {
                shieldEquipmentRoot.gameObject.SetActive(inventory && !inventory.EquippedShieldItem.IsEmpty);
            }
        }

        private void CreateStarterSword(Transform parent)
        {
            GameObject root = new GameObject("Starter Sword");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = SwordGripOffset;
            root.transform.localRotation = Quaternion.Euler(SwordGripRotation);
            root.transform.localScale = Vector3.one * SwordGripScale;

            GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blade.name = "Blade";
            blade.transform.SetParent(root.transform, false);
            blade.transform.localPosition = new Vector3(0f, 0.46f, 0f);
            blade.transform.localScale = new Vector3(0.055f, 0.88f, 0.035f);
            blade.GetComponent<Renderer>().sharedMaterial = swordMaterial;
            DestroySafely(blade.GetComponent<Collider>());

            GameObject guard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            guard.name = "Guard";
            guard.transform.SetParent(root.transform, false);
            guard.transform.localPosition = new Vector3(0f, 0.05f, 0f);
            guard.transform.localScale = new Vector3(0.36f, 0.06f, 0.055f);
            guard.GetComponent<Renderer>().sharedMaterial = shieldTrimMaterial;
            DestroySafely(guard.GetComponent<Collider>());

            GameObject grip = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            grip.name = "Grip";
            grip.transform.SetParent(root.transform, false);
            grip.transform.localPosition = new Vector3(0f, -0.09f, 0f);
            grip.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            grip.transform.localScale = new Vector3(0.035f, 0.18f, 0.035f);
            grip.GetComponent<Renderer>().sharedMaterial = shieldTrimMaterial;
            DestroySafely(grip.GetComponent<Collider>());
        }

        private bool AttachEquipmentModel(Transform parent, string resourceName, string objectName, Vector3 offset, Vector3 rotation, float scale)
        {
            GameObject prefab = Resources.Load<GameObject>(EquipmentPath + resourceName);
            if (!prefab)
            {
                Debug.LogWarning($"Dark Knight 3D equipment model not found at Resources/{EquipmentPath}{resourceName}");
                return false;
            }

            GameObject instance = Instantiate(prefab, parent);
            instance.name = objectName;
            instance.transform.localPosition = offset;
            instance.transform.localRotation = Quaternion.Euler(rotation);
            instance.transform.localScale = Vector3.one * scale;

            foreach (Collider collider in instance.GetComponentsInChildren<Collider>())
            {
                DestroySafely(collider);
            }

            foreach (Renderer renderer in instance.GetComponentsInChildren<Renderer>())
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = true;
            }

            Debug.Log($"Dark Knight 3D equipped {objectName}.");
            return true;
        }

        private void CreateStarterShield(Transform parent)
        {
            GameObject root = new GameObject("Starter Shield");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = ShieldGripOffset;
            root.transform.localRotation = Quaternion.Euler(ShieldGripRotation);
            root.transform.localScale = Vector3.one * ShieldGripScale;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            body.name = "Shield Body";
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = Vector3.zero;
            body.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            body.transform.localScale = new Vector3(0.38f, 0.035f, 0.5f);
            body.GetComponent<Renderer>().sharedMaterial = shieldMaterial;
            DestroySafely(body.GetComponent<Collider>());

            GameObject boss = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            boss.name = "Shield Boss";
            boss.transform.SetParent(root.transform, false);
            boss.transform.localPosition = new Vector3(0f, 0.04f, 0f);
            boss.transform.localScale = new Vector3(0.16f, 0.055f, 0.16f);
            boss.GetComponent<Renderer>().sharedMaterial = shieldTrimMaterial;
            DestroySafely(boss.GetComponent<Collider>());
        }

        private static Material NewMaterial(string name, Color color, float metallic)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.name = name;
            material.color = color;
            material.SetFloat("_Metallic", metallic);
            material.SetFloat("_Glossiness", 0.42f);
            return material;
        }

        private void ResetRuntimeGraph()
        {
            if (graph.IsValid())
            {
                graph.Destroy();
            }

            currentRootPlayable = Playable.Null;
            currentPlayable = default;
            currentOverlayPlayable = default;
            currentState = ModelState.None;
            observedAttackSequence = -1;
        }

        private static Transform FindChildRecursive(Transform root, string childName)
        {
            if (!root) return null;
            if (root.name == childName) return root;

            for (int i = 0; i < root.childCount; i++)
            {
                Transform found = FindChildRecursive(root.GetChild(i), childName);
                if (found) return found;
            }

            return null;
        }

        private static bool ClipLoaded(AnimationClip clip)
        {
            return clip != null;
        }

        private static void DestroySafely(Object target)
        {
            if (!target) return;

            if (Application.isPlaying) Object.Destroy(target);
            else Object.DestroyImmediate(target);
        }
    }
}
