using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameMain
{
    public enum EStatus
    {
        [LabelText("准备")]
        Ready,
        [LabelText("游戏中")]
        Gaming,
        [LabelText("复活")]
        Relive,
        [LabelText("暂停")]
        Pause,
        [LabelText("失败")]
        Failed,
        [LabelText("成功")]
        Success,
    }

    public abstract class AGameBaseRoot<T> : AMonoManager<T> where T : AMonoManager<T>, IManager
    {
        [LabelText("小游戏类型")]
        public abstract EMiniGameType eMiniGameType { get; }

        [LabelText("当前游戏状态")]
        public EStatus currentStatus;
        private EStatus _lastStatus;

        //是否正在播放动画
        private bool _isPlayAnimationFlag;

        public bool IsForbidenInput => currentStatus != EStatus.Gaming || _isPlayAnimationFlag == true;

        public List<ILogicBase> logicBases;

        public void ChangeStatus(EStatus newStatus)
        {
            if (currentStatus == newStatus)
            {
                Debugger.LogError($"游戏:{eMiniGameType}, currentStatus={currentStatus}, 未改变");
                return;
            }

            switch (newStatus)
            {
                case EStatus.Ready:
                    break;
                case EStatus.Gaming:
                    break;
                case EStatus.Relive:
                    break;
                case EStatus.Pause:
                    break;
                case EStatus.Failed:

                    OnGameFailed();
                    break;
                case EStatus.Success:

                    OnGameSuccess();
                    break;
                default:
                    break;
            }

            currentStatus = newStatus;
            Debugger.Log($"{eMiniGameType}=当前游戏状态={currentStatus}");
        }

        public void StartAnimation()
        {
            if (_isPlayAnimationFlag)
            {
                Debugger.LogError($"{GetType().Name}:当前处于播放动画状态!!!");
                return;
            }

            _isPlayAnimationFlag = true;
        }

        public void StopAnimation()
        {
            if (_isPlayAnimationFlag == false)
            {
                Debugger.LogError($"{GetType().Name} 注意停止动画:当前状态不是动画状态");
                return;
            }
            _isPlayAnimationFlag = false;
        }

        public virtual void StartGame(int levelId, params object[] parameters)
        {
            ChangeStatus(EStatus.Gaming);
            OnGameStart();
        }

        public void PauseGame()
        {
            if (_lastStatus == EStatus.Pause)
            {
                Debugger.LogError($"游戏:{eMiniGameType},已经处于暂停状态");
                return;
            }

            _lastStatus = currentStatus;
            currentStatus = EStatus.Pause;
        }

        public void ResumeGame()
        {
            //状态判断
            if (_lastStatus != currentStatus)
            {
                ChangeStatus(_lastStatus);
            }
        }
        
        protected void OnGameStart()
        {
            foreach (var logic in logicBases)
            {
                logic.OnStart();
            }
        }

        protected void OnGameSuccess()
        {
            foreach (var logic in logicBases)
            {
                logic.OnSuccess();
            }
        }

        protected void OnGameFailed() 
        {
            foreach (var logic in logicBases)
            {
                logic.OnFailed();
            }
        }

#if UNITY_EDITOR

        [Button("刷新")]
        protected virtual void OnValidate()
        {
            logicBases = transform.GetComponentsInChildren<ILogicBase>().ToList();
        }

#endif
    }
}