using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] musicTracks;
    public Sound[] sfxTracks;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SetupSounds(musicTracks);
        SetupSounds(sfxTracks);
    }


    private void Start()
    {
        PlayMusic("LevelMusic");
    }
    void SetupSounds(Sound[] sounds)
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
    }

    // PLAY
    public void PlayMusic(string id)
    {
        Sound s = FindSound(musicTracks, id);

        if (s != null)
            s.source.Play();
    }

    public void PlaySFX(string id)
    {
        Sound s = FindSound(sfxTracks, id);

        if (s != null)
            s.source.Play();
    }

    // STOP
    public void StopMusic(string id)
    {
        Sound s = FindSound(musicTracks, id);

        if (s != null)
            s.source.Stop();
    }

    public void StopSFX(string id)
    {
        Sound s = FindSound(sfxTracks, id);

        if (s != null)
            s.source.Stop();
    }

    // VOLUME
    public void SetVolume(string id, float volume)
    {
        Sound s = FindSound(musicTracks, id);

        if (s == null)
            s = FindSound(sfxTracks, id);

        if (s != null)
        {
            s.volume = volume;
            s.source.volume = volume;
        }
    }

    // LOOP
    public void SetLoop(string id, bool loop)
    {
        Sound s = FindAnySound(id);

        if (s != null)
        {
            s.loop = loop;
            s.source.loop = loop;
        }
    }

    // HELPERS
    Sound FindSound(Sound[] sounds, string id)
    {
        foreach (Sound s in sounds)
        {
            if (s.id == id)
                return s;
        }

        Debug.LogWarning($"Sound not found: {id}");
        return null;
    }

    Sound FindAnySound(string id)
    {
        Sound s = FindSound(musicTracks, id);

        if (s == null)
            s = FindSound(sfxTracks, id);

        return s;
    }
}