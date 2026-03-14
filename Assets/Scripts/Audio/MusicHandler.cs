using UnityEngine;


public class MusicHandler : MonoBehaviour
{
    
    
    [SerializeField] private float volume = 0.65f;
    public AudioClip[] musicClips;
    [SerializeField] private bool playOnAwake = true;

    private void Start()
    {
        if (playOnAwake)
            PlaySound();
    }

    public void PlaySound(int index = 0)
    {
        if (musicClips.Length > 0)
        {
            if (index <= musicClips.Length - 1 && musicClips[index] != null)
            {
                SoundManager.Instance.Music_SetVolume(volume);
                SoundManager.Instance.Music_Play(musicClips[index]);
            }
        }
    }
}
