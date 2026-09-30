using cfg;
using GameMain;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace GameMain.Garden.Editor
{
#if UNITY_EDITOR

    public partial class GardenMapEditor : MonoBehaviour
    {
        [FoldoutGroup("编辑器")]
        [LabelText("自定义格子")]
        public UICustomGrids customGrids;

        [FoldoutGroup("编辑器")]
        [LabelText("俄罗斯花园ID")]
        public GardenLevelId gardenLevelId;

        [FoldoutGroup("编辑器")]
        [LabelText("鲜花的根节点")]
        public RectTransform flowerRoot;

        [FoldoutGroup("编辑器")]
        [LabelText("鲜花预制体")]
        public GameObject flowerPrefab;

        [FoldoutGroup("编辑器")]
        [LabelText("取出花朵的数量")]
        [Range(0, 20)]
        public int maxTakeOutGridCount;

        private static GardenFlowerGridTypeCategory _gardenGridTypeCategory;
        public static GardenFlowerGridTypeCategory GardenGridTypeCategory
        {
            get
            {
                if (_gardenGridTypeCategory == null)
                {
                    _gardenGridTypeCategory = AssetLoader.GetCategory_Editor<GardenFlowerGridTypeCategory>();
                }

                return _gardenGridTypeCategory;
            }
        }

        public static GardenLevelCategory GardenLevelCategory => AssetLoader.GetCategory_Editor<GardenLevelCategory>();

        private Dictionary<Vector2Int, GameObject> _gridXY2Obj = new();

        private GardenEditorRefresh _refresh;

        private Dictionary<int, List<GardenFlowerGridTypeConfig>> _takeOutGrids = new();

        private List<Vector2Int> _mapGridsXYCache = new();

        private List<Vector2Int> _runtimeGridsXY = new();
        private List<int> _canUseGridsTypeIds = new();

        //成功取出的格子
        private List<GardenEditorGridData> _successTakeOutGrids = new();
        //剩下的地图初始格子
        private List<GardenEditorGridData> _initGrids = new();

        //一波刷新3个格子
        private int _oneWaveRefreshCount = 3;

        private void OnCreateGridObj(Vector2Int gridXY, GameObject gridObj)
        {
            gridObj.transform.GetComponentInChildren<TextMeshProUGUI>().text = $"{gridXY.x},{gridXY.y}";
        }

        private void NewRefreshRule()
        {
            customGrids.OnCreateGridObj = OnCreateGridObj;

            if (_refresh == null || _refresh.LevelId != gardenLevelId)
            {
                //随机抽出格子的形状
                var levelConfig = GardenLevelCategory.GetOrDefault(gardenLevelId);

                if (levelConfig == null)
                {
                    Debug.LogError($"不存在的关卡配置 gardenLevelId:{gardenLevelId}");
                    return;
                }

                _refresh = new(levelConfig);
            }
        }

        [FoldoutGroup("编辑器")]
        [GUIColor("red")]
        [Button("随机生成初始格子", ButtonSizes.Large)]
        private void RandomGeneratorGrids()
        {
            NewRefreshRule();

            bool isSuccess = false;
            int maxFailedCount = 10;
            for (int i = 0; i < maxFailedCount; i++)
            {
                InitMap();

                //取出的格子
                var takeOutGrids = GetTakeOutFlowerGrids();
                isSuccess = StartTakeOutFlowerFromMap(takeOutGrids);

                if (isSuccess)
                {
                    break;
                }
            }

            if (isSuccess == false)
            {
                Debug.LogError($"生成格子失败 gardenLevelId:{gardenLevelId}");
                return;
            }

            CopySuccessTakeOutGridsConfig();

            //地图上的格子 转换成 格子类型
            var initGrids = MapGridExchangeToFlowerGrid(true);
            CopyInitGridsConfig();

            //生成剩下的格子
            CreateInitGrids(initGrids);
        }

        [FoldoutGroup("编辑器")]
        [GUIColor("red")]
        [Button("还原当前关卡的数据", ButtonSizes.Large)]
        private void OnGardenLevelChange()
        {
            var levelConfig = GardenLevelCategory.Get(gardenLevelId);

            var stringBuilder = new StringBuilder();

            //初始地图的数据
            _initGrids.Clear();
            foreach (var item in levelConfig.InitMapGridType)
            {
                stringBuilder.Append($"{item[0]},{item[1]},{item[2]}|");

                var gridTypeId = item[0];
                var startXY = new Vector2Int(item[1], item[2]);
                var gridTypeConfig = GardenEditorRefresh.GardenFlowerGridTypeCategory.Get(gridTypeId);

                _initGrids.Add(new GardenEditorGridData(gridTypeConfig, startXY));
            }

            _initMapGridsConfigText = stringBuilder.ToString();

            stringBuilder.Clear();

            //取出的格子数据
            foreach (var list in levelConfig.InitWaitGridType)
            {
                foreach (var item in list)
                {
                    stringBuilder.Append($"{item},");
                }

                stringBuilder.Append("|");
            }
            _successTakeOutConfigText = stringBuilder.ToString();

            CreateInitGrids(_initGrids);
        }

        private void InitMap()
        {
            _gridXY2Obj.Clear();

            var gridRowCount = customGrids.rowCount;
            var gridColumnCount = customGrids.colCount;
            flowerRoot.gameObject.DestroyAllChildren();

            //填满格子
            for (int y = 0; y < gridRowCount; y++)
            {
                for (int x = 0; x < gridColumnCount; x++)
                {
                    var gridXY = new Vector2Int(x, y);

                    var gridInstance = GameObject.Instantiate(flowerPrefab, flowerRoot);
                    gridInstance.transform.localPosition = customGrids.GridXYToLocalPosition(gridXY);

                    _gridXY2Obj.Add(gridXY, gridInstance);
                }
            }
        }

        private void CreateInitGrids(List<GardenEditorGridData> lastGrids)
        {
            flowerRoot.gameObject.DestroyAllChildren();

            NewRefreshRule();

            var flowerFloderPath = "Assets/GameMain/AssetBundle/Prefab/FlowerPrefab/Flower.Garden";

            foreach (var item in lastGrids)
            {
                var runtimeGridsXY = GetRuntimeGridsXY(item.GridStartXY, item.GridTypeConfig);

                var flowerId = _refresh.RefreshFlowerId();
                var flowerTypeConfig = _refresh.FlowerTypeCategory.GetOrDefault(flowerId);

                var gardenFlowerPrefab = AssetLoader.LoadGameObject(flowerFloderPath, flowerTypeConfig.GardenSpinePrefab);

                foreach (var gridXY in runtimeGridsXY)
                {
                    if (_gridXY2Obj.TryGetValue(gridXY, out var flowerObj) && flowerObj != null)
                    {
                        Debug.LogError($"地图上已经有花朵:{gridXY}");
                        continue;
                    }

                    var gridInstance = GameObject.Instantiate(gardenFlowerPrefab, flowerRoot);
                    gridInstance.transform.localPosition = customGrids.GridXYToLocalPosition(gridXY);
                    gridInstance.transform.name = $"{gardenFlowerPrefab.name}-{item.GridTypeConfig.Id}-{item.GridStartXY.x}-{item.GridStartXY.y}";

                    _gridXY2Obj[gridXY] = gridInstance;
                }
            }
        }

        [VerticalGroup("初始格子")]
        [LabelText("剩下的格子的配置")]
        [TextArea]
        [ReadOnly]
        public string _initMapGridsConfigText;

        [VerticalGroup("初始格子")]
        [Button("复制初始的格子")]
        [GUIColor("yellow")]
        private void CopyInitGridsConfig()
        {
            InitDataToLunbanConfig(_initGrids);
            GUIUtility.systemCopyBuffer = _initMapGridsConfigText;
        }

        [VerticalGroup("取出格子")]
        [LabelText("成功取出的格子的配置")]
        [TextArea]
        [ReadOnly]
        public string _successTakeOutConfigText;

        [VerticalGroup("取出格子")]
        [Button("复制等待区的格子")]
        [GUIColor("yellow")]
        private void CopySuccessTakeOutGridsConfig()
        {
            TakeOutDataToLunbanConfig(_successTakeOutGrids);
            GUIUtility.systemCopyBuffer = _successTakeOutConfigText;
        }

        private Dictionary<int, List<GardenFlowerGridTypeConfig>> GetTakeOutFlowerGrids()
        {
            var waveCount = Mathf.CeilToInt(maxTakeOutGridCount / (float)_oneWaveRefreshCount);
            _takeOutGrids.Clear();

            for (int i = 0; i < waveCount; i++)
            {
                int refreshCount = _oneWaveRefreshCount;

                var takeCount = _oneWaveRefreshCount * (i + 1);
                if (takeCount > maxTakeOutGridCount)
                {
                    refreshCount = maxTakeOutGridCount - _oneWaveRefreshCount * i;
                }

                var list = _refresh.RefreshFlowerType(refreshCount);
                _takeOutGrids.Add(i, list);
            }

            return _takeOutGrids;
        }

        private bool StartTakeOutFlowerFromMap(Dictionary<int, List<GardenFlowerGridTypeConfig>> takeOut)
        {
            _successTakeOutGrids.Clear();

            foreach (var gridItems in takeOut)
            {
                foreach (var gridTypeConfig in gridItems.Value)
                {
                    var result = TryTakeOutGridFromMap(gridTypeConfig, out var gridStartXY);
                    if (result)
                    {
                        _successTakeOutGrids.Add(new GardenEditorGridData(gridTypeConfig, gridStartXY));
                    }
                    else
                    {
                        _successTakeOutConfigText = "取出格子失败";
                        return false;
                    }
                }
            }

            if (IsCanRemove())
            {
                _successTakeOutConfigText = "有行列可消除";
                return false;
            }

            Debug.Log($"取出格子数据成功 levelId:{gardenLevelId}");
            return true;
        }

        private bool IsCanRemove()
        {
            for (int y = 0; y < customGrids.rowCount; y++)
            {
                var isCamRemove = true;

                for (int x = 0; x < customGrids.colCount; x++)
                {
                    var gridXY = new Vector2Int(x, y);

                    if (_gridXY2Obj[gridXY] == null)
                    {
                        isCamRemove = false;
                        break;
                    }
                }

                if (isCamRemove)
                {
                    return true;
                }
            }

            for (int x = 0; x < customGrids.colCount; x++)
            {
                var isCamRemove = true;

                for (int y = 0; y < customGrids.rowCount; y++)
                {
                    var gridXY = new Vector2Int(x, y);

                    if (_gridXY2Obj[gridXY] == null)
                    {
                        isCamRemove = false;
                        break;
                    }
                }

                if (isCamRemove)
                {
                    return true;
                }
            }

            return false;
        }

        private List<Vector2Int> GetMapGridsXY()
        {
            _mapGridsXYCache.Clear();

            var gridRowCount = customGrids.rowCount;
            var gridColumnCount = customGrids.colCount;

            for (int y = 0; y < gridRowCount; y++)
            {
                for (int x = 0; x < gridColumnCount; x++)
                {
                    var gridXY = new Vector2Int(x, y);

                    _mapGridsXYCache.Add(gridXY);
                }
            }

            return _mapGridsXYCache;
        }

        public Vector2Int GridIndex2XY(int gridIndex, int colCount, int rowCount)
        {
            return new Vector2Int(gridIndex % colCount, gridIndex / colCount);
        }

        private List<Vector2Int> GetRuntimeGridsXY(Vector2Int startGridXY, GardenFlowerGridTypeConfig config)
        {
            _runtimeGridsXY.Clear();

            for (int i = 0; i < config.GridData.Count; i++)
            {
                //没有格子数据过滤
                if (config.GridData[i] <= 0) continue;

                var gridXY = GridIndex2XY(i, config.Col, config.Row);
                var runtimeGridXY = startGridXY + gridXY;
                _runtimeGridsXY.Add(runtimeGridXY);
            }

            return _runtimeGridsXY;
        }

        private bool TryTakeOutGridFromMap(GardenFlowerGridTypeConfig config, out Vector2Int startGridXY)
        {
            var list = GetMapGridsXY();

            while (list.Count > 0)
            {
                var randomIndex = UtilityTool.RandomNumber(0, list.Count);
                var randomStartXY = list[randomIndex];
                list.RemoveAt(randomIndex);

                var runtimeGridsXY = GetRuntimeGridsXY(randomStartXY, config);
                var result = IsCanPutInMap(runtimeGridsXY);

                //这个格子可以取出
                if (result)
                {
                    SetFlowerNull(runtimeGridsXY, true);
                    startGridXY = randomStartXY;
                    return true;
                }
            }

            startGridXY = Vector2Int.zero;
            return false;
        }

        private bool IsMathInMap(GardenFlowerGridTypeConfig config, Vector2Int startGridXY, out List<Vector2Int> runtimeGridsXY)
        {
            runtimeGridsXY = GetRuntimeGridsXY(startGridXY, config);
            return IsCanPutInMap(runtimeGridsXY);
        }

        private void SetFlowerNull(List<Vector2Int> runtimeGrids, bool destroy)
        {
            foreach (var gridXY in runtimeGrids)
            {
                _gridXY2Obj.TryGetValue(gridXY, out var flowerObj);
                _gridXY2Obj[gridXY] = null;

                if (destroy)
                {
                    GameObject.DestroyImmediate(flowerObj);
                }
            }
        }

        private bool IsCanPutInMap(List<Vector2Int> runtimeGridsXY)
        {
            foreach (var gridXY in runtimeGridsXY)
            {
                if (_gridXY2Obj.TryGetValue(gridXY, out var flowerObj))
                {
                    if (flowerObj == null)
                    {
                        //已经被取出
                        return false;
                    }
                }
                else
                {
                    //格子的范围
                    return false;
                }
            }

            return true;
        }

        private List<int> GetCanUseGridTypeIds()
        {
            _canUseGridsTypeIds.Clear();

            //默认加入一个格子的
            foreach (var item in _refresh.GridTypeCacheDict)
            {
                _canUseGridsTypeIds.Add(item.Key);
            }

            return _canUseGridsTypeIds;
        }

        private List<GardenEditorGridData> MapGridExchangeToFlowerGrid(bool destroy)
        {
            _initGrids.Clear();

            var canUseGridType = _refresh.GridTypeCacheDict;

            var gridRowCount = customGrids.rowCount;
            var gridColumnCount = customGrids.colCount;

            for (int y = 0; y < gridRowCount; y++)
            {
                for (int x = 0; x < gridColumnCount; x++)
                {
                    var gridStartXY = new Vector2Int(x, y);

                    if (_gridXY2Obj.TryGetValue(gridStartXY, out var flowerObj) && flowerObj != null)
                    {
                        var allKeys = GetCanUseGridTypeIds();

                        while (allKeys.Count > 0)
                        {
                            //开始匹配格子
                            var index = UtilityTool.RandomNumber(0, allKeys.Count);
                            var gridTypeId = allKeys[index];
                            allKeys.RemoveAt(index);

                            var gridTypeConfig = GardenEditorRefresh.GardenFlowerGridTypeCategory.GetOrDefault(gridTypeId);
                            var result = IsMathInMap(gridTypeConfig, gridStartXY, out var runtimeGridsXY);

                            if (result)
                            {
                                //匹配成功加入 必有一个 格子的匹配成功
                                _initGrids.Add(new GardenEditorGridData(gridTypeConfig, gridStartXY));
                                SetFlowerNull(runtimeGridsXY, destroy);
                                break;
                            }
                            else
                            {
                                if (allKeys.Count <= 0)
                                {
                                    allKeys.Add(101);
                                    allKeys.Add(102);
                                    allKeys.Add(103);
                                }
                            }
                        }
                    }
                }
            }

            return _initGrids;
        }

        private string TakeOutDataToLunbanConfig(List<GardenEditorGridData> successTakeOuts)
        {
            StringBuilder stringBuilder = new StringBuilder();

            int loopCount = Mathf.CeilToInt(maxTakeOutGridCount / (float)_oneWaveRefreshCount);
            for (int i = 0; i < loopCount; i++)
            {
                for (int j = 0; j < _oneWaveRefreshCount; j++)
                {
                    var listIndex = i * _oneWaveRefreshCount + j;

                    if (successTakeOuts.Count > listIndex)
                    {
                        var configItem = successTakeOuts[listIndex];
                        stringBuilder.Append($"{configItem.GridTypeConfig.Id},");
                    }
                }

                stringBuilder.Append($"|");
            }
            _successTakeOutConfigText = stringBuilder.ToString();
            return _successTakeOutConfigText;
        }

        private string InitDataToLunbanConfig(List<GardenEditorGridData> lastGrids)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Clear();

            foreach (var configItem in lastGrids)
            {
                stringBuilder.Append($"{configItem.GridTypeConfig.Id},{configItem.GridStartXY.x},{configItem.GridStartXY.y}|");
            }

            _initMapGridsConfigText = stringBuilder.ToString();
            return _initMapGridsConfigText;
        }

    }
#endif
}