using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LevelSelection : MonoBehaviour
{
    [SerializeField] private UI_LevelButton buttonPrefab;
    [SerializeField] private Transform buttonParent;

    private void Start()
    {
        CreateLevelButtons();
    }

    private void CreateLevelButtons()
    {
        int levelsAmount = SceneManager.sceneCountInBuildSettings - 1; // remove "The End" scene

        for (int i = 1; i < levelsAmount; ++i)
        {
            UI_LevelButton newButton = Instantiate(buttonPrefab, buttonParent);
            newButton.SetupButton(i);
        }
    }
}
