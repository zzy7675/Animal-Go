using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Fruit Management")]
    [SerializeField] private bool needRandomFruit;
    [SerializeField] private int fruitsCollected;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void Score()
    {
        Debug.Log("You receive 5 points");
    }

    public void AddFruit() => fruitsCollected++;
    public bool NeedRandomFruit() => needRandomFruit;
}
