using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameMain.Garden;
using TMPro;
using UnityEngine;

namespace GameMain.Water
{
    public class HallWindow : ABaseWindow
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnGame_EndlessMode;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnGame_LevelMode;
        
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton MainChallengeBtn;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton MainRankBtn;
        
        
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton MainCollectionBtn;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public HallToggleNode mHallToggleNode;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public Transform LeftNode;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public Transform RightNode;


        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public Transform MainNode;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public Transform ShopNode;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public Transform CollectionNode;

        
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public Transform CollectionContentNode;
        
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public Transform ShopContentNode;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TextMeshProUGUI LevelModel_Level;
        
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UINormal_StartGame mBtnGame_LevelStartGame;

        private CollectionPrefab collectionPrefab;
        private ShopPrefab shopPrefab;
        protected override void OnShow(object parameters)
        {
            base.OnShow(parameters);
            //mBtnGame_LevelMode.AddListener(OnEnterGardenGameClick);
            
           // MainTreasureChestBtn.AddListener(OnTreasureChestClick);
            MainChallengeBtn.AddListener(OnChallengeClick);

            MainRankBtn.AddListener(OnRankClick);
          
            MainCollectionBtn.AddListener(OnCollectionClick);
            mHallToggleNode.InitAction(1, OnClickAction);

            LeftNode.transform.localPosition = new Vector3(-1000, 0, 0);
            RightNode.transform.localPosition = new Vector3(1000, 0, 0);
            mBtnGame_EndlessMode.transform.localScale = Vector3.zero;
            mBtnGame_LevelMode.transform.localScale = Vector3.zero;
            OnShowBtn();

            var level = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().levelId;
            Debugger.Log($"当前关卡ID:{level},显示关卡:{level-1000}");
            LevelModel_Level.text = $"第{level-1000}关";
        }

        public async UniTask OnHideBtn()
        {
            LeftNode.transform.DOLocalMoveX(-1000, 0.5f);
            RightNode.transform.DOLocalMoveX(1000, 0.5f);
            mBtnGame_EndlessMode.transform.DOScale(0, 0.5f);
            mBtnGame_LevelMode.transform.DOScale(0, 0.5f);
            await UniTask.Delay(500);
        }

        public async UniTask OnShowBtn()
        {
            LeftNode.transform.DOLocalMoveX(0, 0.5f);
            RightNode.transform.DOLocalMoveX(0, 0.5f);
            mBtnGame_EndlessMode.transform.DOScale(1, 0.5f);
            mBtnGame_LevelMode.transform.DOScale(1, 0.5f);
            await UniTask.Delay(500);
        }

        private async void OnClickAction(int obj)
        {
            ShopNode.gameObject.SetActive(obj == 0);
            MainNode.gameObject.SetActive(obj == 1);
            CollectionNode.gameObject.SetActive(obj == 2);
            if (obj == 2)
            {
                if (collectionPrefab==null)
                {
                    collectionPrefab= await LoadPrefab<CollectionPrefab>("CollectionPrefab",CollectionContentNode);
                }

                collectionPrefab.Init();
            }
            
            if (obj == 0)
            {
                if (shopPrefab==null)
                {
                    shopPrefab= await LoadPrefab<ShopPrefab>("ShopPrefab",ShopContentNode);
                }

                shopPrefab.Init();
            }
        }
        
        private async Task<T> LoadPrefab<T>(string Prefabname,Transform tf)
        {
            var go= await ResourceMgr.Instance.LoadAssetAsync<GameObject>(Prefabname);
            var instance = Instantiate(go, tf);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localScale = Vector3.one;
            var prefab= instance.GetComponent<T>();
            return prefab;
        }
        
        private void OnCollectionClick()
        {
            UIWindowMgr.Instance.OpenWindow<CollectWindow>();
        }
        //int index = 1002;
        private void OnRankClick()
        {
   
          //  ItemData_BagProp item = new ItemData_BagProp(index, 1);
          //  UIWindowMgr.Instance.OpenWindowAsync<CommGetFlowerstView>(item);
          //  index++;
              UIWindowMgr.Instance.OpenWindowAsync<RankWindow>(1);
              //UIWindowMgr.Instance.OpenWindowAsync<RankWindow1>(1);
        }

        private void OnChallengeClick()
        {
        }
        
        public override void OnTop()
        {
            OnShowBtn();
            HyzShareManager.Instance.GetMyShareInfo(HyzShareManager.SharePosType.None, (info) =>
            {
                AchievementMgr.Instance.SetAchievementShare(info);
            }, () =>
            {
                
            });
           // PlatformMgr.Instance.DirectedShare("share", "share");
        }

     
        private void OnEnterGardenGameClick()
        {
            UIWindowMgr.Instance.OpenWindow<GardenWindow>();
        }
    }
}