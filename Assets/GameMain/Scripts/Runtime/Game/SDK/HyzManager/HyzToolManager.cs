using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class HyzToolManager : MonoSingleton<HyzToolManager>
{
    public EPlatform ePlatform;
    
    public const string ToolFunParentUrl = "https://gamesupport.huanyz.com/wechat/";
    
    /// <summary>
    /// 获得微信UUID
    /// </summary>
    public void GetWechatUUID(string appid,string secret,string jsCode,Action<WechatResponseData> succeed, Action failed)
    {
        GetWeChatInfo info = new GetWeChatInfo()
        {
            clientId = HyzProjectSetting.ClientId,
            channel = $"{HyzProjectSetting.ProjectTag}_{ePlatform.ToString()}",
            platform = ePlatform.ToString(),
        };
        info.data = new()
        {
            appid = appid,
            secret = secret,
            js_code = jsCode
        };

        info.SetSign();

        StartCoroutine(HyzProjectSetting.HyzNetPost<GetWeChatInfo, WechatResponseInfo>(ToolFunParentUrl + "code2Session", info, (data) =>
        {
            Debug.LogError($"微信UUID结果:{JsonConvert.SerializeObject(data)}");
            if (data.retCode == 0)
            {
                succeed?.Invoke(data.data);
            }
            else
            {
                failed?.Invoke();
            }
        }, (data) =>
        {
            Debug.LogError("获得微信UUID失败");
            failed?.Invoke();
        }));
    }

    #region 请求参数

    /// <summary>
    /// 拉取微信UUID信息
    /// </summary>
    public class GetWeChatInfo : HyzProjectSetting.PlatformPostData
    {
        public WechatRequest data;
        public void SetSign()
        {
            var str = "";
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            dic.Add(nameof(data.appid), data.appid);
            dic.Add(nameof(data.secret), data.secret);
            dic.Add(nameof(data.js_code), data.js_code);
            for (int i = 0; i < dic.Count; i++)
            {
                KeyValuePair<string, string> item = dic.ElementAt(i);
                if (item.Value == null || string.IsNullOrEmpty(item.Value))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(str))
                {
                    str += $"{item.Key}={item.Value}";
                }
                else
                {
                    str += $"&{item.Key}={item.Value}";
                }
            }

            var tempStr = $"{clientId}{str}{version}{salt}{HyzProjectSetting.ClientSecret}";
            //Debug.LogError("拼接2:" + tempStr);
            string temp = HyzProjectSetting.SignTool.CreateSign(tempStr);
            sign = temp;
        }
    }
    /// <summary>
    /// 微信请求数据
    /// </summary>
    [Serializable]
    public class WechatRequest
    {
        public string appid;
        public string secret;
        public string js_code;
    }

    /// <summary>
    /// 微信请求返回信息
    /// </summary>
    public class WechatResponseInfo : HyzProjectSetting.SuccedResponseBase
    {
        public WechatResponseData data; //当前排行榜总条数	
    }
    
    /// <summary>
    /// 微信请求返回数据
    /// </summary>
    [Serializable]
    public class WechatResponseData
    {
        public string openid;
        public string session_key;
        public string unionid;
        public int errcode;
        public string errmsg;
    }
    #endregion
}
