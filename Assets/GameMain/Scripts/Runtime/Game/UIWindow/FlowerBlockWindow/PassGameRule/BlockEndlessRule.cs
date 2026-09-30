using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace GameMain.FlowerBlock
{
    public class BlockEndlessRule : ABaseComponent, IBlockPassGameRule
    {
        public Action OnGameSuccessCallBack { get; set; }
        public Action OnGameFailedCallBack { get; set; }

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.Garden.GardenEndlessScoreProgress mGardenEndlessScoreProgress;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.FlowerBlock.FlowerBlockCombo mFlowerBlockCombo;

        [LabelText("当前无尽分数")]
        public int currentEndlessScore;

        public bool CollectButterflyAnimationIsComplete()
        {
            return false;
        }

        public void Initialize()
        {
            currentEndlessScore = 0;

            BlockGameRoot.Instance.blockMap.OnAfterRemoveFlowerBlockCallBack += OnAfterRemoveFlowerBlock;
            BlockGameRoot.Instance.blockMap.OnFlowerBlockTopFullCallBack += OnFlowerBlockTopFull;

            mGardenEndlessScoreProgress.Initialize(-1);
        }

        public void UnInitialize()
        {
            BlockGameRoot.Instance.blockMap.OnAfterRemoveFlowerBlockCallBack -= OnAfterRemoveFlowerBlock;
            BlockGameRoot.Instance.blockMap.OnFlowerBlockTopFullCallBack -= OnFlowerBlockTopFull;
        }

        private void OnAfterRemoveFlowerBlock(FlowerBlockRemoveResult result)
        {
            int addScore = 0;

            //连击额外的分数
            if (mFlowerBlockCombo.currentComboCount > 1)
            {
                var comboAddScore = ConfigMgr.Instance.Tables.GlobalConfigCategory.BlockEndlessComboBaseMultiScore * (mFlowerBlockCombo.currentComboCount - 1);
                Debugger.Log($"无尽模式连击加分+:{comboAddScore}");
                addScore += comboAddScore;
            }

            if (ConfigMgr.Instance.Tables.GlobalConfigCategory.BlockEndlessRowColAddScore
                 .DictTryGetOrLast(result.removeRowsIndex.Count, out var removeScore, (key) =>
                 {
                     return key > 1;
                 }))
            {
                Debugger.Log($"无尽模式消除加分+:{removeScore}");
                addScore += removeScore;
            }

            Debugger.Log($"无尽模式总的加分+:{addScore}");

            mGardenEndlessScoreProgress.AddScore(addScore, true, null);
        }

        private void OnFlowerBlockTopFull()
        {
            OnGameFailedCallBack?.Invoke();
        }
    }
}