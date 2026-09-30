using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ShowTips : MonoBehaviour
{
    public TextMeshProUGUI numText;
    public void InitShow(Vector2 localPos,int count,Action overCallBack = null)
    {
        transform.localPosition = localPos;
        numText.text = $"-{count}";
        
        DOTween.Sequence()
            .Append(transform.DOLocalMoveY(localPos.y + 120, 0.5f))
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                overCallBack?.Invoke();
                Destroy(gameObject); 
            });
    }
}
