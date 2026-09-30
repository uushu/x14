using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAni : MonoBehaviour
{
    public Vector3 StartV3;
    public Vector3 EndV3;
    public float DelayTime=1;
    public RectTransform transform;
    public bool isDefaultPlay;
    public void Start()
    {
        if (isDefaultPlay)
        {
            ShowAni();
        }
    }

    public void ShowAni()
    {
        DOTween.Sequence()
            .Append(transform.DOLocalMove(EndV3, DelayTime))
            .SetLoops(-1,LoopType.Yoyo)
            .SetEase(Ease.Linear)
            .SetTarget(this);
    }

    public void CloseAni()
    {
        transform.localPosition = StartV3;
        transform.localRotation = Quaternion.identity;
        DOTween.Kill(this);
    }
}
