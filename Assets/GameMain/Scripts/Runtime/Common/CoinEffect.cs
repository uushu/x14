using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CoinEffect : MonoBehaviour
{
    [Header("金币预制体")]
    public GameObject Coin;
    [Header("金币数量")]
    public int num;
    [Header("最终目的地")]
    public Transform targetPos;
    [Header("金币飞行速度")]
    public float time = 1;
    private int childnum = 0;
 
    private List<FlyControl> ts = new List<FlyControl>();
 
    public Action OnCoinEffectEnd;
    public Action OneCoinEffectEnd;
    public void StartEffect()
    {
        for (int i = 0; i < num; i++)
        {
            GameObject go = Instantiate(Coin, Vector3.zero, Quaternion.identity) as GameObject;
            go.SetActive(true);
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(0, 0, 0);
            go.transform.localScale = new Vector3(1, 1, 1);
            FlyControl cc = go.GetComponent<FlyControl>();
            if (cc != null)
            {
                ts.Add(cc);
            }
            cc.FlyCoinOne();
        }
    }
    IEnumerator StartEffect1()
    {
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i <= ts.Count; i++)
        {
            this.DOKill();
            yield return new WaitForSeconds(0.1f);
            var rm = Random.Range(0, ts.Count);
            yield return new WaitForSeconds(0.01f);
            ts[rm].FlyCoinTwo(targetPos,OneCoinEffectEnd);
            ts.Remove(ts[rm]);
            i = 0;
           
        }
        OnCoinEffectEnd?.Invoke();
    }
    /// <summary>
    /// 金币飞行效果，在Update里面调用
    /// </summary>
    public void CoinEffectShow(Action action=null,Action oneAction=null)
    {
        OnCoinEffectEnd = action;
        OneCoinEffectEnd = oneAction;
        //第一次飞行
        StartEffect();
        //第二次飞行
        StartCoroutine(StartEffect1());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            CoinEffectShow();
        }
    }
}