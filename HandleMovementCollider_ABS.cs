using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleMovementCollider_ABS : StateMachineBehaviour
{
    StateManager states; 

    public int index; 

    //This method is called when a transition starts
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (states == null)
            states = animator.transform.GetComponentInParent<StateManager>();
        states.CloseMovementCollider(index);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (states == null)
            states = animator.transform.GetComponentInParent<StateManager>();
        states.OpenMovementCollider(index);
    }
}
