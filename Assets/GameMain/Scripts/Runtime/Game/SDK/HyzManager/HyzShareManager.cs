using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using GameMain;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 环游者分享
/// </summary>
public class HyzShareManager : MonoSingleton<HyzShareManager>
{
    public EPlatform ePlatform;
    
    /*
     * 内网测试地址: http://192.168.0.201:8589/mini/share/
     */
    //方法父路径
    public const string FunParentUrl = "https://gamesupport.huanyz.com/mini/share/";

    /*
     *  new HyzSharedRoleInfo()
        {
            shareUuid = "",
            currentUuid = "",//当前登录用户uuid--被分享者
            nickName = "",//被分享者的昵称
            iconUrl="",//头像
            catId="",//猫ID
            posType = 0,//位置类型
        };
     */
    /// <summary>
    /// 上传被分享者信息
    /// </summary>
    public void UpLoadSharedInfo(HyzSharedRoleInfo roleInfo)
    {
        UpLoadSharedRoleInfo info = new UpLoadSharedRoleInfo()
        {
            clientId = HyzProjectSetting.ClientId,
            channel = $"{HyzProjectSetting.ProjectTag}_{ePlatform.ToString()}",
            platform = ePlatform.ToString(),
        };

        info.data = roleInfo;
        info.SetSign();

        Debug.Log($"请求参数：{JsonConvert.SerializeObject(info)}");
        StartCoroutine(HyzProjectSetting.HyzNetPost<UpLoadSharedRoleInfo, SuccedResponseBase>( FunParentUrl + "bind", info,
            (data) =>
            {
                Debug.Log($"上传分享结果:{JsonUtility.ToJson(data)}");
            },
            (data) =>
            {
                Debug.LogError("上传分享失败");
            }));
    }

    /// <summary>
    /// 获得分享信息
    /// </summary>
    public void GetMyShareInfo(SharePosType posType,Action<GetShareResponseData> succeed, Action failed)
    {
        GetShareInfo info = new GetShareInfo
        {
            clientId = HyzProjectSetting.ClientId,
            channel = $"{HyzProjectSetting.ProjectTag}_{ePlatform.ToString()}",
            platform = ePlatform.ToString(),
            data = new GetShareData
            {
                uuid = PlatformMgr.UserId,
                posType = (int)posType,
                size = 50,
            }
        };

        info.SetSign();

        StartCoroutine(HyzProjectSetting.HyzNetPost<GetShareInfo, GetShareResponseBag>(FunParentUrl + "bindData", info, (data) =>
        {
            Debug.Log($"获得分享信息结果:{JsonConvert.SerializeObject(data)}");
            if (data.data != null)
            {
                succeed?.Invoke(data.data);
                return;
            }

            Debug.LogError("获得分享信息为空");
            succeed?.Invoke(null);
        }, (data) =>
        {
            Debug.LogError("获得分享信息失败");
            failed?.Invoke();
        }));
    }

    #region 请求参数
    /// <summary>
    /// 提交被分享者参数
    /// </summary>
    public class UpLoadSharedRoleInfo : HyzProjectSetting.PlatformPostData
    {
        public HyzSharedRoleInfo data;

        public void SetSign()
        {
            var str = "";
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>
            {
                {nameof(data.shareUuid), data.shareUuid},
                {nameof(data.currentUuid), data.currentUuid},
                {nameof(data.nickName), data.nickName.ToString()},
                {nameof(data.iconUrl), data.iconUrl.ToString()},
                {nameof(data.catId), data.catId.ToString()},
                {nameof(data.posType), data.posType.ToString()}
            };
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
    /// 被分享者信息
    /// </summary>
    [Serializable]
    public class HyzSharedRoleInfo
    {
        public string shareUuid;//分享者uuid
        public string currentUuid;//当前登录用户uuid--被分享者
        public string nickName;//被分享者的昵称
        public string iconUrl;//头像
        public string catId;//猫ID
        public int posType;//位置类型(筛选)
    }

    /// <summary>
    /// 获得分享参数
    /// </summary>
    [Serializable]
    public class GetShareInfo : HyzProjectSetting.PlatformPostData
    {
        public GetShareData data;

        public void SetSign()
        {
            var str = "";
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>
            {
                {nameof(data.uuid), data.uuid},
                {nameof(data.size), data.size.ToString()},
                {nameof(data.posType), data.posType.ToString()},
            };
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
    public class GetShareData
    {
        public string uuid;//当前用户UUID
        public int size; //详细数据数量
        public int posType;//位置类型(筛选)
    }

    /// <summary>
    /// 通用成功信息
    /// </summary>
    [Serializable]
    public class SuccedResponseBase
    {
        public int retCode;
        public string retDes;
    }
    /// <summary>
    /// 通用失败信息
    /// </summary>
    [Serializable]
    public class FailedResponse
    {
        public int code;
        public string msg;
    }
    
    /// <summary>
    /// 获取分享信息
    /// </summary>
    [Serializable]
    public class GetShareResponseBag : SuccedResponseBase
    {
        public GetShareResponseData data;
    }
    /// <summary>
    /// 详细分享信息
    /// </summary>
    [Serializable]
    public class GetShareResponseData
    {
        public int num; //分享总条数	
        public List<GetSharedResponseRoleData> list; //分享详细信息列表
    }
    
    /// <summary>
    /// 分享详细信息
    /// </summary>
    [Serializable]
    public class GetSharedResponseRoleData
    {
       public string shareUuid;//分享者uuid
       public string currentUuid;//被分享者唯一ID
       
       public string receiptTime;//时间戳
       public string nickName;//昵称
       public string iconUrl;//头像
       public string catId;//猫ID
       public SharePosType posType;//位置类型
    }
    #endregion
    
    #region 游戏参数

    /// <summary>
    /// 分享类型
    /// </summary>
    public enum SharePosType
    {
        None = -1,
        LeftPos = 0,
        RightPos = 1,
        InvitePos = 2,
    }

    #endregion
}

