using UnityEngine;

public class SoundMaker : MonoBehaviour
{
    public AudioSource audioSource;
    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        AudioSource exampleAudioSource = GameManager.Instance.audioSourceExample;
        if (exampleAudioSource != null)
        {
            audioSource.clip = exampleAudioSource.clip;
            audioSource.outputAudioMixerGroup = exampleAudioSource.outputAudioMixerGroup;
            audioSource.mute = exampleAudioSource.mute;
            audioSource.bypassEffects = exampleAudioSource.bypassEffects;
            audioSource.bypassListenerEffects = exampleAudioSource.bypassListenerEffects;
            audioSource.bypassReverbZones = exampleAudioSource.bypassReverbZones;
            audioSource.playOnAwake = exampleAudioSource.playOnAwake;
            audioSource.loop = exampleAudioSource.loop;
            audioSource.priority = exampleAudioSource.priority;
            audioSource.volume = exampleAudioSource.volume;
            audioSource.pitch = exampleAudioSource.pitch;
            audioSource.panStereo = exampleAudioSource.panStereo;
            audioSource.spatialBlend = exampleAudioSource.spatialBlend;
            audioSource.reverbZoneMix = exampleAudioSource.reverbZoneMix;
            audioSource.dopplerLevel = exampleAudioSource.dopplerLevel;
            audioSource.spread = exampleAudioSource.spread;
            audioSource.rolloffMode = exampleAudioSource.rolloffMode;
            audioSource.minDistance = exampleAudioSource.minDistance;
            audioSource.maxDistance = exampleAudioSource.maxDistance;
            audioSource.spatialize = exampleAudioSource.spatialize;
            audioSource.spatializePostEffects = exampleAudioSource.spatializePostEffects;
        }
        else
        {
            Debug.Log("Gamemanager audio source null");
        }
    }

    public void PlaySound(AudioClip sound, float volume, bool destroyOnEnd = true)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
            audioSource.PlayOneShot(sound);
        }
        if (destroyOnEnd)
        {
            Destroy(gameObject, sound.length + 0.1f);
        }
    }
}
