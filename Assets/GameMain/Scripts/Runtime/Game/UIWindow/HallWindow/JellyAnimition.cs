using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class JellyAnimition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DOTween.Sequence().Append(
            gameObject.transform.DOShakeScale(0.5f, 0.15f, 2, 20)
        ).AppendInterval(3f).SetLoops(-1, LoopType.Yoyo);
       // gameObject.transform.DOShakeScale (0.5f, 0.15f, 2, 20).SetLoops (99999, LoopType.Yoyo).SetDelay(3f);
    }

   
}
