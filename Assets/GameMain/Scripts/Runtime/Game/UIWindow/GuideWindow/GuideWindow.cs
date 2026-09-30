using DG.Tweening;
using System;
using UnityEngine;

namespace GameMain.Garden
{
    public class GuideWindow : ABaseWindow
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Image mImgForbidenInputUI;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Image mImgFinger;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.GameObject mUseRefreshPop;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.GameObject mFreeUseBagPop;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.NewGuide.GuideMask mGuideMask;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Image mImgDragTarget;

        protected override void OnShow(object parameters)
        {
            base.OnShow(parameters);

            mGuideMask.Init(UIWindowMgr.Instance.uiCamera);
            mImgFinger.gameObject.SetActive(false);
            mUseRefreshPop.SetActive(false);
            mFreeUseBagPop.SetActive(false);
            mImgDragTarget.gameObject.SetActive(false);
        }

        public void ShowForbidenInputUI(bool isForbiden)
        {
            mImgForbidenInputUI.gameObject.SetActive(isForbiden);
        }

        public void SetGuideMaskTarget(RectTransform guideTarget)
        {
            mGuideMask.SetMaskTarget(guideTarget);
        }

        public void DoFingerDragAnimation(Vector3 startPosition, Vector3 endPosition, Vector2 sizeDelta)
        {
            ShowForbidenInputUI(false);
            mImgFinger.gameObject.SetActive(true);
            mImgFinger.GetComponent<Canvas>().overrideSorting = true;
            mImgFinger.transform.position = startPosition;
            var sequence = DOTween.Sequence();
            mImgFinger.SetAlpha(0f);
            var t1 = mImgFinger.DOFade(1, 0.2f);
            var t2 = mImgFinger.transform.DOMove(endPosition, 1.5f);
            var t3 = mImgFinger.DOFade(0, 0.2f);
            sequence.Append(t1);
            sequence.Join(t2);
            sequence.Append(t3);
            sequence.SetLoops(-1, LoopType.Restart).SetDelay(0.2f);

            //目标高亮
            mImgDragTarget.gameObject.SetActive(true);
            var sequence2 = DOTween.Sequence();
            var tt1 = mImgDragTarget.DOFade(0.2f, 1f);
            var tt2 = mImgDragTarget.DOFade(1f, 1f);
            sequence2.Append(tt1);
            sequence2.Append(tt2);
            sequence2.SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetTarget(mImgDragTarget.transform);

            mImgDragTarget.transform.position = endPosition;
            mImgDragTarget.GetComponent<RectTransform>().sizeDelta = sizeDelta;
        }
        
        public void DoFinterClickAnimation(Vector3 fingerPosition)
        {
            ShowForbidenInputUI(false);
            mImgFinger.gameObject.SetActive(true);
            mImgFinger.GetComponent<Canvas>().overrideSorting = false;
            mImgDragTarget.gameObject.SetActive(false);

            var screenPoint = RectTransformUtility.WorldToScreenPoint(UIWindowMgr.Instance.uiCamera, fingerPosition);
            var fingerParent = mImgFinger.transform.parent.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(fingerParent, screenPoint, UIWindowMgr.Instance.uiCamera, out var localPoint);
            mImgFinger.transform.localPosition = localPoint + new Vector2(100, -50);

            mImgFinger.transform.DOBlendableLocalMoveBy(new Vector3(-30, 30f), 0.9f).SetDelay(0.3f).SetLoops(-1, LoopType.Restart).SetTarget(mImgFinger.transform);
        }

        public void StopFingerAnimation()
        {
            mImgFinger.gameObject.SetActive(false);
            mImgFinger.transform.DOKill();
            mImgDragTarget.gameObject.SetActive(false);
            mImgDragTarget.transform.DOKill();
        }

        public void ShowUseRefreshPopDialog(RectTransform rect, Action btnClick)
        {
            ShowForbidenInputUI(false);
            mUseRefreshPop.SetActive(true);
            mGuideMask.SetMaskTarget(rect);
            mFreeUseBagPop.SetActive(false);
            StopFingerAnimation();

            rect.AddEventListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, (_) =>
            {
                mGuideMask.SetMaskTarget(null);
                mUseRefreshPop.SetActive(false);
                rect.RemoveEventListener();
                btnClick.Invoke();
            });
        }

        public void ShowFreeUsePopDialog(Action btnClick)
        {
            mGuideMask.SetMaskTarget(null);
            ShowForbidenInputUI(false);
            mFreeUseBagPop.SetActive(true);
            mUseRefreshPop.SetActive(false);
            StopFingerAnimation();

            var rect = mFreeUseBagPop.transform.Find("imgMask").GetComponent<RectTransform>();
            rect.AddEventListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, (_) =>
            {
                mFreeUseBagPop.SetActive(false);
                rect.RemoveEventListener();
                btnClick.Invoke();
            });
        }
    }
}