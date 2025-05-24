using UnityEngine;

public class AudioManagerLoader : MonoBehaviour
{
    public GameObject audioManagerPrefab;

    void Awake()
    {
        if (AudioManager.Instance == null)
        {
            Instantiate(audioManagerPrefab);
        }
    }
}
