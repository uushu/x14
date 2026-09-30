using GameMain.RedDot;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    public class DailyTaskMgr : ASingleBase<DailyTaskMgr>
    {
        private List<ItemData_DailyTask> _dailyTaskData;

        public override void Initialize()
        {
            base.Initialize();

            _dailyTaskData = new List<ItemData_DailyTask>();

            var taskConfig = ConfigMgr.Instance.Tables.DailyTaskCategory.DataList;
            foreach (var config in taskConfig)
            {
                var taskData = TaskMgr.Instance.AddDailyTaskData(config);

                _dailyTaskData.Add(new ItemData_DailyTask(config, taskData));
            }

            if (TaskIsCrossDayClear())
            {
                Debugger.Log("跨天,清空每日任务数据");
                TaskMgr.Instance.ClearDailyTaskProgress();
                LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Task>().taskClearTimeStamp = TimeTool.GetNowTimeStamp();
            }

            RefreshRedDot();
        }

        private bool TaskIsCrossDayClear()
        {
            var taskClearTimeStamp = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Task>(false).taskClearTimeStamp;
            var timeNow = TimeTool.GetNowTimeStamp();

            if (TimeTool.IsSameDay(taskClearTimeStamp, timeNow) == false)
            {
                return true;
            }

            return false;
        }

        public List<ItemData_DailyTask> GetSortDailyTask()
        {
            _dailyTaskData.Sort(new ItemData_DailyTaskCompare());

            return _dailyTaskData;
        }

        public ItemData_DailyTask GetTaskData(int taskId)
        {
            foreach (var item in _dailyTaskData)
            {
                if (item.Config.Id == taskId)
                { 
                    return item;
                }
            }

            Debugger.LogError($"不存在的任务 taskId:{taskId}");
            return null;
        }

        public bool GetTaskAward(int taskId)
        {
            var taskData = GetTaskData(taskId);

            if (taskData == null)
            {
                return false;
            }

            if (taskData.TaskTypeBase.Status != TaskTypeBase.ETaskStatus.Complete)
            {
                Debugger.LogError("任务状态不是:Complete");
                return false;
            }

            //领取道具成功
            taskData.TaskTypeBase.SetTaskFinish();
            RefreshRedDot();
            return true;
        }

        public void RefreshRedDot()
        {
            foreach (var item in _dailyTaskData)
            {
                if (item.TaskTypeBase.Status == TaskTypeBase.ETaskStatus.Complete)
                {
                    RedDotMgr.Instance.ChangeValue(string.Format(RedDotPath.DailyTask, item.TaskId), 1);
                }
                else
                {
                    RedDotMgr.Instance.ChangeValue(string.Format(RedDotPath.DailyTask, item.TaskId), 0);
                }
            }
        }

        public override void UnInitialize()
        {
            base.UnInitialize();
            _dailyTaskData = null;
        }
    }
}