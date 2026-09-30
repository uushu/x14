using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DoShakeRotation : MonoBehaviour
{
    private Tween tween;

    public void PlayAnim()
    {
        if (tween!=null)
        {
            return;
        }
        tween = DOTween.Sequence()
            .AppendInterval(2f)
            .Append(gameObject.transform.DOShakeRotation(1.6f, new Vector3(0, 0, 10), 5, 5, false,
                ShakeRandomnessMode.Harmonic))
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    public void StopAnim()
    {
        tween.Kill();
        tween=null;
    }
}