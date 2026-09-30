using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

namespace GameMain
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIBaseFloating : MonoBehaviour
    {
        public CanvasGroup canvasGroup;

        [LabelText("延迟移动时间")]
        public float delayMoveTime;

        [LabelText("动画飞行时间")]
        public float animationTime = 1;

        [LabelText("移动&渐隐")]
        public bool moveAndFadeOut = false;

        [LabelText("渐隐时间")]
        public float fadeOutTime = 1;

        [LabelText("飞行高度")]
        public float moveHeight = 30;

        public void StartFloating()
        {
            DelayTime();
        }

        private async void DelayTime()
        {
            canvasGroup.alpha = 1;

            await UniTask.Delay(TimeSpan.FromSeconds(delayMoveTime));

            var t1 = transform.DOBlendableLocalMoveBy(new Vector3(0, moveHeight, 0), animationTime).SetEase(Ease.OutQuad).ToUniTask();

            var t2 = DOTween.To(() => canvasGroup.alpha, (v) => canvasGroup.alpha = v, 0, fadeOutTime).ToUniTask();

            if (moveAndFadeOut)
            {
                await UniTask.WhenAll(t1, t2);
            }
            else
            {
                await t1;

                await t2;
            }

            gameObject.gameObject.SetActive(false);
        }
    }
}