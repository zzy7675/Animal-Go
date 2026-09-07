using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LevelButton : MonoBehaviour
{
    private int levelIndex;
    public string sceneName;
    [SerializeField] private TextMeshProUGUI levelNumberText;

    public void SetupButton(int newLevelIndex)
    {
        levelIndex = newLevelIndex;
        sceneName = "Level_" + levelIndex;
        levelNumberText.text = "Level " + levelIndex;
    }
    public void LoadLevel()
    {
        SceneManager.LoadScene(sceneName);
    }
}
