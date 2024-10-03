using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set;}
    private AudioSource audioSource;
    private AudioSource musicSource;
    private AudioSource slashSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        slashSource = transform.GetChild(1).GetComponent<AudioSource>();
        musicSource = transform.GetChild(0).GetComponent<AudioSource>();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //Destroy duplicate gameobjects
        else if (instance != null && instance != this)
            Destroy(gameObject);

        //Assign initial volume values
        MusicVolumeChange(0);
        SoundVolumeChange(0);
    }

    public void PlayAudioClip(AudioClip _audioClip)
    {
        audioSource.PlayOneShot(_audioClip);
    }

    public bool IsSlashPlaying()
    {
        return slashSource.isPlaying;
    }

    public void PlaySlashes(AudioClip _audioClip)
    {
        slashSource.PlayOneShot(_audioClip);
        slashSource.volume = audioSource.volume * 0.5f;
    }

    public void StopSlashes()
    {
        if(slashSource.isPlaying)
        slashSource.Stop();
    }

    public void SourceVolumeChange(float baseVolume, string sourceName, float change, AudioSource source)
    {
        //baseVolume is the base volume for sound effects

        //initialize the current volume
        float currentVolume = PlayerPrefs.GetFloat(sourceName, 1);

        currentVolume += change;

        if (currentVolume > 1)
        {
            currentVolume = 0;
        }
        else if (currentVolume < 0)
        {
            currentVolume = 1;
        }
        //Assign the final volume to the audio source
        float finalVolume = currentVolume * baseVolume;
        source.volume = finalVolume;
        //Apply the changes to the player prefs
        PlayerPrefs.SetFloat(sourceName, currentVolume);
    }

    public void MusicVolumeChange(float _change)
    {
       SourceVolumeChange(0.3f, "musicVolume", _change, musicSource);
    }
    public void SoundVolumeChange(float _change)
    {
       SourceVolumeChange(1f, "soundVolume", _change, audioSource);
    }
}
