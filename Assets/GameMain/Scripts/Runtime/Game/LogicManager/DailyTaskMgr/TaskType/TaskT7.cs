using cfg;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    //完成所有任务
    public class TaskT7 : TaskTypeBase
    {
        public const int TaskTypeId = 7;

        public override string TaskDesc
        {
            get
            {
                if (Status == ETaskStatus.Running)
                {
                    return string.Format("完成所有任务<color=red>{0}</color>/{1}", SaveTaskProgress.progress, MaxProgress);
                }
                else
                {
                    return string.Format("完成所有任务<color=green>{0}</color>/{1}", SaveTaskProgress.progress, MaxProgress);
                }
            }
        }

        public TaskT7(Save_Task.TaskProgress taskProgress, List<int> parameters) : base(taskProgress, parameters)
        {
            
        }
    }
}