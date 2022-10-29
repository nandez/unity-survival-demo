using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private float speed = 60f;

    private Camera cam;
    private Vector3 cameraForward;

    // TODO: idealmente la dirección debería ser un enumerado para unificar los valores;
    // de momento utilizamos un entero para simplificar, donde asumimos los siguientes valores:
    // 1=arriba, 2=abajo, 3=derecha, 4=izquierda, 0=sin movimiento
    private int moveDirection;
    private Coroutine setDirectionCoroutine;

    void Awake()
    {
        cam = GetComponent<Camera>();

        // Como la camara tiene rotación y se encuentra mirando hacia el terreno, tenemos que
        // calcular cual sería el vector para su "forward" dado que si utilizamos el local
        // eventualmente la camara atravesaría el terreno.
        cameraForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
    }

    void Update()
    {
        if (moveDirection == 1)
            transform.Translate(cameraForward * speed * Time.deltaTime, Space.World);
        else if (moveDirection == 2)
            transform.Translate(-cameraForward * speed * Time.deltaTime, Space.World);
        else if (moveDirection == 3)
            transform.Translate(transform.right * speed * Time.deltaTime);
        else if (moveDirection == 4)
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
    }

    private IEnumerator SetDirection(int direction)
    {
        // Usamos un pequeño delay antes de setear la dirección de movimiento, para evitar
        // que el movimiento sea instantáneo.
        yield return new WaitForSeconds(0.25f);
        moveDirection = direction;
    }
}
