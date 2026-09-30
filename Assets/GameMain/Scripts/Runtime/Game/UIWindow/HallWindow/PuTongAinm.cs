using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PuTongAinm : MonoBehaviour
{
    public float MaxOvershoot=1.1f;
    public int MaxLoops=-1;
    public float Duration=1.5f;
    public float Delay=1f;
    public float EndScale=1f;
    public Ease EaseType =Ease.OutElastic;
    void Start()
    {
        transform.DOScale(new Vector3(EndScale,EndScale, EndScale), Duration).SetEase(EaseType,MaxOvershoot).SetDelay(Delay).SetLoops(MaxLoops, LoopType.Yoyo);
    }
}
