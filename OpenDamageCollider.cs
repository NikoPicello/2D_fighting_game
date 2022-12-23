using System.Collections;
using UnityEngine;

public class OpenDamageCollider : StateMachineBehaviour
{

    StateManager states; 
    public HandleDamageColliders.DamageType damageType; 
    public HandleDamageColliders.DCtype dcType; 
    public float delay; 

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (states == null) {
            Debug.Log("OnStateEnter");
            states = animator.transform.GetComponentInParent<StateManager>();
        }
        states.handleDC.OpenCollider(dcType, delay, damageType);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (states == null)
            states = animator.transform.GetComponentInParent<StateManager>();
            
        states.handleDC.CloseColliders();
    }
}
