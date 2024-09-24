using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class CustomerHandler : MonoBehaviour
{
    public Customer customer;
    public OrderDetail order;
    public bool orderCompleted;

    private AudioSource audioSource;
    public Image speechBubble;
    public TextMeshProUGUI bubbleText;

    public Slider timebar;
    private float waitingTime = 60;
    public bool pauseTime = true;

    private LevelManager levelManager;
    private Image sprite;

    public AudioClip entranceAudio;
    public AudioClip checkoutAudio;

    public void CustomAwake()
    {
        audioSource = GetComponent<AudioSource>();
        sprite = GetComponent<Image>();
        levelManager = FindAnyObjectByType(typeof(LevelManager)).GetComponent<LevelManager>();
        
        if(levelManager.currLevel == 3 )
        {
            waitingTime += 30;
        }
        timebar.maxValue = waitingTime;

        bubbleText.text = "";
        speechBubble.gameObject.SetActive(false);

        sprite.color -= new Color(0, 0, 0, 1);
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(EnterStore());
    }

    private void Update()
    {
        if (!pauseTime)
        {
            waitingTime -= Time.deltaTime;
            timebar.value = waitingTime;
            if(waitingTime <= 0)
            {
                pauseTime = true;
                StartCoroutine(ExitStore());
            }
        }
        if(audioSource != null)
        {
            audioSource.volume = SaveData.Instance.saveFile.voiceVolume;
        }
        if(levelManager.pauseTime)
        {
            audioSource.Pause();
            pauseTime = true;
        }
        else
        {
            audioSource.UnPause();
            pauseTime = false;
        }
    }

    public void Checkout()
    {
        audioSource.PlayOneShot(checkoutAudio, levelManager.sfxSlider.value);
        pauseTime = true;
        float point = 1;
        if (waitingTime < 30) point -= 0.5f;
        else if (waitingTime <= 0) point -= 0.05f;
        levelManager.AddScore(point);
        StartCoroutine(ExitStore());
    }

    IEnumerator ExitStore()
    {
        GetComponent<Collider2D>().enabled = false;
        speechBubble.gameObject.SetActive(false);

        if (orderCompleted)
        {
            audioSource.PlayOneShot(customer.sinmung, levelManager.voiceSlider.value);
        }

        while (sprite.color.a > 0)
        {
            sprite.color -= new Color(0, 0, 0, Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
        yield break;
    }

    IEnumerator EnterStore()
    {
        audioSource.clip = entranceAudio;
        audioSource.Play();

        levelManager.customerTally.text = (int.Parse(levelManager.customerTally.text) - 1).ToString();
        while (sprite.color.a < 1)
        {
            sprite.color += new Color(0, 0, 0, Time.deltaTime);
            yield return null;
        }

        audioSource.Stop();

        List<int> lottery = new List<int>() { 0, 1, 2};
        for(int i = 0; i < lottery.Count - 1; i++)
        {
            var temp = lottery[i];
            int rand = UnityEngine.Random.Range(i, lottery.Count);
            lottery[i] = lottery[rand];
            lottery[rand] = temp;
        }

        speechBubble.gameObject.SetActive(true);

        while (levelManager.pauseTime) yield return null;

        yield return SayOrder(lottery[0]);
        yield return new WaitForSeconds(0.25f);

        while (levelManager.pauseTime) yield return null;

        yield return SayOrder(lottery[1]);
        yield return new WaitForSeconds(0.25f);

        while (levelManager.pauseTime) yield return null;

        yield return SayOrder(lottery[2]);

        GetComponent<Collider2D>().enabled = true;
        pauseTime = false;
        yield break;
    }

    IEnumerator SayOrder(int lottery)
    {
        switch(lottery)
        {
            case 0:
                if (order.qtyNyukCung > 0)
                {
                    bubbleText.text += ConvertNumber(order.qtyNyukCung) + " Nyuk Cung.\n";
                    audioSource.Play();
                    while (audioSource.isPlaying && !levelManager.pauseTime || levelManager.pauseTime) yield return null;
                    audioSource.clip = customer.nyukCung;
                    audioSource.Play();
                    while (audioSource.isPlaying && !levelManager.pauseTime || levelManager.pauseTime) yield return null;
                }
                break;
            case 1:
                if (order.qtyThewFuSui > 0)
                {
                    bubbleText.text += ConvertNumber(order.qtyThewFuSui) + " Thew Fu Sui.\n";
                    audioSource.Play();
                    while (audioSource.isPlaying && !levelManager.pauseTime || levelManager.pauseTime) yield return null;
                    audioSource.clip = customer.thewFuSui;
                    audioSource.Play();
                    while (audioSource.isPlaying && !levelManager.pauseTime || levelManager.pauseTime) yield return null;
                }
                break;
            case 2:
                if (order.qtyBongLiPiang > 0)
                {
                    bubbleText.text += ConvertNumber(order.qtyBongLiPiang) + " Bong Li Piang.\n";
                    audioSource.Play();
                    while (audioSource.isPlaying && !levelManager.pauseTime || levelManager.pauseTime) yield return null;
                    audioSource.clip = customer.bongLiPiang;
                    audioSource.Play();
                    while (audioSource.isPlaying && !levelManager.pauseTime || levelManager.pauseTime) yield return null;
                }
                break;
        }
    }

    private string ConvertNumber(int number)
    {
        switch (number)
        {
            case 1:
                audioSource.clip = customer.jit;
                return "Jit";
            case 2:
                audioSource.clip = customer.ngi;
                return "Ngi";
            case 3:
                audioSource.clip = customer.sam;
                return "Sam";
            case 4:
                audioSource.clip = customer.si;
                return "Si";
            case 5:
                audioSource.clip = customer.ng;
                return "Ng";
            case 6:
                audioSource.clip = customer.liuk;
                return "Liuk";
            case 7:
                audioSource.clip = customer.chit;
                return "Chit";
            case 8:
                audioSource.clip = customer.pat;
                return "Pat";
            case 9:
                audioSource.clip = customer.kiu;
                return "Kiu";
            case 10:
                audioSource.clip = customer.ship;
                return "Ship";
        }
        return "";
    }
}

[System.Serializable]
public class OrderDetail
{
    public int qtyNyukCung;
    public int qtyThewFuSui;
    public int qtyBongLiPiang;
}