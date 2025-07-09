using Spine.Unity;
using UnityEngine;

public class DotControl : MonoBehaviour
{
    [SerializeField] SkeletonAnimation skeletonAnimation;
    void Start()
    {
        skeletonAnimation.AnimationState.ClearTrack(0);
    }
    public void OnColectionEnter()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, "animation", false);
    }
}
