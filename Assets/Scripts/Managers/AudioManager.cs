using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public struct AudioSet
{
    public AudioClip clip;
    public bool loop;
    [Range(0, 1)]
    public float volume;
    [Range(-3, 3)]
    public float pitch;

    [Header("3D Settings")]
    public bool is3D;
    public float minDistance;
    public float maxDistance;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public TimeCycle time;
    [Space]
    public AudioSource instantPlaySource;
    public AudioSource ambientSource;

    public AudioSet craftingButtonSFX;

    public AnimationCurve ambientAudioCurve;

    private bool updateAmbient = true;

    private void Awake()
    {
        if (instance != null)
            return;
        else
            instance = this;
    }

    private void Update()
    {
        InitiateAmbientAudio();
    }

    private void InitiateAmbientAudio()
    {
        if (updateAmbient == true)
            StartCoroutine(UpdateAmbientAudio());
    }

    private IEnumerator UpdateAmbientAudio()
    {
        updateAmbient = false;
        yield return new WaitForSeconds(0.35f);
        ambientSource.volume = ambientAudioCurve.Evaluate(time.Delta());
        updateAmbient = true;
    }

    public void InstantPlay(AudioSet audioSet)
    {
        instantPlaySource.loop = audioSet.loop;
        instantPlaySource.clip = audioSet.clip;
        instantPlaySource.volume = audioSet.volume;
        instantPlaySource.pitch = audioSet.pitch;

        if (audioSet.is3D)
        {
            instantPlaySource.spatialBlend = 1;
            instantPlaySource.minDistance = audioSet.minDistance;
            instantPlaySource.maxDistance = audioSet.maxDistance;
        }
         instantPlaySource.Play();
    }
}
