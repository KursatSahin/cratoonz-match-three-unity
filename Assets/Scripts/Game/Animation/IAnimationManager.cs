using Core.Services;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Game.Animation
{
    public interface IAnimationManager : IService
    {
        /// <summary>
        /// Returns the current state of the animation manager
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// Plays tween in animation queue until it is finished
        /// </summary>
        void Play();

        /// <summary>
        /// Resets the animation manager sequences and the main sequence
        /// </summary>
        void Reset();

        /// <summary>
        /// Cancels the current sequence (Sequence will be completed immediately)
        /// </summary>
        UniTask Cancel();

        /// <summary>
        /// Waits for the animations complete
        /// </summary>
        UniTask Wait();

        /// <summary>
        /// Appends a tween into swap sequence with starting time
        /// </summary>
        /// <param name="animGroup"></param>
        /// <param name="tween">Tween</param>
        /// <param name="startingTime"></param>
        void Enqueue(AnimGroup animGroup, Tween tween, float startingTime = 0f);
    }

    public enum AnimGroup
    {
        Swap = 100,
        Rollback = 200,
        Destroy = 300,
        Gravity = 400,
    }
}