using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    [Header("Navigation Settings")]
    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private float clickableDistance = 100f;

    [Header("Game Resource Settings")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private GameObject woodItemPrefab;
    [SerializeField] private GameObject stoneItemPrefab;
    [SerializeField] private GameObject metalItemPrefab;

    [Header("Actionable Items Settings")]
    [SerializeField] private LayerMask actionableLayer;
    [SerializeField] private float actionableDistance = 0.5f;
    [SerializeField] private Texture2D axeCursorSprite;
    [SerializeField] private Texture2D pickAxeCursorSprite;
    [SerializeField] private Texture2D attackCursorSprite;
    [SerializeField] private Texture2D normalCursorSprite;

    public GameObject CurrentItem { get; private set; }

    private NavMeshAgent navAgent;
    private float initialSpeed;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        initialSpeed = navAgent.speed;
    }

    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Al hacer click, procesamos el rayo para ver si el jugador puede navegar a la posicion clickeada..
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit groundHitInfo;
            if (Physics.Raycast(ray, out groundHitInfo, clickableDistance, walkableLayer))
            {
                navAgent.SetDestination(groundHitInfo.point);
            }
        }

        // Verificamos si el mouse se encuentra posicionado sobre algun item accionable
        RaycastHit actionableHitInfo;
        if (Physics.Raycast(ray, out actionableHitInfo, clickableDistance, actionableLayer))
        {
            var target = actionableHitInfo.collider.gameObject;
            var distanceToItem = Vector3.Distance(transform.position, target.transform.position);

            // Cambiamos los cursores dependiendo del tipo de objeto..
            ChangeCursor(target.tag);

            // Verificamos la distancia al objeto para disparar la acción cuando el jugador presiona el mouse.
            if (distanceToItem <= actionableDistance && Input.GetMouseButtonDown(0))
            {
                ActivateItem(target.tag);
            }
        }
        else
        {
            ChangeCursor(null);
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        // Si colisionamos con la zona de construcción, verificamos si
        // estamos portando un item para procesarlo..
        if (coll.gameObject.CompareTag("BuildZone") && CurrentItem != null)
        {
            var homeZoneCtrl = coll.gameObject.GetComponent<HomeZoneController>();
            if (homeZoneCtrl != null)
            {
                // Llamamos al DeliverResource del componente HomeZoneController
                // para que se construya la parte de la casa correspondiente..
                homeZoneCtrl.DeliverResource(CurrentItem);

                // Notificamos al GameManager para que actualice la cantidad de
                // recursos que hemos recolectado.
                if (CurrentItem.CompareTag("WoodItem"))
                    GameManager.Resources["wood"].Add(1);
                else if (CurrentItem.CompareTag("StoneItem"))
                    GameManager.Resources["stone"].Add(1);
                else if (CurrentItem.CompareTag("MetalItem"))
                    GameManager.Resources["metal"].Add(1);

                // Limpiamos la variable para que se pueda volver a recolectar un recurso.
                Destroy(CurrentItem);
                navAgent.speed = initialSpeed;
            }
        }
    }

    private void ChangeCursor(string tag)
    {
        switch (tag)
        {
            case "WoodSource":
                Cursor.SetCursor(axeCursorSprite, Vector2.zero, CursorMode.Auto);
                break;
            case "StoneSource":
            case "MetalSource":
                Cursor.SetCursor(pickAxeCursorSprite, Vector2.zero, CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                break;
        }
    }

    private void ActivateItem(string tag)
    {
        // Verificamos el tipo de objeto según su tag..
        if (tag.Equals("WoodSource"))
        {
            // Validamos si el jugador no esta llevando algun item y si es posible
            // agregar mas recursos de este tipo.
            if (CurrentItem == null && GameManager.Resources["wood"].CanAdd())
            {
                // Instanciamos el recurso para mostrar una representación visual del item
                // que esta llevando el jugador, setamos su parent en el punto de agarre y
                // seteamos su tag para procesar una vez que llegue a la zona de construcción.
                CurrentItem = Instantiate(woodItemPrefab, holdPoint.position, Quaternion.identity);
                CurrentItem.transform.SetParent(holdPoint);
                CurrentItem.tag = "WoodItem";
                navAgent.speed = initialSpeed * 0.5f;
            }
        }
        else if (tag.Equals("StoneSource"))
        {
            // Validamos si el jugador no esta llevando algun item y si es posible
            // agregar mas recursos de este tipo.
            if (CurrentItem == null && GameManager.Resources["stone"].CanAdd())
            {
                CurrentItem = Instantiate(stoneItemPrefab, holdPoint.position, Quaternion.identity);
                CurrentItem.transform.SetParent(holdPoint);
                CurrentItem.tag = "StoneItem";
                navAgent.speed = initialSpeed * 0.5f;
            }
        }
        else if (tag.Equals("MetalSource"))
        {
            // Validamos si el jugador no esta llevando algun item y si es posible
            // agregar mas recursos de este tipo.
            if (CurrentItem == null && GameManager.Resources["metal"].CanAdd())
            {
                CurrentItem = Instantiate(metalItemPrefab, holdPoint.position, Quaternion.identity);
                CurrentItem.transform.SetParent(holdPoint);
                CurrentItem.tag = "MetalItem";
                navAgent.speed = initialSpeed * 0.5f;
            }
        }
    }
}
