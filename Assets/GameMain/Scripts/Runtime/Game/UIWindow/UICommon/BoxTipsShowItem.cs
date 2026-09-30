using System.Collections;
using System.Collections.Generic;
using GameMain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoxTipsShowItem : MonoBehaviour
{
    public Image bg;
    public Image icon;
    public TextMeshProUGUI numText;
    
    public void Init(string bgPath,string iconPath, int num)
    {
        ResourceMgr.Instance.SetImageSprite(bg, bgPath);
        ResourceMgr.Instance.SetImageSprite(icon, iconPath);
        numText.text = num.ToString();
    }
}
