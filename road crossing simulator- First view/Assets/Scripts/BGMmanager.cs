using UnityEngine;

public class BGMmanager : MonoBehaviour
{
    public static BGMmanager instance;

    private AudioSource audioSource;

    void Awake()
    {
        // 单例模式，确保只有一个BGM存在
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 场景切换不销毁
        }
        else
        {
            Destroy(gameObject); // 已存在一个就销毁自己
            return;
        }

        audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.loop = true;       // 循环播放
            audioSource.playOnAwake = true; // 游戏开始自动播放
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource not found on BGMManager!");
        }
    }
}

