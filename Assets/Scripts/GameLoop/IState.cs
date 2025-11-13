public interface IState
{
    void Enter();   // Called when entering the state
    void Update();  // Called every frame while in the state
    void Exit();    // Called when exiting the state
}
