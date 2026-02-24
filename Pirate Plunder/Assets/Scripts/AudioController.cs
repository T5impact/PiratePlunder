using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [Header("Winning")]
    [SerializeField] AudioSource counterSource;
    [SerializeField] AudioSource payoutSource;
    [SerializeField] AudioSource winSmallSource;
    [SerializeField] AudioSource winMediumSource;
    [SerializeField] AudioSource winLargeSource;
    [SerializeField] AudioSource winGrandSource;

    public void SilencePayout(float delay)
    {
        StartCoroutine(SilenceAudioSource(payoutSource, 0.5f, delay));
    }
    public void PlayWinSmall()
    {
        winSmallSource.Play();
        payoutSource.Play();
    }
    /*public void SilenceSmallPayout(float delay)
    {
        StartCoroutine(SilenceAudioSource(payoutSmall, 0.5f, delay));
    }*/
    public void PlayWinMedium()
    {
        winMediumSource.Play();
        payoutSource.Play();
    }
    /*public void SilenceMediumPayout(float delay)
    {
        StartCoroutine(SilenceAudioSource(payoutMedium, 0.5f, delay));
    }*/
    public void PlayWinLarge() 
    { 
        winLargeSource.Play();
        payoutSource.Play();
    }
    /*public void SilenceLargePayout(float delay)
    {
        StartCoroutine(SilenceAudioSource(payoutLarge, 0.5f, delay));
    }*/
    public void PlayWinGrand()
    {
        winGrandSource.Play();
        payoutSource.Play();
    }
    /*public void SilenceGrandPayout(float delay)
    {
        StartCoroutine(SilenceAudioSource(payoutGrand, 0.5f, delay));
    }*/
    public void StartCounterSound()
    {
        counterSource.pitch = 1;
        counterSource.Play();
    }
    public void SetCounterPitch(float pitch)
    {
        counterSource.pitch = pitch;
    }
    public void StopCounterSound()
    {
        counterSource.Stop();
    }

    [Header("Cannon")]
    [SerializeField] AudioSource cannonFireSource;
    [SerializeField] AudioSource cannonHitSource;
    [SerializeField] AudioSource cannonMissSource;
    [SerializeField] AudioSource cannonballGetSource;
    [SerializeField] AudioSource metalCloseSource;
    [SerializeField] AudioSource metalOpenSource;

    public void PlayCannonFire()
    {
        cannonFireSource.Play();
    }
    public void PlayCannonHit()
    {
        cannonHitSource.Play();
    }
    public void PlayCannonMiss()
    {

        cannonMissSource.Play();
    }
    public void PlayCannonballGet(float pitch)
    {
        cannonballGetSource.pitch = pitch;
        cannonballGetSource.Play();
    }
    public void PlayMetalClose()
    {
        metalCloseSource.Play();
    }
    public void PlayMetalOpen()
    {
        metalOpenSource.Play();
    }

    IEnumerator SilenceAudioSource(AudioSource source, float length, float delay)
    {
        yield return new WaitForSeconds(delay);

        float defaultVolume = source.volume;

        float timer = 0;
        while (timer < length)
        {
            source.volume = defaultVolume * (1 - timer / length);
            timer += Time.deltaTime;
            yield return null;
        }

        source.Pause();
        source.volume = defaultVolume;
    }
}
