using UnityEngine;

public class SoundController : Singleton<SoundController>
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

    public enum SoundType
    {
        BG,
        SWIPE,
        BOX_ARRIVED,
        LOST,
        WIN
    }

    /// <summary>
    /// Callback for playing an sound effect based on enum value if sfx is active in settings
    /// </summary>
    /// <param name="soundType"></param>
    public void PlaySfx(SoundType soundType)
    {
        if (DataManager.Instance.Settings.sfx == true)
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

    /// <summary>
    /// Callback for start or stop playing background music if sound is active in settings
    /// </summary>
    public void PlayMusic()
    {
        if (DataManager.Instance.Settings.sound == true)
        {
            MusicSource.clip = bgMusic;
            MusicSource.loop = true;
            MusicSource.Play();
        }
        else
        {
            StopMusic();
        }
    }

    /// <summary>
    /// Callback for stop playing background music
    /// </summary>
    public void StopMusic()
    {
        MusicSource.Stop();
    }

    /// <summary>
    /// Callback for playing random sound effect
    /// </summary>
    /// <param name="clips"></param>
    public void RandomSoundEffect(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(LowPitchRange, HighPitchRange);
        EffectsSource.pitch = randomPitch;
        EffectsSource.clip = clips[randomIndex];
        EffectsSource.Play();
    }
}
