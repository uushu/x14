using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using GameMain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankItem : EnhancedScrollerCellView
{
    public Image rankImage;
    public TextMeshProUGUI rankText;
    public Image bgImage;
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;

    public GameObject TitleGo;
    public Image TitleImage;
    public Image TitleName;
    public TextMeshProUGUI TitlescoreText;
    
    public Sprite[] rankSprites; //0:第一名 1:第二名 2:第三名 
    public Sprite[] bgSprites; //0:第一名 1:第二名 2:第三名 3:普通 4:自己
    public string[] colors; //0:第一名 1:第二名 2:第三名 3:普通 4:自己

    private RankRoleInfo data;
    public void SetData(RankRoleInfo pData,int RankType )
    {
        this.data = pData;
        if (int.TryParse(pData.rank, out int rankInt))
        {
            if (rankInt <= 3)
            {
                rankImage.gameObject.SetActive(true);
                rankText.gameObject.SetActive(false);
                rankImage.sprite = rankSprites[rankInt - 1];
            }
            else
            {
                rankImage.gameObject.SetActive(false);
                rankText.gameObject.SetActive(true);
            }

            bgImage.sprite = pData.isSelf ? bgSprites[4] : bgSprites[rankInt <= 3 ? rankInt - 1 : 3];
        }
        else//"未上榜"
        {
            rankImage?.gameObject.SetActive(false);
            rankText.gameObject.SetActive(true);
            bgImage.sprite = pData.isSelf ? bgSprites[4] : bgSprites[3];
        }
      var usecolor=  ColorUtility.TryParseHtmlString(colors[ rankInt <= 3 ? rankInt - 1<0?0:rankInt - 1 : 3 ], out Color color) ? color : Color.white;
      if ( pData.isSelf)
      {   rankImage.gameObject.SetActive(false);
          rankText.gameObject.SetActive(true);
          usecolor=  ColorUtility.TryParseHtmlString(colors[ 4 ], out Color color2) ? color2 : Color.white;
      }
        rankText.text = pData.rank;
        rankText.color= usecolor;
        nameText.text = pData.name;
        nameText.color = usecolor;
        TitleGo.gameObject.SetActive(RankType ==2);
        levelText.gameObject.SetActive(RankType ==1||RankType ==3);
        if (RankType ==1)
        {
            levelText.text =pData.level>0? $"第{pData.level-1000}关":$"第{0}关";
        }
        else if (RankType ==2)
        {
            //尝试获取指定称号配置
           var config= ConfigMgr.Instance.Tables.GardenEndlessTitleCategory.GetOrDefault(data.titleId);
           if(config!=null)
           {
               ResourceMgr.Instance.SetImageSprite(TitleImage, config.TitleIcon, true);
               ResourceMgr.Instance.SetImageSprite(TitleName, config.TitleSprite, false);
               TitlescoreText.text = $"{pData.level}";
           }
           else
           { 
               //找不到就获取默认称号配置(ID默认称号是1)
               var defaultConfig= ConfigMgr.Instance.Tables.GardenEndlessTitleCategory.GetOrDefault(1);
               if(defaultConfig!=null)
               {
                   ResourceMgr.Instance.SetImageSprite(TitleImage,defaultConfig.TitleIcon,true);
                   ResourceMgr.Instance.SetImageSprite(TitleName,defaultConfig.TitleSprite,false);
                   TitlescoreText.text =$"{pData.level}";
               
               }
               else
               {
                   //找不到默认称号就隐藏
                   TitleGo.SetActive(false);
               }
           }
        }
        else
        {
            levelText.text =$"{pData.level}点";
        }
     
        levelText.color=usecolor;
        StartCoroutine(icon.SetHttpUrl(pData.iconUrl));
    }
}
