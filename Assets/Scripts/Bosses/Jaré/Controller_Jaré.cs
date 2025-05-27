using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


public enum BossState
{
    Idle,
    Rabada,
    Bote,
    Moidida,
    Enraged,
    Tired,
    Death
}

public class Controller_Jaré : MonoBehaviour
{
    public BossState currentState = BossState.Idle;

    public float health = 100f;
    public float cdBetweenAttacks = 3f;
    public float enragedCDModifier = 0.5f;
    public float tiredCDModifier = 1.5f;
    public float idleTime = 2f;

    public bool isEnraged = false;
    public bool isTired = false;
    public bool isIdle = false;

    private float attackTimer;
    public Animator animator;


    void Start()
    {
        StartCoroutine(BossLoop());
        animator = GetComponent<Animator>();
    }

    IEnumerator BossLoop()
    {
        while (currentState != BossState.Death)
        {
            if(health <= 50 && !isEnraged)
             EnterEnraged();
            
            if(health <= 20 && !isTired)
                EnterTired();

            if(!isIdle)
            {
                BossState nextAttack = ChooseNextAttack();
                ChangeState(nextAttack);

                yield return ExecuteAttack(nextAttack);

                ChangeState(BossState.Idle);
                isIdle = true;
                yield return new WaitForSeconds(idleTime);
                isIdle = false;
            }

            yield return null;
        }
    }

    BossState ChooseNextAttack()
    {
        int randomAttack = Random.Range(0, 3);
        switch (randomAttack)
        {
            case 0:
                return BossState.Rabada;
            case 1:
                return BossState.Bote;
            case 2:
                return BossState.Moidida;
            default:
                return BossState.Idle;
        }
    }

    IEnumerator ExecuteAttack(BossState attack)
    {
        float delay = cdBetweenAttacks;
        if(isEnraged)
            delay *= enragedCDModifier;
        if(isTired)
            delay *= tiredCDModifier;
        
        switch (attack)
        {
            case BossState.Rabada:
                // Execute Rabada attack
                animator.SetTrigger("Rabada");
                Debug.Log("Executing Rabada attack");
                break;
            case BossState.Bote:
                // Execute Bote attack
                animator.SetTrigger("bote");
                Debug.Log("Executing Bote attack");
                break;
            case BossState.Moidida:
                // Execute Moidida attack
                animator.SetTrigger("moidida");
                Debug.Log("Executing Moidida attack");
                break;
        }
        yield return new WaitForSeconds(delay);
        animator.SetTrigger("idle");
    }

    void EnterEnraged()
    {
        isEnraged = true;
        Debug.Log("Jaré is enraged!");
    }
    void EnterTired()
    {
        isEnraged = false;
        isTired = true;
        Debug.Log("Jaré is tired!");
    }
    void ChangeState(BossState newState)
    {
        currentState = newState;
        Debug.Log("Jaré changed state to: " + newState);
    }
    public void TakeDamage(float damage)
    {
        if(currentState == BossState.Idle)
        {
            health -= damage;
            Debug.Log($"Jaré levou {damage} de dano! Vida atual: {health}");
            if (health <= 0)
            {
                currentState = BossState.Death;
                Debug.Log("Jaré morreu!");
                Destroy(gameObject);
            }
        }
        
    }

}
