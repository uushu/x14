using cfg;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    [System.Serializable]
    public class Save_Towers : ASaveData
    {
        [System.Serializable]
        public class SaveTower
        {
            public int towerId;

            public int towerLevel;
        }

        public List<SaveTower> saveTowers;

        public override void SetDefaultData(GlobalConfigCategory category)
        {
            saveTowers = new();
        }

        public SaveTower SafeGetTower(int towerId)
        {
            foreach (var tower in saveTowers)
            {
                if (tower.towerId == towerId)
                {
                    return tower;
                }
            }

            var newTower = new SaveTower() { towerId = towerId, towerLevel = 1 };
            saveTowers.Add(newTower);
            return newTower;
        }

        public void LevelUpTower(int towerId, int addLevel)
        {
            var saveTower = SafeGetTower(towerId);
            saveTower.towerLevel += addLevel;
        }

        public int GetTowerGrowthLevel(int towerId)
        {
            var saveTower = SafeGetTower(towerId);
            return saveTower.towerLevel;
        }
    }
}