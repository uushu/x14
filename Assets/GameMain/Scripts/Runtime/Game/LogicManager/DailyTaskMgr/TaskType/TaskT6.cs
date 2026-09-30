using cfg;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    //看X个广告
    public class TaskT6 : TaskTypeBase
    {
        public const int TaskTypeId = 6;

        public override string TaskDesc => string.Format(Config.TaskDesc, MaxProgress);

        public TaskT6(Save_Task.TaskProgress taskProgress, List<int> parameters) : base(taskProgress, parameters)
        {

        }
    }
}