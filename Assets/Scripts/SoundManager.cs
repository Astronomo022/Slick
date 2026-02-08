using UnityEngine;

// Adapted from https://www.jezner.com/2025/02/04/building-a-sound-manager-in-unity/
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;
    private AudioSource soundEffectAudio;
    private AudioSource musicAudio;
    private AudioClip[] soundEffectClips;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Make sure there is only one instance of the SoundManager
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // Find the audio source to which sound effects should be added
        // and the one for music, and assign them to the appropriate variables
        AudioSource[] sources = GetComponents<AudioSource>();
        foreach (AudioSource source in sources)
        {
            if (source.clip == null)
            {
                soundEffectAudio = source;
            }
            else
            {
                musicAudio = source;
            }
        }
    }
    void Awake()
    {
        // Load all sound effects from the Sounds/SFX folder
        soundEffectClips = Resources.LoadAll<AudioClip>("Sounds/SFX");
        //foreach (var sfx in soundEffectClips)
        //    Debug.Log(sfx.name);
    }

    public void PlaySoundEffect(string soundEffectName)
    {
        string soundEffectFileName = "slickgame_soundeffects_" + soundEffectName; // Assuming the file name is the same as the sound effect name
        AudioClip clip = System.Array.Find(soundEffectClips, c => c.name == soundEffectFileName);
        if (clip != null && soundEffectAudio != null)
        {
            soundEffectAudio.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Sound effect '{soundEffectFileName}' not found or AudioSource not available.");
        }
    }

    public void PlayMusic()
    {
        AudioClip clip = Resources.Load<AudioClip>("Sounds/Music/V2/slick boss");
        if (clip != null && musicAudio != null)
        {
            musicAudio.clip = clip;
            musicAudio.Play();
        }
        else
        {
            Debug.LogWarning($"Music clip 'slickgame_music_bossfight' not found or AudioSource not available.");
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            PlayMusic();
        }
    }

    /*
    public void PlaySoundEffect(string fileName)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Sounds/SFX/{fileName}");
        if (clip != null && soundEffectAudio != null)
        {
            soundEffectAudio.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Sound effect '{fileName}' not found or AudioSource not available.");
        }
    }
     */
}
