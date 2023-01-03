using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioClip bgMusic;
    public AudioClip swipeEffect;
    public AudioClip boxArrivedEffect;
    public AudioClip gameLostEffect;
    public AudioClip gameWinEffect;

    public AudioSource EffectsSource;
    public AudioSource MusicSource;

    public float LowPitchRange = .95f;
    public float HighPitchRange = 1.05f;

    public static SoundController Instance = null;

    public enum SoundType
    {
        BG,
        SWIPE,
        BOX_ARRIVED,
        LOST,
        WIN
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        PlayMusic();
    }

    public void Play(SoundType soundType)
    {
        if (PlayerPrefs.GetInt("SFX") == 1)
        {
            switch (soundType)
            {
                case SoundType.BOX_ARRIVED:
                    EffectsSource.clip = boxArrivedEffect;
                    break;
                case SoundType.LOST:
                    EffectsSource.clip = gameLostEffect;
                    break;
                case SoundType.SWIPE:
                    EffectsSource.clip = swipeEffect;
                    break;
                case SoundType.WIN:
                    EffectsSource.clip = gameWinEffect;
                    break;
            }
            EffectsSource.Play();
        }
    }

    public void PlayMusic()
    {
        if (!PlayerPrefs.HasKey("MUSIC"))
        {
            PlayerPrefs.SetInt("MUSIC", 1);
        }
        if (PlayerPrefs.GetInt("MUSIC") == 1)
        {
            MusicSource.clip = bgMusic;
            MusicSource.loop = true;
            MusicSource.Play();
        }
    }
    public void StopMusic()
    {
        MusicSource.Stop();
    }

    public void RandomSoundEffect(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(LowPitchRange, HighPitchRange);
        EffectsSource.pitch = randomPitch;
        EffectsSource.clip = clips[randomIndex];
        EffectsSource.Play();
    }
}
