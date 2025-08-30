using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening; // DOTween ���ӽ����̽� �߰�

public class ArtifactMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform menuPanel; // �޴� ��ü
    private Vector2 originalPosition;
    private Vector2 targetPosition;
    private Tween currentTween; // Tweening

    [Header("��Ƽ��Ʈ ���� ����")]
    [SerializeField] private Transform artifactMenuPanel; // GridLayoutGroup ���� �г�

    private List<Image> slots;

    void Start()
    {
        menuPanel = GetComponent<RectTransform>();
        originalPosition = menuPanel.anchoredPosition;
        targetPosition = new Vector2(originalPosition.x, originalPosition.y - 80f);
        // ����ġ���� 80 �Ʒ��� �̵�

        // �г� ���ڽš��� �����ϰ�, �ٷ� �Ʒ� �ڽĵ鸸 �������� ����
        slots = new List<Image>();
        foreach (Transform child in artifactMenuPanel)
        {
            var img = child.GetComponent<Image>();
            if (img != null)
            {
                img.enabled = false;   // ������� �� �� ���̰�
                slots.Add(img);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentTween?.Kill(); // ���� Tween�� ���� ���̸� ����
        currentTween = menuPanel.DOAnchorPos(targetPosition, 0.3f).SetEase(Ease.OutQuad);
        // targetPosition���� �̵��ϴ� Tween ����
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        currentTween?.Kill();
        currentTween = menuPanel.DOAnchorPos(originalPosition, 0.3f).SetEase(Ease.OutQuad);
    }

    /// <summary>ù ��° ����ִ� ������ ã�� Ȱ��ȭ�ϰ� ��������Ʈ�� ä��</summary>
    public bool TryAddArtifact(Sprite sprite)
    {
        foreach (var slot in slots)
        {
            if (!slot.enabled)
            {
                slot.enabled = true;
                slot.sprite = sprite;
                return true;
            }
        }
        Debug.Log("������ ���� á���ϴ�!");
        return false;
    }
}