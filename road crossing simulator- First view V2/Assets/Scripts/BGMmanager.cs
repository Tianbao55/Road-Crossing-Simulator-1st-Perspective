using UnityEngine;

public class BGMmanager : MonoBehaviour
{
    public static BGMmanager instance; // Singleton instance

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton pattern: ensure only one BGM exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Don't destroy on scene change
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate BGM
            return;
        }

        audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.loop = true;        // Enable looping
            audioSource.playOnAwake = true; // Play automatically at start
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource not found on BGMManager!");
        }
    }
}

