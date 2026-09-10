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
    SFX_PlayerKnocked,
    SFX_Respawn1,
    SFX_Respawn2,
    SFX_Walljump
}


public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    [SerializeField] private int bgmIndex;

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
        if (bgm.Length <= 0)
        {
            return;
        }

        InvokeRepeating(nameof(PlayMusicIfNeeded), 0, 2);
    }

    public void PlayMusicIfNeeded()
    {
        if (!bgm[bgmIndex].isPlaying)
        {
            PlayRandomBGM();
        }
    }

    public void PlayRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    public void PlayBGM(int bgmToPlay)
    {
        if (bgm.Length <= 0)
        {
            Debug.LogWarning("You have no music on audio manager.");
            return;
        }
        for (int i = 0; i < bgm.Length; ++i)
        {
            bgm[i].Stop();
        }
        bgmIndex = bgmToPlay;
        bgm[bgmToPlay].Play();
    }

    public void PlaySFX(int sfxToPlay, bool randomPitch = false)
    {
        if (sfxToPlay >= sfx.Length)
        {
            return;
        }
        if (randomPitch)
            sfx[sfxToPlay].pitch = Random.Range(0.9f, 1.1f);
        sfx[sfxToPlay].Play();
    }

    public void StopSFX(int sfxToStop)
    {
        sfx[sfxToStop].Stop();
    }
}
