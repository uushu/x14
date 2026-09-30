using UnityEngine;

namespace GameMain
{
    public interface IManager
    {
        int Priority { get; }

        void Initialize();

        void OnUpdate(float deltaTime);

        void UnInitialize();
    }

    public abstract class AManager : IManager
    {
        public int Priority => 999;

        public virtual void Initialize()
        {
        }

        public virtual void OnUpdate(float deltaTime)
        {
        }

        public virtual void UnInitialize()
        {
        }
    }

    public abstract class AMonoManager<T> : MonoBehaviour, IManager where T : AMonoManager<T>, IManager
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<T>();
                }

                return _instance;
            }
        }

        public virtual int Priority { get; } = 999;

        public bool IsInitialize { get; set; }

        public void Initialize()
        {
            if (IsInitialize)
            {
                Debugger.LogError($"管理器已经初始化:{typeof(T)}");
                return;
            }
            IsInitialize = true;
            
            OnEnter();
        }

        protected virtual void OnEnter() { }

        public void OnUpdate(float deltaTime)
        {
            if (IsInitialize)
            {
                OnUpdateInternal(deltaTime);
            }
        }

        protected virtual void OnUpdateInternal(float deltaTime) { }

        public void UnInitialize()
        {
            if (IsInitialize == false)
            {
                Debugger.LogError($"管理器已经被反初始化:{typeof(T)}");
                return;
            }
            IsInitialize = false;
            
            OnExit();
        }

        protected virtual void OnExit() { }

        public override string ToString()
        {
            return $"{GetType()} Priority:{Priority}";
        }
    }
}