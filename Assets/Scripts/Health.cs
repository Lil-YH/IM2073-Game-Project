using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    ScoreManager scoreManager;
    private float scoreAddAmount = 10;
    public bool isDead = false;

    KillCounter killCounter;

    public float maxHealth;
    [HideInInspector]
    public float currentHealth;
    RagDoll ragdoll;
    UIHealthBar healthBar;
    AiAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScoreManager>();

        killCounter = GameObject.FindGameObjectWithTag("GameController").GetComponent<KillCounter>();

        agent = GetComponent<AiAgent>();
        ragdoll = GetComponent<RagDoll>();
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<UIHealthBar>();

        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach(var rigidBody in rigidBodies)
        {
            HitBox hitBox = rigidBody.gameObject.AddComponent<HitBox>();
            hitBox.health = this;
        }
    }

    public void TakeDamage(float amount, Vector3 direction)
    {
        currentHealth -= amount;
        healthBar.SetHealthBarPercentage(currentHealth / maxHealth);
        if(currentHealth == (maxHealth * 0.4))
        {
            agent.audioSource.PlayOneShot(agent.damagedSound);
        }
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            Die();
        }
    }

    public void Die()
    {
        //ragdoll.ActivateRagdoll();
        //healthBar.gameObject.SetActive(false);
        AiDeathState deathState = agent.stateMachine.GetState(AiStateId.Death) as AiDeathState;
        if (agent.stateMachine.currentState != AiStateId.Death)
        {
            agent.stateMachine.ChangeState(AiStateId.Death);
            killCounter.AddCount();
            scoreManager.AddScore(scoreAddAmount);
        }
    }

}
