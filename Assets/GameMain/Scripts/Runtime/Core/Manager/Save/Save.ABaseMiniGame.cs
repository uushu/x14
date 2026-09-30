using cfg;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public abstract class Save_ABaseMiniGame : ASaveData
    {
        public int levelId;

        //最大连胜次数
        public int maxContinusSuccessCount;

        //当前的连胜次数
        public int currentSuccessCount;

        //无尽模式最高分
        public int endlessBestScore;

        //无尽模式的本局分数
        public int leastBestScore;

        //无尽模式新纪录
        public bool endlessIsNewRecord;

        //无尽模式挑战次数
        public int currentChanglleCount;

        //无尽称号Id
        public int myTitleId;

        public override void SetDefaultData(GlobalConfigCategory category)
        {
            levelId = 1001;
        }

        public void OnGameFailed()
        {
            currentSuccessCount = 0;
        }

        public void OnGameSuccess()
        {
            currentSuccessCount++;
            maxContinusSuccessCount = Mathf.Max(currentSuccessCount, maxContinusSuccessCount);
        }

        public void SetNextLevelId(int nextLevelId)
        {
            Debugger.Log($"设置下一关成功<{GetType().Name}> nextLevelId:{nextLevelId}");
            levelId = nextLevelId;
        }

        public void SetEndlessMaxScore(int endlessScore, int titleCofigId)
        {
            leastBestScore = endlessScore;

            if (endlessScore >= endlessBestScore)
            {
                endlessBestScore = endlessScore;
                endlessIsNewRecord = true;
                myTitleId = titleCofigId;
                GameEntrance.UpdateRoleEndlessBestRank(endlessScore, titleCofigId);
            }
            else
            {
                endlessIsNewRecord = false;
            }
        }

        public void AddChanglleCount()
        {
            currentChanglleCount++;
        }
    }
}