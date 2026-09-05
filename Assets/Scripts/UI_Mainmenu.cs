using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Mainmenu : MonoBehaviour
{
    public string sceneName;

    public void NewGame()
    {
        SceneManager.LoadScene(sceneName);
    }
}
