using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Pool;

namespace GameMain
{
    public class MessageCenter
    {
        private static readonly Dictionary<Type, List<Delegate>> _handlers = new();
        private static readonly Dictionary<Type, List<Delegate>> _asyncHandlers = new();

        public static void Subscribe<T>(Action<T> callBack) where T : IMessage
        {
            var messageType = typeof(T);
            if (_handlers.TryGetValue(messageType, out var list) == false)
            {
                list = new List<Delegate>();
                _handlers.Add(messageType, list);
            }

            if (list.Contains(callBack) == false)
            {
                list.Add(callBack);
            }
            else
            {
                Debugger.LogError($"重复添加事件:{messageType}");
            }
        }

        public static void SubscribeAsync<T>(Func<T, UniTask> callBack) where T : IMessage
        {
            var messageType = typeof(T);
            if (_asyncHandlers.TryGetValue(messageType, out var list) == false)
            {
                list = new List<Delegate>();
                _asyncHandlers.Add(messageType, list);
            }

            if (list.Contains(callBack) == false)
            {
                list.Add(callBack);
            }
            else
            {
                Debugger.LogError($"重复添加事件:{messageType}");
            }
        }

        public static void SubscribeOnce<T>(Action<T> callBack) where T : IMessage
        {
            Action<T> wraper = null;
            wraper = (message) =>
            {
                callBack(message);
                UnSubscribe(wraper);
            };

            Subscribe(wraper);
        }

        public static void UnSubscribe<T>(Action<T> callBack) where T : IMessage
        {
            var messageType = typeof(T);
            if (_handlers.TryGetValue(messageType, out var list))
            {
                var isOk = list.Remove(callBack);

                if (isOk == false)
                {
                    Debugger.LogError($"事件移除失败:{messageType}");
                }

                if (list.Count == 0)
                {
                    _handlers.Remove(messageType);
                }
            }
        }

        public static void UnSubscribeAsync<T>(Func<T, UniTask> callBack) where T : IMessage
        {
            var messageType = typeof(T);
            if (_asyncHandlers.TryGetValue(messageType, out var list))
            {
                var isOk = list.Remove(callBack);

                if (isOk == false)
                {
                    Debugger.LogError($"事件移除失败:{messageType}");
                }

                if (list.Count == 0)
                {
                    _asyncHandlers.Remove(messageType);
                }
            }
        }

        public static void Publish<T>(T message) where T : IMessage
        {
            var messageType = typeof(T);
            if (_handlers.TryGetValue(messageType, out var list))
            {
                foreach (var callBack in list.Cast<Action<T>>())
                {
                    callBack.Invoke(message);
                }
            }
            else
            {
                Debugger.LogError($"没有监听消息：{messageType}");
            }
        }

        public static async UniTask PublishAsync<T>(T message) where T : IMessage
        {
            var messageType = typeof(T);
            if (_asyncHandlers.TryGetValue(messageType, out var list))
            {
                var tasks = ListPool<UniTask>.Get();

                foreach (var callBack in list.Cast<Func<T, UniTask>>())
                {
                    await callBack.Invoke(message);
                }
            }
            else
            {
                Debugger.LogError($"没有监听消息 Async：{messageType}");
            }
        }
    }
}