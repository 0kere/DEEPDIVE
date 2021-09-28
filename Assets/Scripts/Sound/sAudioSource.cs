using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sAudioSource : MonoBehaviour
{
    public AudioSource source;
    public AudioClip clip;
    public float pitch = 1f;
    public float initialVolume = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        if (source == null)
        {
            if (TryGetComponent(out AudioSource _source))
            {
                source = _source;
            }
            else
            {
                source = gameObject.AddComponent<AudioSource>();
            }
        }
        if (!source.isPlaying && source.clip == null)
        {
            source.clip = clip;
            SetVolume(initialVolume);
            source.Play();
        }
        source.pitch = pitch;
    }

    public void PlayNewClip(AudioClip clip)
    {
        source.Stop();
        SetVolume(1);
        source.clip = clip;
        source.Play();
    }

    public void SetVolume(float volume)
    {
        source.volume = Mathf.Clamp01(volume);
    }

    public void StopPlaying()
    {
        source.Stop();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
