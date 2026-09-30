using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
	public class EventGroup
	{
		private readonly Dictionary<System.Type, List<Action<IEventMessage>>> _cachedListener = new Dictionary<System.Type, List<Action<IEventMessage>>>();

		public void AddListener<TEvent>(System.Action<IEventMessage> listener) where TEvent : IEventMessage
		{
			System.Type eventType = typeof(TEvent);

			
			if (_cachedListener.TryGetValue(eventType, out var listeners) == false)
			{
				_cachedListener.Add(eventType, listeners = new List<Action<IEventMessage>>());
			}

			if (listeners.Contains(listener) == false)
			{
				listeners.Add(listener);
				
				UniEvent.AddListener(eventType, listener);
			}
			else
			{
				Debug.LogError($"重复添加事件 eventType : {eventType}");
			}
		}

		public void RemoveAllListener()
		{
			foreach (var pair in _cachedListener)
			{
				System.Type eventType = pair.Key;
				
				for (int i = 0; i < pair.Value.Count; i++)
				{
					UniEvent.RemoveListener(eventType, pair.Value[i]);
				}
				
				pair.Value.Clear();
			}
			
			_cachedListener.Clear();
		}
	}
}