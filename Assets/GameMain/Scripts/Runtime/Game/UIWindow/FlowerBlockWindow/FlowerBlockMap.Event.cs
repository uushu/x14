using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameMain.FlowerBlock
{
    public partial class FlowerBlockMap
    {
        public void Initialize_Event() { }

        public void UnInitialize_Event() { }

        //记录按下时的格子位置
        private Vector2Int _downGridXY;

        private void GetHeadGridXY(ItemUI_FlowerBlock blockItem, out Vector2Int headGridXY, out Vector3 headPoint,out float xOffset)
        {
            //坐标系要一致 再同一个父节点下&&父节点位置=0
            //中心点
            var localCenterPoint = blockItem.transform.localPosition;
            //计算头部的点
            var gridDataCount = blockItem.BlockGridTypeConfig.GridData.Count;
            xOffset = (gridDataCount - 1) * 0.5f * uicustomGrids.gridWidth;
            headPoint = new Vector3(localCenterPoint.x - xOffset, localCenterPoint.y, 0);

            headGridXY = uicustomGrids.LocalPosToGridXY(headPoint);
        }

        private void ShowDragImage(int colCount, float positionX)
        {
            dragImage.gameObject.SetActive(true);
            dragImage.sizeDelta = new Vector2(uicustomGrids.gridWidth * colCount, dragImage.sizeDelta.y);
            dragImage.transform.localPosition = new Vector3(positionX, dragImage.transform.localPosition.y, 0);
        }

        private void HideDragImage()
        {
            dragImage.gameObject.SetActive(false);
        }

        public void OnDrag(ItemUI_FlowerBlock blockItem, PointerEventData eventData)
        {
            GetHeadGridXY(blockItem, out var headGridXY, out var headPoint, out var xOffset);

            (var minx, var maxX) = GetBlockItemClampX(blockItem);

            //限制 X 在格子范围内
            var leftPoint = uicustomGrids.GridXYToLocalPosition(new Vector2Int(minx, headGridXY.y));
            var rightPoint = uicustomGrids.GridXYToLocalPosition(new Vector2Int(maxX, headGridXY.y));

            headPoint.x = Mathf.Clamp(headPoint.x, leftPoint.x, rightPoint.x);
            headPoint.x += xOffset;

            blockItem.transform.localPosition = headPoint;
            ShowDragImage(blockItem.flowers.Count, blockItem.transform.localPosition.x);

            if (eventData.delta.x > 0)
            {
                blockItem.Flower_PlayMove(EFlowerBlockMoveDirection.Right);
            }
            else if (eventData.delta.x < 0)
            {
                blockItem.Flower_PlayMove(EFlowerBlockMoveDirection.Left);
            }
        }

        public void OnPointerDown(ItemUI_FlowerBlock blockItem, PointerEventData eventData)
        {
            GetHeadGridXY(blockItem, out var headGridXY, out var headPoint, out var xOffset);
            _downGridXY = headGridXY;
        }

        public void OnPointerUp(ItemUI_FlowerBlock blockItem, PointerEventData eventData)
        {
            HideDragImage();

            GetHeadGridXY(blockItem, out var headGridXY, out var headPoint, out var xOffset);
            var gridXY = headGridXY;
            var gridDataCount = blockItem.BlockGridTypeConfig.GridData.Count;

            //限制 X 在格子范围内
            gridXY.x = Mathf.Clamp(gridXY.x, 0, uicustomGrids.colCount - gridDataCount);

            var localPoint = uicustomGrids.GridXYToLocalPosition(gridXY);
            blockItem.transform.localPosition = new Vector3(localPoint.x + xOffset, localPoint.y, 0);

            if (_downGridXY != gridXY)
            {
                //消除逻辑
                BlockGameRoot.Instance.StartAnimation();
                BlockGameRoot.Instance.OnBeforeDragFlowerBlockItemCallback?.Invoke();
                TryEliminateBlock(true, () =>
                {
                    BlockGameRoot.Instance.StopAnimation();
                    BlockGameRoot.Instance.OnAfterDragFlowerBlockItemCallback?.Invoke();
                });
            }
            else
            {
                AudioMgr.Instance.PlayAudioOneShot(AudioName.Block_FlowerDrag);
            }
        }
    }
}