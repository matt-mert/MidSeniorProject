namespace Challenges._2._ModifiedSnake.Scripts.Abstract
{
    /// <summary>
    /// Spawns food on the map periodically
    /// </summary>
    public interface IFoodGenerator
    {
        void StartGeneration();
        void StopGeneration();
    }
}