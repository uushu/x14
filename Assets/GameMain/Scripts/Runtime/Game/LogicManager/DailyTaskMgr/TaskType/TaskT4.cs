using cfg;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    //通关X关
    public class TaskT4 : TaskTypeBase
    {
        public const int TaskTypeId = 4;

        public override string TaskDesc
        {
            get
            {
                if (Status == ETaskStatus.Running)
                {
                    return string.Format("通关<color=red>{0}</color>/{1}关", SaveTaskProgress.progress, MaxProgress);
                }
                else
                {
                    return string.Format("通关<color=green>{0}</color>/{1}关", SaveTaskProgress.progress, MaxProgress);
                }
            }
        }

        public TaskT4(Save_Task.TaskProgress taskProgress, List<int> parameters) : base(taskProgress, parameters)
        {
            
        }
    }
}