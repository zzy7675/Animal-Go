using UnityEngine;
using UnityEngine.UI;

public class UI_ContinueButton : MonoBehaviour
{
    [SerializeField] private Button buttonContinue;
    [SerializeField] private Button buttonNewGame;
    [SerializeField] private Button buttonQuit;


    private void OnEnable()
    {
        ChangeNavigationControl();
    }

    private void ChangeNavigationControl()
    {
        Navigation buttonQuitNavigation = buttonQuit.navigation;
        buttonQuitNavigation.selectOnDown = buttonContinue;
        buttonQuit.navigation = buttonQuitNavigation;

        Navigation buttonNewGameNavigation = buttonNewGame.navigation;
        buttonNewGameNavigation.selectOnUp = buttonContinue;
        buttonNewGame.navigation = buttonNewGameNavigation;
    }

}
