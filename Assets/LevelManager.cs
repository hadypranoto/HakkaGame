using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public int currLevel;
    public int customerAmount;
    public List<Customer> customers;

    public List<GameObject> itemSpawners;
    public GameObject customerPrefab;
    public List<Transform> spots;

    public GameObject completionScreen;
    public Slider score;
    public float time;
    public bool pauseTime;
    public Sprite filledStar;
    public Sprite emptyStar;
    public List<Image> stars;

    public TextMeshProUGUI customerTally;
    public Image timeDial;

    public List<Level> levels;

    public List<Image> completionStars;
    public GameObject nextLevelButton;
    public TextMeshProUGUI completionText;

    public GameObject pauseMenu;
    public AudioSource clickAudio;
    private AudioSource bgmAudio;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Slider voiceSlider;

    private SaveData saveData;

    public Image blackscreen;
    public GameObject tutorialObj;
    public bool levelDone = false;

    public AudioSource timerAudio;
    public AudioClip alarmClip;
    public GameObject closedSign;

    public AudioClip levelClearSound;
    public List<AudioClip> starsAudio;

    private AudioSource audioSource;
    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        bgmAudio = GameObject.FindGameObjectWithTag("BGM").GetComponent<AudioSource>();
        saveData = FindAnyObjectByType<SaveData>();
        saveData.DeserializeData();

        currLevel = saveData.currentLevel;

        bgmSlider.value = saveData.saveFile.bgmVolume;
        sfxSlider.value = saveData.saveFile .sfxVolume;
        voiceSlider.value = saveData.saveFile .voiceVolume;

        if (currLevel == 1)
        {
            itemSpawners[0].SetActive(true);
            itemSpawners[1].SetActive(false);
            itemSpawners[2].SetActive(false);
        }else if(currLevel == 2)
        {
            itemSpawners[0].SetActive(true);
            itemSpawners[1].SetActive(true);
            itemSpawners[2].SetActive(false);
        }else if(currLevel == 3)
        {
            itemSpawners[0].SetActive(true);
            itemSpawners[1].SetActive(true);
            itemSpawners[2].SetActive(true);
        }

        time = 100;

        stars[0].sprite = emptyStar;
        stars[1].sprite = emptyStar;
        stars[2].sprite = emptyStar;
        score.maxValue = customerAmount =  levels[currLevel - 1].details.Count;
        customerTally.text = customerAmount.ToString();
        score.value = 0;

        completionScreen.SetActive(false);
    }

    IEnumerator Start()
    {
        blackscreen.gameObject.SetActive(true);
        blackscreen.color = new Color(0, 0, 0, 1);
        while (blackscreen.color.a > 0)
        {
            blackscreen.color -= new Color(0, 0, 0, Time.deltaTime);
            yield return null;
        }
        blackscreen.gameObject.SetActive(false);

        if (currLevel == 1)
        {
            PauseGame();
            tutorialObj.SetActive(true);
            while (tutorialObj.activeInHierarchy)
            {
                yield return null;
            }
            ResumeGame();
        }

        yield break;
    }

    public Button pauseButton;
    public Button resumeButton;
    private void Update()
    {
        if (!pauseTime)
        {
            if(currLevel == 3)
            {
                time -= Time.deltaTime * 0.75f;
            }
            else time -= Time.deltaTime;
            timeDial.fillAmount = time/100;

            if(time > 0 && time <= 15 && !timerAudio.isPlaying)
            {
                timerAudio.loop = true;
                timerAudio.Play();
            }
            if(time <= 0 && !closedSign.activeInHierarchy)
            {
                timerAudio.loop = false;
                timerAudio.Stop();
                timerAudio.PlayOneShot(alarmClip, sfxSlider.value);
                closedSign.SetActive(true);
            }

            if(spots[0].childCount == 0 && spots[1].childCount == 0 && spots[2].childCount == 0)
            {
                if (time <= 0 || int.Parse(customerTally.text) == 0)
                {
                    pauseTime = true;
                    LevelEvaluation();
                }
            }

            if (int.Parse(customerTally.text) == 0 || time <= 0) return;
            var currQueue = levels[currLevel - 1].details[customerAmount - int.Parse(customerTally.text)];
            if (currQueue.enterTime >= time)
            {
                if (spots[1].childCount == 0)
                {
                    InitCustomer(1);
                }
                else if (spots[0].childCount == 0)
                {
                    InitCustomer(0);
                }
                else if (spots[2].childCount == 0)
                {
                    InitCustomer(2);
                }
            }            
        }
        else
        {
            saveData.SerializeData();
        }

        saveData.saveFile.bgmVolume = bgmAudio.volume = bgmSlider.value;
        saveData.saveFile.sfxVolume = sfxSlider.value;
        saveData.saveFile.voiceVolume = voiceSlider.value;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseTime)
            {
                pauseButton.onClick.Invoke();
            }
            else
            {
                resumeButton.onClick.Invoke();
            }
        }
    }

    public void PlayOneShot(AudioClip audioClip)
    {
        clickAudio.PlayOneShot(audioClip, sfxSlider.value);
    }

    public void ClickUISound()
    {
        clickAudio.PlayOneShot(clickAudio.clip, sfxSlider.value);
    }

    public void InitCustomer(int spot)
    {
        var currQueue = levels[currLevel - 1].details[customerAmount - int.Parse(customerTally.text)];
        var customer = Instantiate(customerPrefab, spots[spot].transform).GetComponent<CustomerHandler>();
        customer.order = currQueue.orderDetails;

        var lottery = UnityEngine.Random.Range(0, customers.Count);
        customer.customer = customers[lottery];
        customer.GetComponent<Image>().sprite = customer.customer.sprite;

        if(currLevel >= 1)
        {
            if (currLevel == 1)
            {
                customer.order.qtyNyukCung = UnityEngine.Random.Range(1, 11);
            }
            else
            {
                customer.order.qtyNyukCung = UnityEngine.Random.Range(0, 11);
            }
        }
        if (currLevel >= 2)
        {
            customer.order.qtyThewFuSui = UnityEngine.Random.Range(0, 3);
        }
        if (currLevel >= 3)
        {
            customer.order.qtyBongLiPiang = UnityEngine.Random.Range(0, 11);
        }

        if(customer.order.qtyNyukCung == 0 && customer.order.qtyThewFuSui == 0 && customer.order.qtyBongLiPiang == 0)
        {
            lottery = UnityEngine.Random.Range(0, 3);
            switch(lottery)
            {
                case 0:
                    customer.order.qtyNyukCung = UnityEngine.Random.Range(1, 11);
                    break;
                case 1:
                    customer.order.qtyThewFuSui = UnityEngine.Random.Range(1, 3);
                    break;
                case 2:
                    customer.order.qtyBongLiPiang = UnityEngine.Random.Range(1, 11);
                    break;
            }
        }

        customer.CustomAwake();
    }

    public void PauseGame()
    {
        pauseTime = true;
        timerAudio.Pause();
    }

    public void ResumeGame()
    {
        pauseTime = false;
        timerAudio.UnPause();
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene("Level");
    }

    public void NextLevel()
    {
        saveData.currentLevel++;
        SceneManager.LoadScene("Level");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LevelEvaluation()
    {
        timerAudio.Stop();
        levelDone = true;
        int stars = 0;

        if (score.value >= score.maxValue)
        {
            completionStars[2].sprite = filledStar;
            completionStars[1].sprite = filledStar;
            completionStars[0].sprite = filledStar;
            stars = 3;
        }else if (score.value >= score.maxValue * 0.75)
        {
            completionStars[1].sprite = filledStar;
            completionStars[0].sprite = filledStar;
            stars = 2;
        }else if (score.value >= score.maxValue / 2)
        {
            completionStars[0].sprite = filledStar;
            stars = 1;
        }

        if(stars == 0)
        {
            completionText.text = "Level Failed";
            nextLevelButton.SetActive(false);
        }else if(stars > 0)
        {
            switch (currLevel)
            {
                case 1:
                    if(stars >= saveData.saveFile.level1Stars)
                    {
                        saveData.saveFile.level1Stars = stars;
                    }
                    break;
                case 2:
                    if (stars >= saveData.saveFile.level2Stars)
                    {
                        saveData.saveFile.level2Stars = stars;
                    }
                    break;
                case 3:
                    if (stars >= saveData.saveFile.level3Stars)
                    {
                        saveData.saveFile.level3Stars = stars;
                    }
                    nextLevelButton.SetActive(false);
                    break;
            }
            saveData.SerializeData();
            completionText.text = "Level Completed";
            audioSource.PlayOneShot(levelClearSound, saveData.saveFile.sfxVolume);
        }
        
        completionScreen.SetActive(true);
    }

    public void AddScore(float point)
    {
        score.value += point;
        if(score.value >= score.maxValue / 2 && stars[0].sprite != filledStar)
        {
            audioSource.PlayOneShot(starsAudio[0], saveData.saveFile.sfxVolume);
            stars[0].sprite = filledStar;
        }
        if (score.value >= score.maxValue * 0.75 && stars[1].sprite != filledStar)
        {
            audioSource.PlayOneShot(starsAudio[1], saveData.saveFile.sfxVolume);
            stars[1].sprite = filledStar;
        }
        if (score.value >= score.maxValue && stars[2].sprite != filledStar)
        {
            audioSource.PlayOneShot(starsAudio[2], saveData.saveFile.sfxVolume);
            stars[2].sprite = filledStar;
        }
    }
}

[Serializable]
public class Level
{
    public List<CustomerDetails> details;

    [Serializable]
    public class CustomerDetails
    {
        public int enterTime;
        [HideInInspector] public OrderDetail orderDetails;
    }
}