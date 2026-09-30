using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ShakeAni : MonoBehaviour
{
    public RectTransform btnImage;
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
            .Append(btnImage.DORotate(new Vector3(0,0,10),0.1f))
            .Append(btnImage.DORotate(new Vector3(0,0,-10),0.1f))
            .Append(btnImage.DORotate(new Vector3(0,0,0),0.1f))
            .AppendInterval(0.5f)
            .SetLoops(-1)
            .SetEase(Ease.Linear)
            .SetTarget(this);
    }

    public void CloseAni()
    {
        btnImage.localPosition =new Vector3(36.6f, 36.6f, 0);
        btnImage.localRotation = Quaternion.identity;
        DOTween.Kill(this);
    }
}

