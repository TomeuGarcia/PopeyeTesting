using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class StateMachine
{
  public StateMachine(IState startingState) => ChangeState(startingState);

  public IState CurrentState { get; private set; }

  public void ChangeState(IState state)
  {
    //exit the current state
    CurrentState?.Exit();
    
    //Set the new state
    CurrentState = state;
    
    //Enter the new state
    CurrentState?.Enter();
  }

  public void Tick()
  {
    //Get the next state if the conditions are met to make a transition
    IState nextState = CurrentState.ProcessTransitions();
    
    //Make sure that there is a state to transition to this frame
    if (nextState!=null)
    {
      ChangeState(nextState);
    }
  }


}
