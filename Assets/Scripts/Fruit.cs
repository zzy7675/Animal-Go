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
    protected Animator anim;
    protected SpriteRenderer sr;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        gameManager = GameManager.instance;
        SetRandomFruitIfNeeded();
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            gameManager.AddFruit();
            AudioManager.instance.PlaySFX(((int)SFXType.SFX_Pickup2), true);
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
