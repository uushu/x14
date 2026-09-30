using cfg;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Pool;

namespace GameMain
{
    public class TaskMgr : ASingleBase<TaskMgr>
    {
        public enum ETaskTask
        {
            DailyTask = 1,  //每日任务
        }

        private readonly Dictionary<ETaskTask, List<TaskTypeBase>> _taskDict = new();

        private readonly EventGroup _eventGroup = new();

        public override void Initialize()
        {
            base.Initialize();

            _taskDict.Clear();

            _eventGroup.RemoveAllListener();
            _eventGroup.AddListener<GameEventDefine.Task_AddProgress>(ProcessMessage);
        }

        private void ProcessMessage(IEventMessage message)
        {
            if (message is GameEventDefine.Task_AddProgress taskProgress)
            {
                List<TaskTypeBase> taskList = ListPool<TaskTypeBase>.Get();

                GetTaskList(taskProgress.TaskTypeId, ref taskList);

                foreach (TaskTypeBase taskType in taskList)
                {
                    taskType.TryAddTaskProgress(taskProgress.AddTaskProgress);
                }

                foreach (var item1 in _taskDict)
                {
                    //最后一个任务是完成所有的任务
                    int completeCount = 0;
                    for (int i = 0; i < item1.Value.Count - 1; i++)
                    {
                        if (item1.Value[i].IsComplete)
                        {
                            completeCount++;
                        }
                    }

                    item1.Value.LastOrDefault().SetProgress(completeCount);
                }

                DailyTaskMgr.Instance.RefreshRedDot();
                ListPool<TaskTypeBase>.Release(taskList);
            }
        }

        private TaskTypeBase NewTaskBy(int taskType, Save_Task.TaskProgress progress, List<int> parameters)
        {
            switch (taskType)
            {
                case 1:
                    return new TaskT1(progress, parameters);

                case 2:
                    return new TaskT2(progress, parameters);

                case 3:
                    return new TaskT3(progress, parameters);

                case 4:
                    return new TaskT4(progress, parameters);

                case 5:
                    return new TaskT5(progress, parameters);

                case 6:
                    return new TaskT6(progress, parameters);

                case 7:
                    return new TaskT7(progress, parameters);
            }

            Debugger.LogError($"不存在的任务类型:{taskType}");
            return null;
        }

        private void GetTaskList(int taskType, ref List<TaskTypeBase> tasks)
        {
            tasks.Clear();

            foreach (var item in _taskDict)
            {
                foreach (var item2 in item.Value)
                {
                    if (item2.TaskType == taskType)
                    {
                        tasks.Add(item2);
                    }
                }
            }
        }

        public void ClearDailyTaskProgress()
        {
            var taskList = _taskDict[ETaskTask.DailyTask];

            if (taskList != null)
            {
                foreach (var item2 in taskList)
                {
                    item2.ClearProgress();
                }
            }

            LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Task>().ClearDailyTask();
        }

        public TaskTypeBase AddDailyTaskData(DailyTaskConfig taskConfig)
        {
            if (_taskDict.ContainsKey(ETaskTask.DailyTask) == false)
            {
                _taskDict.Add(ETaskTask.DailyTask, new());
            }

            var taskData = NewTaskBy(taskConfig.TaskType,
                    LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Task>().GetFromDailyTask(taskConfig.Id), taskConfig.Parameters);

            _taskDict[ETaskTask.DailyTask].Add(taskData);

            return taskData;
        }

        public TaskTypeBase GetTaskData(ETaskTask taskType, int taskId)
        {
            var list = _taskDict.GetValueOrDefault(taskType);

            if (list != null)
            {
                foreach (var item in list)
                {
                    if (item.SaveTaskProgress.taskConfigId == taskId)
                    {
                        return item;
                    }
                }
            }

            Debugger.LogError($"不存在的任务 taskType:{taskType} taskId:{taskId}");
            return null;
        }

        public override void UnInitialize()
        {
            base.UnInitialize();

            _taskDict.Clear();
            _eventGroup.RemoveAllListener();
        }
    }
}