using Cysharp.Threading.Tasks;
using GameMain.Garden;
using Sirenix.OdinInspector;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace GameMain
{
    public abstract class AEndlessScoreProgress : MonoBehaviour
    {
        public UICustomSlider progressSlider;

        public List<ItemUI_ScoreProgress> scoreProgress;

        [LabelText("当前分数")]
        public int currentScore;

        [LabelText("区间最小分数")]
        public int fromProgrss;

        [LabelText("区间最大分数")]
        public int toProgress;

        public ParticleSystem sliderHandlerGlow;

        public GameObject completeScoreRoot;

        public Animation completeScoreAnimation;

        public TextMeshProUGUI textCompleteScore;

        private int _lastScoreTarget;

        public SkeletonGraphic heartSpine;

        public void Initialize(int levelId)
        {
            currentScore = 0;
            ChangeScoreTarget(currentScore, 0);
            progressSlider.SetProgress(0, fromProgrss, toProgress);
            completeScoreRoot.SetActive(false);
        }

        protected virtual List<int> GetMyTargetScores(int currentScore, int lastStartScore, int maxCount)
        { 
            throw new NotImplementedException();
        }

        private void ChangeScoreTarget(int currentScore, int lastStartScore)
        {
            var scores = GetMyTargetScores(currentScore, lastStartScore, scoreProgress.Count);
            if (scores.Count <= 0)
            {
                Debugger.LogError("scores<=0");
                return;
            }

            var startScore = scores.FirstOrDefault();
            var endScore = scores.LastOrDefault();

            var totalLength = endScore - startScore;
            var uiWidth = progressSlider.sliderParent.sizeDelta.x;

            for (int i = 0; i < scoreProgress.Count; i++)
            {
                var currentLength = scores[i] - startScore;
                var percent = currentLength / (float)totalLength;

                var positionX = uiWidth * percent - uiWidth * 0.5f;
                scoreProgress[i].transform.localPosition = scoreProgress[i].transform.localPosition.SetX(positionX);
                scoreProgress[i].RefreshProgressItem(scores[i], currentScore);
            }

            fromProgrss = scores.FirstOrDefault();
            toProgress = scores.LastOrDefault();
        }

        private void RefreshProgress(int currentScore)
        {
            int completeScore = 0;
            foreach (var target in scoreProgress)
            {
                target.RefreshProgressItem(currentScore);

                if (currentScore >= target.ScoreTarget && _lastScoreTarget < target.ScoreTarget)
                {
                    completeScore = target.ScoreTarget;
                }
            }

            if (completeScore > 0)
            {
                _lastScoreTarget = completeScore;
                ShowCompleteScore(completeScore);
            }
        }

        public void AddScore(int addScore, bool heart, Action complete)
        {
            currentScore += addScore;

            sliderHandlerGlow.Play();
            float tweenSpeed = 60f;
            float maxTime = 2.5f;
            float minTime = 0.5f;

            if (currentScore >= toProgress)
            {
                var lastScore = progressSlider.DeltaLength - progressSlider.CurrentValue;
                var time1 = Mathf.Clamp(lastScore / tweenSpeed, minTime, maxTime);
                var remainScore = addScore - lastScore;
                var time2 = Mathf.Clamp(remainScore / tweenSpeed, minTime, maxTime);

                if (heart) PlayHeartSpine(time1 + time2);

                progressSlider.SetProgressTween(progressSlider.DeltaLength, fromProgrss, toProgress, time1, (value) =>
                {
                    RefreshProgress(Mathf.FloorToInt(value));
                }, () =>
                {
                    ChangeScoreTarget(currentScore, toProgress);
                    progressSlider.SetProgress(0, fromProgrss, toProgress);

                    progressSlider.SetProgressTween(remainScore, fromProgrss, toProgress, time2, (value2) =>
                    {
                        RefreshProgress(Mathf.FloorToInt(value2));
                    }, complete);
                });
            }
            else
            {
                var toValue = progressSlider.CurrentValue + addScore;
                var tweenTime = Mathf.Clamp(addScore / tweenSpeed, minTime, maxTime);
                if (heart) PlayHeartSpine(tweenTime);

                progressSlider.SetProgressTween(toValue, fromProgrss, toProgress, tweenTime, (value) =>
                {
                    RefreshProgress(Mathf.FloorToInt(value));
                }, complete);
            }
        }

        private async void PlayHeartSpine(float time)
        {
            heartSpine.AnimationState.SetAnimation(0, "idle2", false);
            heartSpine.AnimationState.GetCurrent(0).TimeScale = 1f;
            await UniTask.WaitForSeconds(time);
            heartSpine.AnimationState.SetAnimation(0, "idle", true);
            heartSpine.AnimationState.GetCurrent(0).TimeScale = 0.5f;
        }

        private async void ShowCompleteScore(int completeScore)
        {
            AudioMgr.Instance.PlayAudioOneShot(AudioName.CompleteGameAudio);
            completeScoreAnimation.Play("CompleteAnimation");
            completeScoreRoot.SetActive(true);
            textCompleteScore.text = UtilityTool.ShortNumberToString(completeScore);
            await UniTask.WaitForSeconds(2f);
            completeScoreRoot.gameObject.SetActive(false);
        }

#if UNITY_EDITOR

        [Button("测试加分数")]
        private void TestAddScore()
        {
            var randomNumber = UtilityTool.RandomNumber(777, 999);
            Debug.LogError(randomNumber);
            AddScore(randomNumber, true, null);
        }

        private void OnValidate()
        {
            scoreProgress = transform.GetComponentsInChildren<ItemUI_ScoreProgress>().ToList();
        }
#endif
    }
}