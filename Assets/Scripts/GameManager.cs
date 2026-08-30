using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Fruit Management")]
    [SerializeField] private bool needRandomFruit;
    [SerializeField] private int fruitsCollected;

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay;
    public Player player;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void respawnPlayer()
    {
        player.InRespawn(true);
        StartCoroutine(respawnPlayerRoutine());
    }

    public void UpdateRespawnPoint(Transform checkpoint) => respawnPoint = checkpoint;
    private IEnumerator respawnPlayerRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        player = newPlayer.GetComponent<Player>();
    }

    public void Score()
    {
        Debug.Log("You receive 5 points");
    }

    public void AddFruit() => fruitsCollected++;
    public bool NeedRandomFruit() => needRandomFruit;
}
