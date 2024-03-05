using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundMenager : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private TextMeshProUGUI scrollbarText;
    [SerializeField] private AudioSource sourceAmbiance;
    [SerializeField] private AudioSource sourceUI;
    [SerializeField] private AudioSource sourceMusic;


    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip[] steps;
    [SerializeField] private AudioClip[] stepsGreen;
    float volumeAmbient => sourceAmbiance.volume;
    float volumeUI => sourceUI.volume;
    float volumeMusic => sourceMusic.volume;


    float transitionTime = 0.25f;
    int lastStepSound;

    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    public static SoundMenager instaince;
    private void Awake()
    {
        if (instaince != null)
        {
            Debug.LogError("instaince != null");
        }
        instaince = this;
    }
    public void PlayWin()
    {
        PlayClip(sourceUI, volumeUI, winSound);
    }
    public void PlayLose()
    {
        PlayClip(sourceUI, volumeUI, loseSound);
    }
    public void ScrollbarChanged()
    {
        sourceMusic.volume = scrollbar.value;
        sourceAmbiance.volume = scrollbar.value;
        sourceUI.volume = scrollbar.value;
        scrollbarText.text = Mathf.Round(scrollbar.value * 100).ToString() + "%";
    }
    public AudioClip GetStep()
    {
        int i = 0;
        do
        {
            i = Random.Range(0, steps.Length);
        } while (lastStepSound == i);
        lastStepSound = i;
        return steps[i];
    }
    public AudioClip GetGreenSteps()
    {
        int i = 0;
        do
        {
            i = Random.Range(0, stepsGreen.Length);
        } while (lastStepSound == i);
        lastStepSound = i;
        return stepsGreen[i];
    }
    public void PlaySound(AudioClip audioClip)
    {
        PlayClip(sourceAmbiance, volumeAmbient, audioClip);
    }
    public void PlayMusic(AudioClip audioClip)
    {
        PlayClip(sourceMusic, volumeMusic, audioClip);
    }
    private void PlayClip(AudioSource source, float volume, AudioClip playClip)
    {
        if (source.isPlaying)
        {
            StartCoroutine(MixClips(source, volume, playClip));
        }
        else
        {
            source.volume = volume;
            source.clip = playClip;
            source.Play();
        }
    }

    IEnumerator MixClips(AudioSource source, float volume, AudioClip playClip)
    {
        float precentage = 0;
        while (source.volume > 0)
        {
            source.volume = Mathf.Lerp(volume, 0, precentage);
            precentage += Time.deltaTime / transitionTime;
            yield return null;
        }
        source.clip = playClip;
        source.Play();
        precentage = 0;
        while (source.volume < volume)
        {
            source.volume = Mathf.Lerp(0, volume, precentage);
            precentage += Time.deltaTime / transitionTime;
            yield return null;
        }
    }
}
