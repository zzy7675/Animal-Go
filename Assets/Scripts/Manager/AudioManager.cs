using UnityEngine;

public enum SFXType
{
    SFX_Death,
    SFX_EnemyKicked,
    SFX_Finish,
    SFX_Jump,
    SFX_MenuSelect1,
    SFX_MenuSelect2,
    SFX_NoMoney,
    SFX_Pickup1,
    SFX_Pickup2,
    SFX_PlayerK,
    SFX_Respawn1,
    SFX_Respawn2,
    SFX_Walljump
}


public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource[] sfx;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this.gameObject);
        }
    }

    public void PlaySFX(int sfxToPlay)
    {
        if (sfxToPlay >= sfx.Length)
        {
            return;
        }

        sfx[sfxToPlay].Play();
    }

    public void StopSFX(int sfxToStop)
    {
        sfx[sfxToStop].Stop();
    }
}
