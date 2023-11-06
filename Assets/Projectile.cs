using UnityEngine;

public class Projectile : MonoBehaviour
{
    Health _target;
    float _damage = 10f;
    bool _towerProjectile;

    [SerializeField] float speed = 10f;

    Vector3 lastPosition = Vector3.zero;

    public void Setup(Health target, float damage, bool towerProjectile)
    {
        _target = target;
        _damage = damage;
        _towerProjectile = towerProjectile;
    }

    void Update()
    {
        if (_target != null)
        {
            lastPosition = _target.center.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, lastPosition, speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(lastPosition - transform.position);

        if (Vector3.Distance(transform.position, lastPosition) < 0.2f)
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
    }
}
