using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MeleeEnemyController : MonoBehaviour
{
    [SerializeField] private float health;

    private PlayerController player;

    private NavMeshAgent navAgent;

    void Start()
    {
        // Buscamos las referencias necesarias..
        navAgent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        // Nos movemos hacia el jugador...
        navAgent.SetDestination(player.gameObject.transform.position);
        navAgent.isStopped = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.TagNames.Player))
        {
            navAgent.SetDestination(transform.position);
            navAgent.isStopped = true;
        }
        else if (other.gameObject.CompareTag(Constants.TagNames.Projectile))
        {
            var projectile = other.gameObject.GetComponent<ProjectileController>();

            // Cuando el enemigo es golpeado por un proyectil del jugador procesamos el daño..
            if (projectile.owner == player.gameObject)
            {
                health -= projectile.GetDamage();

                if (health <= 0)
                    Destroy(gameObject);

                // Destruimos el proyectil.
                Destroy(other.gameObject);
            }
        }
    }
}
