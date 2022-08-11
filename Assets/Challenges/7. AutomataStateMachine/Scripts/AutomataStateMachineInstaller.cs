using Zenject;

namespace Challenges._3._AutomataStateMachine.Scripts
{
    public class SwitchToMagenta
    {
        
    }
    /// <summary>
    /// The factory is used to create a new general-use state machine
    /// </summary>
    public class AutomataStateMachineFactory : PlaceholderFactory<IAutomataStateMachine>
    {
    }
    public class AutomataStateMachineInstaller : MonoInstaller
    {
        

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<SwitchToMagenta>().OptionalSubscriber();
            Container.BindFactory<IAutomataStateMachine, AutomataStateMachineFactory>().To<AutomataStateMachine>();
        }
    }
}