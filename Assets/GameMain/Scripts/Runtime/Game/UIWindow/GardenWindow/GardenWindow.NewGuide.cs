using GameMain.AOT;
using GameMain.BTree;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.Garden
{
    public partial class GardenWindow
    {
        private BehaviourTree _runnigTree;

        private void TryAddNewGuide(int levelId)
        {
            switch (levelId)
            {
                case 1001:

                    BuildLevelTree_1001();
                    break;

                case 1002:
                    if (LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_NewGuide>().GuideIsFinish(levelId) == false)
                    {
                        LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_NewGuide>().AddFinishGuide(levelId);
                        BuildLevelTree_1002();
                    }
                    break;

                default:
                    break;
            }
        }

        private void OnGuideTreeComplete(bool success)
        {
            Debugger.Log($"引导树完成 success:{success}");
            _runnigTree = null;
        }

        private void BuildLevelTree_1001()
        {
            if (_runnigTree != null)
            {
                _runnigTree.Cancel();
            }

            var rootNode = new RootNode();
            _runnigTree = new GameMain.BTree.BehaviourTree(rootNode);
            _runnigTree.OnFinishedEvent += OnGuideTreeComplete;

            var sequence = new SequenceNode();
            rootNode.AddChild(sequence);

            //禁用设置按钮
            var enableSetting = new GardenEnableBtnSetting(false);
            sequence.AddChild(enableSetting);

            //禁用道具按钮
            var bagProp = new GardenGuideActiveBagProps(false);
            sequence.AddChild(bagProp);

            //1 等待通关规则弹窗打开
            var waitPassRuleOpen = new WaitWindowOpen(nameof(GardenLevelGoalWindow));
            sequence.AddChild(waitPassRuleOpen);

            //2 等待通关规则弹窗关闭
            var waitPassClose = new WaitWindowClose(nameof(GardenLevelGoalWindow));
            sequence.AddChild(waitPassClose);

            //3 拖拽到目标点
            var dragToPosition = new GardenDragToFixGrid(1, new List<Vector2Int>()
            {
                new Vector2Int(0, 7), new Vector2Int(1, 7), new Vector2Int(2, 7), new Vector2Int(3, 7),
            });
            sequence.AddChild(dragToPosition);

            //4 拖拽到目标点2
            dragToPosition = new GardenDragToFixGrid(2, new List<Vector2Int>()
            {
                new Vector2Int(6, 6), new Vector2Int(7, 6), new Vector2Int(6, 7), new Vector2Int(7, 7),
            });
            sequence.AddChild(dragToPosition);

            //打开成功界面
            var waitSuccessOpen = new WaitWindowOpen(nameof(GardenSuccessWindow));
            sequence.AddChild(waitSuccessOpen);

            //引导下一关按钮
            var nextLevel = new FingerGuideNextLevel();
            sequence.AddChild(nextLevel);

            _runnigTree.Start();
        }

        private void BuildLevelTree_1002()
        {
            if (_runnigTree != null)
            {
                _runnigTree.Cancel();
            }

            var rootNode = new RootNode();
            _runnigTree = new GameMain.BTree.BehaviourTree(rootNode);
            _runnigTree.OnFinishedEvent += OnGuideTreeComplete;

            var sequence = new SequenceNode();
            rootNode.AddChild(sequence);

            //取消禁用道具按钮
            var bagProp = new GardenGuideActiveBagProps(true);
            sequence.AddChild(bagProp);

            //1 等待通关规则弹窗打开
            var waitPassRuleOpen = new WaitWindowOpen(nameof(GardenLevelGoalWindow));
            sequence.AddChild(waitPassRuleOpen);

            //2 等待通关规则弹窗关闭
            var waitPassClose = new WaitWindowClose(nameof(GardenLevelGoalWindow));
            sequence.AddChild(waitPassClose);

            //弹出免费使用刷新道具遮罩
            var useRefresh = new GardenGuideUseRefresh();
            sequence.AddChild(useRefresh);

            //禁用道具按钮
            var bagProp2 = new GardenGuideEnableBagProps(false);
            sequence.AddChild(bagProp2);

            //延迟一会
            var delayTime = new DelayNode(1f);
            sequence.AddChild(delayTime);

            //拖拽到
            var dragToPosition = new GardenDragToFixGrid(1, new List<Vector2Int>()
            {
                new Vector2Int(3, 3), new Vector2Int(4, 3), new Vector2Int(3, 4), new Vector2Int(4, 4),
            });
            sequence.AddChild(dragToPosition);

            //曲线禁用道具按钮
            bagProp2 = new GardenGuideEnableBagProps(true);
            sequence.AddChild(bagProp2);

            //延迟一会
            delayTime = new DelayNode(3f);
            sequence.AddChild(delayTime);

            //免费使用
            var freeUse = new GardenGuideFreeUse();
            sequence.AddChild(freeUse);

            //取消禁用设置按钮
            var enableSetting2 = new GardenEnableBtnSetting(true);
            sequence.AddChild(enableSetting2);

            _runnigTree.Start();
        }
    }
}