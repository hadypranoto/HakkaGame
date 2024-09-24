using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Image blackscreen;
    public GameObject mainMenu;
    public GameObject levelMenu;
    public GameObject settingsMenu;
    public GameObject creditsScreen;
    private AudioSource bgmAudio;
    private AudioSource sfxAudio;

    private void Awake()
    {
        mainMenu.SetActive(true);
        levelMenu.SetActive(false);
        settingsMenu.SetActive(false);
        creditsScreen.SetActive(false);
    }

    IEnumerator Start()
    {
        var saveData = SaveData.Instance;
        saveData.DeserializeData();

        bgmAudio = GameObject.FindGameObjectWithTag("BGM").GetComponent<AudioSource>();
        bgmAudio.volume = saveData.saveFile.bgmVolume;

        sfxAudio = GetComponent<AudioSource>();
        sfxAudio.volume = saveData.saveFile.sfxVolume;

        blackscreen.gameObject.SetActive(true);
        blackscreen.color = new Color(0, 0, 0, 1);
        while (blackscreen.color.a > 0)
        {
            blackscreen.color -= new Color(0, 0, 0, Time.deltaTime);
            yield return null;
        }
        blackscreen.gameObject.SetActive(false);
    }

    public void ClickUISound()
    {
        sfxAudio.PlayOneShot(sfxAudio.clip, SaveData.Instance.saveFile.sfxVolume);
    }

    public List<LevelUI> levels;
    public void CheckLevelInfo()
    {
        var saveData = FindAnyObjectByType<SaveData>();
        var count = 1;
        foreach(LevelUI level in levels)
        {
            level.CheckLevelInfo(count);
            count++;
        }

        levels[0].locks.SetActive(false);
        levels[1].locks.SetActive(true);
        levels[1].GetComponent<Button>().interactable = false;
        levels[2].locks.SetActive(true);
        levels[2].GetComponent<Button>().interactable = false;

        if (saveData.saveFile.level1Stars > 0)
        {
            levels[1].locks.SetActive(false);
            levels[1].GetComponent<Button>().interactable = true;
        }
        if (saveData.saveFile.level2Stars > 0)
        {
            levels[2].locks.SetActive(false);
            levels[2].GetComponent<Button>().interactable = true;
        }
    }

    public void LoadLevel(int level)
    {
        var saveData = FindAnyObjectByType<SaveData>();
        saveData.currentLevel = level;
        StartCoroutine(LoadLevelIE());
    }

    IEnumerator LoadLevelIE()
    {
        blackscreen.gameObject.SetActive(true);

        blackscreen.color = new Color(0, 0, 0, 0);
        while (blackscreen.color.a < 1)
        {
            blackscreen.color += new Color(0, 0, 0, Time.deltaTime);
            yield return null;
        }

        SceneManager.LoadScene("Level");
        yield break;
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    public void SetActive(GameObject menu)
    {
        menu.SetActive(true);
    }

    public void SetInactive(GameObject menu)
    {
        menu.SetActive(false);
    }

    public void ApplicationExit()
    {
        Application.Quit();
    }
}
