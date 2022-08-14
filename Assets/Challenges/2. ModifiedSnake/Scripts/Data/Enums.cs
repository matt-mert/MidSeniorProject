namespace Challenges._2._ModifiedSnake.Scripts.Data
{
    public enum OccupancyType
    {
        None,
        Food,
        SnakeBlock,
    }

    public enum BlockType
    {
        Default,
        Deadly,
        BridgePort,
        BridgePlatform,
        BridgeAccept,
        BridgeReject
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