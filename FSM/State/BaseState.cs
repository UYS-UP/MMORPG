using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class BaseState
{
   protected StateID id;
   public StateID ID => id;

   protected Dictionary<Transition, StateID> stateTransitions = new Dictionary<Transition, StateID>();
   protected StateMachine stateMachine;

   protected BaseState(StateMachine stateMachine)
   {
      this.stateMachine = stateMachine;
   }

   public void AddTransition(Transition transition, StateID stateID)
   {
      if(transition == Transition.NullTransition) return;
      if(stateID == StateID.NullState) return;
      stateTransitions.TryAdd(transition, stateID);
   }

   public void RemoveTransition(Transition transition)
   {
      if(transition == Transition.NullTransition) return;
      if(!stateTransitions.ContainsKey(transition)) return;
      stateTransitions.Remove(transition);
   }

   public StateID GetNextState(Transition transition)
   {
      return stateTransitions.GetValueOrDefault(transition, StateID.NullState);
   }

   public abstract void OnEnter();
   public abstract void OnExit();
   public abstract void OnUpdate(float deltaTime);
   public abstract void OnFixedUpdate(float fixedDeltaTime);

}
