using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager instance;
    public int chosenSkinIndex;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SetSkinIndex(int skinIndex) => chosenSkinIndex = skinIndex;
    public int GetChosenSkinIndex() => chosenSkinIndex;
}
