using cfg;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    public class ItemData_DailyTaskCompare : IComparer<ItemData_DailyTask>
    {
        public int Compare(ItemData_DailyTask x, ItemData_DailyTask y)
        {
            var xStatus = x.TaskTypeBase.Status;
            var yStatus = y.TaskTypeBase.Status;

            if (xStatus == yStatus)
            {
                return x.Config.Id.CompareTo(y.Config.Id);
            }

            return xStatus.CompareTo(yStatus);
        }
    }

    public class ItemData_DailyTask
    {
        public int TaskId => Config.Id;

        public DailyTaskConfig Config { get; private set; }

        public TaskTypeBase TaskTypeBase { get; private set; }

        public int JumpId => Config.JumpId;

        public string TaskDesc => Config.TaskType_Ref.TaskDesc;

        private List<ItemData_BagProp> _taskAward;
        public List<ItemData_BagProp> TaskAward
        {
            get
            {
                if (_taskAward == null)
                {
                    _taskAward = new();
                    foreach (var item in Config.TaskAward)
                    {
                        _taskAward.Add(new(item.Key, item.Value));
                    }
                }
                return _taskAward;
            }
        }

        public ItemData_DailyTask(DailyTaskConfig config, TaskTypeBase taskType)
        {
            Config = config;

            TaskTypeBase = taskType;
        }
    }
}