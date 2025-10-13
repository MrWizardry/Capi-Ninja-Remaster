using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player_Damage_Ranged : MonoBehaviour
{
    public GameObject moveableRange;
    public Transform damagePointer;
    public float damageRange = 0.5f;
    public LayerMask enemy;
    public float maxRangeDistance = 5f;  
    private bool attacking;
    public int damage = 20; 

    private Vector2 screenPosition;
    private Vector2 worldPosition;

    private AnimationManager animManager;
    [SerializeField] private float timeSinceLastAtk;
    [SerializeField] private float timeBetweenAtk;

    void Start()
    {
        animManager = GetComponent<AnimationManager>();
    }
    void Update()
    {
        screenPosition = Input.mousePosition;
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        Vector2 direction = worldPosition - (Vector2)transform.position;
        
        if (direction.magnitude > maxRangeDistance)
        {
            direction = direction.normalized * maxRangeDistance;
        }

        moveableRange.transform.position = (Vector2)transform.position + direction;

        damagePointer.position = moveableRange.transform.position;

        timeSinceLastAtk += Time.deltaTime;
        attacking = Custom_Input.GetKeyDown("Attack");
        if (attacking && timeSinceLastAtk >= timeBetweenAtk)
        {
            animManager.PlayHighPriority("Attack");
        
            timeSinceLastAtk = 0;
        }
    }

    void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(damagePointer.position, damageRange, enemy);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyLife>().TakeDamage(damage);
            timeSinceLastAtk = timeBetweenAtk;
            StartCoroutine(AttackEffect());
        }
    }
    private IEnumerator AttackEffect()
    {
        animManager.StartDash();
        yield return new WaitForSeconds(0.1f);
        animManager.EndDash();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(damagePointer.position, damageRange);
    }
}
