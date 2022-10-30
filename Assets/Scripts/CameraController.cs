using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float marginSafeBuffer = 1f;


    [Header("Map Settings")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minZ;
    [SerializeField] private float maxZ;

    private Camera cam;
    private Vector3 camForward;
    private Vector3 camOffset;
    private Vector3 camHalfViewZone;


    // TODO: idealmente la dirección debería ser un enumerado para unificar los valores;
    // de momento utilizamos un entero para simplificar, donde asumimos los siguientes valores:
    // 1=arriba, 2=abajo, 3=derecha, 4=izquierda, 0=sin movimiento
    private int moveDirection;
    private Coroutine setDirectionCoroutine;


    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        // Como la camara tiene rotación y se encuentra mirando hacia el terreno, tenemos que
        // calcular cual sería el vector para su "forward" dado que si utilizamos el local
        // eventualmente la camara atravesaría el terreno.
        camForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

        // Calculamos el offset entre el la camara y el punto en el mundo hacia dodne está
        // apuntando la camara para limitar luego el movimiento y evitar que se siga desplazando
        // fuera de los límites del terreno.
        (var minWorldPoint, var maxWorldPoint) = Utilities.GetWorldSize();
        camOffset = transform.position - (maxWorldPoint + minWorldPoint) / 2f;
        camHalfViewZone = (maxWorldPoint - minWorldPoint) / 2f + Vector3.one * marginSafeBuffer;
    }

    void Update()
    {
        if (moveDirection == 1 && transform.position.z - camOffset.z + camHalfViewZone.z <= maxZ)
            transform.Translate(camForward * speed * Time.deltaTime, Space.World);

        else if (moveDirection == 2 && transform.position.z - camOffset.z - camHalfViewZone.z >= minZ)
            transform.Translate(-camForward * speed * Time.deltaTime, Space.World);

        else if (moveDirection == 3 && transform.position.x + camHalfViewZone.x <= maxX)
            transform.Translate(transform.right * speed * Time.deltaTime);

        else if (moveDirection == 4 && transform.position.x - camHalfViewZone.x >= minX)
            transform.Translate(-transform.right * speed * Time.deltaTime);
    }

    public void OnScreenBorderPointerEnter(int direction)
    {
        // Seteamos la direccion con una couroutina y la guardamos en la referencia
        // para poder cancelar a demanda.
        setDirectionCoroutine = StartCoroutine(SetDirection(direction));
        moveDirection = direction;
    }

    public void OnScreenBorderPointerExit()
    {
        // Al quitar el mouse del borde, cancelamos la corutina y reseteamos la dirección a 0.
        StopCoroutine(setDirectionCoroutine);
        moveDirection = 0;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private IEnumerator SetDirection(int direction)
    {
        // Usamos un pequeño delay antes de setear la dirección de movimiento, para evitar
        // que el movimiento sea instantáneo.
        yield return new WaitForSeconds(0.25f);
        moveDirection = direction;
    }
}
