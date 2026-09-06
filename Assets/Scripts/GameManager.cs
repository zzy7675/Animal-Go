using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Traps")]
    public GameObject arrowPrefab;


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

    public void CreateObject(GameObject prefab, Transform target, float delay)
    {
        StartCoroutine(CreateObjectRoutine(prefab, target, delay));
    }
    private IEnumerator CreateObjectRoutine(GameObject prefab, Transform target, float delay)
    {
        Vector3 newPosition = target.position;

        yield return new WaitForSeconds(delay);

        GameObject newObject = Instantiate(prefab, newPosition, Quaternion.identity);
    }

    private void LoadTheEndScene() => SceneManager.LoadScene("TheEnd");

    public void LevelFinished()
    {
        UI_InGame.instance.fadeEffect.ScreenFade(1, 1.5f, LoadTheEndScene);
    }
}
