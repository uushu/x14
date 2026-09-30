using System;
using System.Collections.Generic;
using System.Linq;
using cfg;

namespace GameMain
{
    public class AchievementMgr : ASingleBase<AchievementMgr>
    {
        private EventGroup _eventGroup = new();

        public override void Initialize()
        {
            base.Initialize();
            _eventGroup.AddListener<GameEventDefine.UseInGameBagProp>(ProcessMessage);
            _eventGroup.AddListener<GameEventDefine.GameRemoveRowCols>(ProcessMessage);
            _eventGroup.AddListener<GameEventDefine.Task_AddProgress>(ProcessMessage);
            _eventGroup.AddListener<GameEventDefine.GetBagProp>(ProcessMessage);
            _eventGroup.AddListener<GameEventDefine.FlowerGardenPass>(ProcessMessage);

            var level = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().levelId;
            var achievementConfigs = ConfigMgr.Instance.Tables.AchievementCategory.DataList;
            foreach (var config in achievementConfigs)
            {
                AddAchievementData(config);
            }

            var lastSignDayTimeStamp = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Sign>().lastSignDayTimeStamp;
            if (!TimeTool.IsSameDay(lastSignDayTimeStamp, TimeTool.GetNowTimeStamp()))
            {
                var dic = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Achievement>(true).AllAchievmentDic;
                AchievementDicTypeId.TryGetValue(2, out var list);
                var signDay = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Sign>().signDay;
                foreach (var id in list)
                {
                    dic.Dictionary[id].progress = (int)signDay;
                }
            }
        }

        public Dictionary<int, List<int>> AchievementDicTypeId = new Dictionary<int, List<int>>();

        public void AddAchievementData(AchievementConfig achievementConfig)
        {
            var achievementData = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Achievement>(true);
            if (achievementData.AllAchievmentDic == null)
            {
                achievementData.AllAchievmentDic = new SerializableDictionary<int, AchievementProgress>();
            }

            if (!achievementData.AllAchievmentDic.Dictionary.ContainsKey(achievementConfig.Id))
            {
                achievementData.AllAchievmentDic.Dictionary[achievementConfig.Id] = new AchievementProgress()
                {
                    achievementConfigId = achievementConfig.Id,
                    progress = 0,
                    isClaim = false
                };
            }

            if (!AchievementDicTypeId.ContainsKey(achievementConfig.TaskType))
            {
                AchievementDicTypeId[achievementConfig.TaskType] = new List<int>();
            }

            AchievementDicTypeId[achievementConfig.TaskType].Add(achievementConfig.Id);
        }

        public List<AchievementProgress> GetAchievementProgress()
        {
            List<AchievementProgress> achievementProgresses = new List<AchievementProgress>();
            var achievementData = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Achievement>(true);
            foreach (var items in AchievementDicTypeId)
            {
                if (items.Key != 6)
                {
                    foreach (var id in items.Value)
                    {
                        achievementProgresses.Add(achievementData.AllAchievmentDic.Dictionary[id]);
                    }
                }
            }

            return achievementProgresses;
        }

        public List<AchievementProgress> GetAchievementTopProgress()
        {
            List<AchievementProgress> achievementProgresses = new List<AchievementProgress>();
            var achievementData = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Achievement>(true);
            foreach (var items in AchievementDicTypeId)
            {
                if (items.Key == 6)
                {
                    foreach (var id in items.Value)
                    {
                        achievementProgresses.Add(achievementData.AllAchievmentDic.Dictionary[id]);
                    }
                }
            }

            return achievementProgresses;
        }


        private void ProcessMessage(IEventMessage obj)
        {
            var achievementData = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Achievement>(true);
            ///游戏内使用道具 俄罗斯花园-复活
            if (obj is GameEventDefine.UseInGameBagProp use && use.BagPropId == 10004)
            {
                AchievementDicTypeId.TryGetValue(4, out var list);
                if (list != null)
                    foreach (var id in list)
                    {
                        achievementData.AllAchievmentDic.Dictionary[id].progress += use.BagProPCount;
                    }
            }
            else if (obj is GameEventDefine.FlowerGardenPass pass)
            {
                AchievementDicTypeId.TryGetValue(3, out var list);
                if (list != null)
                {
                    foreach (var id in list)
                    {
                        achievementData.AllAchievmentDic.Dictionary[id].progress = pass.level - 1000;
                    }
                }
            }
            ///消除几行几列
            else if (obj is GameEventDefine.GameRemoveRowCols remove)
            {
                List<int> list = new List<int>();
                if (remove.ColCount + remove.RowCount == 2)
                {
                    AchievementDicTypeId.TryGetValue(7, out list);
                }

                if (remove.ColCount + remove.RowCount == 3)
                {
                    AchievementDicTypeId.TryGetValue(8, out list);
                }

                if (remove.ColCount + remove.RowCount == 4)
                {
                    AchievementDicTypeId.TryGetValue(9, out list);
                }

                if (remove.ColCount + remove.RowCount == 5)
                {
                    AchievementDicTypeId.TryGetValue(10, out list);
                }

                if (remove.ColCount + remove.RowCount == 6)
                {
                    AchievementDicTypeId.TryGetValue(11, out list);
                }

                if (list != null)
                    foreach (var id in list)
                    {
                        achievementData.AllAchievmentDic.Dictionary[id].progress += 1;
                    }
            }
            ///获得成就点击
            else if (obj is GameEventDefine.GetBagProp bag && bag.BagPropId == GameTool.AchievementPintId)
            {
                var count = BagMgr.Instance.GetBagPropCount(GameTool.AchievementPintId);
                AchievementDicTypeId.TryGetValue(6, out var list);
                if (list != null)
                    foreach (var id in list)
                    {
                        achievementData.AllAchievmentDic.Dictionary[id].progress = (int)count;
                    }
            }
            //看广告
            else if (obj is GameEventDefine.Task_AddProgress progress && progress.TaskTypeId == 6)
            {
                AchievementDicTypeId.TryGetValue(1, out var list);
                if (list != null)
                    foreach (var id in list)
                    {
                        achievementData.AllAchievmentDic.Dictionary[id].progress += progress.AddTaskProgress;
                    }
            }

            GameEventDefine.SendAchievementProgressChange();
        }

        public void SetAchievementShare(HyzShareManager.GetShareResponseData data)
        {
            var achievementData = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Achievement>(true);
            AchievementDicTypeId.TryGetValue(5, out var list);
            if (list != null)
            {
                foreach (var id in list)
                {
                    achievementData.AllAchievmentDic.Dictionary[id].progress = data.list.Count;
                }

                GameEventDefine.SendAchievementProgressChange();
            }
        }


        public void SetAchievementCompleted(AchievementProgress data )
        {
            var achievementData = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Achievement>(true);
            achievementData.AllAchievmentDic.Dictionary[data.achievementConfigId].isClaim = true;
         //   callback?.Invoke();
        }

        public bool IsMainTaskFinish()
        {
            var achievementData = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Achievement>(true);
            foreach (var item in achievementData.AllAchievmentDic.Dictionary.Values)
            {
                var config = ConfigMgr.Instance.Tables.AchievementCategory.Get(item.achievementConfigId);

                if (item.isClaim == false && item.progress >= config.Parameters)
                {
                    return true;
                }
            }

            return false;
        }

        public override void UnInitialize()
        {
            _eventGroup.RemoveAllListener();
        }
    }
}