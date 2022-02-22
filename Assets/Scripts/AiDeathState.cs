using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiDeathState : AiState
{

    
    public AiStateId GetId()
    {
        return AiStateId.Death;
    }
    public void Enter(AiAgent agent)
    {
        agent.ragdoll.ActivateRagdoll();
        agent.ui.gameObject.SetActive(false);
        agent.navMeshAgent.isStopped = true;
        agent.audioSource.PlayOneShot(agent.deathSound);
        foreach (Transform child in agent.enemyObject.GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = LayerMask.NameToLayer("Environment");  // add any layer you want. 
        }
    }
    public void Update(AiAgent agent)
    {

    }
    public void Exit(AiAgent agent)
    {

    }
    
}
