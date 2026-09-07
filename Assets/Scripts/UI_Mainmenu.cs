using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Mainmenu : MonoBehaviour
{
    private UI_FadeEffect fadeEffect;
    public string sceneName;

    [SerializeField] private GameObject[] uiElements;


    private void Awake()
    {
        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
    }

    public void SwitchUI(GameObject uiToEnable)
    {
        foreach (GameObject ui in uiElements)
        {
            ui.SetActive(false);
        }

        uiToEnable.SetActive(true);
    }

    private void Start()
    {
        fadeEffect.ScreenFade(0, 1.5f);
    }
    public void NewGame()
    {
        fadeEffect.ScreenFade(1, 1.5f, LoadLevelScene);
    }

    private void LoadLevelScene() => SceneManager.LoadScene(sceneName);
}
