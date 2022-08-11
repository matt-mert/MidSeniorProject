using GGPlugins.GGStateMachine.Scripts.Abstract;

namespace Challenges._3._AutomataStateMachine.Scripts
{
    #region EDITABLE

    //REMOVE THIS
    public struct ExampleDefinition
    {
            
    }

    #endregion
   
    public interface IAutomataStateMachine : IGGStateMachine
    {
        
        #region EDITABLE - Define your functions here

        /* This is an example function
         * Replace the ExampleDefinition struct with ANYTHING you want (You can add more parameters as well)
         * You need to add support for identified states (Explained in GGStateMachine)
         * You don't need to add support for parameters when defining a transition (The transition itself can decide the parameters)
         */
        void DefineTransition<TFromState, TToState>(ExampleDefinition example);
        
        //This transition can happen from any state to TToState 
        void DefineTransition<TToState>(ExampleDefinition example);

        #endregion
    }
}
