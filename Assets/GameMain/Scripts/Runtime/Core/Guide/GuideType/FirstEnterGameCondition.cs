using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.NewGuide
{
    public class FirstEnterGameCondition : IGuideCondition
    {
        public EGuideTriggerType GuideTriggerType => EGuideTriggerType.FirstEnterGame;

        public bool Condition(int conditionId)
        {
            return true;
        }
    }
}