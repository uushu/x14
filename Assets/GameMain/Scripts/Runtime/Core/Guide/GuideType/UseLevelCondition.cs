using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.NewGuide
{
    public class UseLevelCondition : IGuideCondition
    {
        public EGuideTriggerType GuideTriggerType => throw new System.NotImplementedException();

        public bool Condition(int conditionId)
        {
            return false;
        }
    }
}