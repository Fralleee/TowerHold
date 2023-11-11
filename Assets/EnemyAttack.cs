using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Projectile projectilePrefab;
    public float baseDamage = 10f;
    public float attackRange = 2f;
    public float timeBetweenAttacks = 1f;
    float lastAttackTime = 0f;
    [HideInInspector] public Target target;
    ITargeter targeter;
    Animator animator;
    Enemy enemy;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        targeter = GetComponentInParent<ITargeter>();
    }

    void Update()
    {
        if (target == null)
        {
            target = targeter.GetTarget(attackRange);
        }
        else if (Time.time - lastAttackTime > timeBetweenAttacks)
        {
            Shoot();
            lastAttackTime = Time.time;
        }
    }

    void Shoot()
    {
        var projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        projectile.Setup(target, baseDamage, false);
        animator.SetTrigger("Attack");
    }
}
