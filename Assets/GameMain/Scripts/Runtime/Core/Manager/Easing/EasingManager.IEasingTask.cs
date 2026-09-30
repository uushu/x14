using Cysharp.Threading.Tasks;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace GameMain
{
    public class EasingManager
    {
        public static IEasingTask DoFloat(float from, float to, float duration, UnityAction<float> action, float delay = 0)
        {
            return new FloatEasingTask(from, to, duration, delay, action);
        }

        public static IEasingTask DoAfter(float seconds, Action action, bool unscaledTime = false)
        {
            return new WaitTask(seconds, unscaledTime).SetOnFinish(action);
        }

        public static IEasingTask DoAfter(Func<bool> condition)
        {
            return new WaitForConditionTask(condition);
        }

        public static IEasingTask DoNextFrame()
        {
            return new NextFrameTask();
        }
        public static IEasingTask DoNextFrame(Action action)
        {
            return new NextFrameTask().SetOnFinish(action);
        }
    }

    public interface IEasingTask
    {
        bool IsActive { get; }
        IEasingTask SetEasing(EasingType easingType);
        IEasingTask SetEasingCurve(AnimationCurve easingCurve);
        IEasingTask SetOnFinish(Action onfinish);
        IEasingTask SetUnscaledTime(bool unscaledTime);
        IEasingTask SetDelay(float delay);
        void Cancel();
    }

    public abstract class AEmptyEasingTask : IEasingTask
    {
        public bool IsActive { get; protected set; }
        protected Action onFinishCallback;
        protected float delayExecute;
        protected bool unscaledTime;
        protected CancellationTokenSource cts;

        protected bool useCurve;
        protected AnimationCurve easingCurve;
        protected EasingType easingType = EasingType.Linear;

        public IEasingTask SetDelay(float delay)
        {
            this.delayExecute = delay;
            return this;
        }

        public IEasingTask SetEasing(EasingType easingType)
        {
            this.easingType = easingType;
            useCurve = false;
            return this;
        }

        public IEasingTask SetEasingCurve(AnimationCurve easingCurve)
        {
            this.easingCurve = easingCurve;
            useCurve = true;
            return this;
        }

        public IEasingTask SetOnFinish(Action onfinish)
        {
            this.onFinishCallback = onfinish;
            return this;
        }

        public IEasingTask SetUnscaledTime(bool unscaledTime)
        {
            this.unscaledTime = unscaledTime;
            return this;
        }

        public void Cancel()
        {
            if (cts != null && cts.IsCancellationRequested == false)
            {
                Debug.Log($"任务被取消:{GetType()}");
                cts.Cancel();
                cts = null;
            }

            IsActive = false;
        }

        protected async UniTask StartTask()
        {
            IsActive = true;
            cts = new CancellationTokenSource();
            await UniTask.WaitForSeconds(this.delayExecute, unscaledTime, PlayerLoopTiming.Update, cts.Token);
            await OnExecuteTask();
            if (cts != null && cts.IsCancellationRequested == false)
            {
                onFinishCallback?.Invoke();
            }
            IsActive = false;
            cts = null;
        }

        protected virtual async UniTask OnExecuteTask() { await UniTask.CompletedTask; }
    }

    public class NextFrameTask : AEmptyEasingTask
    {
        public NextFrameTask()
        {
            StartTask().Forget();
        }

        protected override async UniTask OnExecuteTask()
        {
            await UniTask.Yield(PlayerLoopTiming.Update, cts.Token);
        }
    }

    public class NextFixedFrameTask : AEmptyEasingTask
    {
        public NextFixedFrameTask()
        {
            StartTask().Forget();
        }

        protected override async UniTask OnExecuteTask()
        {
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cts.Token);
        }
    }

    public class WaitTask : AEmptyEasingTask
    {
        protected float duration;

        public WaitTask(float duration, bool unscaledTime)
        {
            this.duration = duration;
            this.unscaledTime = unscaledTime;
        }

        protected override async UniTask OnExecuteTask()
        {
            await UniTask.WaitForSeconds(duration, unscaledTime, PlayerLoopTiming.Update, cts.Token);
        }
    }

    public class WaitForConditionTask : AEmptyEasingTask
    {
        private Func<bool> _condition;

        public WaitForConditionTask(Func<bool> condition)
        {
            if (_condition == null)
            {
                throw new ArgumentNullException($"{nameof(WaitForConditionTask)}参数错误 condition=null");
            }

            _condition = condition;
            StartTask().Forget();
        }

        protected override async UniTask OnExecuteTask()
        {
            do
            {
                await UniTask.Yield();

            } while (!_condition());
        }
    }

    public abstract class EasingTask<T> : AEmptyEasingTask
    {
        protected T fromValue;
        protected T toValue;
        protected float duration;

        protected UnityAction<T> onValueChange;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract T Lerp(T a, T b, float t);

        public EasingTask(T from, T to, float duration, float delay, UnityAction<T> callback)
        {
            this.fromValue = from;
            this.toValue = to;
            this.duration = duration;
            this.onValueChange = callback;
            this.delayExecute = delay;
            StartTask().Forget();
        }

        protected override async UniTask OnExecuteTask()
        {
            float time = 0;

            while (time < duration && cts?.IsCancellationRequested == false)
            {
                time += unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

                float t;
                if (useCurve)
                {
                    t = easingCurve.Evaluate(time / duration);
                }
                else
                {
                    t = EasingFunctions.ApplyEasing(time / duration, easingType);
                }

                T value = Lerp(fromValue, toValue, t);
                onValueChange?.Invoke(value);

                await UniTask.Yield();
            }

            onValueChange?.Invoke(toValue);
        }
    }

    public class FloatEasingTask : EasingTask<float>
    {
        public FloatEasingTask(float from, float to, float duration, float delay, UnityAction<float> valueChange)
            : base(from, to, duration, delay, valueChange)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override float Lerp(float a, float b, float t)
        {
            return Mathf.LerpUnclamped(a, b, t);
        }
    }

    public class Vector3EasingTask : EasingTask<Vector3>
    {
        public Vector3EasingTask(Vector3 from, Vector3 to, float duration, float delay, UnityAction<Vector3> valueChange)
            : base(from, to, duration, delay, valueChange)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            return Vector3.LerpUnclamped(a, b, t);
        }
    }

    public class Vector2EasingTask : EasingTask<Vector2>
    {
        public Vector2EasingTask(Vector2 from, Vector2 to, float duration, float delay, UnityAction<Vector2> valueChange) :
            base(from, to, duration, delay, valueChange)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return Vector2.LerpUnclamped(a, b, t);
        }
    }

    public class ColorEasingTask : EasingTask<Color>
    {
        public ColorEasingTask(Color from, Color to, float duration, float delay, UnityAction<Color> valueChange)
            : base(from, to, duration, delay, valueChange)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Color Lerp(Color a, Color b, float t)
        {
            return Color.LerpUnclamped(a, b, t);
        }
    }
}