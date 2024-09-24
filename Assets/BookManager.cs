using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    private AudioSource audioSource;
    public List<AudioClip> audioClips;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(int i)
    {
        audioSource.PlayOneShot(audioClips[i], FindAnyObjectByType<LevelManager>().voiceSlider.value);
    }
}
