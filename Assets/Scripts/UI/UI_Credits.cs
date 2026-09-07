using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Credits : MonoBehaviour
{
    private UI_FadeEffect fadeEffect;
    [SerializeField] private RectTransform rectT;
    [SerializeField] private float scrollSpeed = 200;
    [SerializeField] private float offScreenPosition = 1800;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool creditsSkipped;

    private void Start()
    {
        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
        fadeEffect.ScreenFade(0, 1);
    }
    private void Update()
    {
        rectT.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        if (rectT.anchoredPosition.y > offScreenPosition)
        {
            GoToMainMenu();
        }
    }

    public void SkipCredits()
    {
        if (!creditsSkipped)
        {
            scrollSpeed *= 10;
            creditsSkipped = true;
        } else
        {
            GoToMainMenu();
        }
    }

    private void GoToMainMenu()
    {
        fadeEffect.ScreenFade(1, 1, SwitchToMainMenuScene);
    }

    private void SwitchToMainMenuScene()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
