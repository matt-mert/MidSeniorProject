using System.Threading;
using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Systems
{
    

    
    public class InputManager : IInputManager, IGameSystem
    {
        private readonly IMap _map;
        private readonly IInputListener[] _inputListeners;
        private CancellationTokenSource _src;
        private bool _loopActive = false;
        private Direction _currentDirection;

        public InputManager(IMap map, IInputListener[] inputListeners)
        {
            _map = map;
            _inputListeners = inputListeners;
            _currentDirection = Direction.Up;
        }

        private void CallListeners()
        {
            foreach (var listener in _inputListeners)
            {
                listener.SetActiveDirection(_currentDirection);
            }
        }

        private async UniTask InputLoop(CancellationToken token)
        {
            CallListeners();
            bool isCancelled = false;
            while (!isCancelled)
            {
                Direction newDirection;
                if (Input.GetKeyDown(KeyCode.W))
                {
                    newDirection = Direction.Up;
                }else if (Input.GetKeyDown(KeyCode.A))
                {
                    newDirection = Direction.Left;
                }else if (Input.GetKeyDown(KeyCode.S))
                {
                    newDirection = Direction.Down;
                }else if (Input.GetKeyDown(KeyCode.D))
                {
                    newDirection = Direction.Right;
                }
                else
                {
                    isCancelled = await UniTask.NextFrame(token).SuppressCancellationThrow();
                    continue;
                }

                if (_map.Invert(newDirection) != _currentDirection)
                {
                    _currentDirection = newDirection;
                    CallListeners();
                }

                isCancelled = await UniTask.NextFrame(token).SuppressCancellationThrow();
            }
        }

        

       


        public void StartController()
        {
            if (_loopActive) return;
            _src = new CancellationTokenSource();
            InputLoop(_src.Token);
            _loopActive = true;
        }

        

        
        public void StopController()
        {
            if (!_loopActive) return;   
            _src?.Cancel();
            _src?.Dispose();
            _src = null;
            _loopActive = false;
        }

        


        

        public void StartSystem()
        {
            StartController();
        }

        public void StopSystem()
        {
            StopController();
        }

        public void ClearSystem()
        {
            _currentDirection = Direction.Up;
            CallListeners();
        }
    }
}
