using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace GameMain
{
    public class TimerMgr : AMonoManager<TimerMgr>
    {
        [ReadOnly] 
        [LabelText("计时器名字")]
        public List<string> timerTag;

        private readonly List<Timer> _timers = new();

        private readonly List<Timer> _removeList = new();
        
        public event Action<float> UpdaterActions;

        protected override void OnEnter()
        {
        }

        protected override void OnExit()
        {
        }

        public void AddTimer(Timer timer)
        {
            if (_timers.Contains(timer))
            {
                Debugger.LogError($"定时器已经添加:{timer}");
                return;
            }

            Debugger.Log($"添加定时器:{timer}");
            _timers.Add(timer);
        }

        public void RemoveTimer(Timer timer)
        {
            if (timer == null)
            {
                Debugger.LogError("参数错误:timer=null");
                return;
            }

            if (_removeList.Contains(timer))
            {
                Debugger.Log($"移除列表已经添加定时器:{timer}");
                return;
            }

            _removeList.Add(timer);
        }

        protected override void OnUpdateInternal(float deltaTime)
        {
            UpdaterActions?.Invoke(deltaTime);

            timerTag.Clear();

            for (int i = 0; i < _removeList.Count; i++)
            {
                if (_timers.Contains(_removeList[i]))
                {
                    Debugger.Log($"移除定时器:{_removeList[i]}");
                    _timers.Remove(_removeList[i]);
                }
                else
                {
                    Debugger.Log($"不存在定时器:{_removeList[i]}");
                }
            }

            _removeList.Clear();

            for (int i = _timers.Count - 1; i >= 0; i--)
            {
                var result = _timers[i].OnUpdate(deltaTime);

                if (result)
                {
                    RemoveTimer(_timers[i]);
                }
                else
                {
                    timerTag.Add(_timers[i].TimerTag);
                }
            }
        }
    }
}