using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.FlowerBlock
{
    public class FlowerBlockWaitArea : ILogicBase
    {
        [LabelText("等待区的格子")]
        public List<RectTransform> waitBlocks;

        //下一次刷新的格子
        private List<RefreshBlockItemData> _nextRefreshResult;
        public List<RefreshBlockItemData> NextRefreshResult => _nextRefreshResult;

        [LabelText("单格子的宽")]
        private float _itemWidth = 70f;

        [LabelText("单格子的高")]
        private float _spriteHeight = 17f;

        [LabelText("行间距")]
        private float _colSpace = 0f;

        [LabelText("总的宽度")]
        private float _totalWidth => (_itemWidth + _colSpace) * 7 + _itemWidth;

        public override void Initialize()
        {
            base.Initialize();
        }

        public void OnStartGame(int levelId)
        {
        }

        private void HidelAll()
        {
            foreach (var item in waitBlocks)
            {
                item.gameObject.SetActive(false);
            }
        }

        public void ReadyNextRefreshBlocks()
        {
            HidelAll();

            _nextRefreshResult = BlockGameRoot.Instance.RefreshRule.RefreshBlockItems();

            for (int i = 0; i < waitBlocks.Count; i++)
            {
                var refreshItem = waitBlocks[i];

                if (_nextRefreshResult.Count > i)
                {
                    refreshItem.gameObject.SetActive(true);
                    var refreshData = _nextRefreshResult[i];

                    //设置大小
                    var blockItemWidth = refreshData.Config.GridData.Count * (_itemWidth + _colSpace) - _colSpace;
                    refreshItem.sizeDelta = new Vector2(blockItemWidth, _spriteHeight);

                    //设置刷新的位置
                    var startGridX = refreshData.startGridX;

                    var startX = (_itemWidth + _colSpace) * startGridX + blockItemWidth * 0.5f;
                    refreshItem.transform.localPosition = new Vector3(startX - _totalWidth * 0.5f, 0, 0);
                }
                else
                {
                    refreshItem.gameObject.SetActive(false);
                }
            }
        }

        private void DebugTest()
        {
            Debug.LogError("============");

            foreach (var item in _nextRefreshResult)
            {
                Debug.LogError(item);
            }
        }

        //下标0开始
        public List<RefreshBlockItemData> GetRefresh()
        {
            return _nextRefreshResult;
        }

        public override void UnInitialize()
        {
            base.UnInitialize();

        }
    }
}