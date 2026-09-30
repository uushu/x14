using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
	public static class UniEvent
	{
/// <summary>
/// 用于封装事件消息的包装类，包含消息和对应的帧数信息
/// </summary>
		private class PostWrapper
		{
    /// <summary>
    /// 消息被发送时所在的帧数
    /// </summary>
			public int PostFrame;

    /// <summary>
    /// 封装的事件消息接口
    /// </summary>
			public IEventMessage Message;

    /// <summary>
    /// 释放资源方法，将字段重置为默认值
    /// </summary>
			public void OnRelease()
			{
				PostFrame = 0;  // 将帧数重置为0
				Message = null; // 将消息引用置空
			}
		}

		private static readonly Dictionary<Type, LinkedList<Action<IEventMessage>>> _listeners = new Dictionary<Type, LinkedList<Action<IEventMessage>>>(100);
		public static Dictionary<Type, LinkedList<Action<IEventMessage>>> Listeners => _listeners;
		
		private static readonly List<PostWrapper> _postingList = new List<PostWrapper>(100);

/// <summary>
/// 处理消息队列中的消息
/// </summary>
		internal static void Update()
		{
			for (int i = _postingList.Count - 1; i >= 0; i--)
			{
				var wrapper = _postingList[i];
				if (UnityEngine.Time.frameCount > wrapper.PostFrame)
				{
					SendMessage(wrapper.Message);
					_postingList.RemoveAt(i);
				}
			}
		}

		public static void ClearAll()
		{
			foreach (var eventId in _listeners)
			{
				eventId.Value.Clear();
			}

			_listeners.Clear();

			_postingList.Clear();
		}

		public static void ClearAll<TEvent>() where TEvent : IEventMessage
		{
			System.Type eventType = typeof(TEvent);
			
			if (_listeners.TryGetValue(eventType, out var listener))
			{
				listener.Clear();
			}
		}

		public static void AddListener<TEvent>(System.Action<IEventMessage> listener) where TEvent : IEventMessage
		{
			System.Type eventType = typeof(TEvent);

			AddListener(eventType, listener);
		}

		public static void AddListener(System.Type eventType, System.Action<IEventMessage> listener)
		{
			if (_listeners.TryGetValue(eventType, out var listeners) == false)
			{
				_listeners.Add(eventType, listeners = new LinkedList<Action<IEventMessage>>());
			}

			if (listeners.Contains(listener) == false)
			{
				listeners.AddLast(listener);
			}
			else
			{
				Debug.LogError($"重复添加事件 eventId:{eventType}");
			}
		}

		public static void RemoveListener<TEvent>(System.Action<IEventMessage> listener) where TEvent : IEventMessage
		{
			System.Type eventType = typeof(TEvent);

			RemoveListener(eventType, listener);
		}

		public static void RemoveListener(System.Type eventType, System.Action<IEventMessage> listener)
		{
			if (_listeners.TryGetValue(eventType, out var listeners))
			{
				if (listeners.Contains(listener))
				{
					listeners.Remove(listener);
				}

				if (listeners.Count <= 0)
				{
					_listeners.Remove(eventType);
				}
			}
		}

/// <summary>
/// 发送消息给所有注册的监听器
/// </summary>
/// <param name="message">要发送的事件消息</param>
		public static void SendMessage(IEventMessage message)
		{
			if (_listeners.TryGetValue(message.GetType(), out var listeners) == false)
			{
				return;
			}
			
			if (listeners.Count > 0)
			{
				var currentNode = listeners.Last;
				
				while (currentNode != null)
				{
					var previous = currentNode.Previous;
					currentNode.Value.Invoke(message);
					currentNode = previous;
				}
			}
		}

/// <summary>
/// 静态方法，用于发布事件消息
/// </summary>
/// <param name="message">要发布的事件消息对象，实现IEventMessage接口</param>
		public static void PostMessage(IEventMessage message)
		{
			var wrapper = new PostWrapper();
			wrapper.PostFrame = UnityEngine.Time.frameCount;
			
			wrapper.Message = message;
			_postingList.Add(wrapper);
		}
	}
}