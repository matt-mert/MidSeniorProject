namespace Challenges._2._ModifiedSnake.Scripts.Data
{
    public enum OccupancyType
    {
        None,
        Food,
        SnakeBlock,
        BridgePort,
        BridgeReject,
        BridgeAccept,
        BridgePlatform,
        Deadly
    }

    public enum OccupancyLayer
    {
        Top,
        Ground,
        Bottom
    }
    
    public enum Direction
    {
        Up,Right,Down,Left,None
    }

    public enum BridgeDirection
    {
        UpVertical, RightHorizontal
    }
}