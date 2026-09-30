using System.Collections;
using TMPro;
using UnityEngine;

namespace GameMain.Garden
{
    public class ItemUI_ScoreProgress : MonoBehaviour
    {
        public TextMeshProUGUI textScore;

        public GameObject goComplete;

        public GameObject goNotComplete;

        public int ScoreTarget { get; private set; }

        public void RefreshProgressItem(int socreTarget, int currentScore)
        {
            ScoreTarget = socreTarget;
            textScore.text = UtilityTool.ShortNumberToString(socreTarget);
            RefreshProgressItem(currentScore);
        }

        public void RefreshProgressItem(int currentScore)
        {
            var isComplete = currentScore >= ScoreTarget;
            goComplete.SetActive(isComplete);
            goNotComplete.SetActive(isComplete == false);
        }
    }
}