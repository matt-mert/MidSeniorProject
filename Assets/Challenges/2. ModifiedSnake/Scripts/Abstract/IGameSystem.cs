namespace Challenges._2._ModifiedSnake.Scripts.Abstract
{
    /// <summary>
    /// The game manager will catch all IGameSystem's and call these functions appropriately
    /// </summary>
    public interface IGameSystem
    {
        void StartSystem();
        void StopSystem();
        void ClearSystem();
    }
}