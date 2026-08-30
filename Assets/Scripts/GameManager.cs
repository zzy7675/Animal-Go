using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Fruits Management")]
    [SerializeField] private bool needRandomFruit;
    [SerializeField] private int fruitsCollected;
    [SerializeField] private int totalNumberOfFruits;

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay;
    public Player player;

    [Header("Checkpoints")]
    [SerializeField] public bool canReactivate;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        GetFruitsInfo();
    }

    private void GetFruitsInfo()
    {
        Fruit[] allFruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
        totalNumberOfFruits = allFruits.Length;
    }

    public void respawnPlayer() => StartCoroutine(respawnPlayerRoutine());

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
