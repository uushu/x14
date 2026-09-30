using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace GameMain
{
    public static class ExtendMethod
    {
        public static List<T> GetAllComponents<T>(this GameObject gameObject) where T : Component
        {
            var list = gameObject.GetComponentsInChildren<T>().ToList();

            var t = gameObject.GetComponent<T>();

            if (t != null)
            {
                list.Add(t);
            }

            return list;
        }

        public static List<T> DictKeyToList<T, T2>(this Dictionary<T, T2> dict)
        {
            var list = new List<T>();

            foreach (var item in dict)
            {
                list.Add(item.Key);
            }

            return list;
        }

        public static void DictKeyToList<T, T2>(this Dictionary<T, T2> dict, ref List<T> list)
        {
            list.Clear();

            foreach (var item in dict)
            {
                list.Add(item.Key);
            }
        }

        public static void DictValueToList<T, T2>(this Dictionary<T, T2> dict, ref List<T2> list)
        {
            list.Clear();

            foreach (var item in dict)
            {
                list.Add(item.Value);
            }
        }

        public static List<T2> DictValueToList<T, T2>(this Dictionary<T, T2> dict, Predicate<T2> condition = null)
        {
            var list = new List<T2>();

            foreach (var item in dict)
            {
                if (condition != null)
                {
                    if (condition(item.Value))
                    {
                        list.Add(item.Value);
                    }
                }
                else
                {
                    list.Add(item.Value);
                }
            }

            return list;
        }

        public static void DestroyAllChildren(this GameObject parent, List<GameObject> exclude = null)
        {
            int childCount = parent.transform.childCount;

            for (int i = childCount - 1; i >= 0; i--)
            {
                var go = parent.transform.GetChild(i).gameObject;
                if (exclude != null && exclude.Contains(go))
                {
                    continue;
                }

                GameObject.DestroyImmediate(go);
            }
        }

        public static void DestroyAllChildren<T>(this GameObject parent) where T : Object
        {
            int childCount = parent.transform.childCount;

            for (int i = childCount - 1; i >= 0; i--)
            {
                var child = parent.transform.GetChild(i).gameObject;
                if (child.GetComponent<T>() != null)
                {
                    GameObject.DestroyImmediate(child);
                }
            }
        }

        public static void FlipX(this Transform transform, bool flipx)
        {
            var scaleX = Mathf.Abs(transform.localScale.x);
            var localScale = new Vector3(
                flipx ? -scaleX : scaleX,
                transform.localScale.y,
                transform.localScale.z);

            transform.transform.localScale = localScale;
        }

        public static T GetComponentInParentRecursion<T>(this Transform transform) where T : Object
        {
            var temp = transform;

            while (true)
            {
                if (temp == null)
                {
                    Debugger.LogError($"父节点不存在的组件 {typeof(T)} transform:{transform.name}");
                    return null;
                }

                var t = temp.GetComponent<T>();

                if (t != null)
                {
                    return t;
                }

                temp = temp.parent;
            }
        }

        public static int IndexOfParentTransform(this Transform self)
        {
            var parent = self.parent;
            if (parent == null)
            {
                Debug.LogError("不存在父节点");
                return -1;
            }

            var childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                if (parent.transform.GetChild(i) == self)
                {
                    return i;
                }
            }

            Debug.LogError("不存在父节点");
            return -1;
        }

        public static int IndexOfTransform<T>(this Transform parent, Transform child) where T : Object
        {
            int childCount = parent.transform.childCount;

            int index = 0;

            for (int i = 0; i < childCount; i++)
            {
                if (parent.transform.GetChild(i).GetComponent<T>() == null) continue;

                index++;

                if (parent.transform.GetChild(i) == child.transform)
                {
                    return index;
                }
            }

            return -1;
        }

        public static List<string> GetTransformPath(Transform t1, Transform t2)
        {
            var list = new List<string>();

            int maxIterCount = 30;

            var compare = t2;

            while (compare != t1)
            {
                list.Add(compare.name);

                compare = compare.transform.parent;

                maxIterCount--;

                if (maxIterCount <= 0)
                {
                    Debugger.LogError($"未找到当前节点的路径 t1:{t1.name} t2:{t2.name}");
                    return list;
                }
            }

            return list;
        }

        public static void GetAllTypePath<T>(this GameObject root, Transform child, ref List<string> typeList) where T : Object
        {
            int childCount = child.childCount;

            for (int i = 0; i < childCount; i++)
            {
                if (child.GetChild(i).GetComponent<T>() != null)
                {
                    var list = GetTransformPath(root.transform, child.GetChild(i));

                    list.Reverse();
                    typeList.Add(string.Join("/", list));
                }

                GetAllTypePath<T>(root, child.GetChild(i), ref typeList);
            }
        }

        public static void RemoveAllType<T1>(this GameObject go, Type addType) where T1 : Object
        {
            var components = go.transform.GetComponents<T1>();

            foreach (var component in components)
            {
                if (component.GetType() == addType) continue;

                GameObject.DestroyImmediate(component);
            }

            if (addType != null)
            {
                if (go.GetComponent(addType) == null)
                {
                    go.AddComponent(addType);
                }
            }
        }

        public static void DestroyChildByType<T>(this GameObject go) where T : Object
        {
            int childCount = go.transform.childCount;

            for (int i = childCount - 1; i >= 0; i--)
            {
                var childNode = go.transform.GetChild(i).gameObject;

                if (childNode.GetComponent<T>() != null)
                {
                    GameObject.DestroyImmediate(childNode);
                }
            }
        }

        public static string FormatNumber(this long number)
        {
            return UtilityTool.FormatBigNumber(number);
        }

        public static string FormatNumber(this int number)
        {
            return UtilityTool.FormatBigNumber(number);
        }

        public static string FormatNumber(this float number)
        {
            return UtilityTool.FormatBigNumber((double)number);
        }
        
        public static EventTrigger.Entry AddEventListener(this RectTransform uiObject, EventTriggerType eventType, Action<BaseEventData> callBack)
        {
            var eventTrigger = uiObject.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
                eventTrigger = uiObject.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry()
            {
                eventID = eventType
            };
            entry.callback.AddListener(new UnityAction<BaseEventData>(callBack));
            eventTrigger.triggers.Add(entry);

            return entry;
        }
        
        public static void RemoveEventListener(this RectTransform uiObject)
        {
            var eventTrigger = uiObject.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger != null)
            {
                eventTrigger.triggers.Clear();
                GameObject.Destroy(eventTrigger);
            }
        }
    }
}