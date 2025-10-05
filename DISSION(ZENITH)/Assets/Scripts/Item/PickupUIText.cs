using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupUIText : MonoBehaviour
{
    [SerializeField] GameObject uiTextObject;   // Canvas 안의 "F 키 누르고 아이템 수집" Text 오브젝트
    [SerializeField] string playerTag = "Player";

    void Start()
    {
        if (uiTextObject) uiTextObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) uiTextObject?.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) uiTextObject?.SetActive(false);
    }
}
