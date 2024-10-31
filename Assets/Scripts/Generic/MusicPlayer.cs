using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] musicToPlay;
    private AudioSource audioSource;

    private int lastPlayedIndex = -1;
    private float changeSongTimer = 0.0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        PlayRandomMusic();
    }

    private void Update()
    {
        changeSongTimer -= Time.deltaTime;
        if (changeSongTimer < 0.0f)
        {
            PlayRandomMusic();
        }
    }



    private void PlayRandomMusic()
    {
        if (musicToPlay.Length == 0)
        {
            Debug.LogWarning("No music clips assigned to play.");
            return;
        }

        int newIndex;

        // Ensure a different track is chosen
        do
        {
            newIndex = Random.Range(0, musicToPlay.Length);
        } while (newIndex == lastPlayedIndex && musicToPlay.Length > 1);

        lastPlayedIndex = newIndex;

        // Play the selected audio clip
        audioSource.clip = musicToPlay[newIndex];
        changeSongTimer = musicToPlay[newIndex].length;
        audioSource.Play();
    }
}