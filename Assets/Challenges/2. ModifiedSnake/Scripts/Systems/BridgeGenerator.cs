using System;
using System.Collections.Generic;
using System.Threading;
using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Blocks;
using Challenges._2._ModifiedSnake.Scripts.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Challenges._2._ModifiedSnake.Scripts.Systems
{
    /// <summary>
    /// Generates the desired bridges in the beginning of the game.
    /// </summary>
    public class BridgeGenerator : IBridgeGenerator, IInitializable, IGameSystem
    {
        private readonly SnakeGameData _snakeGameData;
        private readonly IOccupancyHandler _occupancyHandler;
        private readonly IMap _map;
        private readonly BridgeEnterBlock.BridgeEnterBlockPool _bridgeEnterBlockPool;
        private readonly BridgeExitBlock.BridgeExitBlockPool _bridgeExitBlockPool;

        public BridgeGenerator(SnakeGameData snakeGameData, IOccupancyHandler occupancyHandler, IMap map,
            BridgeEnterBlock.BridgeEnterBlockPool bridgeEnterBlockPool, BridgeExitBlock.BridgeExitBlockPool bridgeExitBlockPool)
        {
            _snakeGameData = snakeGameData;
            _occupancyHandler = occupancyHandler;
            _map = map;
            _bridgeEnterBlockPool = bridgeEnterBlockPool;
            _bridgeExitBlockPool = bridgeExitBlockPool;
        }

        public void GenerateBridges()
        {
            if (_snakeGameData.bridgesData.Length == 0) return;

            foreach (BridgeData bridgeData in _snakeGameData.bridgesData)
            {

            }
        }

        public void ClearBridges()
        {

        }

        public void Initialize()
        {
            GenerateBridges();
        }

        public void ClearSystem()
        {
            ClearBridges();
        }

        public void StartSystem()
        {

        }

        public void StopSystem()
        {

        }
    }
}
