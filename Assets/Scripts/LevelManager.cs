using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private NavMeshSurface navigationSurface;

    [Header("Game Resources")]
    [SerializeField] private GameResource wood = new GameResource();
    [SerializeField] private GameResource stone = new GameResource();
    [SerializeField] private GameResource ore = new GameResource();

    [Header("Buildable Zone Objects")]
    [SerializeField] private List<GameObject> homes;
    [SerializeField] private List<GameObject> fences;
    [SerializeField] private List<GameObject> pathways;

    [Header("Enemy Settings")]
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private List<GameObject> enemySpawnAreas;
    [SerializeField] private float enemySpanDelay = 8f;
    [SerializeField] private int maxEnemiesOnLevel = 10;

    [Header("Events")]
    public UnityEvent<GameResource> OnGameResourcesUpdated;
    public UnityEvent<int, int> OnAmmoUpdated;

    private GameState gameState = GameState.PAUSED;
    private List<GameObject> enemies = new List<GameObject>();

    private void Start()
    {
        UpdateNavMesh();

        // Emitimos los eventos iniciales para actualizar los recursos.
        OnGameResourcesUpdated?.Invoke(wood);
        OnGameResourcesUpdated?.Invoke(stone);
        OnGameResourcesUpdated?.Invoke(ore);

        gameState = GameState.RUNNING;
        StartCoroutine(nameof(SpawnEnemies));
    }

    private void UpdateNavMesh()
    {
        navigationSurface.UpdateNavMesh(navigationSurface.navMeshData);
    }

    public void UpdateGameResource(string resourceTag)
    {
        if (string.IsNullOrEmpty(resourceTag))
            return;

        GameObject buildPart = null;

        // Dependiendo del tag del recurso:
        // - Buscamos el primer elemento inactivo de la lista correspondiente
        // - Emitimos el evento OnGameResourcesUpdated para notificar a los listeners (ej: UIController)
        if (resourceTag.Equals(Constants.TagNames.Wood))
        {
            buildPart = homes.FirstOrDefault(t => !t.activeInHierarchy);
            OnGameResourcesUpdated?.Invoke(wood);


        }
        else if (resourceTag.Equals(Constants.TagNames.Stone))
        {
            buildPart = pathways.FirstOrDefault(t => !t.activeInHierarchy);
            OnGameResourcesUpdated?.Invoke(stone);
        }
        else if (resourceTag.Equals(Constants.TagNames.Ore))
        {
            buildPart = fences.FirstOrDefault(t => !t.activeInHierarchy);
            OnGameResourcesUpdated?.Invoke(ore);
        }

        if (buildPart != null)
        {
            // Activamos el game object y actualizamos la malla de navegación.
            buildPart?.SetActive(true);
            UpdateNavMesh();
        }
    }

    /// <summary>
    /// Retorna el recurso según el tag del item activable
    /// </summary>
    public GameResource GetGameResourceByTag(string activableItemTag)
    {
        switch (activableItemTag)
        {
            case Constants.TagNames.Wood:
                return wood;
            case Constants.TagNames.Ore:
                return ore;
            case Constants.TagNames.Stone:
                return stone;
            default:
                return null;
        }
    }

    public void OnTimerExpiredHandler()
    {
        Debug.Log("TIMER EXPIRED!");
    }

    public void OnPlayerDeathHandler(int remainingLives)
    {
        if (remainingLives > 0)
        {
            // TODO: eliminar los enemigos
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(enemySpanDelay);

            if (enemies.Count < maxEnemiesOnLevel)
            {
                // Obtenemos un área de spawn aleatoria
                var spawnArea = enemySpawnAreas[Random.Range(0, enemySpawnAreas.Count)].GetComponent<Collider>();

                // Obtenemos un enemigo aleatorio
                var enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

                // Generamos coordenadas aleatorias dentro del area de spawn
                var x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x);
                var z = Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z);

                // Instanciamos el enemigo en la posición del área de spawn
                var enemy = Instantiate(enemyPrefab, new Vector3(x, 0.26f, z), Quaternion.identity);
                enemies.Add(enemy);
            }
        }
    }
}
