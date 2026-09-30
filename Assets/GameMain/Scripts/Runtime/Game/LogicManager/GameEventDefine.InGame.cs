using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public partial class GameEventDefine
    {
        #region 局内

        public struct UseInGameBagProp : IEventMessage
        {
            public int BagPropId;

            public int BagProPCount;

            public bool IsLookAd;
        }

        public static void SendUseInGameBagProp(int bagPropId, int bagPropCount, bool isLookAd)
        {
            UniEvent.SendMessage(new UseInGameBagProp()
            {
                BagPropId = bagPropId,
                BagProPCount = bagPropCount,
                IsLookAd = isLookAd
            });
        }

        public struct CollectButterFly : IEventMessage
        {
            public EMiniGameType GameType;

            public int LevelId;

            //key=鲜花Id, value=蝴蝶数量
            public Dictionary<int, int> ButterflyDict;
        }

        public static void SendCollectButterfly(EMiniGameType gameType, int levelId, Dictionary<int, int> butterfly)
        {
            UniEvent.SendMessage(new CollectButterFly()
            {
                GameType = gameType,
                LevelId = levelId,
                ButterflyDict = butterfly,
            });
        }

        public struct GameSuccessResult : IEventMessage
        {
            //游戏的类型
            public EMiniGameType GameType;

            //关卡Id
            public int LevelId;
        }

        public static void SendGameSuccessResult(EMiniGameType gameType, int levelId)
        {
            UniEvent.SendMessage(new GameSuccessResult()
            {
                GameType = gameType,
                LevelId = levelId,
            });
        }

        public struct GameRemoveRowCols : IEventMessage
        {
            public int RowCount;

            public int ColCount;
        }

        public static void SendGameRemoveRowCols(int rowCount, int colCount)
        {
            UniEvent.SendMessage(new GameRemoveRowCols()
            {
                RowCount = rowCount,
                ColCount = colCount,
            });
        }

        #endregion
    }
}