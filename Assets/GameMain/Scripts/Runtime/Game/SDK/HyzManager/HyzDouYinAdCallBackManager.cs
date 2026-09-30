#if UNITY_DOUYIN_GAME
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GameMain;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class HyzDouYinAdCallBackManager : MonoSingleton<HyzDouYinAdCallBackManager>
{
    //方法父路径
    public const string FunParentUrl = "https://analytics.oceanengine.com/api/v2/conversion";

    private readonly PlatformDy editorTemp = new PlatformDy();

    private PlatformDy Platform
    {
        get
        {
#if UNITY_EDITOR
            return editorTemp;
#endif
            return (PlatformDy) PlatformMgr.Instance.Platform;
        }
    }

    public const string ClickidKey = "clickid";
    public const string AdIdKey = "adid";
    /// <summary>
    /// 激活
    /// </summary>
    public void DouYinActive()
    {
#if UNITY_EDITOR
        return;
#endif
        Save_Setting setData = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Setting>();
        if (!setData.isAdsUser)
        {
            return;
        }
        if (setData.adsInfo == null)
        {
            Debug.LogError("DouYinActive is error");
            return;
        }
        
        DouYinAdCallBackInfo data = new DouYinAdCallBackInfo();
        data.event_type = "active";
        data.context = new DouYinAdCallBackContext()
        {
            ad = new DouYinAdInfo()
            {
                callback = setData.adsInfo.log_extra.clickid,
                match_type = 3,
            },
            device = new DouYinDevice()
            {
                platform = Platform.SystemTag, //平台
            }
        };
        data.properties = new Dictionary<string, string>()
        {
            {AdIdKey,setData.adsInfo.ad_id.ToString()},
        };

        DouYinAdsCallBack(data);
    }

    /// <summary>
    /// 次留
    /// </summary>
    public void DouYinNextDayOpen()
    {
#if UNITY_EDITOR
        return;
#endif
        Save_Setting setData = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Setting>();
        if (!setData.isAdsUser)
        {
            return;
        }
        if (setData.adsInfo == null)
        {
            Debug.LogError("DouYinNextDayOpen is error");
            return;
        }
        
        DouYinAdCallBackInfo data = new DouYinAdCallBackInfo();
        data.event_type = "next_day_open";
        data.context = new DouYinAdCallBackContext()
        {
            ad = new DouYinAdInfo()
            {
                callback = setData.adsInfo.log_extra.clickid,
                match_type = 3,
            },
            device = new DouYinDevice()
            {
                platform = Platform.SystemTag, //平台
            }
        };
        data.properties = new Dictionary<string, string>()
        {
            {AdIdKey,setData.adsInfo.ad_id.ToString()},
        };

        DouYinAdsCallBack(data);
    }
    
    /// <summary>
    /// 完成第5关卡
    /// </summary>
    public void DouYinCompleteSection()
    {
#if UNITY_EDITOR
        return;
#endif
        Save_Setting setData = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Setting>();
        if (!setData.isAdsUser)
        {
            Debug.LogError("DouYinCompleteSection is no AdsUser");
            return;
        }
        if (setData.adsInfo == null)
        {
            Debug.LogError("DouYinCompleteSection is error");
            return;
        }
        
        DouYinAdCallBackInfo data = new DouYinAdCallBackInfo();
        data.event_type = "complete_section";
        data.context = new DouYinAdCallBackContext()
        {
            ad = new DouYinAdInfo()
            {
                callback = setData.adsInfo.log_extra.clickid,
                match_type = 3,
            },
            device = new DouYinDevice()
            {
                platform = Platform.SystemTag, //平台
            }
        };
        data.properties = new Dictionary<string, string>()
        {
            {AdIdKey,setData.adsInfo.ad_id.ToString()},
            {"levelId",5.ToString()},
        };

        DouYinAdsCallBack(data);
    }
    
    /// <summary>
    /// 看满4个广告
    /// </summary>
    public void DouYinQuest()
    {
#if UNITY_EDITOR
        return;
#endif
        Save_Setting setData = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Setting>();
        if (!setData.isAdsUser)
        {
            return;
        }
        if (setData.adsInfo == null)
        {
            Debug.LogError("DouYinQuest is error");
            return;
        }
        
        DouYinAdCallBackInfo data = new DouYinAdCallBackInfo();
        data.event_type = "quest";
        data.context = new DouYinAdCallBackContext()
        {
            ad = new DouYinAdInfo()
            {
                callback = setData.adsInfo.log_extra.clickid,
                match_type = 3,
            },
            device = new DouYinDevice()
            {
                platform = Platform.SystemTag, //平台
            }
        };
        data.properties = new Dictionary<string, string>()
        {
            {AdIdKey,setData.adsInfo.ad_id.ToString()},
            {"lookAds",4.ToString()},
        };

        DouYinAdsCallBack(data);
    }
    
    /// <summary>
    /// 完成活动关卡
    /// </summary>
    public void DouYinDrawLottery(int activityType)
    {
#if UNITY_EDITOR
        return;
#endif
        Save_Setting setData = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Setting>();
        if (!setData.isAdsUser)
        {
            return;
        }
        if (setData.adsInfo == null)
        {
            Debug.LogError("DouYinDrawLottery is error");
            return;
        }
        
        DouYinAdCallBackInfo data = new DouYinAdCallBackInfo();
        data.event_type = "draw_lottery";
        data.context = new DouYinAdCallBackContext()
        {
            ad = new DouYinAdInfo()
            {
                callback = setData.adsInfo.log_extra.clickid,
                match_type = 3,
            },
            device = new DouYinDevice()
            {
                platform = Platform.SystemTag, //平台
            }
        };
        data.properties = new Dictionary<string, string>()
        {
            {AdIdKey,setData.adsInfo.ad_id.ToString()},
            {"activityType",activityType.ToString()},
        };

        DouYinAdsCallBack(data);
    }
    
    /// <summary>
    /// 抖音回传
    /// </summary>
    private void DouYinAdsCallBack(DouYinAdCallBackInfo requestData)
    {
        Debug.Log($"请求参数:{JsonConvert.SerializeObject(requestData)}");
        StartCoroutine(DouYinAdCallBackNetPost<DouYinAdCallBackInfo, SucceedResponse>(requestData,
            (data) => { Debug.Log($"抖音回传结果:{data.code}:{data.message}"); },
            (data) => { Debug.LogError("抖音回传失败"); }));
    }

    /// <summary>
    /// 抖音回传Http请求
    /// </summary>
    private static IEnumerator DouYinAdCallBackNetPost<T, K>(T info, Action<K> succeed,
        Action<FailedResponse> failed)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(FunParentUrl, string.Empty);
        //设置请求头
        webRequest.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
        // webRequest.SetRequestHeader("clientId", HyzProjectSetting.ClientId);
        // webRequest.SetRequestHeader("secret", HyzProjectSetting.ClientSecret);

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
        Debug.LogWarning($"回传结果数据：{returnDataStr}");
        K returnData = JsonUtility.FromJson<K>(returnDataStr);
        succeed?.Invoke(returnData);
        webRequest.Dispose();
    }
}

/// <summary>
/// 抖音回传数据信息
/// </summary>
[Serializable]
public class DouYinAdCallBackInfo
{
    public string event_type;
    public DouYinAdCallBackContext context;
    public Dictionary<string, string> properties; //properties中的属性需替换为当前事件真实可回传的附加属性字段

    public long timestamp = TimeTool.GetNowTimeStampMillisecond(); //timestamp需替换为事件发生时真实的毫秒级时间戳
}
[Serializable]
public class DouYinAdCallBackContext
{
    public DouYinAdInfo ad;
    public DouYinDevice device;
}
[Serializable]
public class DouYinAdInfo
{
    public string callback = "demo_callback"; //callback的值需填入目标事件数据映射的CLICKID
    public int match_type;
}
[Serializable]
public class DouYinDevice
{
    public string platform;
    public string imei;
    public string idfa;
    public string oaid;
}
#endif