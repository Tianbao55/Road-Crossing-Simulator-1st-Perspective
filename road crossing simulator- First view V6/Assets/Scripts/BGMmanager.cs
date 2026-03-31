using UnityEngine;

public class BGMmanager : MonoBehaviour
{
    // Static instance used for Singleton pattern
    // Ensures there is only one BGM manager in the game
    public static BGMmanager instance;

    // Reference to the AudioSource component
    private AudioSource audioSource;

    void Awake()
    {
        // Singleton check:
        // If there is no existing instance, assign this object as the instance
        if (instance == null)
        {
            instance = this;

            // Prevent this GameObject from being destroyed when loading new scenes
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If another instance already exists, destroy this duplicate
            Destroy(gameObject);
            return;
        }

        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        // Check if AudioSource exists
        if (audioSource != null)
        {
            // Enable looping so the music plays continuously
            audioSource.loop = true;

            // Automatically play when the game starts
            audioSource.playOnAwake = true;

            // Start playing the background music
            audioSource.Play();
        }
        else
        {
            // Warning message if AudioSource component is missing
            Debug.LogWarning("AudioSource not found on BGMManager!");
        }
    }
}