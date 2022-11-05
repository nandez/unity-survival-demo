using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MinimapController : MonoBehaviour
{
    [SerializeField] private Transform player;

    private float initialShadowDistance;

    private void LateUpdate()
    {
        // Usamos la posición del jugador para establecer la nueva posición
        // de la cámara, el valor actual de "y" representa el zoom del minimap.
        var newCameraPosition = player.position;
        newCameraPosition.y = transform.position.y;
        transform.position = newCameraPosition;
    }

    private void OnPreRender()
    {
        // Guardamos la distancia de sombras y la reseteamos a 0
        // para evitar que se visulicen las sombras en el minimapa.
        initialShadowDistance = QualitySettings.shadowDistance;
        QualitySettings.shadowDistance = 0;
    }

    private void OnPostRender()
    {
        // En el post render, restauramos el valor inicial.
        QualitySettings.shadowDistance = initialShadowDistance;
    }
}
