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
    public UnityEvent<GaveOverReason> OnGameOver;

    private int buildingRemaining = 0;

    private void Start()
    {
        UpdateNavMesh();

        // Emitimos los eventos iniciales para actualizar los recursos.
        OnGameResourcesUpdated?.Invoke(wood);
        OnGameResourcesUpdated?.Invoke(stone);
        OnGameResourcesUpdated?.Invoke(ore);

        StartCoroutine(nameof(SpawnEnemies));

        // Calculamos cuantas construcciones nos quedan por activar..
        buildingRemaining = homes.Count + fences.Count + pathways.Count;
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

            if (buildPart != null)
            {
                // Para evitar que las partes de las casas se activen en simultáneo, a medida que vamos activando
                // las etapas, vamos eliminando las etapas anteriores. Por lo que buscamos el índice de la parte a
                // construir en la lista de partes de la casa y eliminamos las anteriores.
                var buildPartIndex = homes.IndexOf(buildPart);

                // Si el índice es mayor a 0, significa que la parte que vamos a construir no es la primera
                // por lo que podemos eliminar el item anterior.
                if (buildPartIndex > 0)
                {
                    // Quitamos la referencia del objeto de la lista y destruimos el game object.
                    var previousBuildPart = homes[buildPartIndex - 1];
                    homes.Remove(previousBuildPart);
                    Destroy(previousBuildPart);
                }
            }

            if (wood.Add())
                OnGameResourcesUpdated?.Invoke(wood);
        }
        else if (resourceTag.Equals(Constants.TagNames.Stone))
        {
            buildPart = pathways.FirstOrDefault(t => !t.activeInHierarchy);

            if (stone.Add())
                OnGameResourcesUpdated?.Invoke(stone);
        }
        else if (resourceTag.Equals(Constants.TagNames.Ore))
        {
            buildPart = fences.FirstOrDefault(t => !t.activeInHierarchy);

            if (ore.Add())
                OnGameResourcesUpdated?.Invoke(ore);
        }

        // En caso de haber construido alguna parte, actualizamos la malla de navegación.
        if (buildPart != null)
        {
            buildPart.SetActive(true);
            buildingRemaining--;

            UpdateNavMesh();
        }

        // Si activamos todas las partes de la construcción, el nivel se ha completado.
        if (buildingRemaining == 0)
            OnGameOver?.Invoke(GaveOverReason.LEVEL_COMPLETED);
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
        OnGameOver?.Invoke(GaveOverReason.OUT_OF_TIME);
    }

    public void OnPlayerDeathHandler(int remainingLives)
    {
        if (remainingLives > 0)
        {
            // TODO: eliminar los enemigos
        }
        else
        {
            OnGameOver?.Invoke(GaveOverReason.PLAYER_DIED);
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(enemySpanDelay);

            // Calculamos cuantos enemigos tenemos en el nivel.
            var enemyCount = FindObjectsOfType<RangedEnemyController>()?.Length ?? 0;
            enemyCount += FindObjectsOfType<MeleeEnemyController>()?.Length ?? 0;

            // Si no superamos el máximo permitido, instanciamos un nuevo enemigo.
            if (enemyCount < maxEnemiesOnLevel)
            {
                // Obtenemos un área de spawn aleatoria
                var spawnArea = enemySpawnAreas[Random.Range(0, enemySpawnAreas.Count)].GetComponent<Collider>();

                // Obtenemos un enemigo aleatorio
                var enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

                // Generamos coordenadas aleatorias dentro del area de spawn
                var x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x);
                var z = Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z);

                // Instanciamos el enemigo en la posición del área de spawn
                Instantiate(enemyPrefab, new Vector3(x, 0.26f, z), Quaternion.identity);
            }
        }
    }
}
