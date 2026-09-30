using cfg;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.FlowerLine
{
    public partial class FlowerLineMap : ILogicBase
    {
        [LabelText("六边形格子")]
        public UIHexagonGrids hexagonGrids;

        [LabelText("地图连线事件")]
        public DragLineListener mapListener;

        [LabelText("生成的起点格子")]
        public FlowerLineDropStartPoints dropPoints;

        [LabelText("格子根节点")]
        public Transform gridRoot;

        [LabelText("鲜花根节点")]
        public Transform flowerRoot;

        private ILineRefreshRule _refreshRule;

        private List<Vector2Int> _bloomGridsXY = new();

        public override void Initialize()
        {
            base.Initialize();

            mapListener.RemoveLineGridCallBack += OnRemoveGridXY;
        }

        private void OnRemoveGridXY(List<DragLineListener.LineGrid> flowers)
        {
            _bloomGridsXY.Clear();
            foreach (var item in flowers)
            {
                _bloomGridsXY.Add(item.GridXY);
            }

            BloomFlowers(_bloomGridsXY);
        }

        private async void BloomFlowers(List<Vector2Int> bloomGridsXY)
        {
            FlowerLineGameRoot.Instance.StartAnimation();

            for (int i = bloomGridsXY.Count - 1; i >= 0; i--)
            {
                var gridXY = bloomGridsXY[i];

                var flower = TryGetFlower(gridXY);
                flower.PlayBloomAnimation(() =>
                {
                    FlowerLineGameRoot.Instance.pool.multiPools.PushToPool(flower);
                });

                SetGridFlowerNull(gridXY, false);

                await UniTask.WaitForSeconds(0.12f);
            }

            await UniTask.WaitForSeconds(2f);
            FlowerLineGameRoot.Instance.StopAnimation();

            //生成新的格子
            GeneratorNewFlowers(bloomGridsXY.Count);
        }

        public async void StartGame(int levelId)
        {
            _refreshRule = new RefreshByWeight();
            _refreshRule.Initialize(levelId);

            await ReloadMap(levelId);

            //1.创建格子的数据
            Initizlie_MapData();

            await TryAddFlowerToMap(EnableGridCount);
        }

        private async UniTask ReloadMap(int levelId)
        {
            gridRoot.gameObject.DestroyAllChildren();

            var config = ConfigMgr.Instance.Tables.LineLevelCategory.GetOrDefault(levelId);
            if (config != null)
            {
                var gridInstance = await ResourceMgr.Instance.InstantiateGameObject(config.LevelPrefab, gridRoot, false);
                hexagonGrids = gridInstance.GetComponent<UIHexagonGrids>();
                hexagonGrids.name = config.LevelPrefab;
                mapListener.hexagonGrids = hexagonGrids;

                dropPoints = hexagonGrids.transform.GetComponentInChildren<FlowerLineDropStartPoints>();
            }
            else
            {
                Debugger.LogError($"不存在的鲜花连线关卡 levelId:{levelId}");
            }
        }

        public override void UnInitialize()
        {
            base.UnInitialize();

            mapListener.RemoveLineGridCallBack -= OnRemoveGridXY;
        }

#if UNITY_EDITOR

        [FoldoutGroup("编辑器")]
        [LabelText("测试的关卡Id")]
        public LineLevelId testLineLevelId;

        [FoldoutGroup("编辑器")]
        [Button("刷新测试连线关卡")]
        private void RefreshTestLine()
        {
            var category = AssetLoader.GetCategory_Editor<LineLevelCategory>();
            var config = category.GetOrDefault(testLineLevelId);

            if (config != null)
            {
                var prefab = AssetLoader.LoadGameObject(FlowerLineEditor.floderPath, config.LevelPrefab);

                if (prefab != null)
                {
                    gridRoot.gameObject.DestroyAllChildren();

                    var instance = (UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject);
                    hexagonGrids = instance.GetComponent<UIHexagonGrids>();
                    hexagonGrids.transform.SetParent(gridRoot, false);
                    hexagonGrids.name = config.LevelPrefab;

                    mapListener.hexagonGrids = hexagonGrids;

                    dropPoints = hexagonGrids.transform.GetComponentInChildren<FlowerLineDropStartPoints>();
                }
            }
        }

        [FoldoutGroup("编辑器")]
        [Button("测试连线引导")]
        private void GuideDragline()
        {
            var flowerGrids = GetAllFlowerGrids();
            flowerGrids.Sort(new GridDataSortBy_DescY());

            foreach (var item in flowerGrids)
            {
                var flower = item.flower;
                var linePath = GetCanDragLineFlowers(item.flower.currentGridXY, flower);
                var maxPath = GetMaxPath(linePath);

                if (maxPath.Count >= DragLineListener.MIN_LINE_GRID_COUNT)
                {
                    //绘制连线
                    foreach (var item3 in maxPath)
                    {
                        var flower2 = FlowerLineGameRoot.Instance.lineMap.TryGetFlower(item3);

                        if (flower2 == null)
                        {
                            Debug.LogError($"地图上不存在的花:{item3}");
                            continue;
                        }

                        mapListener.TryAddLineFlower(item3, flower2);
                    }

                    return;
                }
            }
            Debug.LogError("没有可以连线的线");
        }
#endif
    }
}