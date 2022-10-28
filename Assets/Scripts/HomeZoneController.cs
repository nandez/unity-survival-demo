using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HomeZoneController : MonoBehaviour
{
    [SerializeField] private List<GameObject> homeStages;
    [SerializeField] private List<GameObject> fences;
    [SerializeField] private List<GameObject> pathways;

    public void DeliverResource(GameObject resource)
    {
        if (resource == null)
            return;

        // Dependiendo del tipo de recurso, buscamos el primer elemento de
        // la lista que no esté activo para activarlo y actualizamos la malla de
        // navegación.
        if (resource.CompareTag("WoodItem"))
        {
            var nextHouseStage = homeStages.FirstOrDefault(t => !t.activeInHierarchy);
            nextHouseStage?.SetActive(true);
            GameManager.Instance.UpdateNavMesh();
        }
        else if (resource.CompareTag("StoneItem"))
        {
            var nextPath = pathways.FirstOrDefault(t => !t.activeInHierarchy);
            nextPath?.SetActive(true);
            GameManager.Instance.UpdateNavMesh();
        }
        else if (resource.CompareTag("MetalItem"))
        {
            var nextFence = fences.FirstOrDefault(t => !t.activeInHierarchy);
            nextFence?.SetActive(true);
            GameManager.Instance.UpdateNavMesh();
        }
    }
}
