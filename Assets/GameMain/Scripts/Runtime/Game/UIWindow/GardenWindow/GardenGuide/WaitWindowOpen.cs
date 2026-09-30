using GameMain.BTree;
using System;
using UnityEngine;

namespace GameMain.Garden
{
    public class WaitWindowOpen : ABaseActionNode
    {
        public string WindowName;

        public WaitWindowOpen(string windowName)
        {
            WindowName = windowName;
        }

        protected override void OnStart()
        {
            TimerMgr.Instance.UpdaterActions += OnUpdate;
        }

        private void OnUpdate(float deltaTime)
        {
            if (string.IsNullOrEmpty(WindowName))
            {
                Debug.LogError($"等待窗口打开失败 WindowName:{WindowName}=NULL");
                return;
            }

            if (UIWindowMgr.Instance.GetWindow(WindowName) != null)
            {
                TimerMgr.Instance.UpdaterActions -= OnUpdate;
                SetFinish(true);
            }
        }

        protected override void OnCancel()
        {
            TimerMgr.Instance.UpdaterActions -= OnUpdate;
        }

    }
}