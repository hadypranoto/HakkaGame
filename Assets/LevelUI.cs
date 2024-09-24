using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    public Button clickable;
    public Sprite filledStar;
    public Sprite emptyStar;
    public List<Image> stars;
    public GameObject locks;
    
    public void CheckLevelInfo(int level)
    {
        var saveData = FindAnyObjectByType<SaveData>();
        var starPoints = 0;
        switch (level)
        {
            case 1:
                starPoints = saveData.saveFile.level1Stars;
                break;
            case 2:
                starPoints = saveData.saveFile.level2Stars;
                break;
            case 3:
                starPoints = saveData.saveFile.level3Stars;
                break;
        }
        if(starPoints == 0)
        {
            stars[0].sprite = emptyStar;
            stars[1].sprite = emptyStar;
            stars[2].sprite = emptyStar;
        }
        if(starPoints > 0)
        {
            stars[0].sprite = filledStar;
        }
        if (starPoints > 1)
        {
            stars[1].sprite = filledStar;
        }
        if (starPoints > 2)
        {
            stars[2].sprite = filledStar;
        }
    }
}
