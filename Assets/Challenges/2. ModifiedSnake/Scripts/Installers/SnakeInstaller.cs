using Challenges._2._ModifiedSnake.Scripts.Blocks;
using Challenges._2._ModifiedSnake.Scripts.Data;
using Challenges._2._ModifiedSnake.Scripts.Systems;
using Zenject;

namespace Challenges._2._ModifiedSnake.Scripts.Installers
{
    /// <summary>
    /// The installer is provided to SceneContext.
    /// Zenject will create all of these objects for us
    /// </summary>
    public class SnakeInstaller : MonoInstaller
    {
        public SnakeGameData snakeGameData;

        public SnakeBlock snakeBlockPrefab;
        public SnakeHeadBlock snakeHeadBlockPrefab;
        public FoodBlock foodPrefab;

        public BridgePlatformBlock bridgePlatformBlockPrefab;
        public BridgePortBlock bridgePortBlockPrefab;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            #region Events
            Container.DeclareSignal<LevelCompleteEvent>();
            Container.DeclareSignal<LevelFailedEvent>();
            #endregion

            #region Data
            //If anyone requests a SnakeGameData instance, the 'snakeGameData' reference will be passed on
            Container.Bind<SnakeGameData>().FromInstance(snakeGameData).AsSingle().NonLazy();
            #endregion
            
            #region Managers
            //The systems aren't accessible through concrete types. If someone requested an GameStateHandler, Zenject would throw an exception
            //However if someone requests an IGameStateHandler, the created instance will be provided (Zenject creates these instances and calls their constructor, handles all the injections)
            //Since the GameStateHandler implements the IInitializable interface, Zenject will inject it into it's internal InitializableManager, this is essentially the Start() function for non-monobehaviours
            //Similar to IInitializable, there are: IDisposable, ITickable, ILateTickable etc which are essentially OnDestroy(), OnUpdate(), OnLateUpdate()...
            Container.BindInterfacesTo<GameManager>().AsSingle().NonLazy();
            Container.BindInterfacesTo<InputManager>().AsSingle().NonLazy();
            Container.BindInterfacesTo<FoodGenerator>().AsSingle().NonLazy();
            Container.BindInterfacesTo<OccupancyHandler>().AsSingle().NonLazy();
            Container.BindInterfacesTo<SnakeBodyController>().AsSingle().NonLazy();
            Container.BindInterfacesTo<SnakeMovementController>().AsSingle().NonLazy();
            Container.BindInterfacesTo<GameStateHandler>().AsSingle().NonLazy();

            Container.BindInterfacesTo<BridgeGenerator>().AsSingle().NonLazy();
            Container.BindInterfacesTo<BlockTypeHandler>().AsSingle().NonLazy();
            #endregion

            #region Utility
            Container.BindInterfacesTo<Map>().AsSingle().NonLazy();
            #endregion

            #region Blocks
            //This tells Zenject that if someone requests a SnakeHeadBlock, it should create a new instance of the given prefab under the transform 'SnakeBlocks' and inject that instance
            // .AsSingle() ensures that only ONE instance is created, so the instantiation only takes place when it's requested and the same instance is injected afterwards.
            // .NonLazy() tells zenject to instantiate this prefab ASAP even if nobody's requested it yet.
            Container.Bind<SnakeHeadBlock>().FromComponentInNewPrefab(snakeHeadBlockPrefab).UnderTransformGroup("SnakeBlocks").AsSingle().NonLazy();
            
            //The following are Zenject's own pools. They will handle all the work of pooling (and injecting into the elements in the pool)
            Container.BindMemoryPool<SnakeBlock, SnakeBlock.SnakeBlockPool>().WithInitialSize(snakeGameData.startLength).FromComponentInNewPrefab(snakeBlockPrefab).UnderTransformGroup("SnakeBlocks").NonLazy();
            Container.BindMemoryPool<FoodBlock, FoodBlock.FoodBlockPool>().FromComponentInNewPrefab(foodPrefab).UnderTransformGroup("FoodBlocks").NonLazy();

            //The prefabs required for the bridge to be instantiated.
            Container.BindMemoryPool<BridgePlatformBlock, BridgePlatformBlock.BridgePlatformBlockPool>().FromComponentInNewPrefab(bridgePlatformBlockPrefab).UnderTransformGroup("BridgeBlocks").NonLazy();
            Container.BindMemoryPool<BridgePortBlock, BridgePortBlock.BridgePortBlockPool>().FromComponentInNewPrefab(bridgePortBlockPrefab).UnderTransformGroup("BridgeEnterBlocks").NonLazy();
            #endregion
        }
    }
}