using cfg;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    [System.Serializable]
    public class Save_Achievement : ASaveData
    {
        public SerializableDictionary<int, AchievementProgress> AllAchievmentDic=new ();
      
        public override void SetDefaultData(GlobalConfigCategory category)
        {
        }
    }
    
    [System.Serializable]
    public class AchievementProgress
    {
        public int achievementConfigId;

        public int progress;
        
        public bool isClaim;
    }

}