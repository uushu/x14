using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameMain;
using TMPro;
#if UNITY_DOUYIN_GAME
using TTSDK;
#endif
using UnityEngine;
using UnityEngine.UI;

public class SubscribeWindow : ABaseWindow
{
   public UICustomButton close;
   
   public UICustomButton subscribeBtn;

   public Image RewardImage;
   public TextMeshProUGUI RewardNumText;
   
   List<ItemData_BagProp> rewardDatas = new();
   protected override void OnShow( object parameters)
   {
       rewardDatas.Clear();

       foreach (var item in ConfigMgr.Instance.Tables.GlobalConfigCategory.SubscribeProps)
       {
           rewardDatas.Add(new ItemData_BagProp(item.Key,item.Value));
       }

       if (rewardDatas.Count>0)
       {
           ResourceMgr.Instance.SetImageSprite(RewardImage,rewardDatas[0].Config.Id==GameTool.ItemGoldId?"nuohua_erji_jinbi":rewardDatas[0].Config.Sprite);
           RewardNumText.text =$"x{rewardDatas[0].PropCountString}" ;
       }
      
       
      close.AddListener(() =>
      {
          CloseSelfWindow();
      });
      subscribeBtn.AddListener( () =>
      {
          //CloseSelfWindow();
#if UNITY_DOUYIN_GAME
          if (PlatformMgr.Instance.Platform is PlatformDy platformDy)
          {
              platformDy.RequestFeedSubscribePower( async (data) =>
              {
                  if ((bool)data["success"])
                  {  
                      UIWindowMgr.Instance.ShowPromptBox("订阅成功");
                      
                      GameEventDefine.SendSubscribeSucess();

                      if (LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Common>().isGetSubscribeAward==false)
                      {
                          BagMgr.Instance.AddBagProp(rewardDatas);
                          UIWindowMgr.Instance.OpenWindow<CommGetView>(rewardDatas);
                          LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Common>().isGetSubscribeAward = true;
                      }
                      CloseSelfWindow();
                  }
                  else
                  {
                     // UIWindowMgr.Instance.ShowPromptBox((string)data["errMsg"]);
                  }
                
              }, (error,errorStr) =>
              {
                  CloseSelfWindow();
                  //UIWindowMgr.Instance.ShowPromptBox(errorStr);
              });
          }
#endif
      });   
   }
}
