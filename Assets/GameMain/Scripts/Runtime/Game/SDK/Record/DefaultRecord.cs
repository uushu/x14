using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    public class DefaultRecord : IRecordEvent
    {
        public void Initialize()
        {
        }

        public void ReportAdShowEvent(string adType, string adUnitId, Dictionary<string, object> properties)
        {
        }

        public void ReportEvent(string eventName, Dictionary<string, object> properties)
        {
        }

        public void ReportEvent(string eventName)
        {
        }

        public void ReportEvent(ERecordUserProp recordProp, Dictionary<string, object> properties)
        {
        }

        public void ReportEvent(ERecordUserProp recordProp, string key, object value)
        {
        }

        public void UnInitialize()
        {
        }
    }
}