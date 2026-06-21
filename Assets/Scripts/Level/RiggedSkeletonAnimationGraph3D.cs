using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace DungeonKnight.Level
{
    internal sealed class RiggedSkeletonAnimationGraph3D
    {
        private const int WalkInput = 0;
        private const int AttackInput = 1;

        private readonly float attackPlaybackSpeed;
        private readonly float walkPlaybackSpeed;
        private readonly AnimationClip attackClip;
        private readonly AnimationClip walkClip;
        private PlayableGraph graph;
        private AnimationMixerPlayable mixer;
        private AnimationClipPlayable attackPlayable;
        private AnimationClipPlayable walkPlayable;
        private float attackUntil;
        private bool moving;

        public RiggedSkeletonAnimationGraph3D(
            string graphName,
            Animator animator,
            string attackResourcePath,
            string walkResourcePath,
            float attackPlaybackSpeed,
            float walkPlaybackSpeed)
        {
            this.attackPlaybackSpeed = attackPlaybackSpeed;
            this.walkPlaybackSpeed = walkPlaybackSpeed;
            attackClip = ChooseClip(Resources.LoadAll<AnimationClip>(attackResourcePath));
            walkClip = ChooseClip(Resources.LoadAll<AnimationClip>(walkResourcePath));
            if (walkClip) walkClip.wrapMode = WrapMode.Loop;
            if (!attackClip && !walkClip) return;

            graph = PlayableGraph.Create(graphName);
            graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            mixer = AnimationMixerPlayable.Create(graph, 2);
            AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "Rigged Skeleton Animation", animator);
            output.SetSourcePlayable(mixer);

            if (walkClip)
            {
                walkPlayable = AnimationClipPlayable.Create(graph, walkClip);
                graph.Connect(walkPlayable, 0, mixer, WalkInput);
                mixer.SetInputWeight(WalkInput, 1f);
                walkPlayable.SetSpeed(0d);
            }

            if (attackClip)
            {
                attackPlayable = AnimationClipPlayable.Create(graph, attackClip);
                graph.Connect(attackPlayable, 0, mixer, AttackInput);
                mixer.SetInputWeight(AttackInput, 0f);
                attackPlayable.SetSpeed(0d);
            }

            graph.Play();
        }

        public bool IsAttackActive => graph.IsValid() && attackPlayable.IsValid() && Time.time < attackUntil && attackPlayable.GetSpeed() > 0d;

        public void SetMovement(bool value)
        {
            moving = value;
            if (!IsAttackActive)
            {
                UpdateLocomotionPlayable();
            }
        }

        public float PlayAttack()
        {
            if (!attackClip || !graph.IsValid()) return 0f;

            attackPlayable.SetTime(0d);
            attackPlayable.SetSpeed(attackPlaybackSpeed);
            float duration = attackClip.length / Mathf.Max(0.01f, attackPlaybackSpeed);
            attackUntil = Time.time + duration;
            if (mixer.IsValid())
            {
                mixer.SetInputWeight(WalkInput, 0f);
                mixer.SetInputWeight(AttackInput, 1f);
            }

            if (walkPlayable.IsValid())
            {
                walkPlayable.SetSpeed(0d);
            }

            return duration;
        }

        public void Tick()
        {
            if (graph.IsValid() && Time.time >= attackUntil && attackPlayable.IsValid() && attackPlayable.GetSpeed() > 0d)
            {
                attackPlayable.SetSpeed(0d);
                attackPlayable.SetTime(0d);
                UpdateLocomotionPlayable();
            }

            LoopWalkPlayable();
        }

        public void Destroy()
        {
            if (graph.IsValid()) graph.Destroy();
        }

        private static AnimationClip ChooseClip(AnimationClip[] clips)
        {
            if (clips == null) return null;

            AnimationClip fallback = null;
            foreach (AnimationClip clip in clips)
            {
                if (!clip || clip.length <= 0.05f) continue;
                if (!clip.name.StartsWith("__preview__", System.StringComparison.OrdinalIgnoreCase)) return clip;
                fallback ??= clip;
            }

            return fallback;
        }

        private void UpdateLocomotionPlayable()
        {
            if (!graph.IsValid() || !mixer.IsValid() || !walkPlayable.IsValid()) return;

            mixer.SetInputWeight(WalkInput, 1f);
            mixer.SetInputWeight(AttackInput, 0f);
            walkPlayable.SetSpeed(moving ? walkPlaybackSpeed : 0d);
            if (!moving)
            {
                walkPlayable.SetTime(0d);
            }
        }

        private void LoopWalkPlayable()
        {
            if (!moving || !walkClip || !walkPlayable.IsValid() || walkClip.length <= 0.01f) return;

            double time = walkPlayable.GetTime();
            if (time >= walkClip.length)
            {
                walkPlayable.SetTime(time % walkClip.length);
            }
        }
    }
}
