using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Mainmenu : MonoBehaviour
{
    private UI_FadeEffect fadeEffect;
    public string firstLevelName;

    [SerializeField] private GameObject[] uiElements;
    [SerializeField] private GameObject continueButton;

    [Header("Interactive Camera")]
    [SerializeField] private MenuCharacter menuCharacter;
    [SerializeField] private CinemachineCamera cinemachine;
    [SerializeField] private Transform mainmenuPoint;
    [SerializeField] private Transform skinSelectionPoint;

    private void Awake()
    {
        fadeEffect = GetComponentInChildren<UI_FadeEffect>();

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow, new RefreshRate() { numerator = 60, denominator = 1 });
    }


    public void SwitchUI(GameObject uiToEnable)
    {
        foreach (GameObject ui in uiElements)
        {
            ui.SetActive(false);
        }
        AudioManager.instance.PlaySFX(((int)SFXType.SFX_MenuSelect1));
        uiToEnable.SetActive(true);
    }

    private void Start()
    {
        if (HasLevelProgression())
            continueButton.SetActive(true);
        fadeEffect.ScreenFade(0, 1.5f);

    }

    public void ContinueGame()
    {
        int difficultyIndex = PlayerPrefs.GetInt("GameDifficulty", 1);
        int levelToLoad = PlayerPrefs.GetInt("ContinueLevelNumber", 0);
        int lastSavedSkin = PlayerPrefs.GetInt("LastUsedSkin");
        SkinManager.instance.SetSkinIndex(lastSavedSkin);
        DifficultyManager.instance.LoadDifficulty(difficultyIndex);
        SceneManager.LoadScene("Level_" + levelToLoad);
        AudioManager.instance.PlaySFX(((int)SFXType.SFX_MenuSelect1));
    }

    public void NewGame()
    {
        fadeEffect.ScreenFade(1, 1.5f, LoadLevelScene);
        AudioManager.instance.PlaySFX(((int)SFXType.SFX_MenuSelect1));
    }

    private void LoadLevelScene() => SceneManager.LoadScene(firstLevelName);

    private bool HasLevelProgression()
    {
        bool hasLevelProgression = PlayerPrefs.GetInt("ContinueLevelNumber", 0) > 0;
        return hasLevelProgression;
    }

    public void MoveCameraToMainMenu()
    {
        menuCharacter.MoveTo(mainmenuPoint);
        cinemachine.Follow = mainmenuPoint;
    }

    public void MoveCameraToSkinMenu()
    {
        menuCharacter.MoveTo(skinSelectionPoint);
        cinemachine.Follow = skinSelectionPoint;
    }
}
