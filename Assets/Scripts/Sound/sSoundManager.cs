using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sSoundManager : MonoBehaviour
{
    private static sSoundManager sm;
    public static sSoundManager instance
    {
        get
        {
            if (!sm)
            {
                sm = FindObjectOfType<sSoundManager>() as sSoundManager;
                if (!sm)
                {
                }
                else
                {
                    sm.Init();
                }
            }
            return sm;
        }
    }

    void Init() { }

    public AudioClip gameOverClip;
    public AudioClip highscoreClip;
    public AudioClip buttonPressClip;
    public AudioClip bitPickUpClip;
    public AudioClip colorSwitchClip;

    private IEnumerator colorSwitchRoutine;
    // Start is called before the first frame update
    void Start()
    {
        sColourSwitchManager.switchEvent += PlayColorSwitch;
    }

    private void PlayColorSwitch(ColorPallete pallete, float speed, float cooldown)
    {
        if (colorSwitchRoutine == null)
        {
            colorSwitchRoutine = ColorSwitchClip((1 / speed) + cooldown);
            StartCoroutine(colorSwitchRoutine);
            PlayColorSwitchClip();
        }
    }
    private IEnumerator ColorSwitchClip(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        colorSwitchRoutine = null;
    }

    public static void PlayGameOverClip()
    {
        sSoundManager.SpawnAudioSource(sSoundManager.instance.gameOverClip);
    }

    public static void PlayHighScoreClip()
    { 
        sSoundManager.SpawnAudioSource(sSoundManager.instance.highscoreClip);
    }
    public static void PlayButtonPressClip()
    {
        sSoundManager.SpawnAudioSource(sSoundManager.instance.buttonPressClip);
    }
    public static void PlayBitPickUpClip()
    {
        sSoundManager.SpawnAudioSource(sSoundManager.instance.bitPickUpClip);
    }

    public static void PlayColorSwitchClip()
    {
        sSoundManager.SpawnAudioSource(sSoundManager.instance.colorSwitchClip).SetVolume(.05f);
    }
    private static sAudioSource SpawnAudioSource(AudioClip audioClip)
    {
        for (int i = 0; i < GameManager.instance.audioSources.Count; i++)
        {
            sAudioSource tempSource = GameManager.instance.audioSources[i];
            if (!tempSource.source)
            {
                GameManager.instance.audioSources.RemoveAt(i);
                continue;
            }
            if (!tempSource.source.isPlaying)
            {
                tempSource.PlayNewClip(audioClip);
                return tempSource;
            }
        }
        //create a new source if there is not one available in the pool
        GameObject newSourceObject = new GameObject("AudioSource");
        sAudioSource newSource = newSourceObject.AddComponent<sAudioSource>();
        GameManager.instance.audioSources.Add(newSource);
        newSource.clip = audioClip;
        return newSource;
    }
}
