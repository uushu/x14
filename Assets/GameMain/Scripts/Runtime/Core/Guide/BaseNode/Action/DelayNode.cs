using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.BTree
{
    public class DelayNode : ABaseActionNode
    {
        private Timer _timer;

        public float delayTime;

        public DelayNode(float time)
        {
            delayTime = time;
        }

        protected override void OnStart()
        {
            StartTimer();
        }

        private void StartTimer()
        {
            StopTimer();

            _timer = new("行为树延迟节点", delayTime, 1, OnTimerComplete);
        }

        private void OnTimerComplete(Timer timer)
        {
            SetFinish(true);
        }

        private void StopTimer()
        {
            if (_timer != null)
            {
                TimerMgr.Instance.RemoveTimer(_timer);
                _timer = null;
            }
        }

        protected override void OnCancel()
        {
            StopTimer();
        }
    }
}