using Challenges._2._ModifiedSnake.Scripts.Data;

namespace Challenges._2._ModifiedSnake.Scripts.Abstract
{
    /// <summary>
    /// Input manager catches all IInputListener's and notifies them of the snakes direction
    /// </summary>
    public interface IInputListener
    {
        bool SetActiveDirection(Direction direction);
        
    }
}