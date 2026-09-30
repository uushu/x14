using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using GameMain;
using UnityEngine;

public static class CommonTool
{
    /// <summary>
    /// 时间转换为时间戳--Local
    /// </summary>
    /// <return>秒</return>
    public static long DataTimeConvertToTimeStamp(DateTime dataTime = default)
    {
        if (dataTime == default) dataTime = DateTime.Now;
        TimeSpan ts = dataTime - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
        long timeStampStr = Convert.ToInt64(ts.TotalSeconds);
        return timeStampStr;
    }

    /// <summary>
    /// 时间戳转换为时间--Local
    /// </summary>
    /// <param name="timestamp">秒</param>
    public static DateTime TimestampConvertToDate(long timestamp)
    {
        DateTime dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
        return dtStart.AddSeconds(timestamp);
    }
    
    public static long DataTimeConvertToTimeStampNoZone(DateTime dataTime = default)
    {
        if (dataTime == default) dataTime = dataTime = DateTime.Now;
        TimeSpan ts = dataTime - new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
        long timeStampStr = Convert.ToInt64(ts.TotalSeconds);
        return timeStampStr;
    }
    
    /// <summary>
    /// 世界坐标转局部坐标
    /// </summary>
    /// <param name="tran">转换的坐标系原点</param>
    /// <param name="wordSpace">世界坐标</param>
    public static Vector3 WordPositionToLocalPosition(Transform tran, Vector3 wordSpace)
    {
        return tran.InverseTransformPoint(wordSpace);
    }

    /// <summary>
    /// 获得当天时间标签
    /// </summary>
    public static string GetTodayTag()
    {
        return DateTime.Now.ToString("yyyy-MM-dd");
    }
    
    public static bool EnoughRes(Dictionary<int, int> cost)
    {
        foreach (var list in cost)
        {
            if (!EnoughRes(list.Key, list.Value))
            {
                return false;
            }
        }
        return true;
    }
    public static bool EnoughRes(int id, int Number)
    {
        if (id == 0)
        {
            return true;
        }
        return BagMgr.Instance.GetBagPropCount(id) >= Number;
    }
    
    
    public static void MerNeedItem(Dictionary<int, int> items,Dictionary<int,int> OrgicDic)
    {
        foreach (var item in items)
        {
            if (!OrgicDic.ContainsKey(item.Key))
            {
                OrgicDic.Add(item.Key, item.Value);
            }
            else
            {
                OrgicDic[item.Key] += item.Value;
            }
        }
    }
}