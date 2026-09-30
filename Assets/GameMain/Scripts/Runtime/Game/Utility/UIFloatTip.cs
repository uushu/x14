using TMPro;
using UnityEngine;

namespace GameMain
{
    public class UIFloatTip : UIBaseFloating
    {
        public TextMeshProUGUI txtContent;

        public void SetTextContent(string content)
        {
            txtContent.text = content;

            StartFloating();
        }
    }
}