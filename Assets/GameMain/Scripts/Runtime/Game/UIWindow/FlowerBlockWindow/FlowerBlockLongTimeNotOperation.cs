using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain.FlowerBlock
{
    public class FlowerBlockLongTimeNotOperation : AGameLongTimeNotOperation
    {
        protected override bool _executeUpdate => BlockGameRoot.Instance.currentStatus == EStatus.Gaming;

        public Image imgArrow;

        public RectTransform imgFrame;

        public override void Initialize()
        {
            base.Initialize();
            BlockGameRoot.Instance.OnBeforeDragFlowerBlockItemCallback += OnMoveFlowerBlockItem;

            imgFrame.gameObject.SetActive(false);
        }

        private void OnMoveFlowerBlockItem()
        {
            ResetTimer();
        }

        private void ShowArrow(HintResult result)
        {
            imgFrame.gameObject.SetActive(true);

            var sizeDelta = result.HintBlockItemUI.GetComponent<RectTransform>().sizeDelta;
            var gridWidth = BlockGameRoot.Instance.blockMap.uicustomGrids.gridWidth;
            var gridSpaceX = BlockGameRoot.Instance.blockMap.uicustomGrids.colSpace;
            var extendSizeX = (gridWidth + gridSpaceX) * Mathf.Abs(result.MoveStepCount);

            //计算边框的宽高
            imgFrame.sizeDelta = new Vector2(extendSizeX + sizeDelta.x + 4f, sizeDelta.y);

            //计算边框的位置
            var frameLocalPosition = imgFrame.transform.parent.InverseTransformPoint(result.HintBlockItemUI.transform.position);
            var offsetLocalX = result.MoveStepCount > 0 ? extendSizeX * 0.5f : -extendSizeX * 0.5f;

            imgFrame.transform.localPosition = frameLocalPosition + new Vector3(offsetLocalX, 0, 0);

            //设置箭头的位置
            if (result.MoveStepCount > 0)
            {
                imgArrow.transform.localScale = Vector3.one;

                var lastFlower = result.HintBlockItemUI.flowers.LastOrDefault();
                var arrowLocalPosition = imgArrow.transform.parent.InverseTransformPoint(lastFlower.transform.position);
                arrowLocalPosition += new Vector3(extendSizeX, 0, 0);
                imgArrow.transform.localPosition = arrowLocalPosition;
            }
            else
            {
                imgArrow.transform.localScale = new Vector3(-1, 1, 1);
                var firstFlower = result.HintBlockItemUI.flowers.FirstOrDefault();
                var arrowLocalPosition = imgArrow.transform.parent.InverseTransformPoint(firstFlower.transform.position);
                arrowLocalPosition -= new Vector3(extendSizeX, 0, 0);
                imgArrow.transform.localPosition = arrowLocalPosition;
            }

            imgArrow.DOKill();
            imgArrow.SetAlpha(0);
            imgArrow.DOFade(1, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).SetTarget(imgArrow);
        }

        protected override void ResetTimer()
        {
            base.ResetTimer();
            imgArrow.DOKill();
            imgFrame.gameObject.SetActive(false);
        }

        protected override void TriggerTip()
        {
            var result = BlockGameRoot.Instance.blockMap.CalcFlowerBlockHintResult();
            if (result != null)
            {
                ShowArrow(result);
            }
            else
            {
                base.TriggerTip();
            }
        }

        public override void UnInitialize()
        {
            base.UnInitialize();
            BlockGameRoot.Instance.OnBeforeDragFlowerBlockItemCallback -= OnMoveFlowerBlockItem;
        }
    }
}