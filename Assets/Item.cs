using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public string itemName;
    
    private bool followMouse;
    private Vector3 offset;

    private bool onTray;
    private Transform spotTransform;
    private Transform frontTransform;
    public GameObject dummyItem;

    private bool placeable;
    private GameObject tray;
    private Vector3 itemPrevPos;

    private LevelManager levelManager;
    public AudioClip foodSound;

    private AudioSource audioSource;

    private void Awake()
    {
        levelManager = FindAnyObjectByType<LevelManager>();
        audioSource = GetComponent<AudioSource>();
        itemPrevPos = transform.position;

        spotTransform = transform.parent;
        frontTransform = GameObject.FindGameObjectWithTag("Front").transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "Tray")
        {
            placeable = true;
            tray = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Tray")
        {
            placeable = false;
        }
    }

    private void Update()
    {
        if (levelManager.levelDone)
        {
            OnPointerUp(null);
        }
        if (followMouse)
        {
            transform.position = Input.mousePosition + offset;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (followMouse) return;
        if(Input.GetMouseButtonDown(1) && placeable)
        {
            Destroy(gameObject);
        }
        transform.SetAsLastSibling();
        itemPrevPos = transform.position;
        
        followMouse = true;
        offset = transform.position - Input.mousePosition;

        if (!onTray)
        {
            Instantiate(dummyItem, spotTransform);
        }
        transform.SetParent(frontTransform);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        followMouse = false;
        if (placeable)
        {
            if (!onTray)
            {
                Destroy(spotTransform.GetChild(0).gameObject);
            }
            onTray = true;
        }

        if (onTray)
        {
            transform.SetParent(tray.transform);
            if (!levelManager.levelDone)
                audioSource.PlayOneShot(foodSound, levelManager.sfxSlider.value);
        }

        if (!placeable)
        {
            if(!onTray)
            {
                Destroy(spotTransform.GetChild(0).gameObject);
                transform.SetParent(spotTransform);
                if (!levelManager.levelDone)
                    audioSource.PlayOneShot(foodSound, levelManager.sfxSlider.value);
            }
            transform.position = itemPrevPos;
        }
    }
}
