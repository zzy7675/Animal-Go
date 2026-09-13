using UnityEngine;
using UnityEngine.EventSystems;

public class UI_UpdateLastSelected : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    private UI_Mainmenu mainMenu;
    private void Awake()
    {
        mainMenu = GetComponentInParent<UI_Mainmenu>();
    }
    public void OnSelect(BaseEventData eventData)
    {
        mainMenu.UpdateLastSelected(this.gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
