using UnityEngine;

public enum FruitType
{
    Apple, Banana, Cherries, Kiwi, Melon, Orange, Pineapple, Strawberry
}

public class Fruit : MonoBehaviour
{
    [SerializeField] private FruitType fruitType;
    [SerializeField] private GameObject pickupVFX;
    private GameManager gameManager;
    private Animator anim;


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        gameManager = GameManager.instance;
        SetRandomFruitIfNeeded();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            gameManager.AddFruit();
            Destroy(gameObject);
            Instantiate(pickupVFX, transform.position, Quaternion.identity);
        }
    }

    private void SetRandomFruitIfNeeded()
    {
        if (!gameManager.NeedRandomFruit())
        {
            UpdateFruitNoRandom();
            return;
        }
        anim.SetFloat("fruitIndex", GetRandomFruitIndex());
    }

    private void UpdateFruitNoRandom()
    {
        anim.SetFloat("fruitIndex", (int)fruitType);
    }

    private int GetRandomFruitIndex()
    {
        return Random.Range(0, 8);
    }

}
