public interface IUIPanelLogic
{
    void OnCreate();

    void OnEnter(params object[] onEnterParams);

    void OnUpdate();

    void OnExit();

}
