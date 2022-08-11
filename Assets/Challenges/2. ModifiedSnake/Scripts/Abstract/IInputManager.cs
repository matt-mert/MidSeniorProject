namespace Challenges._2._ModifiedSnake.Scripts.Abstract
{
    /// <summary>
    /// The input manager is responsible for detecting player input to determine direction
    /// </summary>
    public interface IInputManager
    {
        void StartController();
        void StopController();
    }
}