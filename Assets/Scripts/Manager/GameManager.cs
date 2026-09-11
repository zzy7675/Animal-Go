using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private UI_InGame inGameUI;

    [Header("Level Management")]
    [SerializeField] private float levelTimer;
    [SerializeField] private int currentLevelIndex;
    private int nextLevelIndex;

    [Header("Fruits Management")]
    [SerializeField] private bool needRandomFruit;
    [SerializeField] private int fruitsCollected;
    [SerializeField] private int totalNumberOfFruits;
    public Transform fruitParent;

    [Header("Checkpoints")]
    [SerializeField] public bool canReactivate;

    [Header("Managers")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private SkinManager skinManager;
    [SerializeField] private DifficultyManager difficultyManager;
    [SerializeField] private ObjectCreator objectCreator;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        inGameUI = UI_InGame.instance;
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

        nextLevelIndex = currentLevelIndex + 1;
        GetFruitsInfo();
        CreateManagersIfNeeded();
    }

    private void Update()
    {
        levelTimer += Time.deltaTime;

        inGameUI.UpdateTimerUI(levelTimer);
    }

    private void CreateManagersIfNeeded()
    {
        if (AudioManager.instance == null)
            Instantiate(audioManager);

        if (PlayerManager.instance == null)
            Instantiate(playerManager);

        if (SkinManager.instance == null)
            Instantiate(skinManager);

        if (DifficultyManager.instance == null)
            Instantiate(difficultyManager);

        if (ObjectCreator.instance == null)
            Instantiate(objectCreator);
    }

    private void GetFruitsInfo()
    {
        Fruit[] allFruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
        totalNumberOfFruits = allFruits.Length;
        inGameUI.UpdateFruitUI(fruitsCollected, totalNumberOfFruits);

        PlayerPrefs.SetInt("Level" + currentLevelIndex + "TotalFruits", totalNumberOfFruits);


    }

    [ContextMenu("Parent All Fruits")]
    private void ParentAllTheFruits()
    {
        if (fruitParent == null)
            return;
        Fruit[] fruitList = FindObjectsByType<Fruit>(FindObjectsSortMode.None);

        foreach (Fruit fruit in fruitList)
        {
            fruit.transform.parent = fruitParent;
        }

    }

    public void Score()
    {
        Debug.Log("You receive 5 points");
    }

    public void AddFruit()
    {
        fruitsCollected++;
        inGameUI.UpdateFruitUI(fruitsCollected, totalNumberOfFruits);
    }

    public void RemoveFruit()
    {
        fruitsCollected--;
        inGameUI.UpdateFruitUI(fruitsCollected, totalNumberOfFruits);
    }

    public int FruitsCollected()
    {
        return fruitsCollected;
    }

    public bool NeedRandomFruit() => needRandomFruit;

    public void LevelFinished()
    {
        SaveLevelProgression();
        SaveBestTime();
        SaveFruitsInfo();


        LoadNextScene();
    }
    private void SaveFruitsInfo()
    {
        int fruitsCollectedBefore = PlayerPrefs.GetInt("Level" + currentLevelIndex + "FruitsCollected");
        if (fruitsCollectedBefore < fruitsCollected)
            PlayerPrefs.SetInt("Level" + currentLevelIndex + "FruitsCollected", fruitsCollected);

        int totalFruitsInBank = PlayerPrefs.GetInt("TotalFruitsAmount");

        PlayerPrefs.SetInt("TotalFruitsAmount", totalFruitsInBank + fruitsCollected);
    }
    private void SaveBestTime()
    {
        float lastTime = PlayerPrefs.GetFloat("Level" + currentLevelIndex + "BestTime", 99);
        if (levelTimer < lastTime)
            PlayerPrefs.SetFloat("Level" + currentLevelIndex + "BestTime", levelTimer);
    }

    private void SaveLevelProgression()
    {
        PlayerPrefs.SetInt("Level" + nextLevelIndex + "Unlocked", 1);
        if (!NoMoreLevels())
        {
            PlayerPrefs.SetInt("ContinueLevelNumber", nextLevelIndex);

            SkinManager skinManager = SkinManager.instance;
            if (skinManager != null)
            {
                PlayerPrefs.SetInt("LastUsedSkin", SkinManager.instance.GetChosenSkinIndex());
            }
            
        }
    }

    private void LoadNextScene()
    {
        UI_FadeEffect fadeEffect = inGameUI.fadeEffect;

        if (NoMoreLevels())
        {
            inGameUI.fadeEffect.ScreenFade(1, 1.5f, LoadTheEndScene);
        }
        else
        {
            inGameUI.fadeEffect.ScreenFade(1, 1.5f, LoadNextLevel);
        }
    }

    public void RestartLevel()
    {
        UI_InGame.instance.fadeEffect.ScreenFade(1, .75f, LoadCurrentScene);
    }

    private void LoadCurrentScene() => SceneManager.LoadScene("Level_" + currentLevelIndex);

    private void LoadTheEndScene() => SceneManager.LoadScene("TheEnd");

    private void LoadNextLevel()
    {
        SceneManager.LoadScene("Level_" + nextLevelIndex);
    }

    private bool NoMoreLevels()
    {
        int lastLevelIndex = SceneManager.sceneCountInBuildSettings - 2; // exclude "main menu" and "The End" scene
        bool noMoreLevels = (currentLevelIndex == lastLevelIndex);
        return noMoreLevels;
    }
}
