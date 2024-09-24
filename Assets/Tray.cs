using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tray : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private CustomerHandler customer;
    private AudioSource trayAudio;
    public Transform front, back;

    private void Awake()
    {
        trayAudio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Customer")
        {
            customer = collision.GetComponent<CustomerHandler>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(customer != null)
        {
            if (collision.gameObject == customer.gameObject)
            {
                customer = null;
            }
        }
    }

    private bool followMouse;
    private Vector3 offset;

    public void OnPointerDown(PointerEventData eventData)
    {
        followMouse = true;
        offset = transform.position - Input.mousePosition;
        transform.SetParent(front);
    }
    private void Update()
    {
        if (followMouse)
        {
            transform.position = Input.mousePosition + offset;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (customer != null)
        {
            if (CheckOrder())
            {
                foreach(Transform item in transform)
                {
                    Destroy(item.gameObject);
                }
                customer.Checkout();
            }
            else
            {
                trayAudio.PlayOneShot(trayAudio.clip, SaveData.Instance.saveFile.sfxVolume);
                StartCoroutine(DisplayMessage());
            }
        }
        customer = null;
        followMouse = false;
        transform.SetParent(back);
        transform.position = back.position;
    }

    public TextMeshProUGUI message;
    IEnumerator DisplayMessage()
    {
        message.gameObject.SetActive(true);
        message.color = new Color(0, 0, 0, 1);
        while (message.color.a > 0)
        {
            message.color -= new Color(0, 0, 0, Time.deltaTime);
            yield return null;
        }
        message.gameObject.SetActive(true);
        yield break;
    }

    public bool CheckOrder()
    {
        OrderDetail trayContent = new();
        foreach (Transform item in transform)
        {
            var itemName = item.gameObject.GetComponent<Item>().itemName;
            switch (itemName)
            {
                case "Nyuk Cung":
                    trayContent.qtyNyukCung++;
                    break;
                case "Thew Fu Sui":
                    trayContent.qtyThewFuSui++;
                    break;
                case "Bong Li Piang":
                    trayContent.qtyBongLiPiang++;
                    break;
            }
        }
        if (trayContent.qtyNyukCung != customer.order.qtyNyukCung) return false;
        if (trayContent.qtyThewFuSui != customer.order.qtyThewFuSui) return false;
        if (trayContent.qtyBongLiPiang != customer.order.qtyBongLiPiang) return false;
        customer.orderCompleted = true;
        return true;
    }
}
