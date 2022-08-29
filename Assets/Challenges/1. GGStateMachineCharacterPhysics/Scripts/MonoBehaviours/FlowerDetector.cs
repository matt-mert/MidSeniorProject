using Challenges._1._GGStateMachineCharacterPhysics.Scripts.States;
using GGPlugins.GGStateMachine.Scripts.Abstract;
using UnityEngine;

namespace Challenges._1._GGStateMachineCharacterPhysics.Scripts.MonoBehaviours
{
    public class FlowerDetector : MonoBehaviour, IStateMachineUser
    {
        private IGGStateMachine _stateMachine;
        private CharacterController _controller;
        [SerializeField]
        private float flowerCollectRange;
        [SerializeField]
        private Vector3 centerOffset;

        public void SetStateMachine(IGGStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            var closestFlower = Flower.GetClosestFlower(transform.position+centerOffset, out var distance);
            if (distance <= flowerCollectRange)
            {
                _controller.CancelCurrentState();
                _stateMachine.SwitchToState<FlowerEarnedState,float>(closestFlower.strength);
                Debug.Log("State has been changed to FlowerEarnedState");
                closestFlower.Earn();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position+centerOffset,flowerCollectRange);
        }
    }
}
