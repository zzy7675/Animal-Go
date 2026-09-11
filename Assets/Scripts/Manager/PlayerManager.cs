using System;
using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static event Action OnPlayerRespawn;
    public static PlayerManager instance;

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay;
    public Player player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (respawnPoint == null)
        {
            respawnPoint = FindFirstObjectByType<Startpoint>().transform;
        }

        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }
    }

    public void UpdateRespawnPoint(Transform checkpoint) => respawnPoint = checkpoint;

    public void respawnPlayer()
    {
        DifficultyManager difficultyManager = DifficultyManager.instance;
        if (difficultyManager != null && difficultyManager.difficulty == DifficultyType.Hard)
            return;
        StartCoroutine(respawnPlayerRoutine());
    }

    private IEnumerator respawnPlayerRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        player = newPlayer.GetComponent<Player>();
        OnPlayerRespawn?.Invoke();
    }
}
