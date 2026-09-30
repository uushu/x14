using cfg;
using Cysharp.Threading.Tasks;
using GameMain.AOT;
using GameMain.FlowerBlock;
using GameMain.FlowerLine;
using GameMain.Garden;
using GameMain.Water;
using System.Collections.Generic;

namespace GameMain
{
    public static class UIWindowTool
    {
        public static List<ItemData_BagProp> MultiBagProps(List<ItemData_BagProp> list, int multi)
        {
            var retList = new List<ItemData_BagProp>();

            for (int i = 0; i < list.Count; i++)
            {
                retList.Add(new ItemData_BagProp(list[i].PropId, list[i].PropCount * multi));
            }
            
            return retList;
        }

        public static List<ItemData_BagProp> NewBagProps(List<int> propId, List<int> propCount)
        {
            if (propId.Count != propCount.Count)
            {
                Debugger.LogError($"创建道具失败 数量不一致 {propId.Count} != {propCount.Count}");
                return null;
            }

            var list = new List<ItemData_BagProp>();
            for (int i = 0; i < propId.Count; i++)
            {
                list.Add(new ItemData_BagProp(propId[i], propCount[i]));
            }

            return list;
        }

        public static async void JumpWindow(JumpWindowConfig config, System.Action complete = null)
        {
            for (int i = 0; i < config.JumpPath.Count; i++)
            {
                await UIWindowMgr.Instance.OpenWindowAsync(config.JumpPath[i]);
            }

            complete?.Invoke();
        }

        public static void ShowPromptBox(string content)
        {
            UIWindowMgr.Instance.ShowPromptBox(content);
        }
        /// <summary>
        /// 显示遮罩界面
        /// </summary>
        public static async UniTask ShowMaskWindow(float alpha = 0.6f)
        {
            await UIWindowMgr.Instance.ShowMaskWindow(alpha);
        }
        /// <summary>
        /// 关闭遮罩界面
        /// </summary>
        public static void CloseMaskWindow()
        {
            UIWindowMgr.Instance.CloseMaskWindow();
        }

        public static async UniTask ChangeToHallScene(bool isFirstLoginGame)
        {
            AudioMgr.Instance.StopAllAudio();
            await UIWindowMgr.Instance.CloseAllWindow(null);

            //游戏场景返回主界面
            await UIWindowMgr.Instance.OpenWindowAsync<HallWindow>();
            AudioMgr.Instance.PlayBgm(AudioName.BgmHall);
        }

        public static async UniTask FirstEnterGame()
        {
            switch (Init.Instance.miniGameType)
            {
                case EMiniGameType.GardenFlower:

                    await ChangeToGardenGame(true);
                    break;
                case EMiniGameType.BlockFlower:

                    await ChangeToBlockGame(true);
                    break;
                case EMiniGameType.LineFlower:

                    await ChangeToLineGame(true);
                    break;
                default:

                    Debugger.LogError($"未处理的游戏类型:{Init.Instance.miniGameType}");
                    break;
            }
        }

        public static async UniTask ChangeToGardenGame(bool isfirst)
        {
            AudioMgr.Instance.StopAllAudio();

            if (isfirst == false)
            {
                LoadingWindow loading = await OpenLoadingWindow();
                await loading.WaitAnimationOpenComplete();
            }

            await UIWindowMgr.Instance.CloseAllWindow(new List<string>() { nameof(LoadingWindow) });

            GardenWindow garden = await UIWindowMgr.Instance.OpenWindowAsync<GardenWindow>();

            if (isfirst == false)
            {
                var loading = UIWindowMgr.Instance.GetWindow<LoadingWindow>();
                loading.PlayCloseAni();
                await loading.WaitAnimationCloseComplete();
                await CloseLoadingWindow();
            }
        }

        public static async UniTask ChangeToGardenEndlessGame()
        {
            AudioMgr.Instance.StopAllAudio();

            LoadingWindow loading = await OpenLoadingWindow();
            await loading.WaitAnimationOpenComplete();

            await UIWindowMgr.Instance.CloseAllWindow(new List<string>() { nameof(LoadingWindow) });

            var endlessGame = await UIWindowMgr.Instance.OpenWindowAsync<GardenEndlessWindow>();

            loading.PlayCloseAni();
            await loading.WaitAnimationCloseComplete();

            await CloseLoadingWindow();
        }

        public static async UniTask ChangeToBlockGame(bool isfirst)
        {
            AudioMgr.Instance.StopAllAudio();
            if (isfirst == false)
            {
                LoadingWindow loading = await OpenLoadingWindow();
                await loading.WaitAnimationOpenComplete();
            }
            await UIWindowMgr.Instance.CloseAllWindow(new List<string>() { nameof(LoadingWindow) });

            await UIWindowMgr.Instance.OpenWindowAsync<FlowerBlockWindow>();

            if (isfirst == false)
            {
                var loading = UIWindowMgr.Instance.GetWindow<LoadingWindow>();
                loading.PlayCloseAni();
                await loading.WaitAnimationCloseComplete();
                await CloseLoadingWindow();
            }
        }

        public static async UniTask ChangeToBlockEndlessGame()
        {
            AudioMgr.Instance.StopAllAudio();

            LoadingWindow loading = await OpenLoadingWindow();
            await loading.WaitAnimationOpenComplete();

            await UIWindowMgr.Instance.CloseAllWindow(new List<string>() { nameof(LoadingWindow) });

            var endlessGame = await UIWindowMgr.Instance.OpenWindowAsync<FlowerBlockEndlessWindow>();

            loading.PlayCloseAni();
            await loading.WaitAnimationCloseComplete();

            await CloseLoadingWindow();
        }

        public static async UniTask ChangeToLineGame(bool isfirst)
        {
            AudioMgr.Instance.StopAllAudio();
            if (isfirst == false)
            {
                LoadingWindow loading = await OpenLoadingWindow();
                await loading.WaitAnimationOpenComplete();
            }
            await UIWindowMgr.Instance.CloseAllWindow(new List<string>() { nameof(LoadingWindow) });

            //游戏场景返回主界面
            await UIWindowMgr.Instance.OpenWindowAsync<FlowerLineWindow>();

            if (isfirst == false)
            {
                var loading = UIWindowMgr.Instance.GetWindow<LoadingWindow>();
                loading.PlayCloseAni();
                await loading.WaitAnimationCloseComplete();
                await CloseLoadingWindow();
            }
        }
        
        public static async UniTask<LoadingWindow> OpenLoadingWindow(float duration = 0.3f)
        {
            return await UIWindowMgr.Instance.OpenWindowAsync<LoadingWindow>(duration);
        }

        public static async UniTask CloseLoadingWindow()
        {
            await UIWindowMgr.Instance.CloseWindow<LoadingWindow>(false);
        }
    }
}