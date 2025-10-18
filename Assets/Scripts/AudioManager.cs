using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Persistent Sounds")]
    public AudioSource nightAudio;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Only one instance
            return;
        }

        if (nightAudio != null && !nightAudio.isPlaying)
        {
            nightAudio.loop = true;
            nightAudio.Play();
        }
    }

    /// <summary>
    /// Play a one-shot sound at a world position
    /// </summary>
    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;

        AudioSource.PlayClipAtPoint(clip, position, volume);
    }
}
