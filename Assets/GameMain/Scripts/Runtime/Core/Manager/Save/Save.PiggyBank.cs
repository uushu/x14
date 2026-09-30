using cfg;

namespace GameMain
{
    public class Save_PiggyBank : ASaveData
    {
        public long CurPiggyBankMoney;

        public override void SetDefaultData(GlobalConfigCategory category)
        {
        }

        public void RestPiggyBankMoney()
        {
            CurPiggyBankMoney = 0;
            GameEventDefine.SendPiggyBankGoldChange();
        }

        public void AddPiggyBankMoney(long money)
        {
            //money = 100;
            CurPiggyBankMoney += money;
            GameEventDefine.SendPiggyBankGoldChange();
        }
    }
}