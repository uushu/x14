using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public abstract class TaskTypeBase
    {
        public enum ETaskStatus
        {
            Complete,   //完成
            Running,    //进行
            Finish,     //结束
        }

        public TaskTypeConfig Config { get; private set; }

        public int TaskType => Config.Id;

        public virtual string TaskDesc => Config.TaskDesc;

        public Save_Task.TaskProgress SaveTaskProgress { get; private set; }

        public List<int> Parameters { get; private set; }

        public virtual int MaxProgress => Parameters[0];

        public int CurrentProgress => SaveTaskProgress.progress;

        public bool IsComplete => SaveTaskProgress.progress >= MaxProgress;

        public ETaskStatus Status
        {
            get
            {
                if (SaveTaskProgress.isFinish)
                {
                    return ETaskStatus.Finish;
                }

                if (IsComplete)
                {
                    return ETaskStatus.Complete;
                }

                return ETaskStatus.Running;
            }
        }

        public TaskTypeBase(Save_Task.TaskProgress taskProgress, List<int> parameters)
        {
            Config = ConfigMgr.Instance.Tables.TaskTypeCategory.GetOrDefault(taskProgress.taskConfigId);

            SaveTaskProgress = taskProgress;

            Parameters = parameters;
        }

        public virtual bool TryAddTaskProgress(int addProgress)
        {
            if (IsComplete)
            {
                return false;
            }

            SaveTaskProgress.progress += addProgress;
            SaveTaskProgress.progress = Mathf.Min(SaveTaskProgress.progress, MaxProgress);

            Debugger.Log($"任务:{SaveTaskProgress.taskConfigId} 添加进度:{addProgress} 当前进度:{SaveTaskProgress.progress}");

            return true;
        }

        public void SetProgress(int targetProgress)
        {
            if (Status == ETaskStatus.Finish)
            {
                return;
            }

            SaveTaskProgress.progress = Mathf.Min(targetProgress, MaxProgress);
            Debugger.Log($"任务:{SaveTaskProgress.taskConfigId} 设置进度:{targetProgress} 当前进度:{SaveTaskProgress.progress}");
        }

        public void ClearProgress()
        {
            SaveTaskProgress.progress = 0;
        }

        public void SetTaskFinish()
        {
            if (SaveTaskProgress.isFinish)
            {
                Debugger.LogError($"任务已经完成 taskConfigId:{SaveTaskProgress.taskConfigId}");
                return;
            }

            SaveTaskProgress.isFinish = true;
        }
    }
}