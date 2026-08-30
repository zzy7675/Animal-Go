using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int fruitsCollected;

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
}
