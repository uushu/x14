using cfg;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    [System.Serializable]
    public class Save_NewGuide : ASaveData
    {
        //激活但是未完成的强制引导
        public List<int> active_must_guides;

        //已经完成的引导
        public List<int> finish_guides;

        public override void SetDefaultData(GlobalConfigCategory category)
        {
            active_must_guides = new();
            finish_guides = new();
        }

        public void AddFinishGuide(int guideId)
        {
            finish_guides.Add(guideId);
        }

        public bool GuideIsFinish(int guideId)
        {
            return finish_guides.Contains(guideId);
        }

        public void AddActiceMustGuide(int mustGuideId)
        {
            if (active_must_guides.Contains(mustGuideId))
            {
                Debugger.LogError($"激活强制引导 mustGuideId:{mustGuideId} 已经加入列表");
                return;
            }

            active_must_guides.Add(mustGuideId);
        }

        public void FinishMustGuide(int mustGuideId)
        {
            active_must_guides.Remove(mustGuideId);

            AddFinishGuide(mustGuideId);
        }
    }
}