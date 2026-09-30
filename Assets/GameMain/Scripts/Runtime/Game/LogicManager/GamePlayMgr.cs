using GameMain.NewGuide;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    public class GamePlayMgr : AMonoManager<GamePlayMgr>
    {
        private readonly List<IManager> _logicMgr = new()
        {
            GuideManager.Instance,
            MainDataManager.Instance,
            TaskMgr.Instance,
            BagMgr.Instance,
            DailyTaskMgr.Instance,
            SevenSignMgr.Instance,
            CollectFlowerMgr.Instance,
            AchievementMgr.Instance,
            FlowerGardenMgr.Instance,
        };

        private readonly List<IManager> _updateMgr = new()
        {
        };

        protected override void OnEnter()
        {
            base.OnEnter();

            for (int i = 0; i < _logicMgr.Count; i++)
            {
                _logicMgr[i].Initialize();
            }
        }

        protected override void OnUpdateInternal(float deltaTime)
        {
            base.OnUpdateInternal(deltaTime);

            for (int i = 0; i < _updateMgr.Count; i++)
            {
                _updateMgr[i].OnUpdate(deltaTime);
            }
        }

        protected override void OnExit()
        {
            base.OnExit();

            for (int i = 0; i < _logicMgr.Count; i++)
            {
                _logicMgr[i].UnInitialize();
            }
        }
    }
}