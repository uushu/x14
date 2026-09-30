namespace GameMain.AOT
{
    public interface IStateNode
	{
/// <summary>
/// 创建状态机时的回调方法
/// </summary>
/// <param name="machine">被创建的状态机实例</param>
		void OnCreate(StateMachine machine);
		void OnEnter();
		void OnUpdate();
		void OnExit();
	}
}