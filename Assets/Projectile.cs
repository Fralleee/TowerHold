using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector]
    public Health target;
    public float damage = 10f;

    [SerializeField] float speed = 10f;

    Vector3 lastPosition = Vector3.zero;

    void Update()
    {
        if (target != null)
        {
            lastPosition = target.center.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, lastPosition, speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(lastPosition - transform.position);

        if (Vector3.Distance(transform.position, lastPosition) < 0.2f)
        {
            if (target != null)
            {
                target.TakeDamage(Mathf.RoundToInt(damage));
            }
            Destroy(gameObject);
        }
    }
}
