using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;

    public GameObject owner;
    private Vector3 dir;


    void Start()
    {
        // Invocamos el método DestroyProjectile después de un tiempo predefinido.
        Invoke(nameof(DestroyProjectile), lifeTime);
    }

    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    public void Fire(Vector3 direction)
    {
        dir = direction;
    }

    public float GetDamage()
    {
        return damage;
    }
}
