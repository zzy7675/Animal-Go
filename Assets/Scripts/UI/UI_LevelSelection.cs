using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UI_LevelSelection : MonoBehaviour
{
    private UI_Mainmenu uiMainMenu;
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private UI_LevelButton buttonPrefab;
    [SerializeField] private Transform buttonParent;

    [SerializeField] private bool[] levelsUnlocked;

    private void Awake()
    {
        LoadLevelsInfo();
        CreateLevelButtons();
        uiMainMenu = GetComponentInParent<UI_Mainmenu>();
    }

    private void OnEnable()
    {
        uiMainMenu.UpdateLastSelected(firstSelected);

        GameObject firstLevelButton = buttonParent.GetChild(0).gameObject;

        if (firstLevelButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstLevelButton);
        } else
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
        
    }

    private void CreateLevelButtons()
    {
        int levelsAmount = SceneManager.sceneCountInBuildSettings - 1; // remove "The End" scene

        for (int i = 1; i < levelsAmount; ++i)
        {
            if (!IsLevelUnlocked(i))
                return;
            UI_LevelButton newButton = Instantiate(buttonPrefab, buttonParent);
            newButton.SetupButton(i);
        }
    }

    private bool IsLevelUnlocked(int levelIndex) => levelsUnlocked[levelIndex];

    private void LoadLevelsInfo()
    {
        int levelsAmount = SceneManager.sceneCountInBuildSettings - 1;

        levelsUnlocked = new bool[levelsAmount];

        for (int i = 1; i < levelsAmount; ++i)
        {
            bool levelUnlocked = PlayerPrefs.GetInt("Level" + i + "Unlocked", 0) == 1;

            if (levelUnlocked)
                levelsUnlocked[i] = true;
        }

        levelsUnlocked[1] = true;
    }
}
