using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public partial class GameEventDefine
    {
        public struct NewGuide_FirstEnterGame : IEventMessage
        {
            public EMiniGameType eGameType;
        }

        public static void SendFirstEnterGame(EMiniGameType eMiniGameType)
        {
            UniEvent.SendMessage(new NewGuide_FirstEnterGame()
            {
                eGameType = eMiniGameType,
            });
        }
    }
}