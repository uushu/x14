using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.BTree
{
    public class LogNode : ABaseActionNode
    {
        public enum ELogLevel
        {
            Info,
            Warning, 
            Error
        }

        public ELogLevel logLevel;

        public string logString;

        protected override void OnStart()
        {
            switch (logLevel)
            {
                case ELogLevel.Info:

                    Debug.Log(logString);

                    break;
                case ELogLevel.Warning:

                    Debug.LogWarning(logString);

                    break;
                case ELogLevel.Error:

                    Debug.LogError(logString);

                    break;
                default:
                    break;
            }

            SetFinish(true);
        }

        protected override void OnCancel()
        {
        }

    }
}