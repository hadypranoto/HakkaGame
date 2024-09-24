using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSpawn : MonoBehaviour
{
    public GameObject parent;
    public GameObject item;
    public List<Transform> spots;

    public Image timer;
    private float waitingTime;
    public bool pauseTime;

    private LevelManager levelManager;

    private AudioSource audioSource;
    public AudioClip readyAudio;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        timer.fillAmount = 1;
        levelManager = FindAnyObjectByType<LevelManager>();
        switch (FindAnyObjectByType<SaveData>().currentLevel)
        {
            case 1:
                waitingTime = 0.2f;
                break;
            case 2:
                waitingTime = 0.4f;
                break;
            case 3:
                waitingTime = 0.75f;
                break;
        }
    }

    private void Update()
    {
        if (!pauseTime && ReadyToCook())
        {
            if(!audioSource.isPlaying) audioSource.Play();

            timer.fillAmount -= Time.deltaTime * waitingTime;
            if(timer.fillAmount == 0)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(readyAudio, levelManager.sfxSlider.value);
                foreach (Transform spot in spots)
                {
                    var inst = Instantiate(item, spot);
                }
                timer.fillAmount = 1;
            }
        }

        audioSource.volume = levelManager.sfxSlider.value;

        if (levelManager.pauseTime)
        {
            pauseTime = true;
            audioSource.Pause();
        }
        else
        {
            pauseTime = false;
            audioSource.UnPause();
        }
    }

    private bool ReadyToCook()
    {
        foreach (Transform spot in spots)
        {
            if (spot.transform.childCount > 0) return false;
        }        
        return true;
    }
}
