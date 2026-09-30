using cfg;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    //每日分享
    public class TaskT2 : TaskTypeBase
    {
        public const int TaskTypeId = 2;

        public TaskT2(Save_Task.TaskProgress taskProgress, List<int> parameters) : base(taskProgress, parameters)
        {
        }
    }
}