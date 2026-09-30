using GameMain.FlowerLine;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameMain
{
    public class DragLineListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public struct LineGrid
        {
            //格子的位置
            public Vector2Int GridXY;

            //格子高亮特效
            public GameObject GridEffect;

            //格子鲜花
            public Line_FlowerSingle FlowerSingle;
        }

        [LabelText("六边形格子")]
        public UIHexagonGrids hexagonGrids;

        [LabelText("顺序添加的花朵")]
        private List<LineGrid> _flowerAddQueue;

        public Action<List<LineGrid>> RemoveLineGridCallBack { get; set; }

        //最短连线格子的数量
        public const int MIN_LINE_GRID_COUNT = 3;

        private bool _isCanDragLine => FlowerLineGameRoot.Instance.IsForbidenInput == false;

        private void Start()
        {
            _flowerAddQueue = new();
        }

        public bool GetLineGrid(Vector2Int gridXY, out LineGrid lineGrid)
        {
            foreach (var item in _flowerAddQueue)
            {
                if (item.GridXY == gridXY)
                {
                    lineGrid = item;
                    return true;
                }
            }

            lineGrid = default;
            return false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isCanDragLine == false) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(hexagonGrids.gridsRoot,
                eventData.position, UIWindowMgr.Instance.uiCamera, out var localPoint);

            var gridXY = hexagonGrids.LocalPositionToGridXY(localPoint);
            var flower = FlowerLineGameRoot.Instance.lineMap.TryGetFlower(gridXY);

            if (flower != null)
            {
                if (TryAddLineFlower(gridXY, flower) == false)
                {
                    ClearAll();
                }

                if (_flowerAddQueue.Count >= 2)
                {
                    var gridXY2 = _flowerAddQueue[1];

                    if (gridXY2.GridXY == gridXY)
                    {
                        //移除
                        RemoveLineFlower(_flowerAddQueue[0].GridXY);
                    }
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isCanDragLine == false) return;

            ClearAll();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isCanDragLine == false) return;

            if (_flowerAddQueue.Count > 0)
            {
                if (_flowerAddQueue.Count >= MIN_LINE_GRID_COUNT)
                {
                    RemoveLineGridCallBack?.Invoke(_flowerAddQueue);
                }
                ClearAll();
            }
        }

        public bool TryAddLineFlower(Vector2Int addGridXY, Line_FlowerSingle flower)
        {
            var isOk = GetLineGrid(addGridXY, out var lineGrid);

            if (isOk) 
            {
                return true;
            }

            if (_flowerAddQueue.Count > 0)
            {
                var leastFlower = _flowerAddQueue[0];

                if (leastFlower.FlowerSingle.FlowerConfigId != flower.FlowerConfigId)
                {
                    //鲜花的类型不一致
                    return false;
                }

                //是否跳跃了格子
                var isAroundPoint = FlowerLineGameRoot.Instance.lineMap.IsAronudPoint(leastFlower.GridXY, addGridXY);
                if (isAroundPoint == false)
                {
                    return false;
                }
            }

            var gridLight = FlowerLineGameRoot.Instance.pool.gridLightPool.GetFromPoolSync().Item1;
            gridLight.transform.SetParent(flower.transform);
            gridLight.transform.localScale = Vector3.one;
            gridLight.transform.localPosition = Vector3.zero;

            var newGrid = new LineGrid()
            {
                GridXY = addGridXY,
                GridEffect = gridLight,
                FlowerSingle = flower,
            };

            _flowerAddQueue.Insert(0, newGrid);
            return true;
        }

        public void RemoveLineFlower(Vector2Int gridXY)
        {
            var isOk = GetLineGrid(gridXY, out var lineGrid);

            if (isOk == false) return;

            _flowerAddQueue.Remove(lineGrid);

            FlowerLineGameRoot.Instance.pool.gridLightPool.PushToPool(lineGrid.GridEffect);
        }

        public void ClearAll()
        {
            foreach (var item in _flowerAddQueue)
            {
                FlowerLineGameRoot.Instance.pool.gridLightPool.PushToPool(item.GridEffect);
            }
            _flowerAddQueue.Clear();
        }
    }
}