using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupUIText : MonoBehaviour
{
    [SerializeField] GameObject uiTextObject;   // Canvas ���� "F Ű ������ ������ ����" Text ������Ʈ
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
