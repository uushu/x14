using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    //事件名
    public static class RecordEvent
    {
        public const string DebugPrefix = "【打点】";

        //引力引擎元事件
        public const string event_meta_level_start = "level_start";                      //开始关卡
        public const string event_meta_level_success = "level_success";                  //关卡成功 
        public const string event_meta_level_failed = "level_failed";                    //关卡失败
        public const string event_meta_level_relive = "level_relive";                    //关卡复活
        public const string event_meta_level_restart = "level_restart";                  //关卡重新挑战
        public const string event_meta_buy_Prop = "buy_Prop";                            //道具购买
        //未使用
        public const string event_meta_use_prop = "use_prop";                            //使用道具
        public const string event_meta_back_hall = "back_hall";                          //返回大厅
        public const string event_meta_scene_load_start = "scene_load_start";            //开始加载场景
        public const string event_meta_scene_load_complete = "scene_load_complete";      //完成加载场景
        public const string event_meta_scene_load_use_time = "scene_load_use_time";      //加载场景耗时
        public const string event_meta_share_game = "share_game";                        //分享游戏

        //引力引擎事件属性名
        public const string event_name_isFirstDayUser = "firstDay_User";                 //新用户
        public const string event_name_level_id = "level_id";                            //关卡Id int
        public const string event_name_game_type = "game_type";                          //游戏类型
        public const string event_name_time = "time";                                    //时间 秒 int
        public const string event_name_date_time = "date_time";                          //日期
        public const string event_name_score = "score";                                  //分数
        public const string event_name_propTag = "prop_tag";                             //道具标签
        //未使用
        public const string event_name_propId = "prop_id";                               //道具Id int
        public const string event_name_progress = "progress";                            //进度 float
        public const string event_name_scene_name = "scene_name";                        //场景名
        public const string event_name_timestamp = "timestamp";                          //时间戳 int
        public const string event_name_result = "result";                                //结果 true / false

    }
}