using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class AudioController : MonoBehaviour
{

    [Header("Children Components")]
    public AudioSource musicController;
    public AudioSource sfxController;

    [Header("Audio Assets")]
    public AudioClip[] actionSFX;


    [Header("Config")]
    public float musicExtraSpeed;
    

    public static void Audio_PlaySound(int soundCode)
    {

        FindObjectOfType<AudioController>().PlaySound(soundCode);
    }

    public static void Audio_MusicState(bool normal)
    {
        FindObjectOfType<AudioController>().MusicState(normal);
    }

    public void PlaySound(int soundCode)
    {
        sfxController.PlayOneShot(actionSFX[soundCode]);
    }

    public void MusicState(bool normal)
    {
        if (normal)
            musicController.pitch = 1.2f;
        else
            musicController.pitch = 1f;
    }

    

    
    
}
