using UnityEngine;

namespace FrameLine
{
    public static class SimulateUtil
    {
        public static void Simulate(ParticleSystem particleSystem, float passTime)
        {
            particleSystem.Simulate(passTime, false, true);
        }

        public static void Simulate(GameObject obj, AnimationClip clip, float passTime)
        {
            clip.SampleAnimation(obj, passTime);
        }

        public static void Simulate(Animation animation, float passTime)
        {
            animation.clip.SampleAnimation(animation.gameObject, passTime);
        }

        public static void Simulate(Animator animator, float passTime)
        {
            AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
            for (int i = 0; i < clips.Length; i++)
            {
                clips[i].clip.SampleAnimation(animator.gameObject, passTime);
            }
        }
    }
}
