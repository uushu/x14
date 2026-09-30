using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameMain;
using Spine;
using UnityEngine;
using UnityEngine.UI;

public class LoadingWindow : ABaseWindow
{
    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public Spine.Unity.SkeletonGraphic mSkeletonGraphic;

    private TrackEntry _entry;

    public Image mImage;
    
    private bool isComplete;
    protected async override void OnShow( object parameters)
    {
        base.OnShow(parameters);
        var duration = (float)parameters;
        if (duration<0.1)
        {
            duration = 0.1f;
        }

        mImage.SetAlpha(1);
        mSkeletonGraphic.AnimationState.SetEmptyAnimation(0, 0);
        _entry = mSkeletonGraphic.AnimationState.SetAnimation(0, "start", false);
        _entry.Complete += OnAnimationComplete;
        isComplete = false;
       // await UniTask.WaitForSeconds(duration);
       // isComplete = true;
    }
    
    public async UniTask WaitAnimationComplete()
    {
        await UniTask.WaitUntil(() =>
        {
            //var isComplete = _entry.IsComplete;
            return isComplete;
        });
    }
    public void PlayCloseAni() 
    {
        isCloseEnd = false;
        _entry = mSkeletonGraphic.AnimationState.SetAnimation(0, "end", false);
        _entry.TimeScale = 1;
        mImage.DoAlpha(0, 0.4f);
        _entry.Complete += OnAnimationComplete;
    }
    private bool isOpenEnd;
    private bool isCloseEnd;
    private void OnAnimationComplete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "start")
        {
            isOpenEnd = true;
        }
        if (trackEntry.Animation.Name == "end")
        {
            isCloseEnd = true;
        }
        _entry.Complete -= OnAnimationComplete;
    }

    public async UniTask WaitAnimationOpenComplete()
    {
        await UniTask.WaitUntil(() =>
        {
            bool isComplete = _entry.IsComplete && isOpenEnd;
            return isComplete;
        });
    }
        
    public async UniTask WaitAnimationCloseComplete()
    {
        await UniTask.WaitUntil(() =>
        {
            bool isComplete = _entry.IsComplete && isCloseEnd;
            return isComplete;
        });
    }
}
