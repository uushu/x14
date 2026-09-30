using System;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    public class Timer
    {
        private readonly int _totalLoopCount;

        private int _currentLoopCount;

        public int CurrentLoopCount => _currentLoopCount;

        private readonly float _interval;

        private float _intervalRunTimer;

        private readonly Action<Timer> _callBack;

        public string TimerTag { get; private set; }

        public Timer(string timerTag, float firstDelay, float interval, int loopCount, Action<Timer> onCallback)
        {
            TimerTag = timerTag;
            
            _totalLoopCount = loopCount;

            _interval = interval;

            if (firstDelay > 0)
            {
                _intervalRunTimer = firstDelay;

                if (loopCount > 0)
                {
                    _totalLoopCount += 1;
                }
            }
            else
            {
                _intervalRunTimer = interval;
            }

            _callBack = onCallback;
            TimerMgr.Instance.AddTimer(this);
        }

        public Timer(string timerTag, float interval, int loopCount, Action<Timer> onCallback) : this(timerTag, 0,
            interval, loopCount, onCallback)
        {
        }

        public Timer(string timerTag, float interval, Action<Timer> onCallback) : this(timerTag, 0, interval, -1,
            onCallback)
        {
        }

        public bool OnUpdate(float deltaTime)
        {
            _intervalRunTimer -= deltaTime;

            if (_intervalRunTimer <= 0)
            {
                _currentLoopCount++;

                _callBack?.Invoke(this);

                if (_totalLoopCount < 0 || _currentLoopCount < _totalLoopCount)
                {
                    _intervalRunTimer = _interval;

                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            return $"定时器:{TimerTag} 间隔={_interval} 总次数:{_totalLoopCount} 当前次数:{CurrentLoopCount}";
        }
    }
}