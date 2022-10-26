using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
