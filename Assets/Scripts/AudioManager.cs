using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance { get { return instance; } }

    private AudioSource musicSource;
    private AudioSource soundEffectsSource;

    [Range(0f, 1f)]
    public float musicVolume = 1f;
    [Range(0f, 1f)]
    public float soundEffectsVolume = 1f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        soundEffectsSource = gameObject.AddComponent<AudioSource>();

        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        soundEffectsVolume = PlayerPrefs.GetFloat("SoundEffectsVolume", 1f);
    }


    public void PlaySoundEffect(AudioClip clip, float volume = 1f)
    {
        soundEffectsSource.PlayOneShot(clip, volume * soundEffectsVolume);
    }

    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        musicSource.clip = clip;
        musicSource.volume = volume * musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = musicVolume;
    }

    public void SetSoundEffectsVolume(float volume)
    {
        soundEffectsVolume = volume;
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
