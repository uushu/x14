using cfg;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    public static class AudioName
    {
        public const string ChatPhone = "phone_dx";

        public const string GetAward = "bgm_hdjj";

        public static AudioClipConfig BgmHall => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(1);
        public static AudioClipConfig BgmGarden => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(2);
        public static AudioClipConfig UIClickDefault => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(3);
        public static AudioClipConfig BgmFlowerBlock => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(4);

        public static AudioClipConfig GetAwardAudio => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(1001);
        public static AudioClipConfig RankAudio => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(1002);
        public static AudioClipConfig UseBagPropAudio => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(1003);
        public static AudioClipConfig VictoryAudio => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(1004);
        public static AudioClipConfig GardenFailedAudio => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(1005);
        public static AudioClipConfig FlowerDownAudio => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(1006);
        public static AudioClipConfig FlowerSuccessAudio => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(1007);
        public static AudioClipConfig FlowerPutInMapAudio => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(1008);
        public static AudioClipConfig ButterflyHitAudio => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(1009);
        public static AudioClipConfig RotatePropAudio => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(1010);
        public static AudioClipConfig MaisaiketiaodongAudio => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(1011);
        public static AudioClipConfig jinbihuode => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(1012);
        public static AudioClipConfig GaedenGameGoalAudio => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(1013);
        public static AudioClipConfig GardenRemoveBagPropAudio => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(1014);
        public static AudioClipConfig CompleteGameAudio => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(1015);

        public static AudioClipConfig xianhuahuode => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(1017);
        
        public static AudioClipConfig duanweishengji => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(1018);
        public static AudioClipConfig RemoveLine6 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(2006);
        public static AudioClipConfig RemoveLine5 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(2005);
        public static AudioClipConfig RemoveLine4 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(2004);
        public static AudioClipConfig RemoveLine3 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(2003);
        public static AudioClipConfig RemoveLine2 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(2002);
        public static AudioClipConfig RemoveLine1 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(2001);
        public static AudioClipConfig CanRemoveAudio => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(2010);
        public static AudioClipConfig GardenReliveAudio => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(2011);

        public static AudioClipConfig ComboLine6 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(3006);
        public static AudioClipConfig ComboLine5 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(3005);
        public static AudioClipConfig ComboLine4 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(3004);
        public static AudioClipConfig ComboLine3 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(3003);
        public static AudioClipConfig ComboLine2 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(3002);
        public static AudioClipConfig ComboLine1 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(3001);

        //花了个花连击
        public static AudioClipConfig Block_Combo1 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(4001);
        public static AudioClipConfig Block_Combo2 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(4002);
        public static AudioClipConfig Block_Combo3 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(4003);
        public static AudioClipConfig Block_Combo4 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(4004);
        public static AudioClipConfig Block_Combo5 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(4005);

        //花了个花消除
        public static AudioClipConfig Block_Remove1 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(4011);
        public static AudioClipConfig Block_Remove2 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(4012);
        public static AudioClipConfig Block_Remove3 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(4013);
        public static AudioClipConfig Block_Remove4 => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(4014);

        //花了个花其他
        public static AudioClipConfig Block_FlowerDropDown => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(4020);
        public static AudioClipConfig Block_FlowerGenerator => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(4021);
        public static AudioClipConfig Block_FlowerDrag => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(4022);
        public static AudioClipConfig Block_FlowerBloom => ConfigMgr.Instance.Tables.AuidoClipCategory.GetOrDefault(4023);
    }
}
