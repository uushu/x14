using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameMain;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Rendering;

/// <summary>
/// 打点记录类型
/// </summary>
public enum ReportType
{
    None,
    One,//一次
    Track,//事件
}
public class ReportStr
{
    public const string loadStart = "loadStart"; //开始加载
    public const string loadEnd = "loadEnd"; //加载完成
    
    public const string mainShare = "mainShare"; // 主页分享 玩家最高通关关卡为1时，分享游戏给好友
    public const string levelEnter = "levelEnter"; //关卡进入 进入关卡1
    public const string levelStart = "levelStart"; //关卡开始
    
    public const string backMainPage = "backMainPage"; //返回主页
    public const string useProp = "useProp"; //使用道具
    public const string resetLevel = "resetLevel"; //重新挑战
    public const string levelPass = "levelPass"; //关卡通关
    public const string levelFail = "levelFail"; //关卡失败
    
    public const string watchAd = "watchAd"; //观看广告
}

public enum LookAdsType
{
    None = 0,

    俄罗斯花园刷新道具 = 10001,
    俄罗斯花园消除道具 = 10002,
    俄罗斯花园旋转道具 = 10003,
    俄罗斯花园复活 = 10004,
 
    花了个花击碎道具= 10101,
    花了个花刷新道具 = 10102,
    花了个花消除道具 = 10103,
    花了个花复活 = 10104,

    看广告获取体力 = 11000,
    看广告获取无限体力 = 11001,
    
    收藏界面看广告解锁鲜花 = 20001,
    看广告解锁主题 = 20002,
    看广告回满体力 = 20003,
    看广告领取双倍花花宝箱 = 20004,
    看广告领取商店金币 = 20005,
    看广告领取存钱罐金币 = 20006,
    看广告领取主界面金币 = 20007,
    看广告进入无线模式 = 20008,
}

public class ReportManager : AMonoManager<ReportManager>
{
    public override int Priority => 20000;
    private static ReportManager instance;
    public new static ReportManager Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("ReportManager");
                instance = go.AddComponent<ReportManager>();
            }

            return instance;
        }
    }

    float showStartTime;
    float gameStartTime;
    bool isWatchAd = false;
    int curLvl;

    protected override void OnEnter()
    {
        base.OnEnter();
        instance = this;
        Init();
    }
    public void Init()
    {
        ReportRegister();
        PlatformMgr.Instance.AddLifeCycleEvent(() => { ReportHide(Time.realtimeSinceStartup - showStartTime); },() => { ReportShow(); });
    }

    /// <summary>
    /// 注册
    /// </summary>
    public void ReportRegister()
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("register", DateTime.Now.ToString("d"));
        StartCoroutine(SendReport(dic, "register"));
    }

    public void ReportHide(float time)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("duration", Math.Round(time, 2).ToString());
        StartCoroutine(SendReport(dic, "app_hide"));
    }

    public void ReportShow()
    {
        showStartTime = Time.realtimeSinceStartup;
        Dictionary<string, string> dic = new Dictionary<string, string>();
        StartCoroutine(SendReport(dic, "app_show"));
    }

    /// <summary>
    /// 开始加载资源
    /// </summary>
    public void StartLoad()
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("time", DateTime.Now.ToString("d"));
        StartCoroutine(SendReport(dic, ReportStr.loadStart));
    }

    /// <summary>
    /// 加载资源结束
    /// </summary>
    public void EndLoad()
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("time", DateTime.Now.ToString("d"));
        StartCoroutine(SendReport(dic, ReportStr.loadEnd));
    }

    /// <summary>
    /// 主页面分享
    /// </summary>
    public void MainShare(bool result)
    {
         Dictionary<string, string> dic = new Dictionary<string, string>();
         dic.Add("result", result.ToString());
         StartCoroutine(SendReport(dic, ReportStr.mainShare));
    }

    public const string EnterGameLevelKey = "EnterGameLevelKey_1";
    private List<int> recordLevelIdList = null;
    /// <summary>
    /// 进入关卡--每关只统计一次
    /// </summary>
    public void EnterGameLevel(int levelId)
    {
        if (recordLevelIdList == null)
        {
            if(PlayerPrefs.HasKey(EnterGameLevelKey))
            {
                string strData = PlayerPrefs.GetString(EnterGameLevelKey);
                recordLevelIdList = JsonUtility.FromJson<List<int>>(strData);
            }
            else
            {
                recordLevelIdList = new List<int>();
            }
        }
        
        if (recordLevelIdList.Contains(levelId))
        {
            return;
        }
        recordLevelIdList.Add(levelId);
        PlayerPrefs.SetString(EnterGameLevelKey, JsonUtility.ToJson(recordLevelIdList));
        PlayerPrefs.Save();
        
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("level", levelId.ToString());
        StartCoroutine(SendReport(dic, ReportStr.levelEnter));
    }

    /// <summary>
    /// 关卡开始
    /// </summary>
    public void StartGameLevel(int levelId)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("level", levelId.ToString());
        StartCoroutine(SendReport(dic, ReportStr.levelStart));
    }

    /// <summary>
    /// 通关关卡
    /// </summary>
    public void PassGameLevel(int level, float time)
    {
        //每关卡平均通关时长
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("level", level.ToString());
        dic.Add("time", time.ToString());
        StartCoroutine(SendReport(dic, ReportStr.levelPass));
    }

    /// <summary>
    /// 关卡失败
    /// </summary>
    public void LevelFail(int lvl, float progress)
    {
        //每关卡完成进度的各区间内用户流失数
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("level", lvl.ToString());
        dic.Add("progress", progress.ToString());
        StartCoroutine(SendReport(dic, ReportStr.levelFail));
    }

    /// <summary>
    /// 使用道具
    /// </summary>
    public void UseProp(int propId, int lvl, float progress)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("level", lvl.ToString());
        dic.Add("propId", propId.ToString());
        dic.Add("progress", progress.ToString());
        StartCoroutine(SendReport(dic, ReportStr.useProp));
    }

    /// <summary>
    /// 重新挑战
    /// </summary>
    public void ResetLevel(int lvl, string tag)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("level", lvl.ToString());
        dic.Add("tag", tag);
        StartCoroutine(SendReport(dic, ReportStr.resetLevel));
    }

    /// <summary>
    /// 返回主界面
    /// </summary>
    public void BackMainPage(int lvl, string tag)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("level", lvl.ToString());
        dic.Add("tag", tag);
        StartCoroutine(SendReport(dic, ReportStr.backMainPage));
    }

    /// <summary>
    /// 观看广告
    /// </summary>
    public void WatchAd(bool lookAdResult, string watchAdType,int? levelId,int? propId)
    {
        int? targetId = -1;
        if (levelId.HasValue)
        {
            targetId = levelId;
        }
        else
        {
            //TODO:当前等级
            // int? level = LevelMgr.Instance?.CurrentLevelId;
            // targetId = level.HasValue ? level.Value : -1;
        }
        
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("lookAdResult", lookAdResult.ToString());
        dic.Add("watchAdType", watchAdType.ToString());
        dic.Add("level", (targetId ?? -1).ToString());
        dic.Add("propId", (propId ?? -1).ToString());
        StartCoroutine(SendReport(dic, ReportStr.watchAd));
    }

    private IEnumerator SendReport(Dictionary<string,string> dic, string eventName, 
        ReportType reportType = ReportType.Track, UnityAction callback = null)
    {
        PlatformMgr.Instance.Report(eventName,dic);
        yield break;
    }
}