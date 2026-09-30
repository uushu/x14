using cfg;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    //使用X个道具
    public class TaskT5 : TaskTypeBase
    {
        public const int TaskTypeId = 5;

        public override string TaskDesc => string.Format(Config.TaskDesc, MaxProgress);

        public TaskT5(Save_Task.TaskProgress taskProgress, List<int> parameters) : base(taskProgress, parameters)
        {

        }
    }
}