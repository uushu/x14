using cfg;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    [System.Serializable]
    public class Save_Task : ASaveData
    {
        [System.Serializable]
        public class TaskProgress
        {
            public int taskConfigId;

            public int progress;

            //已领取
            public bool isFinish;
        }

        //每日任务
        public List<TaskProgress> dailyTask;

        //任务清空的时间戳
        public long taskClearTimeStamp;

        public override void SetDefaultData(GlobalConfigCategory category)
        {
            taskClearTimeStamp = TimeTool.GetNowTimeStamp();
        }

        public TaskProgress GetFromDailyTask(int taskConfigId)
        {
            if (dailyTask == null)
            {
                dailyTask = new List<TaskProgress>();
            }

            foreach (var task in dailyTask)
            {
                if (task.taskConfigId == taskConfigId)
                {
                    return task;
                }
            }

            var newTask = new TaskProgress() { taskConfigId = taskConfigId, progress = 0 };
            dailyTask.Add(newTask);

            return newTask;
        }

        public void ClearDailyTask()
        {
            dailyTask = new List<TaskProgress>();
        }
    }
}