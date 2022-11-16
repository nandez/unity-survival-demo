using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RangedEnemyController : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    private PlayerController player;
    private NavMeshAgent navAgent;
    private float attackTimer = 0;

    void Start()
    {
        // Buscamos las referencias necesarias..
        navAgent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        // Miramos hacia el jugador...
        navAgent.transform.LookAt(new Vector3(player.transform.position.x, navAgent.transform.position.y, player.transform.position.z));

        var distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= attackRange)
        {
            // Si estamos en rango de ataque paramos el agente de navegación y atacamos.
            navAgent.isStopped = true;

            // Si el cooldown nos habilita, disparamos una roca.
            if (attackTimer <= 0)
            {
                // Creamos el proyectil a partir del prefab y le seteamos el tag correspondiente.
                var projectile = Instantiate(rockPrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                projectile.tag = Constants.TagNames.Projectile;

                // Obtenemos el componente ProjectileController y seteamos el owner del proyectil.
                var projectileCtrl = projectile.GetComponent<ProjectileController>();
                projectileCtrl.owner = gameObject;
                var dir = (player.transform.position - transform.position).normalized;
                projectileCtrl.Fire(dir);

                // Reiniciamos el cooldown.
                attackTimer = attackCooldown;

                // Emitimos el evento para que el sonido se reproduzca.
                //OnRockThrown?.Invoke();
            }
        }
        else
        {
            // Nos movemos hacia el jugador...
            navAgent.SetDestination(player.gameObject.transform.position);
            navAgent.isStopped = false;
        }

        attackTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si el enemigo colisiona con un proyectil, verificamos si es del jugador para tomar daño.
        if (other.gameObject.CompareTag(Constants.TagNames.Projectile))
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
