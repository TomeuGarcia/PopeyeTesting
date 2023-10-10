using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
   IState ProcessTransitions();

   void Enter();

   void Exit();

}
