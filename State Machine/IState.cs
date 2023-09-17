namespace State_Machine
{
    public interface IState
    {
        void OnEnter();
        void OnExit();
        void Update();
    }
}