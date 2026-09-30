using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cfg;
using GameMain;
using UnityEngine;
using Random = System.Random;

public class GameTool : MonoSingleton<GameTool>
{
    public static int ItemGoldId = 1;
  
    public static int AchievementPintId = 4;
    
    public string GetQualityBg(EColorType type)
    {
        string path = "";
        switch (type)
        {
            case EColorType.Green:
                path = "tongy_lvpinz";
                break;
            case EColorType.Blue:
                path = "tongy_lanpinz";
                break;
            case EColorType.Purple:
                path = "tongy_zipinz";
                break;
            case EColorType.Orange:
                path = "tongy_chengpinz";
                break;
            case EColorType.None:
            default:
                break;
        }
        return path;
    }

    public string GetQualityIcon(EColorType type)
    {
        string path = "";
        switch (type)
        {
            case EColorType.Green:
                path = "shangc_splv";
                break;
            case EColorType.Blue:
                path = "shangc_splan";
                break;
            case EColorType.Purple:
                path = "shangc_spzi";
                break;
            case EColorType.Orange:
                path = "shangc_spzi";
                break;
            case EColorType.None:
            default:
                break;
        }
        return path;
    }
}

/// <summary>
/// 随机品质信息
/// </summary>
public class RandomQualityInfo
{
    public EColorType Quality;
    public int MaxCount;
}

public class RandomQualityInfoFloat
{
    public EColorType Quality;
    public float MaxCount;
}

/// <summary>
/// 道具权重信息
/// </summary>
public class PropWightData
{
    public int PropId;
    public int GetWeight;
    public int NumWeight;

    public BagPropConfig data => ConfigMgr.Instance.Tables.BagPropCategory.GetOrDefault(PropId);
}