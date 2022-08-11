using System;
using Cysharp.Threading.Tasks;
using GGPlugins.GGStateMachine.Scripts.Abstract;
using GGPlugins.GGStateMachine.Scripts.Data;
using GGPlugins.GGStateMachine.Scripts.Installers;

namespace Challenges._3._AutomataStateMachine.Scripts
{
    public class AutomataStateMachine : IAutomataStateMachine
    {
        private IGGStateMachine _ggStateMachine;

        public AutomataStateMachine(GGStateMachineFactory factory)
        {
            _ggStateMachine = factory.Create();
        }

        public void DefineTransition<TFromState, TToState>(ExampleDefinition example)
        {
            //throw new NotImplementedException();
        }

        public void DefineTransition<TToState>(ExampleDefinition example)
        {
            //throw new NotImplementedException();
        }


        #region IGGStateMachine Implementations

        public void Dispose()
        {
            _ggStateMachine.Dispose();
        }

        public IGGStateMachine RegisterUniqueState(IGGStateBase state, string identifier = null)
        {
            return _ggStateMachine.RegisterUniqueState(state, identifier);
        }

        public void SetSettings(StateMachineSettings settings)
        {
            _ggStateMachine.SetSettings(settings);
        }

        public void StartStateMachine(string entryStateIdentifier, params object[] parameters)
        {
            _ggStateMachine.StartStateMachine(entryStateIdentifier, parameters);
        }

        public void StartStateMachine(Type type)
        {
            _ggStateMachine.StartStateMachine(type);
        }

        public void StartStateMachine<TParam1>(Type type, TParam1 param)
        {
            _ggStateMachine.StartStateMachine(type, param);
        }

        public void StartStateMachine<TParam1, TParam2>(Type type, TParam1 param1, TParam2 param2)
        {
            _ggStateMachine.StartStateMachine(type, param1, param2);
        }

        public void StartStateMachine<T>() where T : IGGState
        {
            _ggStateMachine.StartStateMachine<T>();
        }

        public void StartStateMachine<T, TParam1>(TParam1 param) where T : IGGState<TParam1>
        {
            _ggStateMachine.StartStateMachine<T, TParam1>(param);
        }

        public void StartStateMachine<T, TParam1, TParam2>(TParam1 param1, TParam2 param2)
            where T : IGGState<TParam1, TParam2>
        {
            _ggStateMachine.StartStateMachine<T, TParam1, TParam2>(param1, param2);
        }

        public void EnqueueState(string identifier, params object[] parameters)
        {
            _ggStateMachine.EnqueueState(identifier, parameters);
        }

        public void EnqueueState(Type type)
        {
            _ggStateMachine.EnqueueState(type);
        }

        public void EnqueueState<TParam1>(Type type, TParam1 param)
        {
            _ggStateMachine.EnqueueState(type, param);
        }

        public void EnqueueState<TParam1, TParam2>(Type type, TParam1 param1, TParam2 param2)
        {
            _ggStateMachine.EnqueueState(type, param1, param2);
        }

        public void EnqueueState<T>() where T : IGGState
        {
            _ggStateMachine.EnqueueState<T>();
        }

        public void EnqueueState<T, TParam1>(TParam1 param) where T : IGGState<TParam1>
        {
            _ggStateMachine.EnqueueState<T, TParam1>(param);
        }

        public void EnqueueState<T, TParam1, TParam2>(TParam1 param1, TParam2 param2) where T : IGGState<TParam1, TParam2>
        {
            _ggStateMachine.EnqueueState<T, TParam1, TParam2>(param1, param2);
        }

        public void RequestExit()
        {
            _ggStateMachine.RequestExit();
        }

        public UniTask WaitUntilMachineExit()
        {
            return _ggStateMachine.WaitUntilMachineExit();
        }

        public void SwitchToState(string identifier, params object[] parameters)
        {
            _ggStateMachine.SwitchToState(identifier, parameters);
        }

        public void SwitchToState(Type type)
        {
            _ggStateMachine.SwitchToState(type);
        }

        public void SwitchToState<TParam1>(Type type, TParam1 param)
        {
            _ggStateMachine.SwitchToState(type, param);
        }

        public void SwitchToState<TParam1, TParam2>(Type type, TParam1 param1, TParam2 param2)
        {
            _ggStateMachine.SwitchToState(type, param1, param2);
        }

        public void SwitchToState<T>() where T : IGGState
        {
            _ggStateMachine.SwitchToState<T>();
        }

        public void SwitchToState<T, TParam1>(TParam1 param) where T : IGGState<TParam1>
        {
            _ggStateMachine.SwitchToState<T, TParam1>(param);
        }

        public void SwitchToState<T, TParam1, TParam2>(TParam1 param1, TParam2 param2) where T : IGGState<TParam1, TParam2>
        {
            _ggStateMachine.SwitchToState<T, TParam1, TParam2>(param1, param2);
        }

        public void EnqueuePreviousState()
        {
            _ggStateMachine.EnqueuePreviousState();
        }

        public void SwitchToPreviousState()
        {
            _ggStateMachine.SwitchToPreviousState();
        }

        public void ClearQueue()
        {
            _ggStateMachine.ClearQueue();
        }

        public StateInfo GetCurrentState()
        {
            return _ggStateMachine.GetCurrentState();
        }

        public bool CheckCurrentState(string identifier)
        {
            return _ggStateMachine.CheckCurrentState(identifier);
        }

        public bool CheckCurrentState(Type type)
        {
            return _ggStateMachine.CheckCurrentState(type);
        }

        public bool CheckCurrentState<T>() where T : IGGState
        {
            return _ggStateMachine.CheckCurrentState<T>();
        }

        public IStateMachineEventHandle RequestEventHandle()
        {
            return _ggStateMachine.RequestEventHandle();
        }

        #endregion
    }
}