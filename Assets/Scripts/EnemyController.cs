using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
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
    }
}
