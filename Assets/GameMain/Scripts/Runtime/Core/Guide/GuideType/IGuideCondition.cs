using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.NewGuide
{
    public interface IGuideCondition
    {
        public EGuideTriggerType GuideTriggerType { get; }

        public bool Condition(int conditionId);
    }
}