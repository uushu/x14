using cfg;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.FlowerBlock
{
    public abstract class ABlockRefreshByWeight : IBlockRefreshRule
    {
        private SortedDictionary<int, int> _weightCahe;
        protected abstract Dictionary<int, int> _flowerIdWeight { get; }
        protected abstract Dictionary<int, int> _butterflyWeight { get; }

        private Dictionary<int, int> _allFlowerIdWeight;

        private List<RefreshBlockItemData> _results;

        //横排最大格子数量
        private const int MAX_GRID_WIDTH_COUNT = 8;

        private int _waveIndex;
        private int _maxFixRefreshCount => _fixRefreshList.Count;
        private bool _isFixRefresh => _waveIndex < _maxFixRefreshCount;

        protected abstract List<List<int>> _fixRefreshList { get; }

        public virtual void Initialize(int levelId)
        {
            _weightCahe = new();
            _results = new();
            _waveIndex = 0;

            _allFlowerIdWeight = new();
            _allFlowerIdWeight.DictMergeFrom(_flowerIdWeight);
            _allFlowerIdWeight.DictMergeFrom(_butterflyWeight);
        }

        private bool TotalGridWidthOutOfRange(List<RefreshBlockItemData> result, RefreshBlockItemData tempAdd)
        {
            int totalWidth = TotalWidth(result);
            totalWidth += tempAdd.Config.GridData.Count;

            return totalWidth >= MAX_GRID_WIDTH_COUNT;
        }

        private int TotalWidth(List<RefreshBlockItemData> result)
        {
            int totalWidth = 0;

            foreach (var item in result)
            {
                totalWidth += item.Config.GridData.Count;
            }

            return totalWidth;
        }

        protected virtual int RefreshMainFlowerId()
        {
            var flowerId = UtilityTool.RandomKeyByWeight(_flowerIdWeight);
            return flowerId;
        }

        private bool ReplaceBuffterflyId(out int butterflyId)
        {
            var flowerId = UtilityTool.RandomKeyByWeight(_allFlowerIdWeight);
            if (_butterflyWeight.ContainsKey(flowerId))
            {
                butterflyId = flowerId;
                return true;
            }
            butterflyId = 0;
            return false;
        }

        private List<RefreshBlockItemData> FixRefresh()
        {
            _results.Clear();

            if (_isFixRefresh == false)
            {
                Debugger.LogError($"当前刷新波次 _waveIndex:{_waveIndex} _maxFixRefreshCount={_maxFixRefreshCount}");
                return _results;
            }

            int fixIndex = _waveIndex;
            var fixRefreshTypes = _fixRefreshList[fixIndex];

            int count = fixRefreshTypes.Count / 2;
            for (int i = 0; i < count; i++)
            {
                var index = i * 2;
                var blockTypeId = fixRefreshTypes[index];
                var startX = fixRefreshTypes[index + 1];

                var result = new RefreshBlockItemData();
                result.waveIndex = fixIndex;
                result.startGridX = startX;
                result.blockTypeId = blockTypeId;
                GeneratorFlowerId(result);

                _results.Add(result);
            }

            return _results;
        }

        private void GeneratorFlowerId(RefreshBlockItemData result)
        {
            var mainFlowerId = RefreshMainFlowerId();
            var mainFlowerConfig = ConfigMgr.Instance.Tables.BlockFlowerTypeCategory.GetOrDefault(mainFlowerId);
            result.mainFlowerId = mainFlowerId;

            result.flowersId.Clear();
            var gridData = result.Config.GridData;

            var isSpecialFlower = mainFlowerConfig.FlowerType == 3;

            for (int i = 0; i < gridData.Count; i++)
            {
                if (isSpecialFlower)
                {
                    //特殊鲜花不生产蝴蝶
                    result.flowersId.Add(mainFlowerId);
                    continue;
                }
                var replaceFlowerId = ReplaceBuffterflyId(out int butterflyId);

                if (replaceFlowerId)
                {
                    result.flowersId.Add(butterflyId);
                }
                else
                {
                    result.flowersId.Add(mainFlowerId);
                }
            }
        }

        protected abstract void GetWeightCahe(int waveIndex, ref SortedDictionary<int, int> result);

        private List<RefreshBlockItemData> WeightRefresh()
        {
            _results.Clear();

            if (_isFixRefresh)
            {
                Debugger.LogError($"当前刷新波次 _waveIndex:{_waveIndex} _maxFixRefreshCount={_maxFixRefreshCount}");
                return _results;
            }

            //减去固定刷新的次数
            var waveIndex = _waveIndex - _maxFixRefreshCount;
            waveIndex = Mathf.Clamp(waveIndex, 0, ConfigMgr.Instance.Tables.BlockGridTypeRefreshCategory.DataList.Count - 1);

            var waveConfig = ConfigMgr.Instance.Tables.BlockGridTypeRefreshCategory.DataList[waveIndex];
            var randomCount = UtilityTool.RandomKeyByWeight(waveConfig.RandomMaxCount);
            _weightCahe.Clear();

            _weightCahe.Add(1011, waveConfig.FlowerType1011);
            _weightCahe.Add(1012, waveConfig.FlowerType1012);
            _weightCahe.Add(1013, waveConfig.FlowerType1013);
            _weightCahe.Add(1014, waveConfig.FlowerType1014);

            _weightCahe.Add(1021, waveConfig.FlowerType1021);
            _weightCahe.Add(1022, waveConfig.FlowerType1022);
            _weightCahe.Add(1023, waveConfig.FlowerType1023);

            _weightCahe.Add(1031, waveConfig.FlowerType1031);
            _weightCahe.Add(1032, waveConfig.FlowerType1032);

            _weightCahe.Add(1041, waveConfig.FlowerType1041);

            //1 生成格子的类型
            int failedCount = 0;
            while (_results.Count < randomCount)
            {
                if (_weightCahe.Count <= 0)
                {
                    break;
                }

                var randomKey = UtilityTool.RandomKeyByWeight(_weightCahe);

                int flowerGridType = Mathf.FloorToInt(randomKey * 0.1f);

                var tempAdd = new RefreshBlockItemData()
                {
                    waveIndex = waveIndex,
                    startGridX = 0,
                    blockTypeId = flowerGridType,
                };

                _weightCahe.Remove(randomKey);

                if (TotalGridWidthOutOfRange(_results, tempAdd))
                {
                    failedCount++;

                    if (failedCount > 100)
                    {
                        Debugger.LogError("随机生成滑块失败次数>100");
                        break;
                    }
                    continue;
                }

                _results.Add(tempAdd);
            }

            //2 生成花儿的类型
            foreach (var result in _results)
            {
                GeneratorFlowerId(result);
            }

            //3 生成随机起点X
            UtilityTool.ShuffleList(_results);   //打乱顺序
            var totalWidth = TotalWidth(_results);
            int[] grids = new int[MAX_GRID_WIDTH_COUNT];
            var startX = 0;
            for (int i = startX; i < (totalWidth + startX); i++)
            {
                grids[i] = 1;
            }

            for (int i = 0; i < _results.Count; i++)
            {
                _results[i].startGridX = startX;
                startX += _results[i].Config.GridData.Count;
            }

            //右边开始移动
            for (int i = _results.Count - 1; i >= 0; i--)
            {
                startX = _results[i].startGridX;

                var rightSteps = new List<int>();
                //是否可以右边移动
                int rightStep = 0;
                for (int x = startX + _results[i].Config.GridData.Count; x <= MAX_GRID_WIDTH_COUNT - _results[i].Config.GridData.Count; x++)
                {
                    if (grids[x] == 0)
                    {
                        rightStep++;
                        rightSteps.Add(rightStep);
                    }
                    else
                    {
                        break;
                    }
                }

                if (rightSteps.Count > 0)
                {
                    //移除 更新数据
                    for (var j = 0; j < _results[i].Config.GridData.Count; j++)
                    {
                        grids[j + _results[i].startGridX] = 0;
                    }

                    if (i == 0) rightSteps.Add(0);

                    //随机移动数量
                    rightStep = UtilityTool.RandomNumber(rightSteps);

                    _results[i].startGridX += rightStep;

                    //添加 更新数据
                    for (var j = 0; j < _results[i].Config.GridData.Count; j++)
                    {
                        grids[j + _results[i].startGridX] = 1;
                    }
                }
            }

            return _results;
        }

        public List<RefreshBlockItemData> RefreshBlockItems()
        {
            if (_isFixRefresh)
            {
                _results = FixRefresh();
            }
            else
            {
                _results = WeightRefresh();
            }

            //增加刷新的次数
            _waveIndex++;
            return _results;
        }

        public void ResetWaveIndex()
        {
            _waveIndex = _maxFixRefreshCount;
        }
    }
}