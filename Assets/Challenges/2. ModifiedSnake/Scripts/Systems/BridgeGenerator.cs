using System;
using System.Collections.Generic;
using System.Threading;
using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Blocks;
using Challenges._2._ModifiedSnake.Scripts.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Systems
{
    /// <summary>
    /// Generates the desired bridges in the beginning of the game.
    /// </summary>
    public class BridgeGenerator : IBridgeGenerator, IGameSystem
    {
        private readonly SnakeGameData _snakeGameData;
        private readonly IOccupancyHandler _occupancyHandler;


        public void GenerateBridges()
        {

        }

        public void ClearBridges()
        {

        }

        public void StartSystem()
        {
            GenerateBridges();
        }

        public void StopSystem()
        {

        }

        public void ClearSystem()
        {
            ClearBridges();
        }
    }
}
