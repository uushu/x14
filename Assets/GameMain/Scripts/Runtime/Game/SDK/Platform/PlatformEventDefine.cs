using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    public static class PlatformEventDefine
    {
        public class GetOpenIdSuccess : IEventMessage
        {
            public string openId;
        }

        public static void SendGetOpenIdSuccess(string _openId)
        {
            UniEvent.SendMessage(new GetOpenIdSuccess() {
                openId = _openId
            });
        }

        public class SaveDataToServer : IEventMessage
        {
        }

        public static void SendSaveDataToServer()
        {
            UniEvent.SendMessage(new SaveDataToServer());
        }
    }
}