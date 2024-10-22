using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public AudioSource EffectsSource;
    public AudioSource MusicSource;
   // public AudioSource WalkingSource;

    public float LowPitchRange = .95f;
    public float HighPitchRange = 1.05f;

    public static SoundManager Instance = null;

    public int simultaneousPlayCount = 0;
    public int maxSimultaneousSounds = 999;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Play(AudioClip clip)
    {
        EffectsSource.clip = clip;
        StartCoroutine(PlaySound(clip));
    }

    public void Play(AudioClip clip, float volume)
    {
        EffectsSource.clip = clip;
        StartCoroutine(PlaySoundVolume(clip,volume));
    }


    IEnumerator PlaySound(AudioClip clip, bool autoScaleVolume = true, float maxVolumeScale = 1f)
    {
        if (simultaneousPlayCount >= maxSimultaneousSounds)
        {
            yield break;
        }

        simultaneousPlayCount++;

        float vol = maxVolumeScale;

        // Scale down volume of same sound played subsequently
        if (autoScaleVolume && simultaneousPlayCount > 0)
        {
            vol = vol / (float)(simultaneousPlayCount);
        }

        MusicSource.PlayOneShot(clip, vol);

        // Wait til the sound almost finishes playing then reduce play count
        float delay = clip.length * 0.7f;

        yield return new WaitForSeconds(delay);

        simultaneousPlayCount--;
    }

    IEnumerator PlaySoundVolume(AudioClip clip, float vol)
    {
        if (simultaneousPlayCount >= maxSimultaneousSounds)
        {
            yield break;
        }

        simultaneousPlayCount++;

        MusicSource.PlayOneShot(clip, vol);

        // Wait til the sound almost finishes playing then reduce play count
        float delay = clip.length * 0.7f;

        yield return new WaitForSeconds(delay);

        simultaneousPlayCount--;
    }

    public void PlayMusic(AudioClip clip)
    {
        MusicSource.clip = clip;
        MusicSource.Play();
    }

    public void RandomSoundEffect(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

        EffectsSource.pitch = randomPitch;
        EffectsSource.clip = clips[randomIndex];
        EffectsSource.Play();
    }

}