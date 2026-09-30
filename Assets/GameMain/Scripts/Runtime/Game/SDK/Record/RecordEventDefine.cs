using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    public class RecordEventDefine
    {
        public class InitRecordResult : IEventMessage
        {
            public bool result;
        }

        public static void SendRecordEventResult(bool result)
        {
            UniEvent.SendMessage(new InitRecordResult() { result = result });
        }
    }
}