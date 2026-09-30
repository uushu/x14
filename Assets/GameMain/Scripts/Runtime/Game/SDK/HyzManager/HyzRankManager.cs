using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 环游者排行榜
/// </summary>
public class HyzRankManager : MonoSingleton<HyzRankManager>
{
    public EPlatform ePlatform;
    /*
     * 内网测试地址: http://192.168.0.202:8402/ranking/replace/
     */
    //方法父路径
    public const string FunParentUrl = "https://rank.huanyz.com/ranking/replace/";

    /// <summary>
    /// 上传排行榜分数
    /// </summary>
    public void UpLoadRank(string rankName, HyzRankRoleInfo roleInfo)
    {
        UpLoadInfo info = new UpLoadInfo()
        {
            clientId = HyzProjectSetting.ClientId,
            channel = $"{HyzProjectSetting.ProjectTag}_{ePlatform.ToString()}",
            platform = ePlatform.ToString(),
        };
        info.data = new UpLoadData()
        {
            serverIndex = ePlatform.ToString(),
            rank = rankName,
            sortRules = new List<int>() {0},
        };
        info.data.items = new RoleRankUpLoadData[]
        {
            new RoleRankUpLoadData()
            {
                sortValues = new List<long>() {roleInfo.level},
                primaryKey = roleInfo.uuid,
                extInfo = JsonConvert.SerializeObject(roleInfo)
            }
        };
        info.SetSign();

        Debug.Log($"请求参数：{JsonConvert.SerializeObject(info)}");

        StartCoroutine(HyzProjectSetting.HyzNetPost<UpLoadInfo, UpLoadRankSucced>(FunParentUrl + "update", info,
            (data) => { Debug.Log($"排行榜:{rankName}---上传结果:{JsonUtility.ToJson(data)}"); },
            (data) => { Debug.LogError("上传排行榜失败"); }));
    }

    /// <summary>
    /// 获得排行榜
    /// </summary>
    public void GetRank(string rankName, Action<List<HyzRankRoleInfo>> succeed, Action failed)
    {
        GetRankInfo info = new GetRankInfo()
        {
            clientId = HyzProjectSetting.ClientId,
            channel = $"{HyzProjectSetting.ProjectTag}_{ePlatform.ToString()}",
            platform = ePlatform.ToString(),
        };
        info.data = new()
        {
            serverIndex = ePlatform.ToString(),
            rank = rankName,
            sortRules = new List<int>() {0},
            page = 1,
            size = 100,
        };

        info.SetSign();

        StartCoroutine(HyzProjectSetting.HyzNetPost<GetRankInfo, GetRankSucceed>(FunParentUrl + "page", info, (data) =>
        {
            Debug.Log($"获得排行榜:{rankName}---结果:{JsonConvert.SerializeObject(data)}");
            if (data.data.pageInfo != null)
            {
                var rankData = new List<HyzRankRoleInfo>();
                for (int i = 0; i < data.data.pageInfo.Length; i++)
                {
                    HyzRankRoleInfo temp =
                        JsonConvert.DeserializeObject<HyzRankRoleInfo>(data.data.pageInfo[i].extInfo);
                    temp.rank = data.data.pageInfo[i].ranking.ToString();
                    rankData.Add(temp);
                }

                succeed?.Invoke(rankData);
                return;
            }

            Debug.LogError("获得排行榜为空");
            succeed?.Invoke(new List<HyzRankRoleInfo>());
        }, (data) =>
        {
            Debug.LogError("获得排行榜失败");
            failed?.Invoke();
        }));
    }
}

public enum EPlatform
{
    [LabelText("默认")] Default,
    [LabelText("微信")] WeiXin,
    [LabelText("抖音")] DouYin,
}

#region 排行榜请求参数

/// <summary>
/// 提交参数
/// </summary>
public class UpLoadInfo : HyzProjectSetting.PlatformPostData
{
    public UpLoadData data;

    public void SetSign()
    {
        var str = "";
        SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
        dic.Add(nameof(data.serverIndex), data.serverIndex);
        dic.Add(nameof(data.rank), data.rank);
        dic.Add(nameof(data.sortTimeRule), data.sortTimeRule.ToString());
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
        //Debug.LogError("拼接:" + tempStr);
        string temp = HyzProjectSetting.SignTool.CreateSign(tempStr);
        //Debug.LogError("sign:" + temp);
        sign = temp;
    }
}

/// <summary>
/// 提交数据
/// </summary>
[Serializable]
public class UpLoadData
{
    public string serverIndex; //服务器索引(名称)
    public string rank; //排行榜名称
    public List<int> sortRules; //排序规则
    public int sortTimeRule; //排序时间规则 1升序，2降序 当rule升序时，发送请求时可以不填
    public RoleRankUpLoadData[] items; //数据
}

/// <summary>
/// 角色数据
/// </summary>
[Serializable]
public class RoleRankUpLoadData
{
    [SerializeField] public List<long> sortValues; //分数值 [100000,100000]
    public string primaryKey; //角色唯一ID
    public string extInfo; //扩展信息
}

/// <summary>
/// 拉取参数
/// </summary>
public class GetRankInfo : HyzProjectSetting.PlatformPostData
{
    public GetRankData data;

    public void SetSign()
    {
        var str = "";
        SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
        dic.Add(nameof(data.serverIndex), data.serverIndex);
        dic.Add(nameof(data.rank), data.rank);
        dic.Add(nameof(data.page), data.page.ToString());
        dic.Add(nameof(data.size), data.size.ToString());
        dic.Add(nameof(data.assignPrimaryKey), data.assignPrimaryKey);
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
/// 拉取数据
/// </summary>
[Serializable]
public class GetRankData
{
    public string serverIndex; //服务器索引(名称)
    public string rank; //排行榜名称
    public List<int> sortRules; //排序规则
    public int page; //页数
    public int size; //每页条数
    public string assignPrimaryKey; //指定查询角色唯一ID
}

[Serializable]
public class GetRankSucceed : HyzProjectSetting.SuccedResponseBase
{
    [SerializeField] public RankResultInfo data;
}

[Serializable]
public class RankResultInfo
{
    [SerializeField] public AssignInfo assignInfo;
    public PageInfo[] pageInfo;
}

[Serializable]
public class AssignInfo
{
    public List<long> sortValues; //分数值 [100000,100000]
    public string primaryKey; //角色唯一ID
    public int ranking; //排名
    public string extInfo; //扩展信息
}

[Serializable]
public class PageInfo
{
    public List<long> sortValues; //分数值 [100000,100000]
    public string primaryKey; //角色唯一ID
    public int ranking; //排名
    public string extInfo; //扩展信息
}

public class UpLoadRankSucced : HyzProjectSetting.SuccedResponseBase
{
    public int data; //当前排行榜总条数	
}

#endregion

public class HyzRankRoleInfo
{
    public string uuid;
    public string rank;
    public string name;
    public int titleId;
    public string iconUrl;
    public int level;
}