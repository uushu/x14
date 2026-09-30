using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using GameMain;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FlyControl : MonoBehaviour
{
    private Vector3 endpos;
    private Vector3 startpos;
 
    private Vector3 vec1;
    private Vector3 vec2;
 
    public GameObject coin;
 
    //第一段位移
    public void FlyCoinOne()
    {
        startpos = transform.position;
        float random_x, random_y;
        random_x = Random.Range(-100, 100);
        random_y = Random.Range(10, 40);
        vec1 = startpos + new Vector3(random_x, random_y, 0);
        coin.transform.DOLocalMove(vec1, 0.3f).SetTarget(this);
    }
    //第二段位移
    public void FlyCoinTwo(Transform targetPos ,Action action)
    {
        endpos = targetPos.position;
        vec2 = endpos;
        coin.transform.DOMove(vec2, 0.6f).SetTarget(this);
        float timer1 = 0;
        Tween t1 = DOTween.To(() => timer1, x => timer1 = x, 1, 0.3f)
            .OnStepComplete(() =>
            {
                AudioMgr.Instance.PlayAudioOneShot(AudioName.jinbihuode);
                action?.Invoke();
                this.gameObject.GetComponent<Image>().DOFade(0, 0.3f).SetTarget(this);
            });
        float timer = 0;
        Tween t = DOTween.To(() => timer, x => timer = x, 1, 0.61f)
            .OnStepComplete(() =>
            {
                Destroy(this.gameObject);
            });
    }
}