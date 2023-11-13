using UnityEngine;

public class Projectile : MonoBehaviour
{
    Target _target;
    float _damage = 10f;
    bool _towerProjectile;

    [SerializeField] float speed = 10f;
    [SerializeField] bool rotateTowardsTarget;
    [SerializeField] bool isSpinning;
    [SerializeField] Vector3 spinAxis = Vector3.up; // Default spin axis
    [SerializeField] float spinSpeed = 360f; // Degrees per second
    [SerializeField] bool useGravity;


    Vector3 targetLastPosition = Vector3.zero;
    Vector3 velocity;

    public void Setup(Target target, float damage, bool towerProjectile)
    {
        _target = target;
        _damage = damage;
        _towerProjectile = towerProjectile;

        if (useGravity)
        {
            targetLastPosition = _target.Center.position;
            CalculateTrajectory();
        }
    }

    void Update()
    {
        if (_target != null)
        {
            targetLastPosition = _target.Center.position;
            if (useGravity)
            {
                CalculateTrajectory();  // Recalculate if target moves
            }
        }

        if (useGravity)
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
            if (rotateTowardsTarget)
            {
                transform.LookAt(transform.position + velocity);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetLastPosition, speed * Time.deltaTime);
            if (rotateTowardsTarget)
            {
                transform.LookAt(targetLastPosition);
            }
        }

        if (isSpinning)
        {
            transform.Rotate(spinAxis, spinSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, targetLastPosition) < 0.2f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        if (_target != null)
        {
            var actualDamage = _target.TakeDamage(Mathf.RoundToInt(_damage));
            if (_towerProjectile)
            {
                ScoreManager.Instance.damageDone += actualDamage;
            }
        }
        Destroy(gameObject);
    }

    void CalculateTrajectory()
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetLastPosition);
        float yOffset = targetLastPosition.y - transform.position.y;

        Vector3 toTarget = targetLastPosition - transform.position;
        toTarget.y = 0;

        float time = distanceToTarget / speed;
        velocity = toTarget.normalized * speed;

        // Adjust for arc height
        velocity.y = yOffset / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        if (rotateTowardsTarget)
        {
            transform.LookAt(targetLastPosition);
        }
    }
}
