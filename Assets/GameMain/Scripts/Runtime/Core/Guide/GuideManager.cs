using cfg;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace GameMain.NewGuide
{
    public enum EGuideTriggerType
    {
        [LabelText("第一次进入游戏")]
        FirstEnterGame = 1,

    }

    public class GuideManager : ASingleBase<GuideManager>
    {
        private EventGroup _guideEventGroup;

        public Save_NewGuide SaveNewGuide => LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_NewGuide>();

        private Dictionary<EGuideTriggerType, IGuideCondition> _condiionKv = new()
        {
            {EGuideTriggerType.FirstEnterGame, new FirstEnterGameCondition() },
        };

        public override void Initialize()
        {
            base.Initialize();

            _guideEventGroup = new();
            _guideEventGroup.AddListener<GameEventDefine.NewGuide_FirstEnterGame>(Processmessage);
        }

        public override void UnInitialize()
        {
            base.UnInitialize();
            _guideEventGroup.RemoveAllListener();
        }

        private bool GuideIsFinish(int guideId)
        {
            return SaveNewGuide.finish_guides.Contains(guideId);
        }

        public bool GuideIsRunning(int guideId)
        {
            return SaveNewGuide.active_must_guides.Contains(guideId);
        }

        private void Processmessage(IEventMessage message)
        {
            if (message is GameEventDefine.NewGuide_FirstEnterGame firstEnterGame)
            { 
                
            }
        }
    }
}