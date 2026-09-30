using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.FlowerBlock
{
    public class BlockEndlessScoreProgress : AEndlessScoreProgress
    {
        protected override List<int> GetMyTargetScores(int currentScore, int lastStartScore, int maxCount)
        {
            var compareScore = Mathf.Min(currentScore, lastStartScore);

            var list = ConfigMgr.Instance.Tables.BlockEndlessScoreTargetCategory.DataList;

            var configIndex = 0;

            for (int i = 0; i < list.Count - 2; i++)
            {
                var before = list[i].Score;
                var after = list[i + 1].Score;

                if (compareScore >= before && compareScore < after)
                {
                    configIndex = i;
                    break;
                }
            }

            int fromConfigIndex = configIndex;
            fromConfigIndex = Mathf.Max(0, fromConfigIndex);

            var toConfigIndex = fromConfigIndex + maxCount - 1;
            toConfigIndex = Mathf.Min(list.Count - 1, toConfigIndex);

            var result = new List<int>();

            for (int i = fromConfigIndex; i <= toConfigIndex; i++)
            {
                result.Add(list[i].Score);
            }

            return result;
        }
    }
}