using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    [Header("Navigation Settings")]
    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private float clickableDistance = 100f;
    [SerializeField] private float carryingSpeedReduction = 0.5f;

    [Header("Actionable Items Settings")]
    [SerializeField] private LayerMask actionableLayer;
    [SerializeField] private float actionableDistance = 0.5f;
    [SerializeField] private Transform holdPoint;

    [Header("Projectile Settings")]
    [SerializeField] private int replenishSpearAmount = 3;
    [SerializeField] private int replenishRockAmount = 10;
    [SerializeField] private int currentSpears = 0;
    [SerializeField] private int currentRocks = 0;

    [Header("Events")]
    public UnityEvent<string> OnMouseOverActionableItem;
    public UnityEvent<int> OnPlayerDeath;
    public UnityEvent<int, int> OnAmmoReplenished;

    // Referencias..
    private LevelManager levelMgr;
    private NavMeshAgent navAgent;

    private GameObject currentItem;
    private float initialSpeed;
    private int playerLives = 3;


    private void Start()
    {
        // Buscamos las referencias necesarias..
        levelMgr = FindObjectOfType<LevelManager>();
        navAgent = GetComponent<NavMeshAgent>();

        // Guardamos la velocidad inicial del jugador..
        initialSpeed = navAgent.speed;

        // Invocamos el evento para notificar la cantidad inicial de munición
        OnAmmoReplenished?.Invoke(currentSpears, currentRocks);
    }

    private void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Cuando el jugador hace click, verificamos si el rayo colisiona con la capa
        // de navegación para setear el como destino en el navigation agent, la posición clickeada.
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit groundHitInfo;
            if (Physics.Raycast(ray, out groundHitInfo, clickableDistance, walkableLayer))
                navAgent.SetDestination(groundHitInfo.point);
        }

        // Verificamos si el mouse se encuentra posicionado sobre algun item accionable
        RaycastHit actionableHitInfo;
        if (Physics.Raycast(ray, out actionableHitInfo, clickableDistance, actionableLayer))
        {
            Debug.Log(actionableHitInfo.collider.gameObject.tag);
            // Obtenemos el item accionable y disparamos el evento con el tag del objeto, para notificar
            // a los listeners del evento "OnMouseOverActionableItem" (ej: MouseCursorController)
            var target = actionableHitInfo.collider.gameObject;
            OnMouseOverActionableItem?.Invoke(target.tag);

            // Verificamos la distancia al objeto para disparar la acción cuando el jugador presiona el mouse.
            var distanceToItem = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToItem <= actionableDistance && Input.GetMouseButtonDown(0))
                ProcessItemActivation(target.tag);
        }
        else
        {
            // En este caso, disparamos el evento con null como parámetro para notificar a los listeners
            // del evento, el mouse no está posicionado sobre nada accionable...
            OnMouseOverActionableItem?.Invoke(null);
        }
    }

    private void ProcessItemActivation(string itemTag)
    {

        if (itemTag.Equals(Constants.TagNames.Workbench) && currentItem != null)
        {
            // Como en este caso el jugador está activando el banco de trabajo y está cargando un
            // item, llamamos al método UpdateGameResource del LevelManager para que actualice la UI
            // y construya la parte de la casa correspondiente.
            levelMgr.UpdateGameResource(currentItem.tag);

            // Limpiamos la variable para que se pueda volver a recolectar un recurso.
            Destroy(currentItem);

            // Restauramos la velocidad inicial del jugador..
            navAgent.speed = initialSpeed;
        }
        else
        {
            // En este caso, intentamos obtener el recurso que corresponde al tag del item que estamos activando.
            var resource = levelMgr.GetGameResourceByTag(itemTag);

            // Verificamos que efectivamente el item a activar matchea con GameResource y
            // si aún no hemos superado el máximo permitido. En tal caso, también verificamos
            // que el jugador no esté cargando un item ya.
            if (resource?.CanAdd() != null && currentItem == null)
            {
                // Creamos el objeto a partir del prefab proporcionado y le seteamos el tag correspondiente.
                currentItem = Instantiate(resource.ItemPrefab, holdPoint.position, Quaternion.identity);
                currentItem.transform.SetParent(holdPoint);
                currentItem.tag = itemTag;

                // Reducimos la velocidad del jugador mientras lleva el item..
                navAgent.speed = initialSpeed * carryingSpeedReduction;

                // Si el item es una roca o madera, recargamos los proyectiles
                // y emitimos el evento con los valores actuales.
                if (currentItem.CompareTag(Constants.TagNames.Wood))
                {
                    currentSpears += replenishSpearAmount;
                    OnAmmoReplenished?.Invoke(currentSpears, currentRocks);
                }
                else if (currentItem.CompareTag(Constants.TagNames.Stone))
                {
                    currentRocks += replenishRockAmount;
                    OnAmmoReplenished?.Invoke(currentSpears, currentRocks);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.gameObject.tag);
        if (col.gameObject.CompareTag(Constants.TagNames.Enemy))
        {
            // Cuando un enemigo colisiona con el jugador, este pierde una vida.
            Physics.IgnoreCollision(col, GetComponent<Collider>());
            PlayerDeath();
        }
        else if (col.gameObject.CompareTag(Constants.TagNames.Rock))
        {
            // Cuando el jugador colisiona con una piedra (único tipo de arma que utilizan
            // los enemigos), entonces verificamos el ownership del proyectil para asegurarnos
            // que fue arrojado por un enemigo.
            var projectileCtrl = col.gameObject.GetComponent<ProjectileController>();
            if (projectileCtrl?.owner.CompareTag(Constants.TagNames.Enemy) == true)
            {
                Destroy(col.gameObject);
                PlayerDeath();
            }
        }
    }

    private void PlayerDeath()
    {
        if (playerLives > 0)
            playerLives--;

        Debug.Log($"Player Death! Lives: {playerLives}");
        OnPlayerDeath?.Invoke(playerLives);
    }
}
