using UnityEngine;
using Zenject;

namespace Challenges._3._AutomataStateMachine.Scripts
{
    public class SwitchToMagentaButton : MonoBehaviour
    {
        [Inject]
        private SignalBus _signalBus;

        public void OnButtonClick()
        {
            _signalBus.Fire(new SwitchToMagenta());
        }
    }
}
