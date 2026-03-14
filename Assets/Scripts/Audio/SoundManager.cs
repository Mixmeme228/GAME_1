using System.Collections;
using UnityEngine;


public class SoundManager : Singleton<SoundManager>
{
    
    
    

    
        

    

    public enum SFX_Type { Default, Unstoppable }

    protected SoundManager() { }
    private SoundEmitter sfx;
    private SoundEmitter unstoppableSfx;
    private SoundEmitter music;
    protected override void Awake()
    {
        base.Awake();

        // if there are no SoundEmitters, create them.

        if (!FindObjectOfType<SoundFXEmitter>())
        {
            sfx = new GameObject("SoundFXs").AddComponent<SoundEmitter>();
            sfx.transform.SetParent(transform);
        }

        if (!FindObjectOfType<UnstoppableSoundEmitter>())
        {
            unstoppableSfx = new GameObject("UnstoppableSoundFXs").AddComponent<UnstoppableSoundEmitter>();
            unstoppableSfx.transform.SetParent(transform);
        }

        if (!FindObjectOfType<MusicEmitter>())
        {
            music = new GameObject("Music").AddComponent<MusicEmitter>();
            music.transform.SetParent(transform);
        }
    }
   

    

    public void SFX_PlayOneShot(AudioClip clip, float volume, float minPitch = 1f, float maxPitch = 1f, bool unstoppable = false)
    {
        AudioSource src = sfx.Source;

        if (unstoppable)
            src = unstoppableSfx.Source;

        if (clip != null && music != null)
        {
            src.pitch = Random.Range(minPitch, maxPitch);
            src.PlayOneShot(clip, volume);
        }
    }

    /// <summary>
    /// Starts a coroutine that uses WaitForSeconds(delay) to play a clip after the seconds have passed.
    /// </summary>
    public void SFX_PlayDelayedOneShot(AudioClip clip, float volume, float minPitch = 1f, float maxPitch = 1f, float delay = 1f)
    {
        StartCoroutine(CO_SFX_PlayDelayedOneShot(clip, volume, minPitch, maxPitch, delay));
    }

    private IEnumerator CO_SFX_PlayDelayedOneShot(AudioClip clip, float volume, float minPitch, float maxPitch, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (clip != null && music != null)
        {
            sfx.Source.pitch = Random.Range(minPitch, maxPitch);
            sfx.Source.PlayOneShot(clip, volume);
        }
    }

    public void SFX_SetVolume(float value)
    {
        if (sfx != null)
            sfx.Source.volume = value;
    }

    public void SFX_Stop()
    {
        if (sfx != null)
            sfx.Source.Stop();
    }
 

   
    public void Music_PlayOneShot(AudioClip clip, float volume)
    {
        if (clip != null && music != null)
            music.Source.PlayOneShot(clip, volume);
    }

    public void Music_Play(AudioClip clip)
    {
        if (clip != null && music != null)
        {
            music.Source.clip = clip;
            music.Source.Play();
        }
    }

    public void Music_SetVolume(float value)
    {
        if (music != null)
            music.Source.volume = value;
    }
  
}
