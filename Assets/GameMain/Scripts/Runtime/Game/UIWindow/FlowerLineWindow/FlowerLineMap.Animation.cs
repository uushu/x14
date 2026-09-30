using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.FlowerLine
{
    public partial class FlowerLineMap
    {
        private const float _moveOneGridTime = 0.07f;

        private List<GridData> _allGrids = new();

        public Action OnAfterGeneratorFlowerCallBack { get; set; }

        private List<GridData> GetAllFlowerGrids()
        {
            _allGrids.Clear();

            foreach (var item in _xy2GridData)
            {
                if (item.Value.flower != null)
                {
                    _allGrids.Add(item.Value);
                }
            }

            return _allGrids;
        }

        private async UniTask AllGridTryMoveToNext()
        {
            var flowerGrids = GetAllFlowerGrids();
            flowerGrids.Sort(new GridDataSortBy_AsceY());

            foreach (var gridData in flowerGrids)
            {
                var currentGridXY = gridData.gridXY;

                if (CalcFlowerNextMovePoint(currentGridXY, out var nextPoint))
                {
                    MoveToNextGridXY(gridData.flower.currentGridXY, nextPoint, gridData.flower);
                }
            }

            await UniTask.WaitForSeconds(_moveOneGridTime);
        }

        private void MoveToNextGridXY(Vector2Int currentGridXY, Vector2Int nextGridXY, Line_FlowerSingle currentFlower)
        {
            var nextLocalPoint = hexagonGrids.GridXYToLocalPosition(nextGridXY);

            if (_xy2GridData.TryGetValue(currentGridXY, out var gridData))
            {
                gridData.flower = null;
            }

            _xy2GridData[nextGridXY].flower = currentFlower;
            currentFlower.DoMoveToNextGrid(nextGridXY, nextLocalPoint, _moveOneGridTime);
        }

        private async UniTask TryAddFlowerToMap(int addFlowerCount)
        {
            while (addFlowerCount > 0)
            {
                AllGridTryMoveToNext().Forget();

                var startXYPoints = dropPoints.GetStartDropPoints;

                //添加新的格子
                for (int i = 0; i < startXYPoints.Count; i++)
                {
                    var startPoint = startXYPoints[i];

                    var isOk = CalcFlowerNextMovePoint(startPoint, out var nextPoint);

                    if (isOk)
                    {
                        _refreshRule.Refresh(out var flowerId, out var levelId);
                        var flowerConfig = ConfigMgr.Instance.Tables.LineFlowerTypeCategory.GetOrDefault(flowerId);
                        var flowerSingle = await FlowerLineGameRoot.Instance.pool.multiPools.GetEntityAsync(flowerConfig.LineSpinePrefab);
                        flowerSingle.transform.SetParent(flowerRoot);
                        flowerSingle.transform.localPosition = dropPoints.dropPoints[i].transform.localPosition;
                        flowerSingle.Initialize_Flower(flowerConfig, false);

                        //添加新的格子
                        MoveToNextGridXY(Line_FlowerSingle.InValidGridXY, nextPoint, flowerSingle);
                        addFlowerCount--;

                        if (addFlowerCount <= 0) break;
                    }
                }

                await UniTask.WaitForSeconds(_moveOneGridTime);
            }

            OnAfterGeneratoeFlowers();
        }

        private void OnAfterGeneratoeFlowers()
        {
            //判断游戏是否失败
            if (GameIsFailed())
            {
                Debugger.LogError("游戏格子没法消除!!!");
            }

            OnAfterGeneratorFlowerCallBack?.Invoke();
        }
    }
}