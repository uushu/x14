using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpineAniChange : MonoBehaviour
{
    public SkeletonGraphic spine;

    public string loopAniName = "idle";
    public List<string> randomAni = new List<string> {"loop"};
    //随机间隔时间
    public Vector2 intervalRandomTime = new Vector2(5, 12);
    //需要随机播放目标动画
    private bool needRandomPlay;
    private string lastAniName;

    bool needLoopIdle = true;
    bool startTimeDown;
    //播放动画是否结束，用于判断是否可以播放下一个动画，不含常驻动画
    private bool isPlayEnd = true;
    //间隔时间
    float intervalTime;
    private readonly List<string> waitAni = new List<string> { };

    private void Awake()
    {
        spine.startingLoop = false;
        spine.AnimationState.Complete += AniComplete;
    }

    private void Start()
    {
        needRandomPlay = randomAni.Count > 0;
        startTimeDown = false;
        PlayAnimation(randomAni[Random.Range(0, randomAni.Count)], 1);
    }
    
    public void PlayAnimation(string aniName, int playCount = 1)
    {
        needLoopIdle = playCount > 0;
        isPlayEnd = false;
        lastAniName = aniName;
        spine.AnimationState.SetAnimation(0, aniName, playCount < 0);
        for (int i = 0; i < playCount - 1; i++)
        {
            waitAni.Add(aniName);
        }
    }
    
    private void AniComplete(Spine.TrackEntry trackEntry)
    {
        if (waitAni.Count > 0)
        {
            var aniName = waitAni[0];
            waitAni.RemoveAt(0);
            spine.AnimationState.SetAnimation(0, aniName, false);
            return;
        }

        if (lastAniName == trackEntry.Animation.Name)
        {
            lastAniName = string.Empty;
            isPlayEnd = true;
        }

        if (needLoopIdle)
        {
            spine.AnimationState.SetAnimation(0, loopAniName, true);
        }
    }
    
    private void Update()
    {
        if (needRandomPlay && startTimeDown)
        {
            intervalTime -= Time.deltaTime;
            if (intervalTime <= 0)
            {
                intervalTime = 0;
                startTimeDown = false;
                PlayAnimation(randomAni[Random.Range(0, randomAni.Count)], 1);
            }
        }

        if (needRandomPlay && isPlayEnd && !startTimeDown)
        {
            intervalTime = Random.Range(intervalRandomTime.x, intervalRandomTime.y);
            startTimeDown = true;
        }
    }
}