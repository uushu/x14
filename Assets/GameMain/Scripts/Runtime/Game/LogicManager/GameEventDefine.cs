using GameMain.Garden;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public static partial class GameEventDefine
    {
        #region 任务

        public class Task_AddProgress : IEventMessage
        {
            public int TaskTypeId;

            public int AddTaskProgress;
        }

        public static void SendAddTaskProgress(int taskTypeId, int addProgress)
        {
            UniEvent.SendMessage(new Task_AddProgress()
            {
                TaskTypeId = taskTypeId,
                AddTaskProgress = addProgress
            });
        }

        #endregion

        public class GameShare : IEventMessage
        {
            //分享
            public int ShareCount;
        }

        public static void SendGameShare(int count)
        {
            UniEvent.SendMessage(new GameShare()
            {
                ShareCount = count
            });
        }

        //使用道具
        public class UseBagProp : IEventMessage
        {
            public int BagPropId;

            public float BagPropCount;
        }

        public static void SendUseBagProp(int bagPropId, float bagPropCount)
        {
            UniEvent.SendMessage(new UseBagProp()
            {
                BagPropId = bagPropId,
                BagPropCount = bagPropCount,
            });
        }

        //获得道具
        public class GetBagProp : IEventMessage
        {
            public int BagPropId;

            public float BagPropCount;
        }

        public static void SendGetBagProp(int bagPropId, float bagPropCount)
        {
            UniEvent.SendMessage(new GetBagProp()
            {
                BagPropId = bagPropId,
                BagPropCount = bagPropCount,
            });
        }
        
        public class GameLookAds : IEventMessage
        {
            //看广告
            public int LookAdCount;
        }
        
        public static void SendGameLookAds(int count)
        {
            UniEvent.SendMessage(new GameLookAds() {
                LookAdCount = count
            });
        }
        
        public class GameCardChange : IEventMessage
        {
            public int cardId;
        }
        
        public static void SendGameCardChange(int _cardId)
        {
            UniEvent.SendMessage(new GameCardChange() {
                cardId = _cardId,
            });
        }
        
        public class GameMainCardChange : IEventMessage
        {
            public int cardId;
        }
        
        public static void SendGameMainCardChange(int _cardId)
        {
            UniEvent.SendMessage(new GameMainCardChange() {
                cardId = _cardId,
            });
        }

        
        public class GameShopBoxExpChange : IEventMessage 
        {
        }
        
        public static void SendShopBoxExpChange() 
        {
            UniEvent.SendMessage(new GameShopBoxExpChange() { });
        }
        
        public class FlowerGardenLevelPass : IEventMessage 
        {
        }
        
        public static void SendFlowerGardenLevelPassChange() 
        {
            UniEvent.SendMessage(new FlowerGardenLevelPass() { });
        }
        
        
        public class FlowerGardenPass : IEventMessage
        {
            public int level;
        }
        
        public static void SendFlowerGardenPass(int leve) 
        {
            UniEvent.SendMessage(new FlowerGardenPass() { level = leve});
        }

        
        public class CollectFlowerAdCountChange : IEventMessage 
        {
        }
        
        public static void SendCollectFlowerAdCountChange() 
        {
            UniEvent.SendMessage(new CollectFlowerAdCountChange() { });
        }
        
        public class BuyPowerCountChange : IEventMessage 
        {
        }
        
        public static void SendBuyPowerCountChange() 
        {
            UniEvent.SendMessage(new BuyPowerCountChange() { });
        }

        public class CollectFlowerTreasureChestChange : IEventMessage
        {
       
        }

        public static void SendCollectFlowerTreasureChestChange()
        {
            UniEvent.SendMessage(new CollectFlowerTreasureChestChange()
            {
               
            });
        }
        
        public class PiggyBankGoldChange : IEventMessage
        {
       
        }

        public static void SendPiggyBankGoldChange()
        {
            UniEvent.SendMessage(new PiggyBankGoldChange()
            {
               
            });
        }
        
        
        public class AchievementProgressChange : IEventMessage
        {
       
        }

        public static void SendAchievementProgressChange()
        {
            UniEvent.SendMessage(new AchievementProgressChange()
            {
               
            });
        }
        
        public class SubscribeSucess : IEventMessage
        {
        }

        public static void SendSubscribeSucess()
        {
            UniEvent.SendMessage(new SubscribeSucess()
            {
            });
        }

        public class CollectFlowerChange : IEventMessage
        {
        }
        public static void SendCollectFlowerChange()
        {
            UniEvent.SendMessage(new CollectFlowerChange()
            {
            });
        }
    }
}