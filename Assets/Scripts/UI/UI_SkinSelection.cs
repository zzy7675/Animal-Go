using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[System.Serializable]
public struct Skin
{
    public string skinName;
    public int skinPrice;
    public bool unlocked;
}

public class UI_SkinSelection : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;
    [Space]

    private DefaultInputActions defaultInput;
    private UI_LevelSelection uiLevelSelection;
    private UI_Mainmenu uiMainMenu;
    [SerializeField] private Skin[] skinList;

    [Header("UI Details")]
    [SerializeField] private int skinIndex;
    [SerializeField] private int maxIndex;
    [SerializeField] private Animator skinDisplay;

    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI bankText;
    [SerializeField] private TextMeshProUGUI buySelectText;


    [Space]
    [SerializeField] private float inputCooldown = .1f;
    private float lastTimeInput;

    private void Awake()
    {
        LoadSkinUnlocks();
        UpdateSkinDisplay();

        uiMainMenu = GetComponentInParent<UI_Mainmenu>();
        uiLevelSelection = uiMainMenu.GetComponentInChildren<UI_LevelSelection>(true);
        defaultInput = new DefaultInputActions();
    }

    private void OnEnable()
    {
        defaultInput.Enable();
        uiMainMenu.UpdateLastSelected(firstSelected);
        EventSystem.current.SetSelectedGameObject(firstSelected);

        defaultInput.UI.Navigate.performed += ctx =>
        {
            if (Time.time - lastTimeInput < inputCooldown)
                return;

            if (ctx.ReadValue<Vector2>().x <= -1)
            {
                PreviousSkin();
            }

            if (ctx.ReadValue<Vector2>().x >= 1)
            {
                NextSkin();
            }
        };
    }

    private void OnDisable()
    {
        defaultInput.Disable();
    }

    private void LoadSkinUnlocks()
    {
        for (int i = 0; i < skinList.Length; ++i)
        {
            string skinName = skinList[skinIndex].skinName;
            bool skinUnlocked = PlayerPrefs.GetInt(skinName + "Unlocked", 0) == 1;

            if (skinUnlocked || i == 0)
                skinList[i].unlocked = true;
        }
    }

    public void SelectSkin()
    {
        if (!skinList[skinIndex].unlocked)
            BuySkin(skinIndex);
        else
        {
            SkinManager.instance.SetSkinIndex(skinIndex);
            uiMainMenu.SwitchUI(uiLevelSelection.gameObject);
        }
        AudioManager.instance.PlaySFX(((int)SFXType.SFX_MenuSelect1));
        UpdateSkinDisplay();
    }

    public void NextSkin()
    {
        lastTimeInput = Time.time;
        skinIndex++;

        if (skinIndex > maxIndex)
            skinIndex = 0;
        AudioManager.instance.PlaySFX(((int)SFXType.SFX_MenuSelect1));
        UpdateSkinDisplay();
    }

    public void PreviousSkin()
    {
        lastTimeInput = Time.time;
        skinIndex--;
        if (skinIndex < 0)
            skinIndex = maxIndex;
        AudioManager.instance.PlaySFX(((int)SFXType.SFX_MenuSelect1));
        UpdateSkinDisplay();
    }

    private void UpdateSkinDisplay()
    {
        bankText.text = "Bank: " + FruitsInBank().ToString();
        for (int i = 0; i < skinDisplay.layerCount; ++i)
        {
            skinDisplay.SetLayerWeight(i, 0);
        }
        skinDisplay.SetLayerWeight(skinIndex, 1);

        if (skinList[skinIndex].unlocked)
        {
            priceText.transform.parent.gameObject.SetActive(false);
            buySelectText.text = "Select";
        } else
        {
            priceText.transform.parent.gameObject.SetActive(true);
            priceText.text = "Price: " + skinList[skinIndex].skinPrice.ToString();
            buySelectText.text = "Buy";
        }
    }

    private void BuySkin(int index)
    {
        if (!HaveEnoughFruits(skinList[index].skinPrice))
        {
            AudioManager.instance.PlaySFX(((int)SFXType.SFX_NoMoney));
            return;
        }

        AudioManager.instance.PlaySFX(((int)SFXType.SFX_Respawn1));
        string skinName = skinList[skinIndex].skinName;
        skinList[skinIndex].unlocked = true;

        PlayerPrefs.SetInt(skinName + "Unlocked", 1);
    }

    private int FruitsInBank() => PlayerPrefs.GetInt("TotalFruitsAmount");

    private bool HaveEnoughFruits(int price)
    {
        if (FruitsInBank() > price)
        {
            PlayerPrefs.SetInt("TotalFruitsAmount", FruitsInBank() - price);
            return true;
        }

        return false;
    }
}
