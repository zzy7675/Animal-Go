using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Difficulty : MonoBehaviour
{
    private UI_Mainmenu uiMainMenu;
    [SerializeField] private GameObject firstSelected;
    private DifficultyManager difficultyManager;

    private void Awake()
    {
        uiMainMenu = GetComponentInParent<UI_Mainmenu>();
    }

    private void OnEnable()
    {
        uiMainMenu.UpdateLastSelected(firstSelected);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    private void Start()
    {
        difficultyManager = DifficultyManager.instance;
    }

    public void SetEasyMode() => difficultyManager.SetDifficulty(DifficultyType.Easy);
    public void SetNormalMode() => difficultyManager.SetDifficulty(DifficultyType.Normal);
    public void SetHardMode() => difficultyManager.SetDifficulty(DifficultyType.Hard);
}
