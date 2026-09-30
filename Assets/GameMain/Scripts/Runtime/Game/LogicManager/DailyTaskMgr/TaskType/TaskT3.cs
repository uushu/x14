using cfg;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    //每日抓猫
    public class TaskT3 : TaskTypeBase
    {
        public const int TaskTypeId = 3;

        public TaskT3(Save_Task.TaskProgress taskProgress, List<int> parameters) : base(taskProgress, parameters)
        {

        }
    }
}