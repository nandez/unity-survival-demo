using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    // Patron Singleton.
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// Contiene los recursos del jugador.
    /// </summary>
    public static Dictionary<string, GameResource> Resources = new Dictionary<string, GameResource>()
    {
        { "wood", new GameResource("Wood", 0, 5 ) },
        { "stone", new GameResource("Stone", 0, 5 ) },
        { "metal", new GameResource("Metal", 0, 5 ) },
    };

    private static NavMeshSurface navMeshSurface;


    public Collider mapWrapperCollider;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        navMeshSurface = GameObject.Find("Ground").GetComponent<NavMeshSurface>();
        UpdateNavMesh();
    }

    public void UpdateNavMesh()
    {
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
    }
}
