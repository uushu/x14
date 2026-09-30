using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;

namespace GameMain.FlowerLine
{
    public class FlowerLineGameRoot : AGameBaseRoot<FlowerLineGameRoot>
    {
        public override EMiniGameType eMiniGameType => EMiniGameType.LineFlower;

        [LabelText("µØÍ¼")]
        public FlowerLineMap lineMap;

        [LabelText("³Ø×Ó")]
        public FlowerLinePool pool;

        protected override void OnEnter()
        {
            base.OnEnter();

            lineMap.Initialize();
            pool.Initialize();
        }

        public override void StartGame(int levelId, params object[] parameters)
        {
            base.StartGame(levelId, parameters);

            lineMap.StartGame(levelId);
        }

        protected override void OnUpdateInternal(float deltaTime)
        {
            base.OnUpdateInternal(deltaTime);

        }

        protected override void OnExit()
        {
            base.OnExit();

            lineMap.UnInitialize();
            pool.UnInitialize();
        }
    }
}