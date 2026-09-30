using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public static class HyzProjectSetting
{
    public const string ProjectTag = "X14";
    public const string ClientId = "4177103009";
    public const string ClientSecret = "5a6b11d997e7457eb7c331caa07c1e8f";

    #region 平台参数

    /// <summary>
    /// 平台参数
    /// </summary>
    public class PlatformPostData
    {
        public string clientId;
        public string version = "1.0.0";
        public string channel;
        public string platform;
        public string salt = "huanyz";
        public string sign;
    }
    
    public class SuccedResponseBase
    {
        public int retCode;
        public string retDes;
    }
    
    public class FailedResponse
    {
        public int code;
        public string msg;
    }
    
    #endregion
    
    #region Http请求
    /// <summary>
    ///  Http请求
    /// </summary>
    public static IEnumerator HyzNetPost<T, K>(string urlName, T info, Action<K> succeed,
        Action<FailedResponse> failed)
    {
        UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(urlName, string.Empty);
        //设置请求头
        webRequest.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
        webRequest.SetRequestHeader("clientId", HyzProjectSetting.ClientId);
        webRequest.SetRequestHeader("secret", HyzProjectSetting.ClientSecret);

        //设置数据
        if (info.GetType() != typeof(string))
        {
            string bodyData = JsonConvert.SerializeObject(info);
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyData));
        }

        //发送请求
        yield return webRequest.SendWebRequest();

        if (webRequest.result is UnityWebRequest.Result.ProtocolError or
            UnityWebRequest.Result.ConnectionError or
            UnityWebRequest.Result.DataProcessingError)
        {
            //失败
            FailedResponse failData = new FailedResponse();
            failData.code = 9999;
            failData.msg = "连接错误/协议差错/数据处理错误...";
            failed?.Invoke(failData);
            webRequest.Dispose();
            yield break;
        }
        //成功
        string returnDataStr = Encoding.UTF8.GetString(webRequest.downloadHandler.data);
        K returnData = JsonUtility.FromJson<K>(returnDataStr);
        succeed?.Invoke(returnData);
        webRequest.Dispose();
    }

    #endregion
    
    #region 签名工具

    public static class SignTool
    {
        public static string CreateSign(string src)
        {
            //var salting = Salting(src);
            //var md5 = GetMD5HashString(salting);
            var md5 = GetMD5HashString(src);
            return md5;
        }

        private static MD5 md5 = MD5.Create();

        private static string GetMD5HashString(string sourceStr)
        {
            return GetMD5HashString(Encoding.UTF8, sourceStr);
        }

        private static string GetMD5HashString(Encoding encode, string sourceStr)
        {
            var sb = new StringBuilder();

            byte[] source = md5.ComputeHash(encode.GetBytes(sourceStr));
            for (int i = 0; i < source.Length; i++)
            {
                sb.Append(source[i].ToString("x2"));
            }

            return sb.ToString();
        }

        private static string Salting(string src)
        {
            var asciiEncoding = new ASCIIEncoding();

            byte ascStartIndex = 65; // A
            byte ascEndIndx = 90; // Z

            var stringBuilder = new StringBuilder();
            for (var i = 0; i < src.Length; i++)
            {
                var saltingCharIndex = ascStartIndex + i;
                var t = saltingCharIndex <= ascEndIndx
                    ? asciiEncoding.GetString(new[] {(byte) saltingCharIndex}) + src.Substring(i, 1)
                    : src.Substring(i, 1);

                stringBuilder.Append(t);
            }

            return stringBuilder.ToString();
        }

        public static string CreateSing_SHA256(string src)
        {
            string ret = "";
            SHA256 mySHA256 = SHA256.Create();
            byte[] array = mySHA256.ComputeHash(Encoding.Default.GetBytes(src));
            for (int i = 0; i < array.Length; i++)
            {
                ret += string.Format("{0:X2}", array[i]);
            }

            return ret.ToLower();
        }
    }

    #endregion
}


/// <summary>
/// Mono单例模板
/// </summary>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;
    private static readonly object locker = new object();
    private static bool isInstanceDestory;

    public static T Instance
    {
        get
        {
            if (isInstanceDestory)
            {
                instance = null;
                return instance;
            }

            lock (locker)
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>();
                    if (FindObjectsByType<T>(FindObjectsSortMode.InstanceID).Length > 1)
                    {
                        Debug.LogError($"不应该存在多个{typeof(T)}单例！");
                        return instance;
                    }

                    if (instance == null)
                    {
                        GameObject singleton = new GameObject();
                        instance = singleton.AddComponent<T>();
                        singleton.name = typeof(T) + "(Singleton)";
                        singleton.hideFlags = HideFlags.None;
                        DontDestroyOnLoad(singleton);
                    }
                    else
                    {
                        if (Application.isPlaying)
                        {
                            instance.hideFlags = HideFlags.None;
                            DontDestroyOnLoad(instance.gameObject);
                        }
                    }
                }

                return instance;
            }
        }
    }

    protected virtual void Awake()
    {
        isInstanceDestory = false;
    }

    protected virtual void OnDestroy()
    {
        isInstanceDestory = true;
    }
}
