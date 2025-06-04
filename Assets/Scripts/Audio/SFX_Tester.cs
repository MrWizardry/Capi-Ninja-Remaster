using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Tester : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip sfxClip;
    public float cooldownTime = 0.5f;
    private float lastPlayTime = -1f;

    public void PlaySFX()
    {
        if (Time.time - lastPlayTime >= cooldownTime)
        {
            audioSource.PlayOneShot(sfxClip);
            lastPlayTime = Time.time;
        }
    }
}
