using cfg;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    //每日首通
    public class TaskT1 : TaskTypeBase
    {
        public const int TaskTypeId = 1;

        public TaskT1(Save_Task.TaskProgress taskProgress, List<int> parameters) : base(taskProgress, parameters)
        {

        }
    }
}