using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public abstract class AGameLongTimeNotOperation : MonoBehaviour
    {
        [LabelText("未操作的时长")]
        public float notOperationTime;

        [LabelText("提示的按钮")]
        public List<RectTransform> tipRects;

        [LabelText("当前未操作的累计时间")]
        public float _timer;

        protected abstract bool _executeUpdate { get; }

        public virtual void Initialize()
        {
            ResetTimer();
        }

        public virtual void UnInitialize()
        {
        }

        protected virtual void ResetTimer()
        {
            _timer = 0;

            foreach (var rect in tipRects)
            {
                rect.transform.DOKill();
                rect.transform.localScale = Vector3.one;
            }
        }

        protected void RandomShowTip()
        {
            if (tipRects.Count <= 0)
            {
                Debugger.LogError("未配置提示的按钮");
                return;
            }

            var randomRect = UtilityTool.RandomNumber(tipRects);

            var sequence = DOTween.Sequence(randomRect);
            randomRect.transform.localScale = Vector3.one;
            var t1 = randomRect.transform.DOScale(Vector3.one * 1.3f, 1f).SetEase(Ease.Linear);
            var t2 = randomRect.transform.DOScale(Vector3.one, 1f).SetEase(Ease.Linear);

            sequence.Append(t1);
            sequence.Append(t2);
            sequence.SetLoops(3, LoopType.Restart);
        }

        protected virtual void TriggerTip()
        {
            RandomShowTip();
        }

        private void Update()
        {
            if (_executeUpdate)
            {
                _timer += Time.deltaTime;

                if (_timer >= notOperationTime)
                {
                    _timer = 0;
                    TriggerTip();
                }
            }
        }

#if UNITY_EDITOR

        [Button("测试长时间未激活")]
        private void TestLongTimeNotOperation()
        {
            _timer = notOperationTime;
        }
#endif
    }
}