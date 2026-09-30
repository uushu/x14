using Cysharp.Threading.Tasks;
using GameMain.BTree;
using UnityEngine;

namespace GameMain.Garden
{
    public class FingerGuideNextLevel : ABaseActionNode
    {
        protected override async void OnStart()
        {
            var successWindow = UIWindowMgr.Instance.GetWindow<GardenSuccessWindow>();
            successWindow.mBtnMainPage.SetInteractable(false);

            await UniTask.WaitForSeconds(2f);

            var guideWindow = await UIWindowMgr.Instance.OpenWindowAsync<GuideWindow>();
            guideWindow.DoFinterClickAnimation(successWindow.mBtnNextLevel.transform.position);

            var btnSuccessRect = successWindow.mBtnNextLevel.GetComponent<RectTransform>();
            btnSuccessRect.AddEventListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, (_) =>
            {
                guideWindow.StopFingerAnimation();
                btnSuccessRect.RemoveEventListener();
                SetFinish(true);
            });
        }

        protected override void OnCancel()
        {
        }
    }
}