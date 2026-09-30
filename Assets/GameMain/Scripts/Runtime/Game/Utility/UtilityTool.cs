using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameMain
{
    public interface IWeight
    { 
        public int Weight { get; }
    }

    public static class UtilityTool
    {
        public static string FormatBigNumber(double number)
        {
            if (number >= 1000000000)
            {
                return (number / 1000000000).ToString("0.##") + "B";
            }
            else if (number >= 1000000)
            {
                return (number / 1000000).ToString("0.##") + "M";
            }
            else if (number >= 1000)
            {
                return (number / 1000).ToString("0.##") + "K";
            }
            return number.ToString("0.##");
        }

        public static string FormatBigNumber2(double number)
        {
            if (number >= 10000)
            {
                return (number / 1000).ToString("0.#") + "k";
            }
            return number.ToString("0");
        }

        public static float GetDecimalPart(float number)
        {
            int integerPart = Mathf.FloorToInt(number); // 获取整数部分
            float decimalPart = number - integerPart; // 计算小数部分
            return decimalPart;
        }

        public static bool IsRectangleIntersecting(Rect rect1, Rect rect2)
        {
            var b1 = rect2.x >= rect1.x + rect1.width;
            var b2 = rect2.x + rect2.width <= rect1.x;
            var b3 = rect2.y >= rect1.y + rect1.height;
            var b4 = rect2.y + rect2.height <= rect1.y;
            return !(b1 || b2 || b3 || b4);
        }

        public static Vector2 AreLinesIntersect(Vector2 p11, Vector2 p12, Vector2 p21, Vector2 p22)
        {
            float x1 = p11.x, y1 = p11.y;
            float x2 = p12.x, y2 = p12.y;
            float x3 = p21.x, y3 = p21.y;
            float x4 = p22.x, y4 = p22.y;

            float denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

            if (Mathf.Approximately(denominator, 0))
            {
                return Vector2.zero;
            }

            float t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / denominator;

            return new Vector2(x1 + t * (x2 - x1), y1 + t * (y2 - y1));
        }

        public static string ToUpperFirst(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            char first = input.FirstOrDefault();
            char upperFirst = char.ToUpper(first);
            return upperFirst + input.Substring(1);
        }

        public static string ReplacePatteren(string input, string pattern, string replacement)
        {
            return Regex.Replace(input, pattern, replacement);
        }

        public static string ReplaceTaskTitlePattern(string input, object replacement)
        {
            return ReplaceTaskTitlePattern(input, replacement.ToString());
        }

        public static string ReplaceTaskTitlePattern(string input, int replacement)
        {
            return ReplaceTaskTitlePattern(input, replacement.ToString());
        }

        public static string ReplaceTaskTitlePattern(string input, string replacement)
        {
            string pattern = @"\[.*?\]";

            return ReplacePatteren(input, pattern, replacement);
        }

        public static string ReplaceTaskTitlePattern(string input, Func<int, string> action)
        {
            string pattern = @"\[.*?\]";

            int index = 0;

            string GetReplace(Match evaluator)
            {
                var replace = action?.Invoke(index);

                index++;

                return replace;
            }

            return Regex.Replace(input, pattern, GetReplace);
        }

        public static float Round(float number, int digits)
        {
            return (float)System.Math.Round((double)number, digits);
        }

        public static string RandomNickName(string name, int min, int max)
        {
            int totalWidth = max.ToString().Length;

            int randomNum = UnityEngine.Random.Range(min, max);

            string numStr = randomNum.ToString().PadLeft(totalWidth, '0');

            return $"{name}{numStr}";
        }

        public static int RandomNumber(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static T RandomNumber<T>(List<T> list)
        {
            if (list.Count <= 0)
            {
                Debugger.LogError("随机的数组数量<=0");
                return default;
            }
            var randomIndex = RandomNumber(0, list.Count);
            return list[randomIndex];
        }

        public static float RandomNumber(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static bool RandomBool(float percent = 0.5f)
        {
            return RandomNumber(0, 10000) <= 10000 * percent;
        }

        public static bool RandomBool10000(int percent)
        {
            return RandomNumber(0, 10000) <= percent;
        }

        public static bool RandomBool100(float number)
        {
            return RandomBool(number / 100f);
        }

        public static string ColorToRgbaString(Color color)
        {
            int r = Mathf.RoundToInt(color.r * 255);
            int g = Mathf.RoundToInt(color.g * 255);
            int b = Mathf.RoundToInt(color.b * 255);
            int a = Mathf.RoundToInt(color.a * 255);

            int rgba = (r << 24 | g << 16 | b << 8 | a);

            return rgba.ToString("x8");
        }

        public static List<T> CloneList<T>(List<T> target)
        {
            var list = new List<T>();

            foreach (var item in target)
            {
                list.Add(item);
            }

            return list;
        }

        public static bool ListContain<T>(List<T> list1, List<T> list2)
        {
            foreach (var item in list1)
            {
                if (list2.Contains(item))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ListIsSame<T>(List<T> list1, List<T> list2) where T : IEquatable<T>
        {
            if (list1 == null && list2 == null) return true;
            if (list1 == null || list2 == null) return false;

            if (list1.Count != list2.Count)
            {
                return false;
            }

            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i].Equals(list2[i]) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public static void ListSub<T>(List<T> list1, List<T> list2)
        {
            foreach (var item in list2)
            {
                list1.Remove(item);
            }
        }

        public static List<T> CloneList<T>(List<T> target, List<T> exclude)
        {
            var list = new List<T>();

            foreach (var item in target)
            {
                if (exclude.Contains(item)) continue;

                list.Add(item);
            }

            return list;
        }

        public static void SuffixList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = Random.Range(0, list.Count);

                var temp = list[i];

                list[i] = list[randomIndex];

                list[randomIndex] = temp;
            }
        }

        public static List<int> SplitNumberToList(int number, int splitCount)
        {
            var list = new List<int>();

            if (number <= splitCount)
            {
                for (int i = 0; i < number; i++)
                {
                    list.Add(1);
                }

                return list;
            }

            for (int i = 0; i < splitCount; i++)
            {
                list.Add(1);
            }

            var remain = number - splitCount;
            for (int i = 0; i < splitCount; i++)
            {
                if (remain > 0)
                {
                    var randomValue = UtilityTool.RandomNumber(0, remain);
                    list[i] += randomValue;

                    remain -= randomValue;
                }
                else
                {
                    break;
                }
            }
            list[list.Count - 1] += remain;

            return list;
        }

        public static void GetNumbers(int number, ref List<int> list)
        {
            while (number > 0)
            {
                int digit = number % 10; // 获取最后一位数字
                Console.WriteLine(digit); // 输出或处理这个数字
                number /= 10; // 去掉最后一位数字

                list.Add(digit);
            }
        }

        public static int GetRandomWeightIndex<T>(List<T> weightList) where T : IWeight
        {
            if (weightList.Count <= 0)
            {
                Debugger.LogError("权重参数错误:weightList<=0");
                return -1;
            }

            int total = 0;

            foreach (var item in weightList)
            {
                total += item.Weight;
            }

            int randomNumber = RandomNumber(0, total);

            for (int i = 0; i < weightList.Count; i++)
            {
                if (weightList[i].Weight >= randomNumber)
                {
                    return i;
                }

                randomNumber -= weightList[i].Weight;
            }

            return weightList.Count - 1;
        }

        public static int RandomKeyByWeight(Dictionary<int, int> weightDict)
        {
            if (weightDict.Count <= 0)
            {
                Debugger.LogError("权重参数错误:weightList<=0");
                return -1;
            }

            int total = 0;

            foreach (var item in weightDict)
            {
                total += item.Value;
            }

            int randomNumber = RandomNumber(0, total);
            float currentValue = 0;
            foreach (var item in weightDict)
            {
                var weightValue = item.Value;
                currentValue += weightValue;

                if (currentValue > randomNumber)
                {
                    return item.Key;
                }
            }

            return weightDict.LastOrDefault().Key;
        }

        public static int RandomKeyByWeight(SortedDictionary<int, int> weightDict)
        {
            if (weightDict.Count <= 0)
            {
                Debugger.LogError("权重参数错误:weightList<=0");
                return -1;
            }

            int total = 0;

            foreach (var item in weightDict)
            {
                total += item.Value;
            }

            int randomNumber = RandomNumber(0, total);

            foreach (var item in weightDict)
            {
                if (item.Value >= randomNumber)
                {
                    return item.Key;
                }

                randomNumber -= item.Value;
            }

            return weightDict.LastOrDefault().Key;
        }

        public static bool DictTryGetOrLast<T1, T2>(this Dictionary<T1, T2> dict, T1 key, out T2 result, Func<T1, bool> condition)
        {
            if (dict.TryGetValue(key, out var value))
            {
                result = value;
                return true;
            }

            if (condition.Invoke(key))
            {
                var last = dict.LastOrDefault();
                result = last.Value;
                return true;
            }

            result = default(T2);
            return false;
        }

        public static void DictMergeFrom(this Dictionary<int, int> target, Dictionary<int, int> from)
        {
            foreach (var item in from)
            {
                if (target.TryGetValue(item.Key, out var value))
                {
                    target[item.Key] = value + item.Value;
                }
                else
                {
                    target.Add(item.Key, item.Value);
                }
            }
        }

        public static Dictionary<int, int> DictMerge(Dictionary<int, int> d1, Dictionary<int, int> d2)
        {
            var dict = new Dictionary<int, int>();
            dict.DictMergeFrom(d1);
            dict.DictMergeFrom(d2);
            return dict;
        }

        public static bool DictIsSame<T1, T2>(Dictionary<T1, T2> t1, Dictionary<T1, T2> t2) where T2 : IEquatable<T2>
        {
            if (t1.Count != t2.Count)
            {
                return false;
            }

            foreach (var item in t1)
            {
                if (t2.TryGetValue(item.Key, out var value) == false)
                {
                    if (value != null && item.Value != null)
                    {
                        if (value.Equals(item.Value) == false)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static void DictEqual<T1, T2>(Dictionary<T1, T2> t1, Dictionary<T1, T2> t2)
        {
            t1.Clear();

            foreach (var item in t2)
            {
                t1.Add(item.Key, item.Value);
            }
        }

        public static void DictCopyFrom<T1, T2>(this Dictionary<T1, T2> self, Dictionary<T1, T2> from)
        {
            self.Clear();
            foreach (var item in from)
            {
                self.Add(item.Key, item.Value);
            }
        }

        public static void ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var randomIndex = RandomNumber(0, list.Count);
                //交换位置
                var temp = list[randomIndex];
                list[randomIndex] = list[i];
                list[i] = temp;
            }
        }

        public static string ShortNumberToString(long money)
        {
            string result = "";

            if (money < 10000)
            {
                result = money.ToString();
            }
            else
            {
                result = string.Format($"{Math.Floor(money / (float)1000)}K");
            }
            return result;
        }
        
        public static string ShortNumberToString(float money)
        {
            return ShortNumberToString((long)money);
        }

        public static List<T> ReverseList<T>(List<T> list)
        {
            var reverseList = new List<T>();

            for (int i = list.Count - 1; i >= 0; i--)
            {
                reverseList.Add(list[i]);
            }

            return reverseList;
        }

        public static async UniTask<(bool, string)> RequestPostData(string url, string postJson, Action<bool, string> action)
        {
            UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            var byteArray = Encoding.UTF8.GetBytes(postJson);
            webRequest.uploadHandler = new UploadHandlerRaw(byteArray);
            webRequest.uploadHandler.contentType = "application/json;charset=utf-8";

            webRequest.timeout = 30;
            await webRequest.SendWebRequest();

            // 检查是否有错误发生
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error: " + webRequest.error);
                action?.Invoke(false, webRequest.error);
                return (false, null);
            }

            // 请求成功，输出服务器返回的响应信息
            var responseText = webRequest.downloadHandler.text;
            action?.Invoke(true, responseText);
            Debug.LogWarning($"post返回:{responseText}");

            await UniTask.Yield();
            webRequest.Dispose();

            return (true, responseText);
        }

        public static IEnumerator DoBezierMove(Transform target, Vector3 moveToPosition, float totalTime, float xPercent, float yPercent, System.Action finish)
        {
            var flag = 1;
            float runTime = 0;
            var startPosition = target.position;

            var distance = Vector3.Distance(target.position, moveToPosition);
            var xDistance = distance * xPercent;

            var direction = (moveToPosition - target.position).normalized;
            var normal = Vector2.Perpendicular(direction).normalized * flag;
            var middle = target.position + direction * xDistance;

            middle += distance * yPercent * new Vector3(normal.x, normal.y, 0);

            while (runTime < totalTime)
            {
                runTime += Time.deltaTime;
                var clapRuntime = Mathf.Min(runTime, totalTime);
                var position = Bezier(clapRuntime / totalTime, startPosition, middle, moveToPosition);
                target.transform.position = position;
                yield return null;
            }

            target.position = moveToPosition;
            finish?.Invoke();
        }

        public static IEnumerator DoBezierMove2(Transform moveTarget, Vector3 targetPosition, float totalTime, float xPercent, float yPercent, System.Action finish)
        {
            var flag = 1;
            if (targetPosition.y > moveTarget.position.y)
            {
                if (targetPosition.x < moveTarget.position.x)
                {
                    flag = -1;
                }
                else
                {
                    flag = 1;
                }
            }
            else
            {
                if (targetPosition.x < moveTarget.position.x)
                {
                    flag = -1;
                }
                else
                {
                    flag = 1;
                }
            }

            float runTime = 0;
            var startPosition = moveTarget.position;

            var distance = Vector3.Distance(moveTarget.position, targetPosition);
            var xDistance = distance * xPercent;

            var direction = (targetPosition - moveTarget.position).normalized;
            var normal = Vector2.Perpendicular(direction).normalized * flag;
            var middle = moveTarget.position + direction * xDistance;

            middle += distance * yPercent * new Vector3(normal.x, normal.y, 0);

            while (runTime < totalTime)
            {
                runTime += Time.deltaTime;
                var clapRuntime = Mathf.Min(runTime, totalTime);
                var position = Bezier(clapRuntime / totalTime, startPosition, middle, targetPosition);

                var rotation = position - moveTarget.transform.position;
                moveTarget.transform.up = rotation;
                moveTarget.transform.position = position;

                yield return null;
            }

            finish?.Invoke();
        }

        private static Vector3 Bezier(float t, Vector3 start, Vector3 middle, Vector3 end)
        {
            var t1 = (1 - t) * (1 - t);
            var t2 = 2 * t * (1 - t);
            var t3 = t * t;
            return t1 * start + t2 * middle + t3 * end;
        }
        
        /// <summary>
        /// 设置网络图片
        /// </summary>
        public static IEnumerator SetHttpUrl(this Image selfImage,string imageUrl)
        {
            selfImage.sprite = null;
            if (string.IsNullOrEmpty(imageUrl))
            {
                Debug.Log("DownloadImage ImageUrl is null or empty");
                //默认头像
                ResourceMgr.Instance.SetImageSprite(selfImage, "default");
                yield break;
            }
            //Debug.Log($"开始下载图片:{imageUrl}");
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUrl))
            {
                yield return webRequest.SendWebRequest();
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("下载图片失败：" + webRequest.error);
                }
                else
                {
                    // 获取下载的纹理
                    Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                    selfImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
            }
        }

        public static Vector3 SetY(this Vector3 value,float y = 0)
        {
            value.y = y;
            return value;
        }
        
        /// <summary>
        /// 刷新自己和子节点的ContentSizeFitter(倒序刷新)
        /// </summary>
        /// <param name="node">目标节点</param>
        public static void RefreshAllContentSizeFitter(this GameObject node)
        {
            ContentSizeFitter[] allComponent = node.transform.GetComponentsInChildren<ContentSizeFitter>(true);

            for (int i = allComponent.Length - 1; i >= 0; i--)
            {
                RectTransform rectTrans = allComponent[i].GetComponent<RectTransform>();
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTrans);
            }
        }
        
        public static List<T> RandomItems<T>(this List<T> list, int count)
        {
            var result = new List<T>();
            var t = new List<T>(list);

            for (var i = 0; i < count && t.Count > 0; i++)
            {
                var randomInt = UnityEngine.Random.Range(0, t.Count);
                result.Add(t[randomInt]);
                t.RemoveAt(randomInt);
            }

            return result;
        }
        
        public static List<T> RandomItemsAndRemove<T>(this List<T> list, int count)
        {
            var result = new List<T>();

            for (var i = 0; i < count && list.Count > 0; i++)
            {
                var randomInt = UnityEngine.Random.Range(0, list.Count);
                result.Add(list[randomInt]);
                list.RemoveAt(randomInt);
            }

            return result;
        }
        
        /// <summary>
        /// 局部坐标转局部坐标(父节点不能为空-通过父节点坐标系转)
        /// </summary>
        /// <param name="localTran">被转换的节点(它的局部坐标)</param>
        /// <param name="targetTran">目标父节点(目标局部原点)</param>
        public static Vector3 LocalPositonToLocalPosition(Transform localTran, Transform targetTran)
        {
            Transform _parent = localTran.parent;
            if (_parent != null)
            {
                Vector3 _wordSpace = _parent.TransformPoint(localTran.localPosition);
                return targetTran.InverseTransformPoint(_wordSpace);
            }

            return default;
        }
        
        public static int GetRandomItem(this List<List<int>> list)
        {
            List<WeightedItem<int>> weightedItems = new List<WeightedItem<int>>();
            foreach (var item in list)
            {
                WeightedItem<int> weightedItem = new WeightedItem<int>(item[0], item[1]);
                weightedItems.Add(weightedItem);
            }
            return weightedItems.GetRandomItem();
        }
    
        // 根据权重随机选择元素的方法
        public static T GetRandomItem<T>(this List<WeightedItem<T>> weightedItems)
        {
            if(weightedItems.Count == 0)
                return default(T);
            // 计算总权重
            int totalWeight = 0;
            foreach (var item in weightedItems)
            {
                totalWeight += item.Weight;
            }

            // 生成一个随机数
            int randomIndex = UnityEngine.Random.Range(0, totalWeight);

            // 累加权重，直到随机数落在某个元素的权重范围内
            for (int i = 0; i < weightedItems.Count; i++)
            {
                randomIndex -= weightedItems[i].Weight;
                if (randomIndex < 0)
                {
                    return weightedItems[i].Value;
                }
            }

            // 如果所有元素的权重都不足以满足随机数，则返回一个默认值或抛出异常（理论上这种情况不会发生，除非总权重计算错误）
            throw new InvalidOperationException("No item selected, total weight calculation error?");
        }
        
        public static void PlaySoundAndTableId(int id,float value = 0.5f)
        {
           
        }
    }
    
    //定义一个包含权重和值的类
    public class WeightedItem<T>
    {
        public T Value { get; set; }
        public int Weight { get; set; }

        public WeightedItem(T value, int weight)
        {
            Value = value;
            Weight = weight;
        }
    }
    /// <summary>
    /// 序列化字典
    /// </summary>
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys;
        [SerializeField]
        private List<TValue> values;

        private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public Dictionary<TKey, TValue> Dictionary
        {
            get { return dictionary; }
        }

        public void OnBeforeSerialize()
        {
            keys = new List<TKey>(dictionary.Keys);
            values = new List<TValue>(dictionary.Values);
        }

        public void OnAfterDeserialize()
        {
            dictionary = new Dictionary<TKey, TValue>();
            for (int i = 0; i < keys.Count && i < values.Count; i++)
            {
                dictionary[keys[i]] = values[i];
            }
        }
    }
}