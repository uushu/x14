using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpineExtend
{
    public class SpineAnimationName
    {
        public const string Idle = "idle";
        public const string Cry = "cry";
        public const string Leisure = "Leisure";
        public const string Walk = "walk";
        public const string Win = "win";
    }

    [RequireComponent(typeof(SkeletonGraphic))]
    public class SpineAnimation : MonoBehaviour
    {
        SkeletonGraphic Graphic;
        public string idleAniamtion = SpineAnimationName.Idle;

        public List<string> randomAnimation = new List<string> {SpineAnimationName.Leisure};
        bool isRandomPlay = false;
        string lastAnimationName;

        /// <summary>
        /// 播放动画是否结束，用于判断是否可以播放下一个动画，不含常驻动画
        /// </summary>
        bool isPlayEnd = true;

        Vector2 RandomTime = new Vector2(10, 15);
        float needRandomTime;
        bool isStartRandom = false;
        bool needLoopIdle = true;
        private int SpineId;
        List<string> waitAnimation = new List<string> { };
        public Transform HeadPoint;

        void Awake()
        {
            HeadPoint = transform.Find("headPoint");
            Graphic = GetComponent<SkeletonGraphic>();
            Graphic.AnimationState.Complete += AnimationState_Complete;
        }

        private void Start()
        {
            //Graphic.AnimationState.SetAnimation(0, idleAniamtion, true);
        }

        private void AnimationState_Complete(Spine.TrackEntry trackEntry)
        {
            if (waitAnimation.Count > 0)
            {
                var aniName = waitAnimation[0];
                waitAnimation.RemoveAt(0);
                Graphic.AnimationState.SetAnimation(0, aniName, false);
                return;
            }

            if (lastAnimationName == trackEntry.Animation.Name)
            {
                lastAnimationName = string.Empty;
                isPlayEnd = true;
            }

            if (needLoopIdle)
                _PlayAnimation(idleAniamtion, true);
        }

        public void SetFlip(bool isFlipX = true)
        {
            if (Graphic != null)
            {
                Graphic.transform.localScale = new Vector3((isFlipX ? -1 : 1), Graphic.transform.localScale.y,
                    Graphic.transform.localScale.z);
            }
            else
            {
                Debug.Log($"Graphic is null:{gameObject.name}");
            }
        }

        public void PlayAnimation(string aniName, int playCount = 1, bool needPlaySound = false)
        {
            needLoopIdle = playCount > 0;
            isPlayEnd = false;
            lastAnimationName = aniName;
            _PlayAnimation(aniName, playCount < 0, needPlaySound);
            for (int i = 0; i < playCount - 1; i++)
                waitAnimation.Add(aniName);
        }

        string lastAniamtion;

        void _PlayAnimation(string aniName, bool loop = false, bool needPlaySound = false)
        {
            if (lastAniamtion != aniName)
            {
                if (needPlaySound)
                {
                    PlaySound(aniName);
                }
            }

            Graphic.AnimationState.SetAnimation(0, aniName, loop);
            lastAniamtion = aniName;
        }

        public void PlaySound(string aniName)
        {
            // var cfg = ConfigManager.GetSpineSoundsCfg(SpineId, aniName);
            // if (cfg != null)
            // {
            //     if (aniName != SpineAnimationName.Idle)
            //     {
            //         AudioManager.Instance.PlayRoleSound(cfg.sounds);
            //     }
            // }
            // else
            {
                Debug.LogError($"spineId: {SpineId}  aniName:{aniName}没有配置声音");
            }
        }

        public void PlayRandomAnimation()
        {
            isRandomPlay = true;
            isStartRandom = true;
            needRandomTime = 0;
        }

        public void StopRandomAnimation()
        {
            isRandomPlay = false;
        }

        void Update()
        {
            if (isRandomPlay && isStartRandom)
            {
                needRandomTime -= Time.deltaTime;
                if (needRandomTime <= 0)
                {
                    needRandomTime = 0;
                    isStartRandom = false;
                    PlayAnimation(randomAnimation[Random.Range(0, randomAnimation.Count)], 2);
                }
            }

            if (isRandomPlay && isPlayEnd && !isStartRandom)
            {
                needRandomTime = Random.Range(RandomTime.x, RandomTime.y);
                isStartRandom = true;
            }
        }

        internal void SetRoleId(int id)
        {
            SpineId = id;
        }
    }
}